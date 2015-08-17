﻿using System;
using System.IO;
using System.Threading.Tasks;
using Bolt.Pipeline;

namespace Bolt.Server.Pipeline
{
    public class HandleErrorMiddleware : MiddlewareBase<ServerActionContext>
    {
        public override async Task Invoke(ServerActionContext context)
        {
            try
            {
                await Next(context);
            }
            catch (Exception e)
            {
                if (context.Configuration.ErrorHandler.Handle(context, e))
                {
                    return;
                }

                try
                {
                    await WriteExceptionAsync(context, e);
                }
                catch (BoltServerException serializationException)
                {
                    if (!context.Configuration.ErrorHandler.Handle(context, serializationException))
                    {
                        throw;
                    }
                }
            }
        }

        protected virtual Task WriteExceptionAsync(ServerActionContext context, Exception error)
        {
            MemoryStream serializedException = new MemoryStream();

            context.RequestAborted.ThrowIfCancellationRequested();
            var httpContext = context.HttpContext;
            httpContext.Response.StatusCode = 500;

            try
            {
                object wrappedException = context.Configuration.ExceptionWrapper.Wrap(error);
                if (wrappedException == null)
                {
                    httpContext.Response.Body.Dispose();
                    return Task.FromResult(0);
                }

                context.Configuration.Serializer.Write(serializedException, wrappedException);
                serializedException.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception e)
            {
                throw new BoltServerException(
                    $"Failed to serialize exception response for action {context.Action.Name}.",
                    ServerErrorCode.SerializeException,
                    context.Action,
                    context.RequestUrl,
                    e);
            }

            if (serializedException.Length == 0)
            {
                httpContext.Response.Body.Dispose();
                return Task.FromResult(0);
            }

            httpContext.Response.ContentLength = serializedException.Length;
            httpContext.Response.ContentType = context.Configuration.Serializer.ContentType;

            return serializedException.CopyToAsync(httpContext.Response.Body, 4096, httpContext.RequestAborted);
        }
    }
}