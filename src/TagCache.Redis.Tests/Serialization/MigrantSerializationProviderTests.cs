using NUnit.Framework;
using TagCache.Redis.Interfaces;
using TagCache.Redis.Migrant;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture(Category = "Migrant tests,serialization")]
    public class MigrantSerializationProviderTests : SerializationProviderTestsBase
    {
        protected override ISerializationProvider GetSerializer()
        {
            return new MigrantSerializationProvider();
        }
    }
}