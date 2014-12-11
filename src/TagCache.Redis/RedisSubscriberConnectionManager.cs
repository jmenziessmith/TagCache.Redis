using StackExchange.Redis;

namespace TagCache.Redis
{
    public class RedisSubscriberConnectionManager
    {
        private volatile ISubscriber _connection;
        private readonly object _connectionLock = new object();
        private RedisConnectionManager _connectionManager;

        public RedisSubscriberConnectionManager(RedisConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public ISubscriber GetConnection()
        {
            var connection = _connection;

            if (connection == null)
            {
                lock (_connectionLock)
                {
                    if (_connection == null)
                    {
                        _connection = _connectionManager.GetConnection().GetSubscriber();
                    }

                    connection = _connection;
                }
            }

            return connection;
        }
    }
}