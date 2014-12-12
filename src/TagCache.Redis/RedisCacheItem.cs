using System;
using System.Collections.Generic;

namespace TagCache.Redis
{
    public interface IRedisCacheItem
    {
        string Key { get; set; }
        List<string> Tags { get; set; }
        DateTime Expires { get; set; }
    }

    [Serializable]
    public class RedisCacheItem : IRedisCacheItem
    {
        public string Key { get; set; }
        public List<string> Tags { get; set; }
        public DateTime Expires { get; set; }
    }

    [Serializable]
    public class RedisCacheItem<T> : RedisCacheItem
    {
        public T Value { get; set; }
    }
}