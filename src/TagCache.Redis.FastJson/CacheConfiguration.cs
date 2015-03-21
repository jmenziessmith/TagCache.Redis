namespace TagCache.Redis.FastJson
{
    public class CacheConfiguration : TagCache.Redis.CacheConfiguration
    {
        public CacheConfiguration(RedisConnectionManager connectionManager) : base(connectionManager)
        {
            this.Serializer = new FastJsonSerializationProvider();
        } 
    }
}
