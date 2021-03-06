﻿using Bolt.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bolt.Sample.SimpleProxy
{
    public partial class Program
    {
        public class Startup
        {
            public void ConfigureServices(IServiceCollection serviceCollection)
            {
                serviceCollection.AddRouting();
                serviceCollection.AddBolt();
                serviceCollection.AddOptions();
            }

            public void Configure(IApplicationBuilder builder)
            {
                // we will add IDummyContract endpoint to Bolt
                builder.UseBolt(r => r.Use<IDummyContract, DummyContract>());
            }
        }
    }
}
