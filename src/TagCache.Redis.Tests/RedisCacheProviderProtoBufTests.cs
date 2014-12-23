using NUnit.Framework;
using TagCache.Redis.ProtoBuf;

namespace TagCache.Redis.Tests
{
    [TestFixture(Category = "Proto-buf tests")]
    public class RedisCacheProviderProtoBufTests : RedisCacheProviderTests
    {
        protected override CacheConfiguration NewCacheConfiguration(RedisConnectionManager connection)
        {
            return new ProtoBuf.CacheConfiguration(connection);
        }
    }
}