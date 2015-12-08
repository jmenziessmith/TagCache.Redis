using System.IO;
using Antmicro.Migrant;
using Antmicro.Migrant.Customization;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.Migrant
{
    public class MigrantSerializationProvider : ISerializationProvider
    {
        public T Deserialize<T>(RedisValue value) where T : class
        {
            using (var stream = new MemoryStream(value))
            {
                T result;
                new Serializer(new Settings(supportForISerializable: true)).TryDeserialize(stream, out result);
                return result;
            }
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            using (var stream = new MemoryStream())
            {
                new Serializer(new Settings(supportForISerializable: true)).Serialize(value, stream);
                return stream.ToArray();
            }
        }
    }
}
