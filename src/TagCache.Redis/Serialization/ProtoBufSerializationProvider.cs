using System.IO;
using ProtoBuf;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.Serialization
{
    public class ProtoBufSerializationProvider : ISerializationProvider
    {
        public T Deserialize<T>(RedisValue value) where T : class
        {
            using (var memoryStream = new MemoryStream(value))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            using (var mStream = new MemoryStream())
            {
                Serializer.Serialize(mStream, value);
                var bytes = mStream.ToArray();
                return bytes;
            }
        }
    }
}