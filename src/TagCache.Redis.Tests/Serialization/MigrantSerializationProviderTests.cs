using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.Migrant;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture(Category = "Migrant tests,serialization")]
    public class MigrantSerializationProviderTests : SerializationProviderTestsBase<RedisCacheItem<TestObject>>
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new MigrantSerializationProvider();
        }
    }
}