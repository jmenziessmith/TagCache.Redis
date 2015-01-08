using System;
using System.Collections.Generic;

namespace TagCache.Redis.Interfaces
{
    public interface IRedisCacheItem
    {
        string Key { get; set; }
        List<string> Tags { get; set; }
        DateTime Expires { get; set; }
    }

    public interface IRedisCacheItem<T> : IRedisCacheItem
    {
        T Value { get; set; }
    }
}