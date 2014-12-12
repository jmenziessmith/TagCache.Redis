using NUnit.Framework;
using TagCache.Redis.Serialization;

namespace TagCache.Redis.Tests
{
    [TestFixture(Category = "JSON tests")]
    public class RedisCacheProviderJsonTests : RedisCacheProviderTests
    {
        protected override CacheConfiguration NewCacheConfiguration(RedisConnectionManager connection)
        {
            return new CacheConfiguration(connection)
            {
                Serializer = new JsonSerializationProvider()
            };
        }
    }
}