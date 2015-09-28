﻿using System;
using System.IO;

namespace Bolt
{
    public interface ISerializer
    {
        string MediaType { get; }

        void Write(Stream stream, object data);

        object Read(Type type, Stream stream);

        IObjectSerializer CreateSerializer(Stream inputStream);

        IObjectSerializer CreateDeserializer(Stream inputStream);
    }

    public static class SerializerExtensions
    {
        public static T Read<T>(this ISerializer serializer, Stream stream)
        {
            return (T)serializer.Read(typeof(T), stream);
        }
    }
}
