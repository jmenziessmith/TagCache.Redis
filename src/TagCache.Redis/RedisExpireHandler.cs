using System;
using BookSleeve;

namespace TagCache.Redis
{
    public class RedisExpireHandler
    {
        private CacheConfiguration _configuration;
        private RedisSubscriberConnection _subscriber;
        internal Action<string> RemoveMethod;

        public RedisExpireHandler(CacheConfiguration configuration)
        {
            _configuration = configuration; 
            SubscribeToExpiryEvents();
        }

        public DateTime LastExpiredDate { get; set; }

        

        void SubscriberMessageReceived(string e, byte[] message)
        {
            if (e.EndsWith("expired"))
            {   
                var key = System.Text.Encoding.UTF8.GetString(message);
                //if (key.StartsWith(_configuration.RootNameSpace))
                //{
                    RemoveMethod(key);   
                //}
            }
        }

        public void SubscribeToExpiryEvents()
        {
            _subscriber = new RedisSubscriberConnectionManager(_configuration.RedisClientConfiguration.Host).GetConnection(true);

            _subscriber.PatternSubscribe("*:expired");

            _subscriber.MessageReceived += SubscriberMessageReceived; 
        } 

    }
    
}
