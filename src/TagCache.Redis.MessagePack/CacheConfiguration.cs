namespace TagCache.Redis.MessagePack
{
    public class CacheConfiguration : Redis.CacheConfiguration
    {
        public CacheConfiguration(RedisConnectionManager connectionManager) : base(connectionManager)
        {
            Serializer = new MessagePackSerializationProvider(); 
        }

    }
}
