﻿using System.Threading;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Bolt.Core;

namespace Bolt.Server
{
    /// <summary>
    /// Context of single contract action. By default all properties are filled by <see cref="BoltRouteHandler"/>. 
    /// Optionaly <see cref="IContractInvoker"/> might override some properties if special handling is required.
    /// </summary>
    public class ServerActionContext : ActionContextBase, IContractProvider
    {
        public HttpContext HttpContext { get; set; }

        public RouteContext RouteContext { get; set; }

        public CancellationToken RequestAborted => HttpContext.RequestAborted;

        public object ContractInstance { get; set; }

        public IObjectSerializer Parameters { get; set; }

        public object Result { get; set; }

        public bool IsExecuted { get; set; }

        public bool IsResponseSend { get; set; }

        public IContractInvoker ContractInvoker { get; set; }

        public Type Contract => ContractInvoker?.Contract;

        public object GetRequiredInstance()
        {
            if (ContractInstance == null)
            {
                throw new InvalidOperationException("There is no contract instance assigned to current context.");
            }

            if (!(ContractInstance.GetType().GetTypeInfo().ImplementedInterfaces.Contains(Contract)))
            {
                throw new InvalidOperationException($"Contract instance of type {Contract.Name} is expected but {ContractInstance.GetType().Name} was provided.");
            }

            return ContractInstance;
        }

        public IObjectSerializer GetRequiredParameters()
        {
            if (Parameters == null)
            {
                throw new InvalidOperationException("There is no paramters instance assigned to current context.");
            }
    
            return Parameters;
        }

        public void EnsureNotExecuted()
        {
            if (IsExecuted)
            {
                throw new InvalidOperationException("Request is already handled.");
            }
        }

        public void EnsureNotSend()
        {
            if (IsResponseSend)
            {
                throw new InvalidOperationException("Response has already been send to client.");
            }
        }

    }
}