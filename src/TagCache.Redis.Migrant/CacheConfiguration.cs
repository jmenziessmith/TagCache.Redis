namespace TagCache.Redis.Migrant
{
    public class CacheConfiguration : Redis.CacheConfiguration
    {
        public CacheConfiguration(RedisConnectionManager connectionManager) : base(connectionManager)
        {
            Serializer = new MigrantSerializationProvider(); 
        }

    }
}
