﻿
using Bolt;
using Bolt.Server;
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
    public partial class PersonRepositoryExecutor : Bolt.Server.Executor
    {
        public override void Init()
        {
            if (ContractDescriptor == null)
            {
                ContractDescriptor = TestService.Core.PersonRepositoryDescriptor.Default;
            }

            AddAction(ContractDescriptor.UpdatePerson, PersonRepository_UpdatePerson);
            AddAction(ContractDescriptor.UpdatePersonThatThrowsInvalidOperationException, PersonRepository_UpdatePersonThatThrowsInvalidOperationException);
            AddAction(ContractDescriptor.DoNothingAsAsync, PersonRepository_DoNothingAsAsync);
            AddAction(ContractDescriptor.DoNothing, PersonRepository_DoNothing);
            AddAction(ContractDescriptor.DoNothingWithComplexParameterAsAsync, PersonRepository_DoNothingWithComplexParameterAsAsync);
            AddAction(ContractDescriptor.DoNothingWithComplexParameter, PersonRepository_DoNothingWithComplexParameter);
            AddAction(ContractDescriptor.GetSimpleType, PersonRepository_GetSimpleType);
            AddAction(ContractDescriptor.GetSimpleTypeAsAsync, PersonRepository_GetSimpleTypeAsAsync);
            AddAction(ContractDescriptor.GetSinglePerson, PersonRepository_GetSinglePerson);
            AddAction(ContractDescriptor.GetSinglePersonAsAsync, PersonRepository_GetSinglePersonAsAsync);
            AddAction(ContractDescriptor.GetManyPersons, PersonRepository_GetManyPersons);
            AddAction(ContractDescriptor.GetManyPersonsAsAsync, PersonRepository_GetManyPersonsAsAsync);
            AddAction(ContractDescriptor.InnerOperation, PersonRepositoryInner_InnerOperation);
            AddAction(ContractDescriptor.InnerOperationExAsync, PersonRepositoryInner_InnerOperationExAsync);
            AddAction(ContractDescriptor.InnerOperation2, PersonRepositoryInner2_InnerOperation2);
            AddAction(ContractDescriptor.InnerOperationExAsync2, PersonRepositoryInner2_InnerOperationExAsync2);

            base.Init();
        }

        public TestService.Core.PersonRepositoryDescriptor ContractDescriptor { get; set; }

        protected virtual async Task PersonRepository_UpdatePerson(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<UpdatePersonParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            var result = instance.UpdatePerson(parameters.Person);
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task PersonRepository_UpdatePersonThatThrowsInvalidOperationException(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<UpdatePersonThatThrowsInvalidOperationExceptionParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            var result = instance.UpdatePersonThatThrowsInvalidOperationException(parameters.Person);
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task PersonRepository_DoNothingAsAsync(Bolt.Server.ServerExecutionContext context)
        {
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            await instance.DoNothingAsAsync();
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepository_DoNothing(Bolt.Server.ServerExecutionContext context)
        {
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            instance.DoNothing();
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepository_DoNothingWithComplexParameterAsAsync(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<DoNothingWithComplexParameterAsAsyncParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            await instance.DoNothingWithComplexParameterAsAsync(parameters.Person);
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepository_DoNothingWithComplexParameter(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<DoNothingWithComplexParameterParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            instance.DoNothingWithComplexParameter(parameters.Person);
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepository_GetSimpleType(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<GetSimpleTypeParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            var result = instance.GetSimpleType(parameters.Arg);
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task PersonRepository_GetSimpleTypeAsAsync(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<GetSimpleTypeAsAsyncParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            await instance.GetSimpleTypeAsAsync(parameters.Arg);
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepository_GetSinglePerson(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<GetSinglePersonParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            var result = instance.GetSinglePerson(parameters.Person);
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task PersonRepository_GetSinglePersonAsAsync(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<GetSinglePersonAsAsyncParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            var result = await instance.GetSinglePersonAsAsync(parameters.Person);
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task PersonRepository_GetManyPersons(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<GetManyPersonsParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            var result = instance.GetManyPersons(parameters.Person);
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task PersonRepository_GetManyPersonsAsAsync(Bolt.Server.ServerExecutionContext context)
        {
            var parameters = await DataHandler.ReadParametersAsync<GetManyPersonsAsAsyncParameters>(context);
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepository>(context);
            var result = await instance.GetManyPersonsAsAsync(parameters.Person);
            await ResponseHandler.Handle(context, result);
        }

        protected virtual async Task PersonRepositoryInner_InnerOperation(Bolt.Server.ServerExecutionContext context)
        {
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepositoryInner>(context);
            instance.InnerOperation();
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepositoryInner_InnerOperationExAsync(Bolt.Server.ServerExecutionContext context)
        {
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepositoryInner>(context);
            await instance.InnerOperationExAsync();
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepositoryInner2_InnerOperation2(Bolt.Server.ServerExecutionContext context)
        {
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepositoryInner2>(context);
            instance.InnerOperation2();
            await ResponseHandler.Handle(context);
        }

        protected virtual async Task PersonRepositoryInner2_InnerOperationExAsync2(Bolt.Server.ServerExecutionContext context)
        {
            var instance = await InstanceProvider.GetInstanceAsync<IPersonRepositoryInner2>(context);
            await instance.InnerOperationExAsync2();
            await ResponseHandler.Handle(context);
        }
    }
}
