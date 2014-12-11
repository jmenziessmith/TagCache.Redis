using System;
using System.Collections.Generic;
using System.Linq;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis
{
    public class RedisCacheItemProvider
    {
        private ISerializationProvider _serializer;

        public RedisCacheItemProvider(ISerializationProvider serializer)
        {
            _serializer = serializer;
        }

        public RedisCacheItem Get(RedisClient client, string key)
        {
            var cacheString = client.Get(key);
            if (!string.IsNullOrEmpty(cacheString))
            {
                return _serializer.Deserialize<RedisCacheItem>(cacheString);
            }
            return null;
        }


        public List<RedisCacheItem> GetMany(RedisClient client, string[] keys)
        {
            var result = new List<RedisCacheItem>();

            foreach (var key in keys)
            {
                var r = Get(client, key);
                if (r != null)
                {
                    result.Add(r);
                }
            }

            return result;
        }

        public bool Set<T>(RedisClient client, T value, string key, DateTime expires, IEnumerable<string> tags)
        {
            if (value != null)
            {
                var cacheItem = Create(value, key, expires, tags);
                var serialized = _serializer.Serialize(cacheItem);
                int expirySeconds = GetExpirySeconds(expires);
                client.Set(key, serialized, expirySeconds);
                return true;
            }
            return false;
        }


        private RedisCacheItem Create(object value, string key, DateTime expires, IEnumerable<string> tags)
        {
            return new RedisCacheItem
            {
                Value = value,
                Key = key,
                Expires = expires,
                Tags = tags == null ? null : tags.ToList(), 
            };
        }

        /// <summary>
        /// calculates how long until the item expires, and adds additional time to allow the cache client to manually expire the item first and handle tags properly
        /// </summary>
        /// <param name="expires"></param>
        /// <returns></returns>
        private int GetExpirySeconds(DateTime expires)
        {
            const int additionalSeconds = 86400; // 1 day
            var seconds = expires.Subtract(DateTime.Now).TotalSeconds; 
            var result = (int)seconds;
            return Math.Max(1,result);
        }

    }
}
