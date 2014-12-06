using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TagCache.Redis.Tests
{
    [Serializable]
    public class TestObject
    {
        public string Foo { get; set; }
        public string Bar { get; set; }
        public int Score { get; set; }
    }
}
