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

using TestService.Core;
using TestService.Core.Parameters;


namespace TestService.Core
{
    public partial interface IInnerTestContractAsync : IInnerTestContract
    {
        Task InnerOperationAsync();
    }
}

namespace TestService.Core
{
    public partial interface IInnerTestContract2Async : IInnerTestContract2
    {
        Task InnerOperation2Async();
    }
}

namespace TestService.Core
{
    public partial interface ITestContractAsync : ITestContract, IInnerTestContractAsync, IInnerTestContract2Async
    {
        Task<Person> UpdatePersonAsync(Person person, System.Threading.CancellationToken cancellation);

        Task<Person> UpdatePersonThatThrowsInvalidOperationExceptionAsync(Person person);

        Task DoNothingAsync();

        Task DoNothingWithComplexParameterAsync(List<Person> person);

        Task<int> GetSimpleTypeAsync(int arg);

        Task<Person> GetSinglePersonAsync(Person person);

        Task<List<Person>> GetManyPersonsAsync();

        Task ThrowsAsync();

        Task ThrowsCustomAsync();
    }
}

namespace TestService.Core
{
    public partial class TestContractProxy : Bolt.Client.Channels.ContractProxy<TestService.Core.TestContractDescriptor>, TestService.Core.ITestContract, IInnerTestContractAsync, IInnerTestContract2Async, ITestContractAsync
    {
        public TestContractProxy(TestService.Core.TestContractProxy proxy) : base(proxy)
        {
        }

        public TestContractProxy(Bolt.Client.IChannel channel) : base(channel)
        {
        }

        public virtual Person UpdatePerson(Person person, System.Threading.CancellationToken cancellation)
        {
            var bolt_Params = new UpdatePersonParameters();
            bolt_Params.Person = person;
            return Channel.Send<Person, UpdatePersonParameters>(bolt_Params, Descriptor.UpdatePerson, cancellation);
        }

        public virtual Task<Person> UpdatePersonAsync(Person person, System.Threading.CancellationToken cancellation)
        {
            var bolt_Params = new UpdatePersonParameters();
            bolt_Params.Person = person;
            return Channel.SendAsync<Person, UpdatePersonParameters>(bolt_Params, Descriptor.UpdatePerson, cancellation);
        }

        public virtual Person UpdatePersonThatThrowsInvalidOperationException(Person person)
        {
            var bolt_Params = new UpdatePersonThatThrowsInvalidOperationExceptionParameters();
            bolt_Params.Person = person;
            return Channel.Send<Person, UpdatePersonThatThrowsInvalidOperationExceptionParameters>(bolt_Params, Descriptor.UpdatePersonThatThrowsInvalidOperationException, GetCancellationToken(Descriptor.UpdatePersonThatThrowsInvalidOperationException));
        }

        public virtual Task<Person> UpdatePersonThatThrowsInvalidOperationExceptionAsync(Person person)
        {
            var bolt_Params = new UpdatePersonThatThrowsInvalidOperationExceptionParameters();
            bolt_Params.Person = person;
            return Channel.SendAsync<Person, UpdatePersonThatThrowsInvalidOperationExceptionParameters>(bolt_Params, Descriptor.UpdatePersonThatThrowsInvalidOperationException, GetCancellationToken(Descriptor.UpdatePersonThatThrowsInvalidOperationException));
        }

        public virtual Task DoNothingAsAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.DoNothingAsAsync, GetCancellationToken(Descriptor.DoNothingAsAsync));
        }

        public virtual void DoNothing()
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.DoNothing, GetCancellationToken(Descriptor.DoNothing));
        }

        public virtual Task DoNothingAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.DoNothing, GetCancellationToken(Descriptor.DoNothing));
        }

        public virtual Task DoNothingWithComplexParameterAsAsync(List<Person> person)
        {
            var bolt_Params = new DoNothingWithComplexParameterAsAsyncParameters();
            bolt_Params.Person = person;
            return Channel.SendAsync(bolt_Params, Descriptor.DoNothingWithComplexParameterAsAsync, GetCancellationToken(Descriptor.DoNothingWithComplexParameterAsAsync));
        }

        public virtual void DoNothingWithComplexParameter(List<Person> person)
        {
            var bolt_Params = new DoNothingWithComplexParameterParameters();
            bolt_Params.Person = person;
            Channel.Send(bolt_Params, Descriptor.DoNothingWithComplexParameter, GetCancellationToken(Descriptor.DoNothingWithComplexParameter));
        }

        public virtual Task DoNothingWithComplexParameterAsync(List<Person> person)
        {
            var bolt_Params = new DoNothingWithComplexParameterParameters();
            bolt_Params.Person = person;
            return Channel.SendAsync(bolt_Params, Descriptor.DoNothingWithComplexParameter, GetCancellationToken(Descriptor.DoNothingWithComplexParameter));
        }

        public virtual int GetSimpleType(int arg)
        {
            var bolt_Params = new GetSimpleTypeParameters();
            bolt_Params.Arg = arg;
            return Channel.Send<int, GetSimpleTypeParameters>(bolt_Params, Descriptor.GetSimpleType, GetCancellationToken(Descriptor.GetSimpleType));
        }

        public virtual Task<int> GetSimpleTypeAsync(int arg)
        {
            var bolt_Params = new GetSimpleTypeParameters();
            bolt_Params.Arg = arg;
            return Channel.SendAsync<int, GetSimpleTypeParameters>(bolt_Params, Descriptor.GetSimpleType, GetCancellationToken(Descriptor.GetSimpleType));
        }

        public virtual Task GetSimpleTypeAsAsync(int arg)
        {
            var bolt_Params = new GetSimpleTypeAsAsyncParameters();
            bolt_Params.Arg = arg;
            return Channel.SendAsync(bolt_Params, Descriptor.GetSimpleTypeAsAsync, GetCancellationToken(Descriptor.GetSimpleTypeAsAsync));
        }

        public virtual Person GetSinglePerson(Person person)
        {
            var bolt_Params = new GetSinglePersonParameters();
            bolt_Params.Person = person;
            return Channel.Send<Person, GetSinglePersonParameters>(bolt_Params, Descriptor.GetSinglePerson, GetCancellationToken(Descriptor.GetSinglePerson));
        }

        public virtual Task<Person> GetSinglePersonAsync(Person person)
        {
            var bolt_Params = new GetSinglePersonParameters();
            bolt_Params.Person = person;
            return Channel.SendAsync<Person, GetSinglePersonParameters>(bolt_Params, Descriptor.GetSinglePerson, GetCancellationToken(Descriptor.GetSinglePerson));
        }

        public virtual Task<Person> GetSinglePersonAsAsync(Person person)
        {
            var bolt_Params = new GetSinglePersonAsAsyncParameters();
            bolt_Params.Person = person;
            return Channel.SendAsync<Person, GetSinglePersonAsAsyncParameters>(bolt_Params, Descriptor.GetSinglePersonAsAsync, GetCancellationToken(Descriptor.GetSinglePersonAsAsync));
        }

        public virtual List<Person> GetManyPersons()
        {
            return Channel.Send<List<Person>, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.GetManyPersons, GetCancellationToken(Descriptor.GetManyPersons));
        }

        public virtual Task<List<Person>> GetManyPersonsAsync()
        {
            return Channel.SendAsync<List<Person>, Bolt.Empty>(Bolt.Empty.Instance, Descriptor.GetManyPersons, GetCancellationToken(Descriptor.GetManyPersons));
        }

        public virtual Task<List<Person>> GetManyPersonsAsAsync(Person person)
        {
            var bolt_Params = new GetManyPersonsAsAsyncParameters();
            bolt_Params.Person = person;
            return Channel.SendAsync<List<Person>, GetManyPersonsAsAsyncParameters>(bolt_Params, Descriptor.GetManyPersonsAsAsync, GetCancellationToken(Descriptor.GetManyPersonsAsAsync));
        }

        public virtual void Throws()
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.Throws, GetCancellationToken(Descriptor.Throws));
        }

        public virtual Task ThrowsAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.Throws, GetCancellationToken(Descriptor.Throws));
        }

        public virtual void ThrowsCustom()
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.ThrowsCustom, GetCancellationToken(Descriptor.ThrowsCustom));
        }

        public virtual Task ThrowsCustomAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.ThrowsCustom, GetCancellationToken(Descriptor.ThrowsCustom));
        }

        public virtual void InnerOperation()
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.InnerOperation, GetCancellationToken(Descriptor.InnerOperation));
        }

        public virtual Task InnerOperationAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.InnerOperation, GetCancellationToken(Descriptor.InnerOperation));
        }

        public virtual Task InnerOperationExAsync()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.InnerOperationExAsync, GetCancellationToken(Descriptor.InnerOperationExAsync));
        }
        public virtual void InnerOperation2()
        {
            Channel.Send(Bolt.Empty.Instance, Descriptor.InnerOperation2, GetCancellationToken(Descriptor.InnerOperation2));
        }

        public virtual Task InnerOperation2Async()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.InnerOperation2, GetCancellationToken(Descriptor.InnerOperation2));
        }

        public virtual Task InnerOperationExAsync2()
        {
            return Channel.SendAsync(Bolt.Empty.Instance, Descriptor.InnerOperationExAsync2, GetCancellationToken(Descriptor.InnerOperationExAsync2));
        }
    }
}

