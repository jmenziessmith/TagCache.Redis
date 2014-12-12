using System;
using System.Collections.Generic;
using ProtoBuf;

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
    [ProtoContract]
    public class RedisCacheItem<T> : RedisCacheItem
    {
        [ProtoMember(1)]
        public override string Key { get; set; }

        [ProtoMember(2)]
        public override List<string> Tags { get; set; }

        [ProtoMember(3)]
        public override DateTime Expires { get; set; }

        [ProtoMember(4)]
        public T Value { get; set; }
    }
}