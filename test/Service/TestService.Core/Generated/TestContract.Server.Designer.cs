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
using TestService.Core;
using TestService.Core.Parameters;


namespace TestService.Core
{
    public partial class TestContractActions : Bolt.Server.ContractActions<TestService.Core.TestContractDescriptor>
    {
        public TestContractActions()
        {
            Add(Descriptor.UpdatePerson, TestContract_UpdatePerson);
            Add(Descriptor.UpdatePersonThatThrowsInvalidOperationException, TestContract_UpdatePersonThatThrowsInvalidOperationException);
            Add(Descriptor.DoNothingAsAsync, TestContract_DoNothingAsAsync);
            Add(Descriptor.DoNothing, TestContract_DoNothing);
            Add(Descriptor.DoNothingWithComplexParameterAsAsync, TestContract_DoNothingWithComplexParameterAsAsync);
            Add(Descriptor.DoNothingWithComplexParameter, TestContract_DoNothingWithComplexParameter);
            Add(Descriptor.GetSimpleType, TestContract_GetSimpleType);
            Add(Descriptor.GetSimpleTypeAsAsync, TestContract_GetSimpleTypeAsAsync);
            Add(Descriptor.GetSinglePerson, TestContract_GetSinglePerson);
            Add(Descriptor.GetSinglePersonAsAsync, TestContract_GetSinglePersonAsAsync);
            Add(Descriptor.GetManyPersons, TestContract_GetManyPersons);
            Add(Descriptor.GetManyPersonsAsAsync, TestContract_GetManyPersonsAsAsync);
            Add(Descriptor.Throws, TestContract_Throws);
            Add(Descriptor.ThrowsCustom, TestContract_ThrowsCustom);
            Add(Descriptor.InnerOperation, InnerTestContract_InnerOperation);
            Add(Descriptor.InnerOperation3, InnerTestContract_InnerOperation3);
            Add(Descriptor.InnerOperationExAsync, InnerTestContract_InnerOperationExAsync);
            Add(Descriptor.InnerOperation2, InnerTestContract2_InnerOperation2);
            Add(Descriptor.InnerOperationExAsync2, InnerTestContract2_InnerOperationExAsync2);
        }

        protected virtual Task TestContract_UpdatePerson(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.UpdatePersonParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            context.Result = instance.UpdatePerson(parameters.Person, context.RequestAborted);
            return Task.FromResult(true);
        }

        protected virtual Task TestContract_UpdatePersonThatThrowsInvalidOperationException(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.UpdatePersonThatThrowsInvalidOperationExceptionParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            context.Result = instance.UpdatePersonThatThrowsInvalidOperationException(parameters.Person);
            return Task.FromResult(true);
        }

        protected virtual async Task TestContract_DoNothingAsAsync(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<ITestContract>();
            await instance.DoNothingAsAsync();
        }

        protected virtual Task TestContract_DoNothing(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<ITestContract>();
            instance.DoNothing();
            return Task.FromResult(true);
        }

        protected virtual async Task TestContract_DoNothingWithComplexParameterAsAsync(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.DoNothingWithComplexParameterAsAsyncParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            await instance.DoNothingWithComplexParameterAsAsync(parameters.Person);
        }

        protected virtual Task TestContract_DoNothingWithComplexParameter(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.DoNothingWithComplexParameterParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            instance.DoNothingWithComplexParameter(parameters.Person);
            return Task.FromResult(true);
        }

        protected virtual Task TestContract_GetSimpleType(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.GetSimpleTypeParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            context.Result = instance.GetSimpleType(parameters.Arg);
            return Task.FromResult(true);
        }

        protected virtual async Task TestContract_GetSimpleTypeAsAsync(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.GetSimpleTypeAsAsyncParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            await instance.GetSimpleTypeAsAsync(parameters.Arg);
        }

        protected virtual Task TestContract_GetSinglePerson(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.GetSinglePersonParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            context.Result = instance.GetSinglePerson(parameters.Person);
            return Task.FromResult(true);
        }

        protected virtual async Task TestContract_GetSinglePersonAsAsync(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.GetSinglePersonAsAsyncParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            context.Result = await instance.GetSinglePersonAsAsync(parameters.Person);
        }

        protected virtual Task TestContract_GetManyPersons(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<ITestContract>();
            context.Result = instance.GetManyPersons();
            return Task.FromResult(true);
        }

        protected virtual async Task TestContract_GetManyPersonsAsAsync(ServerActionContext context)
        {
            var parameters = context.GetRequiredParameters<TestService.Core.Parameters.GetManyPersonsAsAsyncParameters>();
            var instance = context.GetRequiredInstance<ITestContract>();
            context.Result = await instance.GetManyPersonsAsAsync(parameters.Person);
        }

        protected virtual Task TestContract_Throws(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<ITestContract>();
            instance.Throws();
            return Task.FromResult(true);
        }

        protected virtual Task TestContract_ThrowsCustom(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<ITestContract>();
            instance.ThrowsCustom();
            return Task.FromResult(true);
        }

        protected virtual Task InnerTestContract_InnerOperation(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<IInnerTestContract>();
            instance.InnerOperation();
            return Task.FromResult(true);
        }

        protected virtual async Task InnerTestContract_InnerOperation3(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<IInnerTestContract>();
            context.Result = await instance.InnerOperation3();
        }

        protected virtual async Task InnerTestContract_InnerOperationExAsync(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<IInnerTestContract>();
            await instance.InnerOperationExAsync();
        }

        protected virtual Task InnerTestContract2_InnerOperation2(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<IInnerTestContract2>();
            instance.InnerOperation2();
            return Task.FromResult(true);
        }

        protected virtual async Task InnerTestContract2_InnerOperationExAsync2(ServerActionContext context)
        {
            var instance = context.GetRequiredInstance<IInnerTestContract2>();
            await instance.InnerOperationExAsync2();
        }
    }
}