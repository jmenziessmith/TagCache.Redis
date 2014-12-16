using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.ProtoBuf
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
            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, value);
                var bytes = memoryStream.ToArray();
                return bytes;
            }
        }
    }
}