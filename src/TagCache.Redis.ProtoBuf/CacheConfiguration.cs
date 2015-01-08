namespace TagCache.Redis.ProtoBuf
{
    public class CacheConfiguration : Redis.CacheConfiguration
    {
        public CacheConfiguration(RedisConnectionManager connectionManager) : base(connectionManager)
        {
            Serializer = new ProtoBufSerializationProvider(new ProtobufSerializationConfiguration());
        } 
    }
}
