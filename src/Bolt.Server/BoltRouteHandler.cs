﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Bolt.Server.Filters;
using Bolt.Server.Metadata;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
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
            Filters = new List<IServerExecutionFilter>();
            ApplicationServices = applicationServices;
            Configuration = defaultConfiguration.Options;
            _actionResolver = actionResolver;
            _contractResolver = contractResolver;
        }

        public IList<IServerExecutionFilter> Filters { get; }

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

        public virtual async Task RouteAsync(RouteContext context)
        {
            IBoltFeature feature = AssignBoltFeature(context.HttpContext,
                new ServerActionContext()
                {
                    HttpContext = context.HttpContext,
                    RouteContext = context
                });

            var path = context.HttpContext.Request.Path;
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
                    await HandleBoltRootAsync(context);
                }

                return;
            }

            var found = FindContract(_invokers, result[0]);
            if (found == null)
            {
                if (!string.IsNullOrEmpty(Options.Prefix))
                {
                    Logger.LogWarning(BoltLogId.ContractNotFound, "Contract with name '{0}' not found in registered contracts at '{1}'", result[0], path);

                    // we have defined bolt prefix, report error about contract not found
                    await feature.Configuration.ErrorHandler.HandleErrorAsync(new HandleErrorContext
                    {
                        Options = feature.Configuration.Options,
                        ExceptionWrapper = feature.Configuration.ExceptionWrapper,
                        ActionContext = feature.ActionContext,
                        Serializer = feature.Configuration.Serializer,
                        ErrorCode = ServerErrorCode.ContractNotFound
                    });

                    context.IsHandled = true;
                }

                // just pass to next middleware in chain
                return;
            }

            feature.ActionContext.ContractInvoker = found;

            if (result.Length == 1)
            {
                if (!string.IsNullOrEmpty(Options.Prefix))
                {
                    await HandleContractRootAsync(context, found);
                }

                return;
            }

            // at this point Bolt will handle the request
            context.IsHandled = true;
            var actionName = result[1];
            var actionDescriptor = FindAction(feature.ActionContext, actionName);
            if (actionDescriptor == null)
            {
                await feature.Configuration.ErrorHandler.HandleErrorAsync(new HandleErrorContext
                {
                    Options = feature.Configuration.Options,
                    ExceptionWrapper = feature.Configuration.ExceptionWrapper,
                    ActionContext = feature.ActionContext,
                    Serializer = feature.Configuration.Serializer,
                    ErrorCode = ServerErrorCode.ActionNotFound
                });

                return;
            }

            feature.ActionContext.Action = actionDescriptor;
            await Execute(feature.ActionContext, found);
        }

        protected virtual async Task Execute(ServerActionContext ctxt, IContractInvoker invoker)
        {
            IBoltFeature feature = ctxt.HttpContext.GetFeature<IBoltFeature>();

            using (Logger.BeginScope("Execute"))
            {
                Stopwatch watch = null;
                if (Logger.IsEnabled(LogLevel.Verbose))
                {
                    watch = Stopwatch.StartNew();
                }

                feature.FilterProviders = ctxt.HttpContext.ApplicationServices.GetService<IEnumerable<IFilterProvider>>().ToList();

                try
                {
                    await invoker.ExecuteAsync(ctxt);
                    if (!ctxt.IsResponseSend)
                    {
                        await feature.Configuration.ResponseHandler.HandleAsync(ctxt);
                    }

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
                    if (!ctxt.IsResponseSend)
                    {
                        await feature.Configuration.ErrorHandler.HandleErrorAsync(new HandleErrorContext
                        {
                            Options = feature.Configuration.Options,
                            ExceptionWrapper = feature.Configuration.ExceptionWrapper,
                            ActionContext = feature.ActionContext,
                            Serializer = feature.Configuration.Serializer,
                            Error = e
                        });
                    }

                    Logger.LogError(BoltLogId.RequestExecutionError, "Execution of '{0}' failed with error '{1}'", ctxt.Action, e);
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

        protected virtual IBoltFeature AssignBoltFeature(HttpContext context, ServerActionContext actionContext)
        {
            var boltFeature = new BoltFeature(this) { ActionContext = actionContext, Configuration = Configuration };
            context.SetFeature<IBoltFeature>(boltFeature);
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