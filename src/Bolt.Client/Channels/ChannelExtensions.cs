﻿using System;

namespace Bolt.Client.Channels
{
    public static class ChannelExtensions
    {
        public static DirectChannel CreateDelegated(this ClientConfiguration configuration, string server)
        {
            return configuration.CreateDelegated(new Uri(server));
        }

        public static DirectChannel CreateDelegated(this ClientConfiguration configuration, Uri server)
        {
            return new DirectChannel(server, configuration)
            {
                DefaultResponseTimeout = configuration.DefaultResponseTimeout
            };
        }

        public static RecoverableChannel CreateRecoverable(this ClientConfiguration configuration, string server)
        {
            return configuration.CreateRecoverable(new Uri(server));
        }

        public static RecoverableChannel CreateRecoverable(this ClientConfiguration configuration, Uri server)
        {
            return configuration.CreateRecoverable(new SingleServerProvider(server));
        }

        public static RecoverableChannel CreateRecoverable(this ClientConfiguration configuration, IServerProvider serverProvider)
        {
            return new RecoverableChannel(serverProvider, configuration);
        }
    }
}