using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.ProtoBuf
{
    public class ProtobufRedisCacheItemFactory : IRedisCacheItemFactory
    {
        public IRedisCacheItem Create(string key, List<string> tags)
        {
            return new ProtobufRedisCacheItem()
            {
                Key = key,
                Tags = tags
            };
        }

        public IRedisCacheItem<T> Create<T>(string key, List<string> tags, DateTime expires, T value)
        {
            return new ProtobufRedisCacheItem<T>()
            {
                Key = key,
                Tags = tags,
                Expires = expires,
                Value = value
            };
        }
    }
}
