using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.Serialization
{
    public class BinarySerializationProvider : ISerializationProvider
    {
        readonly BinaryFormatter formatter = new BinaryFormatter();

        public T Deserialize<T>(RedisValue value) where T : class
        {
            using (var memoryStream = new MemoryStream(value))
            {
                return (T) formatter.Deserialize(memoryStream);
            }
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            using (var mStream = new MemoryStream())
            {
                formatter.Serialize(mStream, value);
                var bytes = mStream.ToArray();
                return bytes;
            }
        }
    }
}
