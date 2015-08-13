﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Bolt.Server.Metadata;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace Bolt.Server
{
    public class BoltRouteHandler : IBoltRouteHandler, IEnumerable<IContractInvoker>
    {
        private readonly IActionResolver _actionResolver;
        private readonly IContractResolver _contractResolver;
        private readonly List<IContractInvoker> _invokers = new List<IContractInvoker>();

        public BoltRouteHandler(ILoggerFactory factory, IOptions<ServerRuntimeConfiguration> defaultConfiguration, IBoltMetadataHandler metadataHandler,
             IServiceProvider applicationServices, IActionResolver actionResolver, IContractResolver contractResolver)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (defaultConfiguration == null)
            {
                throw new ArgumentNullException(nameof(defaultConfiguration));
            }

            if (applicationServices == null)
            {
                throw new ArgumentNullException(nameof(applicationServices));
            }

            if (actionResolver == null)
            {
                throw new ArgumentNullException(nameof(actionResolver));
            }

            if (contractResolver == null)
            {
                throw new ArgumentNullException(nameof(contractResolver));
            }

            Logger = factory.CreateLogger<BoltRouteHandler>();
            MetadataHandler = metadataHandler;
            ApplicationServices = applicationServices;
            Configuration = defaultConfiguration.Options;
            _actionResolver = actionResolver;
            _contractResolver = contractResolver;
        }

        public IServiceProvider ApplicationServices { get; }

        public ServerRuntimeConfiguration Configuration { get; set; }

        public ILogger Logger { get; }

        public IBoltMetadataHandler MetadataHandler { get; set; }

        private BoltServerOptions Options => Configuration.Options;

        public virtual void Add(IContractInvoker invoker)
        {
            if (invoker == null)
            {
                throw new ArgumentNullException(nameof(invoker));
            }

            if (_invokers.FirstOrDefault(i => i.Contract.Name == invoker.Contract.Name) != null)
            {
                throw new InvalidOperationException($"Invoker for contract '{invoker.Contract.Name}' already registered.");
            }

            invoker.Pipeline.Validate(invoker.Contract);
            Logger.LogInformation(BoltLogId.ContractAdded, "Adding contract: {0}", invoker.Contract.Name);
            _invokers.Add(invoker);

            foreach (MethodInfo action in BoltFramework.GetContractActions(invoker.Contract))
            {
                Logger.LogVerbose("Action: {0}", action.Name);
            }
        }

        public IContractInvoker Get(Type contract)
        {
            return _invokers.FirstOrDefault(i => i.Contract == contract);
        }

        public IEnumerator<IContractInvoker> GetEnumerator()
        {
            return _invokers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual async Task RouteAsync(RouteContext routeContext)
        {
            ServerActionContext actionContext = new ServerActionContext(routeContext);
            actionContext.Configuration = new ServerRuntimeConfiguration(Configuration);
            AssignBoltFeature(actionContext);

            var path = routeContext.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(Options.Prefix))
            {
                if (!path.StartsWithSegments(new PathString("/" + Options.Prefix), out path))
                {
                    return;
                }
            }

            var result = path.Value.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // we have accessed Bolt root
            if (result.Length == 0)
            {
                if (!string.IsNullOrEmpty(Options.Prefix))
                {
                    await HandleBoltRootAsync(routeContext);
                }

                return;
            }

            var found = FindContract(_invokers, result[0]);
            if (found == null)
            {
                if (!string.IsNullOrEmpty(Options.Prefix))
                {
                    Logger.LogWarning(BoltLogId.ContractNotFound, "Contract with name '{0}' not found in registered contracts at '{1}'", result[0], path);
                    actionContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    actionContext.HttpContext.Response.Headers.Set(Options.ServerErrorHeader, ServerErrorCode.ContractNotFound.ToString());
                    routeContext.IsHandled = true;
                }

                // just pass to next middleware in chain
                return;
            }

            actionContext.ContractInvoker = found;
            actionContext.Contract = found.Contract;

            if (result.Length == 1)
            {
                if (!string.IsNullOrEmpty(Options.Prefix))
                {
                    await HandleContractRootAsync(routeContext, found);
                }

                return;
            }

            // at this point Bolt will handle the request
            routeContext.IsHandled = true;
            var actionName = result[1];
            var actionDescriptor = FindAction(actionContext, actionName);
            if (actionDescriptor == null)
            {
                Logger.LogWarning(BoltLogId.ContractNotFound, "Action with name '{0}' not found on contract '{1}'", actionName, actionContext.ContractName);

                actionContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                actionContext.HttpContext.Response.Headers.Set(Options.ServerErrorHeader, ServerErrorCode.ActionNotFound.ToString());
                routeContext.IsHandled = true;
                return;
            }

            actionContext.Action = actionDescriptor;
            await Execute(actionContext);
        }

        protected virtual async Task Execute(ServerActionContext ctxt)
        {
            using (Logger.BeginScope("Execute"))
            {
                Stopwatch watch = null;
                if (Logger.IsEnabled(LogLevel.Verbose))
                {
                    watch = Stopwatch.StartNew();
                }

                try
                {
                    await ctxt.ContractInvoker.ExecuteAsync(ctxt);
                }
                catch (OperationCanceledException)
                {
                    if (!ctxt.RequestAborted.IsCancellationRequested)
                    {
                        // TODO: is this ok ? 
                        ctxt.HttpContext.Response.Body.Dispose();
                        Logger.LogError(BoltLogId.RequestCancelled, "Action '{0}' was cancelled.", ctxt.Action);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(BoltLogId.RequestExecutionError, "Execution of '{0}' failed with error '{1}'", ctxt.Action, e);
                    throw;
                }
                finally
                {
                    if (watch != null)
                    {
                        Logger.LogVerbose(BoltLogId.RequestExecutionTime, "Execution of '{0}' has taken '{1}ms'", ctxt.Action, watch.ElapsedMilliseconds);
                    }
                }
            }
        }

        protected virtual IBoltFeature AssignBoltFeature( ServerActionContext actionContext)
        {
            BoltFeature boltFeature = new BoltFeature(actionContext);
            actionContext.HttpContext.SetFeature<IBoltFeature>(boltFeature);
            return boltFeature;
        }

        protected virtual IContractInvoker FindContract(IEnumerable<IContractInvoker> registeredContracts, string contractName)
        {
            var found = _contractResolver.Resolve(registeredContracts.Select(c => c.Contract), contractName);
            if (found == null)
            {
                return null;
            }

            return registeredContracts.First(c => c.Contract == found);
        }

        protected virtual MethodInfo FindAction(ServerActionContext context, string actionName)
        {
            return _actionResolver.Resolve(context.ContractInvoker.Contract, actionName);
        }

        protected virtual async Task HandleContractRootAsync(RouteContext context, IContractInvoker descriptor)
        {
            if (MetadataHandler == null)
            {
                return;
            }

            var feature = context.HttpContext.GetFeature<IBoltFeature>();

            try
            {
                var handled = await MetadataHandler.HandleContractMetadataAsync(feature.ActionContext);
                if (handled)
                {
                    context.IsHandled = true;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(BoltLogId.HandleContractMetadataError, $"Failed to handle metadata for contract {descriptor.Contract.Name}.", e);
            }
        }

        protected virtual async Task HandleBoltRootAsync(RouteContext context)
        {
            if (MetadataHandler == null)
            {
                return;
            }

            var feature = context.HttpContext.GetFeature<IBoltFeature>();

            try
            {
                var handled = await MetadataHandler.HandleBoltMetadataAsync(feature.ActionContext, _invokers);
                if (handled)
                {
                    context.IsHandled = true;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(BoltLogId.HandleBoltRootError, "Failed to handle root metadata.", e);
            }
        }

        public virtual VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }
    }
}