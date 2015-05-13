﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bolt
{
    /// <summary>
    /// Describes the contract with all available actions. Usually generated by Bolt tool.
    /// </summary>
    public abstract class ContractDescriptor : IEnumerable<ActionDescriptor>
    {
        private readonly List<ActionDescriptor> _actions = new List<ActionDescriptor>();

        protected ContractDescriptor(Type type) : this(type, type.Name)
        {
        }

        protected ContractDescriptor(Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Type = type;
            Name = name;
        }

        public Type Type { get; private set; }

        public string Name { get; }

        public ActionDescriptor Find(MethodInfo info)
        {
            return this.FirstOrDefault(v => Equals(v.Method, info));
        }

        public IEnumerator<ActionDescriptor> GetEnumerator()
        {
            return _actions.GetEnumerator();
        }

        protected ActionDescriptor Add(string action, Type parameters, MethodInfo method)
        {
            ActionDescriptor descriptor = new ActionDescriptor(action, parameters, this, method);
            _actions.Add(descriptor);
            return descriptor;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}