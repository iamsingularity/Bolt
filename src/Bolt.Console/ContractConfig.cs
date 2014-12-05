﻿using Bolt.Generators;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bolt.Console
{
    public class ContractConfig
    {
        [JsonIgnore]
        private ContractDefinition _contractDefinition;

        [JsonProperty(Required = Required.Always)]
        public string Contract { get; set; }

        public string Output { get; set; }

        public List<string> Excluded { get; set; }

        public ClientConfig Client { get; set; }

        public ServerConfig Server { get; set; }

        public string Modifier { get; set; }

        [JsonIgnore]
        public RootConfig Parent { get; set; }

        [JsonIgnore]
        public ContractDefinition ContractDefinition
        {
            get
            {
                if (_contractDefinition == null)
                {
                    _contractDefinition = GetContractDefinition();
                }

                return _contractDefinition;
            }
        }

        public void Generate()
        {
            if (Client != null)
            {
                Client.Parent = this;
            }

            if (Server != null)
            {
                Server.Parent = this;
            }

            ContractDefinition definition = ContractDefinition;
            string output = PathHelpers.GetOutput(Parent.OutputDirectory, Output, ContractDefinition.Name + ".Contract.Designer.cs");
            DocumentGenerator document = Parent.GetDocument(output);
            document.Formatter.Assemblies = Parent.AssemblyCache.ToList();

            document.Add(new ContractGenerator()
                             {
                                 ContractDefinition = definition,
                                 Modifier = Modifier ?? "public"

                             });

            document.Add(new ContractDescriptorGenerator()
                             {
                                 ContractDefinition = definition,
                                 Modifier = Modifier ?? "public"
                             });

            ContractExecution execution = new ContractExecution(definition, Path.GetDirectoryName(output));
            if (Client != null)
            {
                Client.Execute(execution);
            }

            if (Server != null)
            {
                Server.Execute(execution);
            }
        }

        private ContractDefinition GetContractDefinition()
        {
            Type type = Parent.AssemblyCache.GetType(Contract);
            List<Type> excluded = new List<Type>();

            if (Excluded != null && Excluded.Any())
            {
                excluded = Excluded.Select(Parent.AssemblyCache.GetType).ToList();
            }

            return new ContractDefinition(type, excluded.ToArray());
        }

    }
}
