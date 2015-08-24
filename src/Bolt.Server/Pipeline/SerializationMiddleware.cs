﻿using System;
using System.IO;
using System.Threading.Tasks;

using Bolt.Metadata;
using Bolt.Pipeline;

namespace Bolt.Server.Pipeline
{
    public class SerializationMiddleware : MiddlewareBase<ServerActionContext>
    {
        public override async Task InvokeAsync(ServerActionContext context)
        {
            var actionMetadata = context.EnsureActionMetadata();
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
            IObjectSerializer rawParameters;
            try
            {
                rawParameters =
                    context.Configuration.Serializer.CreateDeserializer(
                        await context.HttpContext.Request.Body.CopyAsync(context.RequestAborted));
            }
            catch (Exception e)
            {
                throw new BoltServerException(
                    $"Failed to deserialize parameters for action '{context.Action.Name}'.",
                    ServerErrorCode.DeserializeParameters,
                    context.Action,
                    context.RequestUrl,
                    e);
            }

            using (rawParameters)
            {
                object[] parameterValues = new object[metadata.Parameters.Length];
                for (int i = 0; i < metadata.Parameters.Length; i++)
                {
                    if (i == metadata.CancellationTokenIndex)
                    {
                        parameterValues[i] = context.RequestAborted;
                        continue;
                    }

                    var parameter = metadata.Parameters[i];

                    try
                    {
                        object val;
                        if (rawParameters.TryRead(parameter.Name, parameter.Type, out val))
                        {
                            parameterValues[i] = val;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new BoltServerException(
                            $"Failed to deserialize parameter '{parameter.Name}' for action '{context.Action.Name}'.",
                            ServerErrorCode.DeserializeParameters,
                            context.Action,
                            context.RequestUrl,
                            e);
                    }
                }

                return parameterValues;
            }

        }

        protected virtual async Task HandleResponse(ServerActionContext context)
        {
            context.RequestAborted.ThrowIfCancellationRequested();
            context.HttpContext.Response.StatusCode = 200;

            if (context.EnsureActionMetadata().HasResult && context.ActionResult != null)
            {
                MemoryStream stream = new MemoryStream();
                try
                {
                    context.Configuration.Serializer.Write(stream, context.ActionResult);
                }
                catch (Exception e)
                {
                    throw new BoltServerException(
                        $"Failed to serialize response for action '{context.Action.Name}'.",
                        ServerErrorCode.SerializeResponse,
                        context.Action,
                        context.RequestUrl,
                        e);
                }

                stream.Seek(0, SeekOrigin.Begin);

                if (stream.Length > 0)
                {
                    context.HttpContext.Response.ContentLength = stream.Length;
                    context.HttpContext.Response.ContentType = context.Configuration.Serializer.ContentType;

                    await stream.CopyToAsync(context.HttpContext.Response.Body, BoltFramework.DefaultBufferSize, context.RequestAborted);
                }
                else
                {
                    context.HttpContext.Response.ContentLength = 0;
                }
            }
            else
            {
                context.HttpContext.Response.ContentLength = 0;
            }

            context.HttpContext.Response.Body.Dispose();
        }
    }
}
