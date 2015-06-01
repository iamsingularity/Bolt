using System;
using System.Threading.Tasks;
using Bolt.Client.Helpers;
using Bolt.Core;

namespace Bolt.Client.Channels
{

    /// <summary>
    /// Recoverable Bolt statefull channel.
    /// </summary>
    /// <typeparam name="TContract">The strongly typed Bolt proxy usually generated by Bolt tool.</typeparam>
    public abstract class RecoverableStatefullChannel<TContract> : RecoverableChannel, ISessionProvider
        where TContract : ContractProxy
    {
        private readonly AwaitableCriticalSection _syncRoot = new AwaitableCriticalSection();
        private readonly IClientSessionHandler _sessionHandler;

        private ConnectionDescriptor _activeConnection;
        private string _sessionId;

        protected RecoverableStatefullChannel(Uri server, ClientConfiguration clientConfiguration)
            : base(new SingleServerProvider(server), clientConfiguration)
        {
            _sessionHandler = clientConfiguration.SessionHandler;
        }

        protected RecoverableStatefullChannel(IServerProvider serverProvider, ClientConfiguration clientConfiguration, IClientSessionHandler sessionHandler)
            : base(serverProvider, clientConfiguration)
        {
            if (sessionHandler == null)
            {
                throw new ArgumentNullException(nameof(sessionHandler));
            }

            _sessionHandler = sessionHandler;
        }

        protected RecoverableStatefullChannel(RecoverableStatefullChannel<TContract> proxy)
            : base(proxy)
        {
            _sessionHandler = proxy._sessionHandler;
        }

        protected RecoverableStatefullChannel(
            IServerProvider serverProvider,
            IRequestHandler requestHandler,
            IEndpointProvider endpointProvider, IClientSessionHandler sessionHandler)
            : base(serverProvider, requestHandler, endpointProvider)
        {
            if (sessionHandler == null)
            {
                throw new ArgumentNullException(nameof(sessionHandler));
            }

            _sessionHandler = sessionHandler;
        }

        public string SessionId => _sessionId;

        public virtual bool IsRecoverable => true;

        public override void Open()
        {
            TaskHelpers.Execute(OpenAsync);
        }

        public override async Task OpenAsync()
        {
            await EnsureConnectionAsync();
            IsOpened = true;
        }

        public override void Close()
        {
            TaskHelpers.Execute(() => CloseAsync());
        }

        public override async Task CloseAsync()
        {
            if (IsClosed)
            {
                return;
            }

            using (_syncRoot.Enter())
            {
                if (IsClosed)
                {
                    return;
                }

                try
                {
                    if (_activeConnection != null)
                    {
                        string sessionId = _sessionId;

                        DelegatedChannel channel = new DelegatedChannel(
                            _activeConnection.Server,
                            RequestHandler,
                            EndpointProvider,
                            c =>
                            {
                                BeforeSending(c);
                                _sessionHandler.EnsureSession(c.Request, sessionId);
                            },
                            AfterReceived);

                        TContract contract = CreateContract(channel);
                        await OnProxyClosingAsync(contract);
                    }
                }
                finally
                {
                    _activeConnection = null;
                    _sessionId = null;
                    base.Close();
                }
            }
        }

        protected abstract Task OnProxyClosingAsync(TContract contract);

        protected abstract Task OnProxyOpeningAsync(TContract contract);

        protected override bool HandleError(ClientActionContext context, Exception error)
        {
            var exception = error as BoltServerException;
            if (exception != null && exception.Error == ServerErrorCode.SessionNotFound)
            {
                if (!IsRecoverable)
                {
                    return false;
                }

                CloseConnection();
                return true;
            }

            return base.HandleError(context, error);
        }

        protected void CloseConnection()
        {
            using (_syncRoot.Enter())
            {
                bool exist = _activeConnection != null;
                _activeConnection = null;
                _sessionId = null;

                if (exist)
                {
                    OnConnectionClosed();
                }
            }
        }

        protected override void BeforeSending(ClientActionContext context)
        {
            _sessionHandler.EnsureSession(context.Request, _sessionId);
            base.BeforeSending(context);
        }

        protected override async Task<ConnectionDescriptor> GetConnectionAsync()
        {
            EnsureNotClosed();
            ConnectionDescriptor uri = await EnsureConnectionAsync();
            IsOpened = true;
            return uri;
        }

        protected virtual void OnConnectionOpened(ConnectionDescriptor activeConnection, string sessionId)
        {
        }

        protected virtual void OnConnectionClosed()
        {
        }

        protected TContract CreateContract(IChannel channel)
        {
            return (TContract)Activator.CreateInstance(typeof(TContract), channel);
        }

        protected TContract CreateContract(Uri server)
        {
            return CreateContract(new DelegatedChannel(server, RequestHandler, EndpointProvider, BeforeSending, AfterReceived));
        }

        private async Task<ConnectionDescriptor> EnsureConnectionAsync()
        {
            EnsureNotClosed();

            if (_activeConnection != null)
            {
                return _activeConnection;
            }

            using (await _syncRoot.EnterAsync())
            {
                if (_activeConnection != null)
                {
                    return _activeConnection;
                }

                var connection = ServerProvider.GetServer();
                string sessionId = null;
                ActionDescriptor action = null;

                TContract contract =
                    CreateContract(
                        new DelegatedChannel(
                            connection.Server,
                            RequestHandler,
                            EndpointProvider,
                            c =>
                            {
                                _sessionHandler.EnsureSession(c.Request, sessionId);
                                BeforeSending(c);
                            },
                            ctxt =>
                            {
                                if (sessionId == null)
                                {
                                    action = ctxt.Action;
                                    sessionId = _sessionHandler.GetSessionIdentifier(ctxt.Response);
                                }
                            }));

                Exception error = null;

                try
                {
                    await OnProxyOpeningAsync(contract);
                }
                catch (Exception e)
                {
                    error = e;
                    if (sessionId == null)
                    {
                        throw;
                    }
                }

                if (error != null)
                {
                    try
                    {
                        await OnProxyClosingAsync(contract);
                    }
                    catch (Exception)
                    {
                        // OK, we tried to close pending proxy
                    }

                    throw error;
                }

                if (sessionId == null)
                {
                    throw new BoltServerException(ServerErrorCode.SessionIdNotReceived, action, connection.ToString());
                }

                OnConnectionOpened(_activeConnection, _sessionId);

                _activeConnection = connection;
                _sessionId = sessionId;
                return connection;
            }
        }
    }
}