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
        /// This is a small application to test subscriptions in redis - which are required for automatic expiry of cache items
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            System.Console.WriteLine("Start"); 

            _conn = new BookSleeve.RedisConnection("localhost");
            var c =_conn.Open();
            c.Wait();
            _subConn = new BookSleeve.RedisSubscriberConnection("localhost");
            var s = _subConn.Open();
            s.Wait();
            
            System.Console.WriteLine("Conn : " + _conn.State);
            System.Console.WriteLine("SubConn : " + _subConn.State); 
             
            _subConn.MessageReceived += _subConn_MessageReceived;
            _subConn.Error += _subConn_Error;

            channel = _conn.GetOpenSubscriberChannel();
            System.Threading.Thread.Sleep(100);

            System.Console.WriteLine("Channel : " + channel.State); 
            

            //channel.PatternSubscribe("workers:job-done:*", OnExecutionCompleted).Wait();
            //channel.PatternSubscribe("*:*:*", OnExecutionCompleted).Wait();
            channel.PatternSubscribe("*", OnExecutionCompleted).Wait();
            //channel.PatternSubscribe("key.*", OnExecutionCompleted).Wait();
            //channel.PatternSubscribe("key.2:*", OnExecutionCompleted).Wait();

            channel.MessageReceived += channel_MessageReceived;

            Console.WriteLine("Subscriptions : " + channel.SubscriptionCount);


            _conn.Keys.Remove(12, "_expireys");

            int i = 0;
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                Set(i++);
                if (received > 0)
                {
                    Console.WriteLine("Subscriptions have worked");
                }
                else
                {
                    Console.WriteLine("Subscriptions have not worked yet");
                }
                Console.WriteLine(channel.State);


                var exp = _conn.SortedSets.Range(12, "_expireys", 0, GetTime(0));
                exp.Wait();
                Console.WriteLine("Expireys: " + exp.Result.Count());

            }
        }

        static void channel_MessageReceived(string arg1, byte[] arg2)
        {
            received++;
            Console.WriteLine("msg");
        }

        public static void Set(int i)
        {
            var set = _conn.Strings.Set(12, "key." + i, "n"+i);
            set.Wait();
            var get = _conn.Strings.GetString(12, "key." + i);
            _conn.Sets.Add(12, "_tags.tag1", "key." + i);
            _conn.SortedSets.Add(12, "_expireys", "key." + i, GetTime(12));

            get.Wait();
            //_conn.Publish("key." + i, "set").Wait();
            Console.Write("Set(" + get.Result  +")..."); 
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
            System.Console.WriteLine("ExecutionComplete");
        }

        public static void _subConn_Error(object sender, BookSleeve.ErrorEventArgs e)
        {
            received++;
            System.Console.WriteLine("Error");
        }

        public static void _subConn_MessageReceived(string arg1, byte[] arg2)
        {
            received++;
            System.Console.WriteLine("Message");
        }



    }
}
