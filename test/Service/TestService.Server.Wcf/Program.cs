﻿using System;
using System.ServiceModel;

using TestService.Core;

namespace TestService.Server.Wcf
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(PersonRepository), Servers.WcfServer);
            host.Open();

            Console.WriteLine("Host running ... ");
            Console.ReadLine();
        }
    }
}