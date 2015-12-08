using NUnit.Framework; 

namespace TagCache.Redis.Tests
{
    [TestFixture(Category = "Migrant tests")]
    public class RedisCacheProviderMigrantTests : RedisCacheProviderTests
    {
        protected override CacheConfiguration NewCacheConfiguration(RedisConnectionManager connection)
        {
            return new CacheConfiguration(connection);
        }
    }
}