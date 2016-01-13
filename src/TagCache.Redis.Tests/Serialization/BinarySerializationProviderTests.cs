using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.Serialization;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture]
    public class BinarySerializationProviderTests : SerializationProviderTestsBase<RedisCacheItem<TestObject>>
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new BinarySerializationProvider();
        }
        protected override Redis.CacheConfiguration GetCacheConfiguration(RedisConnectionManager redis)
        {
            return new Redis.CacheConfiguration(redis);
        }
    }
}