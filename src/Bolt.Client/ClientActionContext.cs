﻿using System;
using System.Net;
using System.Threading;

namespace Bolt.Client
{
    public class ClientActionContext : ActionContextBase, IDisposable
    {
        public ClientActionContext(ActionDescriptor action, HttpWebRequest request, Uri server, CancellationToken cancellation)
            : base(action)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            Request = request;
            Server = server;
            Cancellation = cancellation;
        }

        public Uri Server { get; private set; }

        public HttpWebRequest Request { get; private set; }

        public CancellationToken Cancellation { get; private set; }

        public HttpWebResponse Response { get; set; }

        public TimeSpan ResponseTimeout { get; set; }

        public void Dispose()
        {
            if (Response != null)
            {
                Response.Dispose();
                Response = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}