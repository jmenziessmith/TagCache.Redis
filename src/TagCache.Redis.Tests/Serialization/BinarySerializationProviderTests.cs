using NUnit.Framework; 
using TagCache.Redis.Serialization; 
using System;
using System.Collections.Generic;
using System.Linq; 

namespace TagCache.Redis.Tests.Serialization
{
    [TestFixture]
    public class BinarySerializationProviderTests
    {

        [Test]
        public void Serialize_RedisCacheItem_ReturnsString()
        {
            var value = new RedisCacheItem<string>
            {
                Expires = new DateTime(2014, 01, 02),
                Key = "JsonSerializationProviderTests.Key",
                Value = "Test",
                Tags = new List<string>{ "tag1", "tag2", "tag3" }
            };

            var serializer = new BinarySerializationProvider();

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

            var serializer = new BinarySerializationProvider();

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
