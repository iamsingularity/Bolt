﻿using Bolt.Client;
using Bolt.Client.Channels;
using Bolt.Server.IntegrationTest.Core;
using System;
using System.Threading.Tasks;

namespace Bolt.Server.IntegrationTest
{
    public class TestContractStateFullChannel : RecoverableStatefullChannel
    {
        public TestContractStateFullChannel(Uri server, ClientConfiguration clientConfiguration)
            : base(typeof(ITestContractStateFullAsync), server, clientConfiguration)
        {
            Retries = 1;
        }

        public override bool IsRecoverable => true;

        protected override bool HandleError(ClientActionContext context, Exception error)
        {
            if (error is TestContractProxyFailedException)
            {
                CloseConnection();
                return true;
            }

            return base.HandleError(context, error);
        }
    }
}
