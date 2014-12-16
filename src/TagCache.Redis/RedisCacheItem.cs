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
        public virtual string Key { get; set; }
        
        public virtual List<string> Tags { get; set; }
        
        public virtual DateTime Expires { get; set; }
    }

    [Serializable]
    public class RedisCacheItem<T> : RedisCacheItem
    {
        public override string Key { get; set; }

        public override List<string> Tags { get; set; }

        public override DateTime Expires { get; set; }

        public T Value { get; set; }
    }
}