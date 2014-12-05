using System;

namespace TagCache.Redis
{
    /*
    public class RedisExpireHandler
    {
        private CacheConfiguration _configuration;

        public RedisExpireHandler(CacheConfiguration configuration)
        {
            
        }

        public DateTime LastExpiredDate { get; set; }

        public void HandleExpiry(string key)
        {
            var cacheProvider = new RedisCacheProvider();
            cacheProvider.Remove(key);
        }

        void subscriber_MessageReceived(string arg1, byte[] arg2)
        {
            var x = arg1;
            LastExpiredDate = DateTime.Now;
        }

        public void SubscribeToExpiryEvents()
        {
            var subscriber = new RedisSubscriberConnectionManager(_configuration.RedisClientConfiguration.Host).GetConnection(true);

            subscriber.Subscribe("my.key.expire");
            subscriber.PatternSubscribe("*"); 

            subscriber.MessageReceived += subscriber_MessageReceived; 

        } 

    }
    */
}
