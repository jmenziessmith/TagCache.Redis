 using NUnit.Framework;  

namespace TagCache.Redis.Tests
{
    [TestFixture]
    public class TagCacheTests
    {
        private string _redisHost = "localhost";
        private int _redisDB = 0;

        private RedisClient newRedisClient()
        {
            return new RedisClient(new RedisConnectionManager("localhost"), _redisDB, 5000);
        }
         

        [Test]
        public void Add_String_Succeeds()
        {
            var client = newRedisClient();
            string key = "TagCacheTests:Add";
            string value = "Hello World!";
            
            client.Set(key, value,5);
            
            // no exception
        }

        [Test]
        public void Get_MissingKey_ReturnsNull()
        {
            var client = newRedisClient();
            string key = "TagCacheTests:NoValueHere";

            var result = client.Get(key);

            Assert.IsNull(result);
        }

        [Test]
        public void Get_AddedKey_ReturnsValue()
        {
            var client = newRedisClient();
            string key = "TagCacheTests:Add";
            string value = "Hello World!";

            client.Set(key, value,5);
            var result = client.Get(key);

            Assert.IsNotNull(result);
            Assert.AreEqual(value, (string)result);
        }



        [Test]
        public void Remove_AddedKey_ReturnsNull()
        {
            var client = newRedisClient();
            string key = "TagCacheTests:Add";
            string value = "Hello World!";

            client.Set(key, value,5);

            var result = client.Get(key); 

            Assert.AreEqual(value, (string)result); 

            client.Remove(key);
            result = client.Get(key);

            Assert.IsNull(result); 
        }



        [Test]
        public void RemoveMultiple_AddedKey_ReturnsNull()
        {
            var client = newRedisClient();
            string key1 = "TagCacheTests:Add.First";
            string key2 = "TagCacheTests:Add.Second";
            string value1 = "value1";
            string value2 = "value1";

            client.Set(key1, value1,10);
            client.Set(key2, value2,10);
            
            var result1 = client.Get(key1);
            var result2 = client.Get(key2);

            Assert.AreEqual(value1, (string)result1);
            Assert.AreEqual(value2, (string)result2);

            client.Remove(new string[] { key1, key2 });

            result1 = client.Get(key1);
            result2 = client.Get(key2);

            Assert.IsNull(result1);
            Assert.IsNull(result2);
        }
    }
}
