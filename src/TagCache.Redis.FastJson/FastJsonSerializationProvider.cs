using StackExchange.Redis;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.FastJson
{
    public class FastJsonSerializationProvider : ISerializationProvider
    {
        public T Deserialize<T>(RedisValue value) where T : class
        {
            return fastJSON.JSON.ToObject<T>(value);
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            return fastJSON.JSON.ToJSON(value);
        }
    }
}