using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis
{
    public class RedisCacheItemFactory : IRedisCacheItemFactory
    {
        public IRedisCacheItem Create(string key, List<string> tags)
        {
            return new RedisCacheItem()
            {
                Key = key,
                Tags = tags
            };
        }

        public IRedisCacheItem<T> Create<T>(string key, List<string> tags, DateTime expires, T value)
        {
            return new RedisCacheItem<T>()
            {
                Key = key,
                Tags = tags,
                Expires = expires,
                Value = value
            };
        }
    }
}
