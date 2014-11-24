using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using Bolt.Generators;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bolt.Console
{
    public class RootConfig
    {
        private readonly Dictionary<string, DocumentGenerator> _documents = new Dictionary<string, DocumentGenerator>();

        public RootConfig()
        {
            Contracts = new List<ContractConfig>();
        }

        public static RootConfig Load(string file)
        {
            string content = File.ReadAllText(file);
            return Load(Path.GetDirectoryName(file), content);
        }

        public static RootConfig Load(string outputDirectory, string content)
        {
            RootConfig config = JsonConvert.DeserializeObject<RootConfig>(
                content,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include, Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            config.OutputDirectory = outputDirectory;

            foreach (ContractConfig contract in config.Contracts)
            {
                contract.Parent = config;
            }

            return config;
        }

        [JsonProperty(Required = Required.Always)]
        public List<ContractConfig> Contracts { get; set; }

        [JsonIgnore]
        public string OutputDirectory { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(
                this,
                new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public void Generate()
        {
            List<string> directories = Contracts.SelectMany(c => c.Assemblies.Select(Path.GetDirectoryName)).Distinct().ToList();
            AssemblyResolver resolver = new AssemblyResolver(directories.ToArray());
            AppDomain.CurrentDomain.AssemblyResolve += resolver.Resolve;

            try
            {
                foreach (ContractConfig contract in Contracts)
                {
                    contract.Generate();
                }

                foreach (KeyValuePair<string, DocumentGenerator> documentGenerator in _documents)
                {
                    string result = documentGenerator.Value.GetResult();
                    File.WriteAllText(documentGenerator.Key, result);
                }
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= resolver.Resolve;
            }
        }

        public DocumentGenerator GetDocument(string output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            if (!_documents.ContainsKey(output))
            {
                _documents[output] = new DocumentGenerator();
            }

            return _documents[output];
        }

        private class AssemblyResolver
        {
            private readonly string[] _directories;
            private readonly ConcurrentDictionary<string, List<string>> _directoryFiles = new ConcurrentDictionary<string, List<string>>();

            public AssemblyResolver(params string[] directories)
            {
                _directories = directories.Where(d => !string.IsNullOrEmpty(d)).Distinct().ToArray();
            }

            public Assembly Resolve(object sender, ResolveEventArgs e)
            {
                Assembly result = DoResolve(e);
                if (result == null)
                {
                    System.Console.WriteLine("Assembly not resolved: {0}", e.Name);
                }

                return result;
            }

            private Assembly DoResolve(ResolveEventArgs e)
            {
                if (e.RequestingAssembly != null)
                {
                    AssemblyName[] references = e.RequestingAssembly.GetReferencedAssemblies();

                    foreach (AssemblyName name in references)
                    {
                        foreach (string dir in _directories)
                        {
                            if (TryLoadAssembly(name.Name, GetFiles(dir)) != null)
                            {
                                break;
                            }
                        }
                    }
                }

                foreach (string dir in _directories.Where(p => !string.IsNullOrEmpty(p)))
                {
                    Assembly ass = TryLoadAssembly(e.Name, GetFiles(dir));
                    if (ass != null)
                    {
                        return ass;
                    }
                }

                return null;
            }

            private IEnumerable<string> GetFiles(string dir)
            {
                return _directoryFiles.GetOrAdd(
                    dir,
                    d =>
                    {
                        DirectoryInfo directory = new DirectoryInfo(Path.GetDirectoryName(dir));
                        return
                            directory.GetFiles("*.dll", SearchOption.AllDirectories)
                                .Concat(directory.GetFiles("*.exe", SearchOption.AllDirectories))
                                .Select(f => f.FullName)
                                .ToList();
                    });
            }

            private static Assembly TryLoadAssembly(string fullName, IEnumerable<string> files)
            {
                try
                {
                    Assembly assemblyFound = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == fullName);
                    if (assemblyFound != null)
                    {
                        return assemblyFound;
                    }

                    fullName = fullName.Split(new[] { ',' })[0];

                    string found = files.FirstOrDefault(f => string.Equals(fullName, Path.GetFileNameWithoutExtension(f), StringComparison.InvariantCultureIgnoreCase));

                    if (found == null)
                    {
                        return null;
                    }

                    return Assembly.Load(File.ReadAllBytes(found));
                }
                catch (Exception)
                {
                    Debug.Assert(false, "Assembly load failed.");
                    return null;
                }
            }

        }
    }
}