using NUnit.Framework;
using TagCache.Redis.Serialization;

namespace TagCache.Redis.Tests
{
    [TestFixture]
    public class RedisCacheProviderXmlTests : RedisCacheProviderTests
    {
        protected override CacheConfiguration NewCacheConfiguration(RedisConnectionManager connection)
        {
            return new CacheConfiguration(connection)
            {
                Serializer = new XmlSerializationProvider()
            };
        }
    }
}