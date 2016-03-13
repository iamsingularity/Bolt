using System;
using System.Collections.Generic;
using System.Linq;

namespace Bolt.Console.Generators
{
    public class InterfaceGenerator : ContractGeneratorBase
    {
        private readonly List<Type> _generatedInterfaces = new List<Type>();

        private readonly List<string> _generatedAsyncInterfaces = new List<string>();

        public IEnumerable<string> GeneratedAsyncInterfaces => _generatedAsyncInterfaces;

        public bool ForceAsync { get; set; }

        public bool ForceSync { get; set; }

        public List<string> ExcludedInterfaces { get; set; }

        public string InterfaceSuffix { get; set; }

        public string Name { get; set; }

        public string Modifier { get; set; }

        public override void Generate(object context)
        {
            bool shouldGenerateInterface = ContractDefinition.GetEffectiveContracts().Any(ShouldGenerateInterface);

            foreach (var iface in ContractDefinition.GetEffectiveContracts().Except(new[] { ContractDefinition.Root }))
            {
                if (shouldGenerateInterface)
                {
                    GenerateInterface(iface, context, null);
                }
            }

            GenerateInterface(ContractDefinition.Root, context, Name);
        }

        private void GenerateInterface(Type contract, object context, string name)
        {
            string suffix = InterfaceSuffix ?? AsyncSuffix;

            if (ExcludedInterfaces != null)
            {
                if (ExcludedInterfaces.Contains(contract.FullName) ||
                    ExcludedInterfaces.Contains(contract.FullName + suffix))
                {
                    return;
                }
            }

            if (_generatedInterfaces.Contains(contract))
            {
                return;
            }

            if (
                !ContractDefinition.GetEffectiveContracts(contract)
                    .Concat(new[] {contract})
                    .Any(ShouldGenerateInterface))
            {
                return;
            }

            _generatedInterfaces.Add(contract);
            name = name ?? (contract.Name + suffix);
            _generatedAsyncInterfaces.Add(name);

            List<string> asyncBase =
                ContractDefinition.GetEffectiveContracts(contract).Where(ShouldGenerateInterface).Where(
                    i =>
                    {
                        if (ExcludedInterfaces != null)
                        {
                            if (ExcludedInterfaces.Contains(i.FullName))
                            {
                                return false;
                            }
                        }
                        return true;
                    }).Select(t => FormatType(t) + suffix).ToList();

            asyncBase.Insert(0, FormatType(contract));

            ClassGenerator classGenerator =
                CreateClassGenerator(new ClassDescriptor(name, contract.Namespace, asyncBase.ToArray())
                {
                    IsInterface = true
                });
            classGenerator.Modifier = Modifier;
            classGenerator.GenerateBodyAction = g =>
            {
                var methods =
                    (from method in ContractDefinition.GetEffectiveMethods(contract)
                        let async = ShouldBeAsync(method, ForceAsync)
                        let sync = ShouldBeSync(method, ForceSync)

                        where async || sync
                        select new {method, async, sync}).ToList();

                foreach (var method in methods)
                {
                    if (method.async)
                    {
                        g.WriteLine(FormatMethodDeclaration(method.method, MethodDeclarationFormatting.ChangeToAsync) + ";");
                    }
                    else
                    {
                        g.WriteLine(FormatMethodDeclaration(method.method, MethodDeclarationFormatting.ChangeToSync) + ";");
                    }

                    if (!Equals(method, methods.Last()))
                    {
                        g.WriteLine();
                    }
                }
            };

            classGenerator.Generate(context);
        }

        private bool ShouldGenerateInterface(Type contract)
        {
            return ShouldHaveAsyncMethods(contract) || ShouldHaveSyncMethods(contract);
        }


        private bool ShouldHaveSyncMethods(Type contract)
        {
            if (ForceSync)
            {
                return true;
            }

            return ContractDefinition.GetEffectiveMethods(contract).Any(m => ShouldBeSync(m, ForceSync));
        }

        private bool ShouldHaveAsyncMethods(Type contract)
        {
            if (ForceAsync)
            {
                return true;
            }

            return ContractDefinition.GetEffectiveMethods(contract).Any(m => ShouldBeAsync(m, ForceAsync));
        }
    }
}