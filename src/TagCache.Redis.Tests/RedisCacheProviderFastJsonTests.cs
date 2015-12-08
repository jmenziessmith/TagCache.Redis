using NUnit.Framework;
using TagCache.Redis.FastJson;

namespace TagCache.Redis.Tests
{
    [TestFixture(Category = "Fast JSON tests")]
    public class RedisCacheProviderFastJsonTests : RedisCacheProviderTests
    {
        protected override CacheConfiguration NewCacheConfiguration(RedisConnectionManager connection)
        {
            return new CacheConfiguration(connection)
            {
                Serializer = new FastJsonSerializationProvider()
            };
        }
    }
}