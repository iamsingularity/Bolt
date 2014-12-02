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
    public partial class TestContractStateFullInvoker : Bolt.Server.ContractInvoker<Bolt.Service.Test.Core.TestContractStateFullDescriptor>
    {
        public override void Init()
        {
            AddAction(Descriptor.Init, TestContractStateFull_Init);
            AddAction(Descriptor.SetState, TestContractStateFull_SetState);
            AddAction(Descriptor.GetState, TestContractStateFull_GetState);
            AddAction(Descriptor.NextCallWillFailProxy, TestContractStateFull_NextCallWillFailProxy);
            AddAction(Descriptor.Destroy, TestContractStateFull_Destroy);

            base.Init();
        }

        protected virtual async Task TestContractStateFull_Init(Bolt.Server.ServerActionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContractStateFull>(context);
            try
            {
                instance.Init();
                await ResponseHandler.Handle(context);
                InstanceProvider.ReleaseInstance(context, instance, null);
            }
            catch (Exception e)
            {
                InstanceProvider.ReleaseInstance(context, instance, e);
                throw;
            }
        }

        protected virtual async Task TestContractStateFull_SetState(Bolt.Server.ServerActionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<SetStateParameters>(context);
            var instance = InstanceProvider.GetInstance<ITestContractStateFull>(context);
            try
            {
                instance.SetState(parameters.State);
                await ResponseHandler.Handle(context);
                InstanceProvider.ReleaseInstance(context, instance, null);
            }
            catch (Exception e)
            {
                InstanceProvider.ReleaseInstance(context, instance, e);
                throw;
            }
        }

        protected virtual async Task TestContractStateFull_GetState(Bolt.Server.ServerActionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContractStateFull>(context);
            try
            {
                var result = instance.GetState();
                await ResponseHandler.Handle(context, result);
                InstanceProvider.ReleaseInstance(context, instance, null);
            }
            catch (Exception e)
            {
                InstanceProvider.ReleaseInstance(context, instance, e);
                throw;
            }
        }

        protected virtual async Task TestContractStateFull_NextCallWillFailProxy(Bolt.Server.ServerActionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContractStateFull>(context);
            try
            {
                instance.NextCallWillFailProxy();
                await ResponseHandler.Handle(context);
                InstanceProvider.ReleaseInstance(context, instance, null);
            }
            catch (Exception e)
            {
                InstanceProvider.ReleaseInstance(context, instance, e);
                throw;
            }
        }

        protected virtual async Task TestContractStateFull_Destroy(Bolt.Server.ServerActionContext context)
        {
            var instance = InstanceProvider.GetInstance<ITestContractStateFull>(context);
            try
            {
                instance.Destroy();
                await ResponseHandler.Handle(context);
                InstanceProvider.ReleaseInstance(context, instance, null);
            }
            catch (Exception e)
            {
                InstanceProvider.ReleaseInstance(context, instance, e);
                throw;
            }
        }
    }
}

namespace Bolt.Server
{
    public static partial class TestContractStateFullInvokerExtensions
    {
        public static IAppBuilder UseTestContractStateFull(this IAppBuilder app, Bolt.Service.Test.Core.ITestContractStateFull instance)
        {
            return app.UseTestContractStateFull(new StaticInstanceProvider(instance));
        }

        public static IAppBuilder UseTestContractStateFull<TImplementation>(this IAppBuilder app) where TImplementation: Bolt.Service.Test.Core.ITestContractStateFull, new()
        {
            return app.UseTestContractStateFull(new InstanceProvider<TImplementation>());
        }

        public static IAppBuilder UseStateFullTestContractStateFull<TImplementation>(this IAppBuilder app, string sessionHeader = null, TimeSpan? sessionTimeout = null) where TImplementation: Bolt.Service.Test.Core.ITestContractStateFull, new()
        {
            var initSessionAction = TestContractStateFullDescriptor.Default.Init;
            var closeSessionAction = TestContractStateFullDescriptor.Default.Destroy;
            return app.UseTestContractStateFull(new StateFullInstanceProvider<TImplementation>(initSessionAction, closeSessionAction, sessionHeader ?? app.GetBolt().Configuration.SessionHeader, sessionTimeout ?? app.GetBolt().Configuration.StateFullInstanceLifetime));
        }

        public static IAppBuilder UseStateFullTestContractStateFull<TImplementation>(this IAppBuilder app, ActionDescriptor initInstanceAction, ActionDescriptor releaseInstanceAction, string sessionHeader = null, TimeSpan? sessionTimeout = null) where TImplementation: Bolt.Service.Test.Core.ITestContractStateFull, new()
        {
            return app.UseTestContractStateFull(new StateFullInstanceProvider<TImplementation>(initInstanceAction, releaseInstanceAction, sessionHeader ?? app.GetBolt().Configuration.SessionHeader, sessionTimeout ?? app.GetBolt().Configuration.StateFullInstanceLifetime));
        }

        public static IAppBuilder UseTestContractStateFull(this IAppBuilder app, IInstanceProvider instanceProvider)
        {
            var boltExecutor = app.GetBolt();
            var invoker = new Bolt.Service.Test.Core.TestContractStateFullInvoker();
            invoker.Init(boltExecutor.Configuration);
            invoker.InstanceProvider = instanceProvider;
            boltExecutor.Add(invoker);

            return app;
        }
    }
}

