using System;
using StackExchange.Redis;

namespace TagCache.Redis
{
    public class RedisExpireHandler
    {
        private CacheConfiguration _configuration;
        private ISubscriber _subscriber;
        internal Action<string> RemoveMethod;
        public Action<string, string, string> LogMethod { get; set; }

        public RedisExpireHandler(CacheConfiguration configuration)
        {
            _configuration = configuration;
            SubscribeToExpiryEvents();
        }

        public DateTime LastExpiredDate { get; set; }

        void SubscriberMessageReceived(RedisChannel redisChannel, RedisValue value)
        {
            if (((string)redisChannel).EndsWith("expired"))
            {
                var key = System.Text.Encoding.UTF8.GetString(value);

                LogMethod("Expired", key, null);
                RemoveMethod(key);
            }
        }

        public void SubscribeToExpiryEvents()
        {
            _subscriber = new RedisSubscriberConnectionManager(_configuration.RedisClientConfiguration.RedisConnectionManagerConnectionManager).GetConnection();
            _subscriber.Subscribe(new RedisChannel("*:expired", RedisChannel.PatternMode.Pattern), SubscriberMessageReceived);
        }
    }

}
