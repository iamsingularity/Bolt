﻿using System;

namespace Bolt.Server
{
    public class ServerRuntimeConfiguration
    {
        public ServerRuntimeConfiguration()
        {
        }

        public ServerRuntimeConfiguration(ServerRuntimeConfiguration other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            Serializer = other.Serializer;
            ExceptionWrapper = other.ExceptionWrapper;
            Options = other.Options;
        }

        public ISerializer Serializer { get; set; }

        public IExceptionWrapper ExceptionWrapper { get; set; }

        public BoltServerOptions Options { get; set; }

        public void Merge(ServerRuntimeConfiguration other)
        {
            if (other == null)
            {
                return;
            }

            if (other.Serializer != null)
            {
                Serializer = other.Serializer;
            }
            if (other.ExceptionWrapper != null)
            {
                ExceptionWrapper = other.ExceptionWrapper;
            }
            if (other.Options != null)
            {
                Options = other.Options;
            }
            if (other.Serializer != null)
            {
                Serializer = other.Serializer;
            }
        }
    }
}