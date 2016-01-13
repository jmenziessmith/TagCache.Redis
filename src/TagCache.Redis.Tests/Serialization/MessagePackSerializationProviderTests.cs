using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.MessagePack;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture]
    public class MessagePackSerializationProviderTests : SerializationProviderTestsBase<RedisCacheItem<TestObject>>
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new MessagePackSerializationProvider();
        }

        protected override Redis.CacheConfiguration GetCacheConfiguration(RedisConnectionManager redis)
        {
            return new MessagePack.CacheConfiguration(redis);
        }
    }
}