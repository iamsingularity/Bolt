using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Bolt.Server
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IDataHandler _dataHandler;
        private readonly string _errorCodesHeader;

        public ErrorHandler(IDataHandler dataHandler, string errorCodesHeader)
        {
            _dataHandler = dataHandler;
            _errorCodesHeader = errorCodesHeader;
        }

        public bool HandleBoltError(IOwinContext context, ServerErrorCode code)
        {
            CloseWithError(context, code);
            return true;
        }

        public virtual Task HandleError(ServerActionContext context, Exception error)
        {
            if (error is DeserializeParametersException)
            {
                CloseWithError(context.Context, ServerErrorCode.Deserialization);
                return Task.FromResult(0);
            }

            if (error is SerializeResponseException)
            {
                CloseWithError(context.Context, ServerErrorCode.Serialization);
                return Task.FromResult(0);

            }

            if (error is SessionHeaderNotFoundException)
            {
                CloseWithError(context.Context, ServerErrorCode.NoSessionHeader);
                return Task.FromResult(0);

            }

            if (error is SessionNotFoundException)
            {
                CloseWithError(context.Context, ServerErrorCode.SessionNotFound);
                return Task.FromResult(0);
            }

            context.Context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return _dataHandler.WriteExceptionAsync(context, error);
        }

        protected virtual void CloseWithError(IOwinContext context, ServerErrorCode code)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers[_errorCodesHeader] = code.ToString();
            context.Response.Body.Close();
        }

        protected virtual void CloseWithError(IOwinContext context, int code)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers[_errorCodesHeader] = code.ToString(CultureInfo.InvariantCulture);
            context.Response.Body.Close();
        }
    }
}