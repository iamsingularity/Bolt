﻿using Bolt.Core.Serialization;
using Bolt.Server;

using Owin;

using TestService.Core;

namespace TestService.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ServerConfiguration configuration = new ServerConfiguration(new JsonSerializer(), new JsonExceptionSerializer());
            app.MapContract(PersonRepositoryDescriptor.Default, configuration, "api", b => ConfigurePersonRepository(b, configuration));
        }

        private void ConfigurePersonRepository(IAppBuilder obj, ServerConfiguration configuration)
        {
            obj.UseStateFullContractInvoker<PersonRepositoryInvoker, PersonRepository>(configuration, PersonRepositoryDescriptor.Default);
        }
    }
}
