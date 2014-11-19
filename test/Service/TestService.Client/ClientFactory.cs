﻿using Bolt.Client;
using System;
using System.ServiceModel;
using TestService.Core;

namespace TestService.Client
{
    public class ClientFactory
    {
        public static IPersonRepository CreateIISBolt()
        {
            PersonRepository repository = new PersonRepository();
            repository.ServerUrl = Servers.IISBoltServer;
            repository.Prefix = Servers.Prefix;
            repository.Contract = Contracts.PersonRepository;
            repository.ContractDescriptor = new PersonRepositoryDescriptor();
            repository.Retries = 10;
            repository.RetryDelay = TimeSpan.FromSeconds(2);

            new ClientConfiguration().Update(repository);
            return repository;
        }

        public static IPersonRepository CreateBolt()
        {
            PersonRepository repository = new PersonRepository();
            repository.ServerUrl = Servers.BoltServer;
            repository.Prefix = Servers.Prefix;
            repository.Contract = Contracts.PersonRepository;
            repository.ContractDescriptor = new PersonRepositoryDescriptor();
            repository.Retries = 10;
            repository.RetryDelay = TimeSpan.FromSeconds(2);

            new ClientConfiguration().Update(repository);
            return repository;
        }

        public static IPersonRepository CreateWcf()
        {
            ChannelFactory<IPersonRepository> respository = new ChannelFactory<IPersonRepository>(new BasicHttpBinding());
            IPersonRepository channel = respository.CreateChannel(new EndpointAddress(Servers.WcfServer));
            return channel;
        }

        public static IPersonRepository CreateIISWcf()
        {
            ChannelFactory<IPersonRepository> respository = new ChannelFactory<IPersonRepository>(new BasicHttpBinding());
            IPersonRepository channel = respository.CreateChannel(new EndpointAddress(Servers.IISWcfServer));
            return channel;
        }
    }
}