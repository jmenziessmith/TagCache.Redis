using System;
using System.Collections.Generic;
using ProtoBuf;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.ProtoBuf
{ 
    [Serializable]
    [ProtoContract]    
    public class ProtobufRedisCacheItem : IRedisCacheItem
    {
        [ProtoMember(1)]
        public virtual string Key { get; set; }

        [ProtoMember(2)]
        public virtual List<string> Tags { get; set; }

        [ProtoMember(3)]
        public virtual DateTime Expires { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class ProtobufRedisCacheItem<T> : ProtobufRedisCacheItem, IRedisCacheItem<T>
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
