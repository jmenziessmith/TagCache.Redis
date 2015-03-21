using StackExchange.Redis;
using TagCache.Redis.Interfaces;
using Newtonsoft.Json;

namespace TagCache.Redis.Json.Net
{
    public class JsonSerializationProvider : ISerializationProvider
    {
        public T Deserialize<T>(RedisValue value) where T : class
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
