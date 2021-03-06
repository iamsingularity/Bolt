﻿using System;
using System.Threading;

namespace Bolt.Client
{
    public class RequestScope : IDisposable
    {
        private static readonly AsyncLocal<RequestScope> RequestScopeCurrent = new AsyncLocal<RequestScope>();

        public RequestScope(CancellationToken cancellation)
            : this(TimeSpan.Zero, cancellation)
        {
        }

        public RequestScope(TimeSpan timeout)
            : this(timeout, CancellationToken.None)
        {
        }

        public RequestScope(TimeSpan timeout = default(TimeSpan), CancellationToken cancellation = default(CancellationToken))
        {
            Timeout = timeout;
            Cancellation = cancellation;
            Current = this;
        }

        public static RequestScope Current
        {
            get { return RequestScopeCurrent.Value; }
            set { RequestScopeCurrent.Value = value; }
        }

        public TimeSpan Timeout { get; }

        public CancellationToken Cancellation { get; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            Current = null;
        }
    }
}
