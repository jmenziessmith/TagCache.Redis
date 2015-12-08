using System;
using System.Collections.Generic;

namespace TagCache.Redis.Interfaces
{
    public interface IRedisCacheItemFactory
    {
        IRedisCacheItem Create(string key, List<string> tags);
        IRedisCacheItem<T> Create<T>(string key, List<string> tags, DateTime expires, T value);        
    }
}