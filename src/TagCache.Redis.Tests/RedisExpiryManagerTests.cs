using NUnit.Framework;
using System;
using System.Linq;

namespace TagCache.Redis.Tests
{
    [TestFixture]
    public class RedisExpiryManagerTests
    {
        private string _redisHost = "localhost";
        private int _redisDB = 0;

        private RedisClient newRedisClient(RedisConnectionManager redis)
        {
            return new RedisClient(redis, _redisDB, 5000);
        }

        [Test]
        public void SetKeyExpiry_SetsValue()
        {
            var redis = new RedisConnectionManager(_redisHost);
            var client = newRedisClient(redis);
            client.Remove(new RedisExpiryManager(new CacheConfiguration(redis))._setKey);

            var expiryManager = new RedisExpiryManager(new CacheConfiguration(redis));
            var key = "my.expiringkey.1";

            expiryManager.SetKeyExpiry(client, key, new DateTime(2012, 1, 1, 12, 1, 1));

        }

        [Test]
        public void GetExpiredKeys_Date2_ReturnsKeysLessThanDate()
        {
            var redis = new RedisConnectionManager(_redisHost);
            var client = newRedisClient(redis);
            client.Remove(new RedisExpiryManager(new CacheConfiguration(redis))._setKey);

            var expiryManager = new RedisExpiryManager(new CacheConfiguration(redis));
            var key1 = "my.expiringkey.1";
            var key2 = "my.expiringkey.2";
            var key3 = "my.expiringkey.3";

            expiryManager.SetKeyExpiry(client, key1, new DateTime(2012, 1, 1, 12, 1, 1));
            expiryManager.SetKeyExpiry(client, key2, new DateTime(2012, 1, 1, 12, 1, 2));
            expiryManager.SetKeyExpiry(client, key3, new DateTime(2012, 1, 1, 12, 1, 3));

            var result = expiryManager.GetExpiredKeys(client, new DateTime(2012, 1, 1, 12, 1, 2));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains(key1), "key 1 should exist");
            Assert.IsTrue(result.Contains(key2), "key 2 should exist");
            Assert.IsFalse(result.Contains(key3), "key 3 should not exist");
            Assert.AreEqual(2, result.Count());

        }



        [Test]
        public void GetExpiredKeys_DateMax_ReturnsKeysLessThanDate()
        {
            var redis = new RedisConnectionManager(_redisHost);
            var client = newRedisClient(redis);
            client.Remove(new RedisExpiryManager(new CacheConfiguration(redis))._setKey);

            var expiryManager = new RedisExpiryManager(new CacheConfiguration(redis));
            var key1 = "my.expiringkey.1";
            var key2 = "my.expiringkey.2";
            var key3 = "my.expiringkey.3";

            expiryManager.SetKeyExpiry(client, key1, new DateTime(2012, 1, 1, 12, 1, 1));
            expiryManager.SetKeyExpiry(client, key2, new DateTime(2015, 1, 1, 12, 1, 2));
            expiryManager.SetKeyExpiry(client, key3, new DateTime(2020, 1, 1, 12, 1, 3));

            var result = expiryManager.GetExpiredKeys(client, new DateTime(2020, 1, 1, 12, 1, 5));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains(key1), "key 1 should exist");
            Assert.IsTrue(result.Contains(key2), "key 2 should exist");
            Assert.IsTrue(result.Contains(key3), "key 3 should exist");
            Assert.AreEqual(3, result.Count());

        }

        [Test]
        public void GetExpiredKeys_DateMin_ReturnsNone()
        {
            var redis = new RedisConnectionManager(_redisHost);
            var client = newRedisClient(redis);
            client.Remove(new RedisExpiryManager(new CacheConfiguration(redis))._setKey);

            var expiryManager = new RedisExpiryManager(new CacheConfiguration(redis));
            var key1 = "my.expiringkey.1";
            var key2 = "my.expiringkey.2";
            var key3 = "my.expiringkey.3";

            expiryManager.SetKeyExpiry(client, key1, new DateTime(2012, 1, 1, 12, 1, 1));
            expiryManager.SetKeyExpiry(client, key2, new DateTime(2012, 1, 1, 12, 1, 2));
            expiryManager.SetKeyExpiry(client, key3, new DateTime(2012, 1, 1, 12, 1, 3));

            var result = expiryManager.GetExpiredKeys(client, new DateTime(2012, 1, 1, 12, 1, 0));
            Assert.IsNotNull(result);

            Assert.AreEqual(0, result.Count());

        }


        [Test]
        public void CleanupExpiredKeys_RemovesOldItems()
        {
            var redis = new RedisConnectionManager(_redisHost);
            var client = newRedisClient(redis);
            var provider = new RedisCacheProvider(redis);

            client.Remove(new RedisExpiryManager(new CacheConfiguration(redis))._setKey);

            var expiryManager = new RedisExpiryManager(new CacheConfiguration(redis));
            var key1 = "my.expiringkey.1";
            var key2 = "my.expiringkey.2";
            var key3 = "my.expiringkey.3";

            expiryManager.SetKeyExpiry(client, key1, new DateTime(2012, 1, 1, 12, 1, 1));
            expiryManager.SetKeyExpiry(client, key2, new DateTime(2015, 1, 1, 12, 1, 2));
            expiryManager.SetKeyExpiry(client, key3, new DateTime(2020, 1, 1, 12, 1, 3));

            var result = expiryManager.GetExpiredKeys(client, new DateTime(2020, 1, 1, 12, 1, 5));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Contains(key1), "key 1 should exist");
            Assert.IsTrue(result.Contains(key2), "key 2 should exist");
            Assert.IsTrue(result.Contains(key3), "key 3 should exist");
            Assert.AreEqual(3, result.Count());

            expiryManager.RemoveKeyExpiry(client, result);

            result = expiryManager.GetExpiredKeys(client, new DateTime(2020, 1, 1, 12, 1, 5));
            Assert.AreEqual(0, result.Count());

        }
    }
}
