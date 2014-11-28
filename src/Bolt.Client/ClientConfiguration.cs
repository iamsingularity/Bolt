using System;

namespace Bolt.Client
{
    public class ClientConfiguration : Configuration
    {
        public ClientConfiguration(ISerializer serializer, IExceptionSerializer exceptionSerializer, IWebRequestHandler webRequestHandler = null)
            : base(serializer, exceptionSerializer)
        {
            if (webRequestHandler == null)
            {
                webRequestHandler = new DefaultWebRequestHandler();
            }

            ClientDataHandler = new ClientDataHandler(serializer, ExceptionSerializer, webRequestHandler);
            RequestForwarder = new RequestForwarder(ClientDataHandler, webRequestHandler, ServerErrorCodesHeader);
        }

        public IRequestForwarder RequestForwarder { get; set; }

        public IClientDataHandler ClientDataHandler { get; set; }

        public TimeSpan DefaultResponseTimeout { get; set; }
    }
}