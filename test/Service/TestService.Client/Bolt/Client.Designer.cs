﻿

using Bolt;
using Bolt.Client;
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
    public partial class PersonRepository : StatefullChannel, Bolt.Client.IChannel, TestService.Core.IPersonRepository
    {
        public TestService.Core.PersonRepositoryDescriptor ContractDescriptor { get; set; }

        public Person UpdatePerson(Person person)
        {
            var request = new UpdatePersonParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.UpdatePerson;
            var token = GetCancellationToken(descriptor);

            return Send<Person, UpdatePersonParameters>(request, descriptor, token);
        }

        public Task<Person> UpdatePersonAsync(Person person)
        {
            var request = new UpdatePersonParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.UpdatePerson;
            var token = GetCancellationToken(descriptor);

            return SendAsync<Person, UpdatePersonParameters>(request, descriptor, token);
        }

        public Person UpdatePersonThatThrowsInvalidOperationException(Person person)
        {
            var request = new UpdatePersonThatThrowsInvalidOperationExceptionParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.UpdatePersonThatThrowsInvalidOperationException;
            var token = GetCancellationToken(descriptor);

            return Send<Person, UpdatePersonThatThrowsInvalidOperationExceptionParameters>(request, descriptor, token);
        }

        public Task DoNothingAsAsync()
        {
            var descriptor = ContractDescriptor.DoNothingAsAsync;
            var token = GetCancellationToken(descriptor);

            return SendAsync(Empty.Instance, descriptor, token);
        }

        public void DoNothing()
        {
            var descriptor = ContractDescriptor.DoNothing;
            var token = GetCancellationToken(descriptor);

            Send(Empty.Instance, descriptor, token);
        }

        public Task DoNothingWithComplexParameterAsAsync(List<Person> person)
        {
            var request = new DoNothingWithComplexParameterAsAsyncParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.DoNothingWithComplexParameterAsAsync;
            var token = GetCancellationToken(descriptor);

            return SendAsync(request, descriptor, token);
        }

        public void DoNothingWithComplexParameter(List<Person> person)
        {
            var request = new DoNothingWithComplexParameterParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.DoNothingWithComplexParameter;
            var token = GetCancellationToken(descriptor);

            Send(request, descriptor, token);
        }

        public int GetSimpleType(int arg)
        {
            var request = new GetSimpleTypeParameters();
            request.Arg = arg;
            var descriptor = ContractDescriptor.GetSimpleType;
            var token = GetCancellationToken(descriptor);

            return Send<int, GetSimpleTypeParameters>(request, descriptor, token);
        }

        public Task GetSimpleTypeAsAsync(int arg)
        {
            var request = new GetSimpleTypeAsAsyncParameters();
            request.Arg = arg;
            var descriptor = ContractDescriptor.GetSimpleTypeAsAsync;
            var token = GetCancellationToken(descriptor);

            return SendAsync(request, descriptor, token);
        }

        public Person GetSinglePerson(Person person)
        {
            var request = new GetSinglePersonParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.GetSinglePerson;
            var token = GetCancellationToken(descriptor);

            return Send<Person, GetSinglePersonParameters>(request, descriptor, token);
        }

        public Task<Person> GetSinglePersonAsAsync(Person person)
        {
            var request = new GetSinglePersonAsAsyncParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.GetSinglePersonAsAsync;
            var token = GetCancellationToken(descriptor);

            return SendAsync<Person, GetSinglePersonAsAsyncParameters>(request, descriptor, token);
        }

        public List<Person> GetManyPersons(Person person)
        {
            var request = new GetManyPersonsParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.GetManyPersons;
            var token = GetCancellationToken(descriptor);

            return Send<List<Person>, GetManyPersonsParameters>(request, descriptor, token);
        }

        public Task<List<Person>> GetManyPersonsAsAsync(Person person)
        {
            var request = new GetManyPersonsAsAsyncParameters();
            request.Person = person;
            var descriptor = ContractDescriptor.GetManyPersonsAsAsync;
            var token = GetCancellationToken(descriptor);

            return SendAsync<List<Person>, GetManyPersonsAsAsyncParameters>(request, descriptor, token);
        }
        public void InnerOperation()
        {
            var descriptor = ContractDescriptor.InnerOperation;
            var token = GetCancellationToken(descriptor);

            Send(Empty.Instance, descriptor, token);
        }

        public Task InnerOperationExAsync()
        {
            var descriptor = ContractDescriptor.InnerOperationExAsync;
            var token = GetCancellationToken(descriptor);

            return SendAsync(Empty.Instance, descriptor, token);
        }
    }
}

