using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisEvents.ConsoleApp
{
    public static class Program
    {
        static int received = 0;

        public static BookSleeve.RedisSubscriberConnection _subConn { get; set; }
        public static BookSleeve.RedisConnection _conn { get; set; }
        public static BookSleeve.RedisSubscriberConnection channel { get; set; }


        /// <summary>
        /// This is a small application to test event subscriptions in redis - which are required for automatic expiry of cache items
        /// if subscriptions dont work try setting the redis config :
        /// config set notify-keyspace-events Ex
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            System.Console.WriteLine("Start");

            _conn = new BookSleeve.RedisConnection("localhost");
            var c = _conn.Open();
            c.Wait();
            System.Console.WriteLine("Conn : " + _conn.State);

            _conn.Keys.Remove(12, "_expireys");


            _subConn = new BookSleeve.RedisSubscriberConnection("localhost");
            var s = _subConn.Open();
            s.Wait();
            System.Console.WriteLine("SubConn : " + _subConn.State);

            channel = _conn.GetOpenSubscriberChannel();
            System.Threading.Thread.Sleep(100);

            System.Console.WriteLine("Channel : " + channel.State);

            channel.PatternSubscribe("*:expired", OnExecutionCompleted).Wait();

            Console.WriteLine("Subscriptions : " + channel.SubscriptionCount);


            Set(1, 4);
            System.Threading.Thread.Sleep((6 * 1000) );
            if (received > 0)
            {
                Console.WriteLine("Subscriptions have worked");
            }
            else
            {
                Console.WriteLine("Subscriptions have not worked");
            }

            Console.ReadKey();
        }
         

        /// <summary>
        /// Sets a value in redis and 
        /// </summary>
        /// <param name="i">used for the key and value</param>
        public static void Set(int i, int expirySeconds)
        {
            var set = _conn.Strings.Set(12, "testing:key:val" + i, "n" + i, expirySeconds);
            set.Wait();  
            Console.WriteLine("Set testing:key:val{0} to {0}  and expire in {1} seconds" , i, expirySeconds); 
        }

        private static double GetTime(int addMs)
        {
            var str = ((DateTime.Now.AddSeconds(addMs).Ticks / TimeSpan.TicksPerMillisecond) - 63519236482052).ToString();
            return 1/long.Parse(str);
        } 

        /// <summary>
        /// Handle messages received from workers through Redis.</summary>
        public static void OnExecutionCompleted(string key, byte[] message)
        {
            received++;
            var msg = System.Text.Encoding.UTF8.GetString(message);

            System.Console.WriteLine("ExecutionComplete : " + key + " - " + msg);
        }
         
 


    }
}
