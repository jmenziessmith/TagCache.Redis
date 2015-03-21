using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.Json.Net;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture]
    public class JsonSerializationProviderTests : SerializationProviderTestsBase<RedisCacheItem<TestObject>>
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new JsonSerializationProvider();
        }
    }
}
