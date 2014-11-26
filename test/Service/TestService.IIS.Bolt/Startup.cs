﻿using Bolt.Core.Serialization;
using Bolt.Server;

using Owin;

using TestService.Core;

namespace TestService.IIS.Bolt
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServerConfiguration configuration = new ServerConfiguration(new ProtocolBufferSerializer(), new JsonExceptionSerializer());
        }

        private void ConfigurePersonRepository(IAppBuilder obj, ServerConfiguration configuration)
        {
            obj.UseContractInvoker<PersonRepositoryInvoker, PersonRepositoryDescriptor>(
                configuration,
                PersonRepositoryDescriptor.Default,
                new StaticInstanceProvider(new PersonRepository()));
        }
    }
}
