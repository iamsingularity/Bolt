﻿using Bolt.Core.Serialization;
using Bolt.Server;

using Owin;

using TestService.Core;

namespace TestService.Server
{
    public class Rep : PersonRepository
    {
        public override void ThrowsCustom()
        {
            throw new CustomException("test");
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseBolt(new ServerConfiguration(new JsonSerializer(), new JsonExceptionSerializer(new JsonSerializer())));
            app.UsePersonRepository(new Rep());
        }
    }
}
