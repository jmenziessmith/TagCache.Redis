using NUnit.Framework;
using TagCache.Redis.Migrant;

namespace TagCache.Redis.Tests
{
    [TestFixture(Category = "Migrant tests")]
    public class RedisCacheProviderMigrantTests : RedisCacheProviderTests
    {
        protected override CacheConfiguration NewCacheConfiguration(RedisConnectionManager connection)
        {
            return new CacheConfiguration(connection)
            {
                Serializer = new MigrantSerializationProvider()
            };
        }
    }
}