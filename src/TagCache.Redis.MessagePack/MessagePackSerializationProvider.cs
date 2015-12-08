using System.IO;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;
using MsgPack.Serialization;

namespace TagCache.Redis.MessagePack
{
    public class MessagePackSerializationProvider : ISerializationProvider
    {
        public T Deserialize<T>(RedisValue value) where T : class
        {
            using (var stream = new MemoryStream(value))
            {
                return SerializationContext.Default.GetSerializer<T>().Unpack(stream);
            }
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            using (var stream = new MemoryStream())
            {
                SerializationContext.Default.GetSerializer<T>().Pack(stream, value);
                return stream.ToArray();
            }
        }
    }
}
