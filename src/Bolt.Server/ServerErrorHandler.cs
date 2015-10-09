﻿using System;
using System.Globalization;
using Bolt.Server.Pipeline;
using Microsoft.Extensions.Logging;

namespace Bolt.Server
{
    public class ServerErrorHandler : IServerErrorHandler
    {
        public ServerErrorHandler(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            LoggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger<HandleErrorMiddleware>();
        }

        public ILogger Logger { get; }

        public ILoggerFactory LoggerFactory { get; }

        public bool Handle(ServerActionContext context, Exception error)
        {
            if (error is BoltServerException)
            {
                HandleBoltServerError(context, error as BoltServerException);
                return true;
            }

            return false;
        }

        protected virtual void HandleBoltServerError(ServerActionContext actionContext, BoltServerException error)
        {
            int statusCode = 500;

            var response = actionContext.HttpContext.Response;
            response.StatusCode = statusCode;
            if (error.ErrorCode != null)
            {
                response.Headers[actionContext.Configuration.Options.ServerErrorHeader] = error.ErrorCode.Value.ToString(CultureInfo.InvariantCulture);
            }
            else if (error.Error != null)
            {
                response.Headers[actionContext.Configuration.Options.ServerErrorHeader] = error.Error.Value.ToString();
            }

            LogBoltServerError(actionContext, error);
        }

        private void LogBoltServerError(ServerActionContext context, BoltServerException error)
        {
            if (error.Error != null)
            {
                Logger.LogError(
                    BoltLogId.RequestExecutionError,
                    "Execution of '{0}' failed with Bolt error '{1}'",
                    context.Action.Name,
                    error.Error);
            }

            if (error.ErrorCode != null)
            {
                Logger.LogError(
                    BoltLogId.RequestExecutionError,
                    "Execution of '{0}' failed with error code '{1}'",
                    context.Action.Name,
                    error.ErrorCode);
            }
        }

    }
}