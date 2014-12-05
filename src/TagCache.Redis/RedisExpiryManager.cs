using System;
using System.Linq;

namespace TagCache.Redis
{
    public class RedisExpiryManager
    {
        public readonly string _setKey;

        public RedisExpiryManager(CacheConfiguration configuration)
        {
            _setKey = string.Format("{0}:_cacheExpireyKeys", configuration.RootNameSpace);
        }


        public void SetKeyExpiry(RedisClient client, string key, DateTime expiryDate)
        {
            client.SetTimeSet(_setKey, key, expiryDate);
        }

        public void RemoveKeyExpiry(RedisClient client, string[] keys)
        {
            client.RemoveTimeSet(_setKey, keys);
        }

        public string[] GetExpiredKeys(RedisClient client, DateTime maxDate)
        {
            var result = client.GetFromTimeSet(_setKey, maxDate);
            return result.Select(x=>x.Key).ToArray();
        } 

    }
}
