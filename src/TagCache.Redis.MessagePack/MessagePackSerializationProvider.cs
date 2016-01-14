using System;
using System.IO;
using System.Text;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;
using MsgPack.Serialization;

namespace TagCache.Redis.MessagePack
{
    public class MessagePackSerializationProvider : ISerializationProvider
    {
        public T Deserialize<T>(RedisValue value) where T : class
        {
            var type = GetConcreteType<T>();

            using (var stream = new MemoryStream(value))
            {
                var deserialized  = SerializationContext.Default.GetSerializer(type).Unpack(stream);
                return deserialized as T;
            }
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            var type = value.GetType();
            var serializer = SerializationContext.Default.GetSerializer(type);
            using (var stream = new MemoryStream())
            {
                serializer.Pack(stream, value);
                return stream.ToArray();
            }
        }

        private Type GetConcreteType<T>()
        {
            var objectType = typeof (T);

            if (objectType.IsInterface && typeof (IRedisCacheItem).IsAssignableFrom(objectType))
            {
                Type concreteType = typeof(RedisCacheItem);

                if (objectType.IsGenericType)
                {
                    concreteType = typeof(RedisCacheItem<>).MakeGenericType(objectType.GenericTypeArguments);
                }

                return concreteType;
            }

            return objectType;
        }
    }
}
