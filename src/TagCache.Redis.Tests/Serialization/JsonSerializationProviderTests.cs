using TagCache.Redis;
using TagCache.Redis.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCache.Redis.Tests.Serialization
{
    [TestClass]
    public class JsonSerializationProviderTests
    {

        [TestMethod]
        public void Serialize_RedisCacheItem_ReturnsString()
        {
            var value = new RedisCacheItem()
            {
                Expires = new DateTime(2014, 01, 02),
                Key = "JsonSerializationProviderTests.Key",
                Value = "Test",
                Tags = new List<string>{ "tag1", "tag2", "tag3" }
            };

            var serializer = new JsonSerializationProvider();

            var result = serializer.Serialize(value);

            Assert.IsNotNull(result);            
        }

        [TestMethod]
        public void Deserialize_SerializedString_ReturnsRedisCacheItem()
        {
            var value = new RedisCacheItem()
            {
                Expires = new DateTime(2014, 01, 02),
                Key = "JsonSerializationProviderTests.Key",
                Value = "Test",
                Tags = new List<string> { "tag1", "tag2", "tag3" }
            };

            var serializer = new JsonSerializationProvider();

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
