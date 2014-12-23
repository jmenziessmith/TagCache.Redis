using NUnit.Framework;
using TagCache.Redis.MessagePack;

namespace TagCache.Redis.Tests
{
    [TestFixture(Category = "MsgPack tests")]
    public class RedisCacheProviderMessagePackTests : RedisCacheProviderTests
    {
        protected override CacheConfiguration NewCacheConfiguration(RedisConnectionManager connection)
        {
            return new CacheConfiguration(connection)
            {
                Serializer = new MessagePackSerializationProvider()
            };
        }
    }
}