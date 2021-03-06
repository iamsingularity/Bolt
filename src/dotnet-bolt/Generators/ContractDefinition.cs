using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bolt.Tools.Generators
{
    public class ContractDefinition
    {
        private IReadOnlyCollection<Type> _excludedContracts;

        public ContractDefinition(Type root, params Type[] excludedContracts)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (!root.GetTypeInfo().IsInterface)
            {
                throw new InvalidOperationException("Root contract definition must be interface type.");
            }

            Root = root;
            Name = Root.Name[0] == 'I' ? Root.Name.Substring(1) : Root.Name;
            Namespace = root.Namespace;
            _excludedContracts = excludedContracts?.ToList() ?? new List<Type>();

            Validate();
        }

        public Type Root { get; }

        public string Name { get; }

        public string Namespace { get; }

        public IReadOnlyCollection<Type> ExcludedContracts
        {
            get
            {
                return _excludedContracts;
            }

            set
            {
                _excludedContracts = value;
                Validate();
            }
        }

        public virtual IEnumerable<Type> GetEffectiveContracts(Type iface)
        {
            return iface.GetTypeInfo().ImplementedInterfaces.Except(ExcludedContracts ?? Enumerable.Empty<Type>());
        }

        public virtual IReadOnlyCollection<Type> GetEffectiveContracts()
        {
            List<Type> contracts = new List<Type>
            {
                Root
            };

            contracts.AddRange(
                GetInterfaces(Root)
                    .Except(ExcludedContracts ?? Enumerable.Empty<Type>()));

            return contracts;
        }

        public virtual IEnumerable<MethodInfo> GetEffectiveMethods(Type iface)
        {
            return iface.GetTypeInfo().DeclaredMethods;
        }

        public virtual IEnumerable<MethodInfo> GetEffectiveMethods()
        {
            return GetEffectiveContracts().SelectMany(effectiveContract => effectiveContract.GetTypeInfo().DeclaredMethods).Distinct();
        }

        public bool IsValid()
        {
            List<MethodInfo> methods = GetEffectiveMethods().ToList();
            return methods.Select(m => m.Name).Distinct().Count() == methods.Count();
        }

        protected virtual IEnumerable<Type> GetInterfaces(Type contract)
        {
            return GetInterfacesInternal(contract.GetTypeInfo().ImplementedInterfaces).Distinct();
        }

        private IEnumerable<Type> GetInterfacesInternal(IEnumerable<Type> interfaces)
        {
            foreach (Type @interface in interfaces)
            {
                yield return @interface;

                foreach (Type inner in GetInterfacesInternal(@interface.GetTypeInfo().ImplementedInterfaces))
                {
                    yield return inner;
                }
            }
        }

        private void Validate()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException(
                    $"Contract {Root.FullName} contains multiple methods with the same name.");
            }
        }
    }
}