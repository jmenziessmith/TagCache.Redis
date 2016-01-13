using Newtonsoft.Json;

namespace TagCache.Redis.Json.Net
{
    public class CacheConfiguration : TagCache.Redis.CacheConfiguration
    {
        public CacheConfiguration(RedisConnectionManager connectionManager)
            : base(connectionManager)
        {
            this.Serializer = new JsonSerializationProvider();
        }

        public CacheConfiguration(RedisConnectionManager connectionManager, JsonSerializerSettings settings)
            : base(connectionManager)
        {
            this.Serializer = new JsonSerializationProvider(settings);
        }
    }
}
