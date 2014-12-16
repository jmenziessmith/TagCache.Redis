using System;
using System.Collections.Generic;
using ProtoBuf;

namespace TagCache.Redis.Tests
{
    [Serializable]
    [ProtoContract]
    public class TestObject
    {
        [ProtoMember(1)]
        public string Foo { get; set; }

        [ProtoMember(2)]
        public string Bar { get; set; }

        [ProtoMember(3)]
        public int Score { get; set; }

        [ProtoMember(4)]
        public List<string> SomeList { get; set; }
        
        [ProtoMember(5)]
        public TestObject Child { get; set; }
    }
}
