using System;
using System.Collections.Generic;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis
{
    [Serializable]    
    public class RedisCacheItem : IRedisCacheItem
    {        
        public string Key { get; set; }
                
        public List<string> Tags { get; set; }
                
        public DateTime Expires { get; set; }
    }

    [Serializable]    
    public class RedisCacheItem<T> : RedisCacheItem, IRedisCacheItem<T>
    {        
        public T Value { get; set; }
    }
}