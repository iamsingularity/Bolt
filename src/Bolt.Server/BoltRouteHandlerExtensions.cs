using System;
using System.Linq;
using Bolt.Pipeline;
using Bolt.Server.InstanceProviders;
using Bolt.Server.Pipeline;
using Bolt.Server.Session;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bolt.Server
{
    public static class BoltRouteHandlerExtensions
    {
        public static IContractInvoker Use<TContract, TContractImplementation>(
            this IBoltRouteHandler bolt,
            Action<ConfigureContractContext> configure = null) where TContractImplementation : TContract
        {
            return bolt.Use<TContract>(InstanceProvider.From<TContractImplementation>(), configure);
        }

        public static IContractInvoker UseMemorySession<TContract, TContractImplementation>(
            this IBoltRouteHandler bolt,
            Action<ConfigureContractContext> configure = null) where TContractImplementation : TContract
        {
            var config = new ServerRuntimeConfiguration(bolt.Configuration);
            var factory = new MemorySessionFactory(
                config,
                bolt.ApplicationServices.GetRequiredService<IServerSessionHandler>(),
                bolt.ApplicationServices.GetRequiredService<ILoggerFactory>());

            return bolt.Use<TContract>(InstanceProvider.Session.From<TContractImplementation>(factory), configure, config);
        }

        public static IContractInvoker UseDistributedSession<TContract, TContractImplementation>(
            this IBoltRouteHandler bolt,
            Action<ConfigureContractContext> configure = null) where TContractImplementation : TContract
        {
            var config = new ServerRuntimeConfiguration(bolt.Configuration);
            DistributedSessionFactory factory = new DistributedSessionFactory(
                config,
                new DistributedSessionStore(
                    bolt.ApplicationServices.GetRequiredService<IDistributedCache>(),
                    bolt.ApplicationServices.GetRequiredService<ILoggerFactory>()),
                bolt.ApplicationServices.GetRequiredService<IServerSessionHandler>());

            return bolt.UseSession<TContract, TContractImplementation>(factory, configure, config);
        }

        public static IContractInvoker UseDistributedSession<TContract, TContractImplementation>(
            this IBoltRouteHandler bolt,
            ISessionStore sessionStore,
            Action<ConfigureContractContext> configure = null) where TContractImplementation : TContract
        {
            var config = new ServerRuntimeConfiguration(bolt.Configuration);
            var factory = new DistributedSessionFactory(
                config,
                sessionStore,
                bolt.ApplicationServices.GetRequiredService<IServerSessionHandler>());

            return bolt.UseSession<TContract, TContractImplementation>(factory, configure, config);
        }

        public static IContractInvoker UseSession<TContract, TContractImplementation>(
            this IBoltRouteHandler bolt,
            ISessionFactory sessionFactory,
            Action<ConfigureContractContext> configure = null,
            ServerRuntimeConfiguration configuration = null) where TContractImplementation : TContract
        {
            return bolt.Use<TContract>(InstanceProvider.Session.From<TContractImplementation>(sessionFactory), configure, configuration);
        }

        public static IContractInvoker Use<TContract>(
           this IBoltRouteHandler bolt,
           IInstanceProvider instanceProvider,
           Action<ConfigureContractContext> configure = null,
           ServerRuntimeConfiguration configuration = null)
        {
            return bolt.Use(typeof(TContract), instanceProvider, configure, configuration);
        }

        public static IContractInvoker Use(
            this IBoltRouteHandler bolt,
            Type contract,
            IInstanceProvider instanceProvider,
            Action<ConfigureContractContext> configure = null,
            ServerRuntimeConfiguration configuration = null)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (bolt == null)
            {
                throw new ArgumentNullException(nameof(bolt));
            }

            if (instanceProvider == null)
            {
                throw new ArgumentNullException(nameof(instanceProvider));
            }

            var factory = bolt.ApplicationServices.GetRequiredService<IContractInvokerFactory>();
            configuration = configuration ?? new ServerRuntimeConfiguration(bolt.Configuration);

            IContractInvoker invoker = factory.Create(BoltFramework.GetContract(contract), instanceProvider, configuration);
            IPipeline<ServerActionContext> pipeline = null;
            if (configure != null)
            {
                ConfigureContractContext ctxt = new ConfigureContractContext(invoker, bolt.ApplicationServices);
                configure.Invoke(ctxt);

                if (ctxt.Middlewares.Any())
                {
                    pipeline = bolt.ApplicationServices.GetRequiredService<IServerPipelineBuilder>().Build(ctxt.Middlewares);
                }
            }

            if (pipeline != null)
            {
                invoker.Pipeline = pipeline;
            }
            else if (invoker.Pipeline == null)
            {
                // build default pipeline
                invoker.Pipeline = bolt.ApplicationServices.GetRequiredService<IServerPipelineBuilder>().Build();
            }
            bolt.Add(invoker);

            return invoker;
        }
    }
}