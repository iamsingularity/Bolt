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

using Bolt.Service.Test.Core;
using Bolt.Service.Test.Core.Parameters;


namespace Bolt.Service.Test.Core
{
    public partial interface ITestContractInnerAsync : ITestContractInner
    {
        Task SimpleMethodWithComplexParameterAsync(CompositeType compositeType);

        Task MethodWithNotSerializableTypeAsync(NotSerializableType arg);

        Task<NotSerializableType> FunctionWithNotSerializableTypeAsync();
    }
}

namespace Bolt.Service.Test.Core
{
    public partial interface ITestContractAsync : ITestContract, ITestContractInnerAsync
    {
        Task SimpleMethodAsync();
    }
}

namespace Bolt.Service.Test.Core
{
    public partial class TestContractProxy : Bolt.Client.Channels.ContractProxy<Bolt.Service.Test.Core.TestContractDescriptor>, Bolt.Service.Test.Core.ITestContract, ITestContractInnerAsync, ITestContractAsync
    {
        public TestContractProxy(Bolt.Service.Test.Core.TestContractProxy proxy) : base(proxy)
        {
        }

        public TestContractProxy(Bolt.Client.IChannel channel) : base(channel)
        {
        }

        public virtual void SimpleMethodWithSimpleArguments(int val)
        {
            var request = new SimpleMethodWithSimpleArgumentsParameters();
            request.Val = val;
            Channel.Send(request, Descriptor.SimpleMethodWithSimpleArguments, GetCancellationToken(Descriptor.SimpleMethodWithSimpleArguments));
        }

        public virtual void SimpleMethod()
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.SimpleMethod, GetCancellationToken(Descriptor.SimpleMethod));
        }

        public virtual Task SimpleMethodAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.SimpleMethod, GetCancellationToken(Descriptor.SimpleMethod));
        }

        public virtual Task SimpleMethodExAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.SimpleMethodExAsync, GetCancellationToken(Descriptor.SimpleMethodExAsync));
        }

        public virtual void SimpleMethodWithCancellation(System.Threading.CancellationToken cancellation)
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.SimpleMethodWithCancellation, cancellation);
        }

        public virtual CompositeType ComplexFunction()
        {
            return Channel.Send<CompositeType, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.ComplexFunction, GetCancellationToken(Descriptor.ComplexFunction));
        }
        public virtual void SimpleMethodWithComplexParameter(CompositeType compositeType)
        {
            var request = new SimpleMethodWithComplexParameterParameters();
            request.CompositeType = compositeType;
            Channel.Send(request, Descriptor.SimpleMethodWithComplexParameter, GetCancellationToken(Descriptor.SimpleMethodWithComplexParameter));
        }

        public virtual Task SimpleMethodWithComplexParameterAsync(CompositeType compositeType)
        {
            var request = new SimpleMethodWithComplexParameterParameters();
            request.CompositeType = compositeType;
            return Channel.SendAsync(request, Descriptor.SimpleMethodWithComplexParameter, GetCancellationToken(Descriptor.SimpleMethodWithComplexParameter));
        }

        public virtual int SimpleFunction()
        {
            return Channel.Send<int, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.SimpleFunction, GetCancellationToken(Descriptor.SimpleFunction));
        }

        public virtual List<CompositeType> FunctionReturningHugeData()
        {
            return Channel.Send<List<CompositeType>, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.FunctionReturningHugeData, GetCancellationToken(Descriptor.FunctionReturningHugeData));
        }

        public virtual void MethodWithNotSerializableType(NotSerializableType arg)
        {
            var request = new MethodWithNotSerializableTypeParameters();
            request.Arg = arg;
            Channel.Send(request, Descriptor.MethodWithNotSerializableType, GetCancellationToken(Descriptor.MethodWithNotSerializableType));
        }

        public virtual Task MethodWithNotSerializableTypeAsync(NotSerializableType arg)
        {
            var request = new MethodWithNotSerializableTypeParameters();
            request.Arg = arg;
            return Channel.SendAsync(request, Descriptor.MethodWithNotSerializableType, GetCancellationToken(Descriptor.MethodWithNotSerializableType));
        }

        public virtual NotSerializableType FunctionWithNotSerializableType()
        {
            return Channel.Send<NotSerializableType, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.FunctionWithNotSerializableType, GetCancellationToken(Descriptor.FunctionWithNotSerializableType));
        }

        public virtual Task<NotSerializableType> FunctionWithNotSerializableTypeAsync()
        {
            return Channel.SendAsync<NotSerializableType, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.FunctionWithNotSerializableType, GetCancellationToken(Descriptor.FunctionWithNotSerializableType));
        }

        public virtual Task<int> SimpleAsyncFunction()
        {
            return Channel.SendAsync<int, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.SimpleAsyncFunction, GetCancellationToken(Descriptor.SimpleAsyncFunction));
        }

        public virtual void MethodWithManyArguments(CompositeType arg1, CompositeType arg2, DateTime time)
        {
            var request = new MethodWithManyArgumentsParameters();
            request.Arg1 = arg1;
            request.Arg2 = arg2;
            request.Time = time;
            Channel.Send(request, Descriptor.MethodWithManyArguments, GetCancellationToken(Descriptor.MethodWithManyArguments));
        }
        public virtual void ThisMethodShouldBeExcluded()
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.ThisMethodShouldBeExcluded, GetCancellationToken(Descriptor.ThisMethodShouldBeExcluded));
        }
    }
}


