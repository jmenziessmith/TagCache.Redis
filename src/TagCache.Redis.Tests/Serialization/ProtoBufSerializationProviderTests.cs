using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using TagCache.Redis.ProtoBuf;

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture(Category = "Proto-buf tests,serialization")]
    public class ProtoBufSerializationProviderTests
    {
        [TestFixtureSetUp]
        public void ProtoBufSerializationProvider()
        {
            //Setup attributless serialization settings for RedisCacheItem.
            var redisCacheItemType = RuntimeTypeModel.Default.Add(typeof(RedisCacheItem<string>), false);
            redisCacheItemType.Add("Key", "Tags", "Expires", "Value");
        }

        [Test]
        public void Serialize_RedisCacheItem_ReturnsString()
        {
            var value = new RedisCacheItem<string>
            {
                Expires = new DateTime(2014, 01, 02),
                Key = "JsonSerializationProviderTests.Key",
                Value = "Test",
                Tags = new List<string> { "tag1", "tag2", "tag3" }
            };

            var serializer = new ProtoBufSerializationProvider();

            var result = serializer.Serialize(value);

            Assert.IsNotNull(result);
        }

        [Test]
        public void Deserialize_SerializedString_ReturnsRedisCacheItem()
        {
            var value = new RedisCacheItem<string>
            {
                Expires = new DateTime(2014, 01, 02),
                Key = "JsonSerializationProviderTests.Key",
                Value = "Test",
                Tags = new List<string> { "tag1", "tag2", "tag3" }
            };

            var serializer = new ProtoBufSerializationProvider();

            var serialized = serializer.Serialize(value);

            var result = serializer.Deserialize<RedisCacheItem<string>>(serialized);

            Assert.IsNotNull(result);
            Assert.AreEqual(value.Expires, result.Expires);
            Assert.AreEqual(value.Key, result.Key);
            Assert.AreEqual(value.Value, result.Value);
            Assert.IsTrue((value.Tags.Count() == result.Tags.Count()) && !value.Tags.Except(result.Tags).Any());
        }
    }
}