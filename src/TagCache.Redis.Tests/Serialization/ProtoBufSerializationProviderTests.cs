using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.ProtoBuf;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture(Category = "Proto-buf tests,serialization")]
    public class ProtoBufSerializationProviderTests : SerializationProviderTestsBase<ProtobufRedisCacheItem<TestObject>>
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new ProtoBufSerializationProvider();
        }
    }
}