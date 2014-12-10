using System;
using StackExchange.Redis;

namespace TagCache.Redis
{
    public class RedisConnectionManager : IDisposable
    {
        private readonly object _connectionLock = new object();
        private ConnectionMultiplexer _connection;

        public string Host { get; set; }
        public int Port { get; set; }
        public int IOTimeout { get; set; }
        public string Password { get; set; }
        public int MaxUnsent { get; set; }
        public bool AllowAdmin { get; set; }
        public int SyncTimeout { get; set; }

        public RedisConnectionManager(string host = "127.0.0.1", int port = 6379, int ioTimeout = -1, string password = null, int maxUnsent = 2147483647, bool allowAdmin = false, int syncTimeout = 10000)
        {
            Host = host;
            Port = port;
            IOTimeout = ioTimeout;
            Password = password;
            MaxUnsent = maxUnsent;
            AllowAdmin = allowAdmin;
            SyncTimeout = syncTimeout;

            _connection = ConnectionMultiplexer.Connect(Host); //TODO: Construction connection string from configuration
        }

        public ConnectionMultiplexer GetConnection()
        {
            return _connection;
        }
        
        public void Reset(bool abort = false)
        {
            lock (_connectionLock)
            {
                if (_connection != null)
                {
                    _connection.Close(abort);
                    _connection = null;
                }
            }
        }

        public void Dispose()
        {
            lock (_connectionLock)
            {
                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }
}