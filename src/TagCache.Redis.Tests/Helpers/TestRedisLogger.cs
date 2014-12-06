using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.Tests.Helpers
{
    public class TestRedisLogger : IRedisCacheLogger
    {
        public void Log(string method, string arg, string message)
        {
            Console.WriteLine("RedisLog> {0}({1}) : {2}", method, arg, message);
        }
    }
}
