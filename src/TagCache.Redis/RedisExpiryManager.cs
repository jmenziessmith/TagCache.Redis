using System;
using System.Linq;

namespace TagCache.Redis
{
    public class RedisExpiryManager
    {
        public static readonly string _setKey = "_redisCache:_cacheExpireyKeys";


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
