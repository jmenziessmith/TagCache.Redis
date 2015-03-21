using System;
using System.Threading;
using TagCache.Redis;
using TagCache.Redis.Interfaces;

namespace RedisCacheLogMonitor.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var redis = new RedisConnectionManager();
            var cache = new RedisCacheProvider(redis);
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
