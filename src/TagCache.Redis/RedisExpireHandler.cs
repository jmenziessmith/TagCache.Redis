using System;

namespace TagCache.Redis
{
    public class RedisExpireHandler
    {
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
            var subscriber = new RedisSubscriberConnectionManager(RedisConfiguration.Client.Host).GetConnection(true);

            subscriber.Subscribe("my.key.expire");
            subscriber.PatternSubscribe("*"); 

            subscriber.MessageReceived += subscriber_MessageReceived; 

        } 

    }
}
