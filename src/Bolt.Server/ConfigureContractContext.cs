using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolt.Pipeline;
using Microsoft.Framework.DependencyInjection;

namespace Bolt.Server
{
    public class ConfigureContractContext
    {
        private readonly List<IMiddleware<ServerActionContext>> _middlewares = new List<IMiddleware<ServerActionContext>>();
        private readonly IServiceProvider _applicationServices;

        public ConfigureContractContext(IContractInvoker contractInvoker, IServiceProvider applicationServices)
        {
            if (contractInvoker == null) throw new ArgumentNullException(nameof(contractInvoker));
            if (applicationServices == null) throw new ArgumentNullException(nameof(applicationServices));

            ContractInvoker = contractInvoker;
            _applicationServices = applicationServices;
        }

        public IContractInvoker ContractInvoker { get; }

        public ConfigureContractContext Use(Func<ActionDelegate<ServerActionContext>, ServerActionContext, Task> handler) 
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _middlewares.Add(new DelegatedMiddleware<ServerActionContext>(handler));
            return this;
        }

        public ConfigureContractContext Use<T>() where T : IMiddleware<ServerActionContext>
        {
            _middlewares.Add(ActivatorUtilities.GetServiceOrCreateInstance<T>(_applicationServices));
            return this;
        }

        public IPipeline<ServerActionContext> TryBuild()
        {
            if (!_middlewares.Any())
            {
                return null;
            }

            PipelineBuilder<ServerActionContext> builder = new PipelineBuilder<ServerActionContext>();
            foreach (IMiddleware<ServerActionContext> middleware in _middlewares)
            {
                builder.Use(middleware);
            }

            return builder.Build();
        } 
    }
}