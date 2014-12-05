using System;
using TagCache.Redis;
using TagCache.Redis.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic; 
using System.Linq;

namespace TagCache.Redis.Tests
{
    [TestClass]
    public class RedisCacheProviderTests
    {
        private string _redisHost = "localhost";
        private int _redisDB = 12;

        private RedisClient newRedisClient()
        {
            return new RedisClient(_redisHost, _redisDB, 5000);
        }

        [TestMethod]
        public void Set_String_Succeeds()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);

            client.Set(key, value, expires);

            // no exception
        }

        [TestMethod]
        public void Get_MissingKey_ReturnsNull()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.NoValueHere." + DateTime.Now.Ticks;

            var result = client.Get<String>(key);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Get_AddedKey_ReturnsValue()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);

            client.Set(value, key, expires);
            var result = client.Get<String>(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
        }



        [TestMethod]
        public void Remove_AddedKey_ReturnsNull()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);

            client.Set(value, key, expires);

            var result = client.Get<String>(key); 

            Assert.AreEqual(value, result); 

            client.Remove(key);
            result = client.Get<String>(key);

            Assert.IsNull(result); 
        }



        [TestMethod]
        public void RemoveMultiple_AddedKey_ReturnsNull()
        {
            var client = newRedisClient();
            string key1 = "RedisClientTests.Add.First";
            string key2 = "RedisClientTests.Add.Second";
            string value1 = "value1";
            string value2 = "value1";

            client.Set(key1, value1,1);
            client.Set(key2, value2,1);
            
            var result1 = client.Get(key1);
            var result2 = client.Get(key2);

            Assert.AreEqual(value1, result1);
            Assert.AreEqual(value2, result2);

            client.Remove(new string[] { key1, key2 });

            result1 = client.Get(key1);
            result2 = client.Get(key2);

            Assert.IsNull(result1);
            Assert.IsNull(result2);
        }


        [TestMethod]
        public void Get_ExpiredDate_ReturnsNull()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2000, 12, 11);

            client.Set(value, key, expires); 
            var result = client.Get<String>(key);

            Assert.IsNull(result);
        }


        [TestMethod]
        public void Get_ExpiredDate_RemovesFromCache()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2000, 12, 11);

            client.Set(value, key, expires);
            var test = client.Get<String>(key);

            var cacheItemProvider = new RedisCacheItemProvider(new JsonSerializationProvider());

            var result = cacheItemProvider.Get(newRedisClient(), key);

            Assert.IsNull(result);
        }


        [TestMethod]
        public void Get_ExpiredDate_RemovesTags()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value = "Hello World!";
            var tag = "remove tag";
            DateTime expires = new DateTime(2000, 12, 11);

            client.Set(value, key, expires, tag);
            var test = client.Get<String>(key);

            var tagManager = new RedisTagManager();

            var result = tagManager.GetKeysForTag(newRedisClient(), tag);

            Assert.AreEqual(result.Count(x=>x == tag), 0);
        }



        [TestMethod]
        public void GetByTag_SingleItemManyTags_ReturnsSingleValue()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);
            var tags = new List<string> { "tag1", "tag2", "tag3" };

            client.Set(key, value, expires, tags);

            var results = client.GetByTag<String>("tag2");

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count > 0);
        }


        [TestMethod]
        public void GetByTag_ManyItemSingleTag_ReturnsManyValues()
        {
            var client = new RedisCacheProvider();
            string key = "RedisClientTests.Add";
            String value1 = "Hello World!";
            String value3 = "Two";
            String value2 = "Three";
            DateTime expires = new DateTime(2099, 12, 11);
            var tags = new List<string> { "another tag" };

            client.Set(key, value1, expires, tags);
            client.Set(key, value2, expires, tags);
            client.Set(key, value3, expires, tags);

            var results = client.GetByTag<String>(tags[0]);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 3);
        }


    }
}
