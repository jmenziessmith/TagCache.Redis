using NUnit.Framework; 
using System;
using System.Collections.Generic;
using System.Linq; 
using TagCache.Redis.Serialization; 

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture]
    public class XmlSerializationProviderTests
    {

        [Test]
        public void Serialize_RedisCacheItem_ReturnsString()
        {
            var value = new RedisCacheItem()
            {
                Expires = new DateTime(2014, 01, 02),
                Key = "XmlSerializationProviderTests.Key",
                Value = 12,
                Tags = new List<string>{ "tag1", "tag2", "tag3" }
            };

            var serializer = new XmlSerializationProvider();

            var result = serializer.Serialize(value);

            Assert.IsNotNull(result);            
        }

        [Test]
        public void Deserialize_SerializedString_ReturnsRedisCacheItem()
        {
            var value = new RedisCacheItem()
            {
                Expires = new DateTime(2014, 01, 02),
                Key = "XmlSerializationProviderTests.Key",
                Value = 12,
                Tags = new List<string>{ "tag1", "tag2", "tag3" }
            };

            var serializer = new XmlSerializationProvider();

            var serialized = serializer.Serialize(value);

            var result = serializer.Deserialize<RedisCacheItem>(serialized);


            Assert.IsNotNull(result);
            Assert.AreEqual(value.Expires, result.Expires);
            Assert.AreEqual(value.Key, result.Key);
            Assert.AreEqual(value.Value, result.Value);
            Assert.IsTrue((value.Tags.Count() == result.Tags.Count()) && !value.Tags.Except(result.Tags).Any()); 
        }
    }
}
