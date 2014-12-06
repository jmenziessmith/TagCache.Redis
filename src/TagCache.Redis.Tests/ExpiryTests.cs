using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading; 
using NUnit.Framework;
using TagCache.Redis.Tests.Helpers;

namespace TagCache.Redis.Tests
{
    [TestFixture]
    public class ExpiryTests
    {
        private RedisClient newRedisClient(RedisClientConfiguration config)
        {
            return new RedisClient(config.Host, config.DbNo, config.TimeoutMilliseconds);
        }

        [Test]
        public void ItemExpires_RemovedFromCache()
        {
            // this is just testing the built in expiry
            var config = new CacheConfiguration();

            var cache = new RedisCacheProvider(config); 
            cache.Logger = new TestRedisLogger();

            string key = "TagCacheTests:ItemExpires_RemovedFromCache";
            String value = "Hello World!";
            DateTime expires = DateTime.Now.AddSeconds(3);

            string tag1 = "tag1";
            string tag2 = "tag2";

            cache.Set(key, value, expires, new List<string> { tag1, tag2 });
            var result = cache.Get<String>(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);
            
            Thread.Sleep(1000);

            result = cache.Get<String>(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            Thread.Sleep(2500);

            result = cache.Get<String>(key);

            Assert.IsNull(result);  
        }



        [Test]
        public void ItemExpires_Tag_RemovedFromCache()
        {
            // this is testing that when expiry happens, the events kick in and remove the tags
            // see RedisExpireHandler

            var config = new CacheConfiguration();

            var cache = new RedisCacheProvider(config);
            var client = newRedisClient(config.RedisClientConfiguration);

            cache.Logger = new TestRedisLogger();
            Console.WriteLine("Start Logger");

            string key = "TagCacheTests:ItemExpires_Tag_RemovedFromCache";
            String value = "Hello World!";
            DateTime expires = DateTime.Now.AddSeconds(3);
            
            string tag1 = "tag1001";
            string tag2 = "tag1002";

            cache.Set(key, value, expires, new List<string>{tag1, tag2});

            // first check everything has been set
            var result = cache.Get<String>(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            var keysForTag1 = client.GetKeysForTag(tag1);
            Assert.IsNotNull(keysForTag1);
            Assert.IsTrue(keysForTag1.Any(x => x == key));

            var tagsForKey = client.GetTagsForKey(key);
            Assert.IsNotNull(tagsForKey);
            Assert.IsTrue(tagsForKey.Any(x => x == tag1));
            Assert.IsTrue(tagsForKey.Any(x => x == tag2));

            // wait a while
            Thread.Sleep(1000);

            // check it has not expired yet
            result = cache.Get<String>(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value, result);

            keysForTag1 = client.GetKeysForTag(tag1);
            Assert.IsNotNull(keysForTag1);
            Assert.IsTrue(keysForTag1.Any(x => x == key));

            tagsForKey = client.GetTagsForKey(key);
            Assert.IsNotNull(tagsForKey);
            Assert.IsTrue(tagsForKey.Any(x => x == tag1));
            Assert.IsTrue(tagsForKey.Any(x => x == tag2));

            // now wait until it should have been removed
            Thread.Sleep(2500);
            
            // now check its all been removed
            result = cache.Get<String>(key);

            Assert.IsNull(result);
            
            keysForTag1 = client.GetKeysForTag(tag1);
            Assert.IsNotNull(keysForTag1);
            Assert.IsFalse(keysForTag1.Any(x => x == key));

            tagsForKey = client.GetTagsForKey(key);
            Assert.IsNotNull(tagsForKey);
            Assert.IsFalse(tagsForKey.Any(x => x == tag1));
            Assert.IsFalse(tagsForKey.Any(x => x == tag2));

        }
    }
}
