using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using TagCache.Redis;
using TagCache.Redis.Interfaces;

namespace RedisCacheLogMonitor.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var cache = new RedisCacheProvider();
            cache.Logger = new ConsoleLogger();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }

    public class ConsoleLogger : IRedisCacheLogger
    {
        public void Log(string method, string arg, string message)
        {
            Console.WriteLine("RedisLog> {0}({1}) : {2}", method, arg, message);
        }
    }
}
