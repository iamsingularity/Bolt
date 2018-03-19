﻿using System;
using System.Threading.Tasks;

using Bolt.Metadata;
using Bolt.Pipeline;
using System.Linq;
using System.Net.Http.Headers;
using Bolt.Serialization;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Buffers;

namespace Bolt.Server.Pipeline
{
    public class SerializationMiddleware : MiddlewareBase<ServerActionContext>
    {
        public override async Task InvokeAsync(ServerActionContext context)
        {
            if (context.Configuration.DefaultSerializer == null)
            {
                context.Configuration.DefaultSerializer = PickSerializer(context);
            }

            var actionMetadata = context.GetActionOrThrow();

            if (actionMetadata.HasParameters && context.Parameters == null)
            {
                context.Parameters = await DeserializeParameters(context, actionMetadata);
            }

            await Next(context);

            if (!context.ResponseHandled)
            {
                await HandleResponse(context);
                context.ResponseHandled = true;
            }
        }

        protected virtual async Task<object[]> DeserializeParameters(ServerActionContext context, ActionMetadata metadata)
        {
            object[] parameters = null;

            if (metadata.HasSerializableParameters)
            {
                ISerializer serializer = context.GetSerializerOrThrow();
                try
                {
                    // TODO: copy body to another stream to prevent blocking in json deserialization
                    parameters = await serializer.ReadParametersAsync(context.HttpContext.Request.Body, metadata.Parameters, context.HttpContext.Request.ContentLength ?? -1);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new BoltServerException(
                        $"Failed to deserialize parameters for action '{context.Action.Name}'.",
                        ServerErrorCode.DeserializeParameters,
                        context.Action.Name,
                        context.RequestUrl,
                        e);
                }
            }
            else if (context.Action.Parameters.Count > 0)
            {
                parameters = new object[context.Action.Parameters.Count];
            }

            if (metadata.CancellationTokenIndex >= 0)
            {
                parameters[metadata.CancellationTokenIndex] = context.RequestAborted;
            }

            return parameters;
        }

        protected virtual async Task HandleResponse(ServerActionContext context)
        {
            context.RequestAborted.ThrowIfCancellationRequested();
            context.HttpContext.Response.StatusCode = 200;

            if (context.GetActionOrThrow().HasResult && context.ActionResult != null)
            {
                context.HttpContext.Response.ContentType = context.Configuration.DefaultSerializer.MediaType;
                try
                {
                    await context.GetSerializerOrThrow().WriteAsync(context.HttpContext.Response.Body, context.ActionResult, l => OnHandleContentLength(context, l));
                }
                catch (Exception e)
                {
                    throw new BoltServerException(
                        $"Failed to serialize response for action '{context.Action.Name}'.",
                        ServerErrorCode.SerializeResponse,
                        context.Action.Name,
                        context.RequestUrl,
                        e);
                }
            }
            else
            {
                context.HttpContext.Response.ContentLength = 0;
            }

            await context.HttpContext.Response.Body.FlushAsync();
            context.HttpContext.Response.Body.Dispose();
        }

        private void OnHandleContentLength(ServerActionContext context, long contentLength)
        {
            if ( contentLength > 0)
            {
                context.HttpContext.Response.ContentLength = contentLength;
            }
        }

        protected virtual ISerializer PickSerializer(ServerActionContext context)
        {
            if (context.Configuration.AvailableSerializers.Count == 1)
            {
                return context.Configuration.AvailableSerializers[0];
            }

            StringValues value;
            if (context.HttpContext.Request.Headers.TryGetValue("Accept", out value))
            {
                MediaTypeHeaderValue header;
                if (MediaTypeHeaderValue.TryParse(value, out header))
                {
                    var found =
                        context.Configuration.AvailableSerializers.FirstOrDefault(f => f.MediaType == header.MediaType);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return context.Configuration.AvailableSerializers[0];
        }
    }
}
