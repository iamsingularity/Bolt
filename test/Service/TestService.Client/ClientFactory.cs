﻿using Bolt;
using Bolt.Client;

using System;
using System.Runtime.Serialization;
using System.ServiceModel;

using Bolt.Helpers;

using TestService.Core;

namespace TestService.Client
{
    public class ClientFactory
    {
        public static readonly ClientConfiguration Config = new ClientConfiguration(
            new XmlSerializer(),
            new JsonExceptionSerializer(new XmlSerializer()),
            new DefaultWebRequestHandlerEx());

        public static ITestContract CreateIISBolt()
        {
            return Config.CreateProxy<TestContractProxy>(Servers.IISBoltServer);
        }

        public static ITestContract CreateBolt()
        {
            return Config.CreateProxy<TestContractProxy>(Servers.BoltServer);
        }

        public static ITestContract CreateWcf()
        {
            ChannelFactory<ITestContract> respository = new ChannelFactory<ITestContract>(new BasicHttpBinding());
            ITestContract channel = respository.CreateChannel(new EndpointAddress(Servers.WcfServer));
            return channel;
        }

        public static ITestContract CreateIISWcf()
        {
            ChannelFactory<ITestContract> respository = new ChannelFactory<ITestContract>(new BasicHttpBinding());
            ITestContract channel = respository.CreateChannel(new EndpointAddress(Servers.IISWcfServer));
            return channel;
        }
    }
}
