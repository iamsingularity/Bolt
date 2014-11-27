//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.

//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bolt.Server;
using Bolt.Service.Test.Core;
using Bolt.Service.Test.Core.Parameters;
using Owin;


namespace Bolt.Service.Test.Core
{
    public partial class TestContractInvoker : Bolt.Server.ContractInvoker<Bolt.Service.Test.Core.TestContractDescriptor>
    {
        public override void Init()
        {
            if (Descriptor == null)
            {
                Descriptor = Bolt.Service.Test.Core.TestContractDescriptor.Default;
            }

            AddAction(Descriptor.SimpleMethodWithSimpleArguments, TestContract_SimpleMethodWithSimpleArguments);
            AddAction(Descriptor.SimpleMethod, TestContract_SimpleMethod);
            AddAction(Descriptor.SimpleMethodExAsync, TestContract_SimpleMethodExAsync);
            AddAction(Descriptor.SimpleMethodWithCancellation, TestContract_SimpleMethodWithCancellation);
            AddAction(Descriptor.ComplexFunction, TestContract_ComplexFunction);
            AddAction(Descriptor.SimpleMethodWithComplexParameter, TestContractInner_SimpleMethodWithComplexParameter);
            AddAction(Descriptor.SimpleFunction, TestContractInner_SimpleFunction);
            AddAction(Descriptor.SimpleAsyncFunction, TestContractInner_SimpleAsyncFunction);
            AddAction(Descriptor.MethodWithManyArguments, TestContractInner_MethodWithManyArguments);
            AddAction(Descriptor.ThisMethodShouldBeExcluded, ExcludedContract_ThisMethodShouldBeExcluded);

            base.Init();
        }

        public virtual Bolt.Service.Test.Core.TestContractDescriptor ContractDescriptor { get; set; }

        protected virtual async Task TestContract_SimpleMethodWithSimpleArguments(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<SimpleMethodWithSimpleArgumentsParameters>(context);
            var instance = InstanceProvider.GetInstance<ITestContract>(context);
            instance.SimpleMethodWithSimpleArguments(parameters.Val);
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task TestContract_SimpleMethod(Bolt.Server.ServerExecutionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContract>(context);
            instance.SimpleMethod();
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task TestContract_SimpleMethodExAsync(Bolt.Server.ServerExecutionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContract>(context);
            await instance.SimpleMethodExAsync();
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task TestContract_SimpleMethodWithCancellation(Bolt.Server.ServerExecutionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContract>(context);
            instance.SimpleMethodWithCancellation(context.CallCancelled);
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task TestContract_ComplexFunction(Bolt.Server.ServerExecutionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContract>(context);
            var result = instance.ComplexFunction();
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task TestContractInner_SimpleMethodWithComplexParameter(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<SimpleMethodWithComplexParameterParameters>(context);
            var instance = InstanceProvider.GetInstance<ITestContractInner>(context);
            instance.SimpleMethodWithComplexParameter(parameters.CompositeType);
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task TestContractInner_SimpleFunction(Bolt.Server.ServerExecutionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContractInner>(context);
            var result = instance.SimpleFunction();
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task TestContractInner_SimpleAsyncFunction(Bolt.Server.ServerExecutionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContractInner>(context);
            var result = await instance.SimpleAsyncFunction();
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task TestContractInner_MethodWithManyArguments(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<MethodWithManyArgumentsParameters>(context);
            var instance = InstanceProvider.GetInstance<ITestContractInner>(context);
            instance.MethodWithManyArguments(parameters.Arg1, parameters.Arg2, parameters.Time);
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task ExcludedContract_ThisMethodShouldBeExcluded(Bolt.Server.ServerExecutionContext context)
        {
            var instance = InstanceProvider.GetInstance<IExcludedContract>(context);
            instance.ThisMethodShouldBeExcluded();
            await ResponseHandler.Handle(context);
        }
    }
}

namespace Bolt.Server
{
    public static partial class TestContractInvokerExtensions
    {
        public static IAppBuilder UseTestContract(this IAppBuilder app, Bolt.Service.Test.Core.ITestContract instance)
        {
            return app.UseTestContract(new StaticInstanceProvider(instance));
        }

        public static IAppBuilder UseTestContract<TImplementation>(this IAppBuilder app) where TImplementation: Bolt.Service.Test.Core.ITestContract, new()
        {
            return app.UseTestContract(new InstanceProvider<TImplementation>());
        }

        public static IAppBuilder UseStateFullTestContract<TImplementation>(this IAppBuilder app, string sessionHeader = null, TimeSpan? sessionTimeout = null) where TImplementation: Bolt.Service.Test.Core.ITestContract, new()
        {
            return app.UseTestContract(new StateFullInstanceProvider<TImplementation>(sessionHeader ?? app.GetBolt().Configuration.SessionHeader, sessionTimeout ?? app.GetBolt().Configuration.StateFullInstanceLifetime));
        }

        public static IAppBuilder UseTestContract(this IAppBuilder app, IInstanceProvider instanceProvider)
        {
            var boltExecutor = app.GetBolt();
            var invoker = new Bolt.Service.Test.Core.TestContractInvoker();
            invoker.Descriptor = Bolt.Service.Test.Core.TestContractDescriptor.Default;
            invoker.Init(boltExecutor.Configuration);
            invoker.InstanceProvider = instanceProvider;
            boltExecutor.Add(invoker);

            return app;
        }

    }
}

