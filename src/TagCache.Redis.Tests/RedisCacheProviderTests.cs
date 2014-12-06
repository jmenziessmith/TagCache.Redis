using System;
using NUnit.Framework; 
using TagCache.Redis.Serialization;
using System.Collections.Generic; 
using System.Linq;
using TagCache.Redis.Tests.Helpers;

namespace TagCache.Redis.Tests
{
    [TestFixture]
    public class RedisCacheProviderTests
    {
        private string _redisHost = "localhost";
        private int _redisDB = 0;

        private RedisClient newRedisClient()
        {
            return new RedisClient(_redisHost, _redisDB, 5000);
        }



        [Test]
        public void Ctor_Configuration__Succeeds()
        {
            var config = new CacheConfiguration()
            {
                RootNameSpace = "_TestRootNamespace",
                Serializer =  new BinarySerializationProvider(),
                RedisClientConfiguration = new RedisClientConfiguration()
                {
                    Host = _redisHost,
                    DbNo = _redisDB,
                    TimeoutMilliseconds = 500
                }
            };
            var cache = new RedisCacheProvider(config);
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);
            cache.Set(key, value, expires);

            // no exception
        }



        [ExpectedException]
        [Test]
        public void Ctor_Configuration__Fails()
        {
            var config = new CacheConfiguration()
                         {
                             RedisClientConfiguration = new RedisClientConfiguration()
                                                        {
                                                            Host = "nohost",
                                                            TimeoutMilliseconds = 500
                                                        }
                         };
            var cache = new RedisCacheProvider(config);
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);
            cache.Set(key, value, expires);

            Assert.Fail("Exception should be thrown with bad connections");
        }

        [Test]
        public void Set_String_Succeeds()
        {
            var cache = new RedisCacheProvider();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11); 
            cache.Set(key, value, expires);

            // no exception
        }

        [Test]
        public void Get_MissingKey_ReturnsNull()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:NoValueHere." + DateTime.Now.Ticks;

            var result = cache.Get<String>(key);

            Assert.IsNull(result);
        }

        [Test]
        public void Get_AddedKey_ReturnsValue()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);

            cache.Set(key, value, expires);
            var result = cache.Get<String>(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
        }


        [Test]
        public void Get_AddedObject_ReturnsValue()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            var value = new TestObject()
                           {
                               Foo = "Hello",
                               Bar = "World",
                               Score = 11
                           };
            DateTime expires = new DateTime(2099, 12, 11);

            cache.Set(key, value, expires);
            var result = cache.Get<TestObject>(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value.Foo, result.Foo);
            Assert.AreEqual(value.Bar, result.Bar);
            Assert.AreEqual(value.Score, result.Score);
        }


        [Test]
        public void Remove_AddedKey_ReturnsNull()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);

            cache.Set(key, value, expires);

            var result = cache.Get<String>(key); 

            Assert.AreEqual(value, result); 

            cache.Remove(key);
            result = cache.Get<String>(key);

            Assert.IsNull(result); 
        }



        [Test]
        public void RemoveMultiple_AddedKey_ReturnsNull()
        {
            var cache = newRedisClient();
            string key1 = "TagCacheTests:Add.First";
            string key2 = "TagCacheTests:Add.Second";
            string value1 = "value1";
            string value2 = "value1";

            cache.Set(key1, value1,1);
            cache.Set(key2, value2,1);
            
            var result1 = cache.Get(key1);
            var result2 = cache.Get(key2);

            Assert.AreEqual(value1, result1);
            Assert.AreEqual(value2, result2);

            cache.Remove(new string[] { key1, key2 });

            result1 = cache.Get(key1);
            result2 = cache.Get(key2);

            Assert.IsNull(result1);
            Assert.IsNull(result2);
        }


        [Test]
        public void Get_ExpiredDate_ReturnsNull()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2000, 12, 11);

            cache.Set(key, value, expires); 
            var result = cache.Get<String>(key);

            Assert.IsNull(result);
        }


        [Test]
        public void Get_ExpiredDate_RemovesFromCache()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2000, 12, 11);

            cache.Set(key, value, expires);
            var result = cache.Get<String>(key);
              
            Assert.IsNull(result);
        }


        [Test]
        public void Get_ExpiredDate_RemovesTags()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            var tag = "remove tag";
            DateTime expires = new DateTime(2000, 12, 11);

            cache.Set(key, value, expires, tag);
            var test = cache.Get<String>(key);

            var tagManager = new RedisTagManager();

            var result = tagManager.GetKeysForTag(newRedisClient(), tag);

            Assert.AreEqual(result.Count(x=>x == tag), 0);
        }



        [Test]
        public void GetByTag_SingleItemManyTags_ReturnsSingleValue()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key = "TagCacheTests:Add";
            String value = "Hello World!";
            DateTime expires = new DateTime(2099, 12, 11);
            var tags = new List<string> { "tag1", "tag2", "tag3" };

            cache.Set(key, value, expires, tags);

            var results = cache.GetByTag<String>("tag2");

            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count > 0);
        }


        [Test]
        public void GetByTag_ManyItemSingleTag_ReturnsManyValues()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key1 = "TagCacheTests:Add1";
            string key2 = "TagCacheTests:Add2";
            string key3 = "TagCacheTests:Add3";
            String value1 = "Hello World!";
            String value3 = "Two";
            String value2 = "Three";
            DateTime expires = new DateTime(2099, 12, 11);
            var tags = new List<string> { "another tag" };

            cache.Set(key1, value1, expires, tags);
            cache.Set(key2, value2, expires, tags);
            cache.Set(key3, value3, expires, tags);

            var results = cache.GetByTag<String>(tags[0]);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 3);
        }



        [Test]
        public void RemoveByTag_ManyItemSingleTag_ReturnsNoValues()
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new TestRedisLogger();
            string key1 = "TagCacheTests:Add1";
            string key2 = "TagCacheTests:Add2";
            string key3 = "TagCacheTests:Add3";
            String value1 = "Hello World!";
            String value3 = "Two";
            String value2 = "Three";
            DateTime expires = new DateTime(2099, 12, 11);
            var tags = new List<string> { "another tag" };

            cache.Set(key1, value1, expires, tags);
            cache.Set(key2, value2, expires, tags);
            cache.Set(key3, value3, expires, tags);

            var results = cache.GetByTag<String>(tags[0]);

            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count, 3);


            cache.RemoveByTag(tags[0]);
            results = cache.GetByTag<String>(tags[0]);

            Assert.IsNull(results); 
        }


    }
}
