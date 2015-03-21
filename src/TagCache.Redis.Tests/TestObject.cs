using System;
using System.Collections.Generic;

namespace TagCache.Redis.Tests
{
    [Serializable]
    public class TestObject
    {
        public string Foo { get; set; }
        
        public string Bar { get; set; }
        
        public int Score { get; set; }
        
        public List<string> SomeList { get; set; }
        
        public TestObject Child { get; set; }
    }
}
