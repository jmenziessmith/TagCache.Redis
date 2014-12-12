using System;
using StackExchange.Redis;

namespace TagCache.Redis
{
    public class RedisConnectionManager : IDisposable
    {
        private readonly object _connectionLock = new object();
        private static ConnectionMultiplexer _connection;

        public string Host { get; set; }
        public int Port { get; set; }
        public int? ConnectTimeout { get; set; }
        public string Password { get; set; }
        public int MaxUnsent { get; set; }
        public bool AllowAdmin { get; set; }
        public int? SyncTimeout { get; set; }

        public RedisConnectionManager(string host = "127.0.0.1", int port = 6379, int? connectTimeout = null, string password = null, int maxUnsent = 2147483647, bool allowAdmin = false, int? syncTimeout = null)
        {
            Host = host;
            Port = port;
            ConnectTimeout = connectTimeout;
            Password = password;
            MaxUnsent = maxUnsent;
            AllowAdmin = allowAdmin;
            SyncTimeout = syncTimeout;
        }

        private string BuildConnectionString()
        {
            return string.Format("{0}:{1}", Host, Port);
        }

        private ConfigurationOptions BuildConfigurationOptions()
        {
            var configString = BuildConnectionString();

            var result = ConfigurationOptions.Parse(configString);

            if (SyncTimeout != null)
            {
                result.SyncTimeout = SyncTimeout.Value;
            }
            if (ConnectTimeout != null)
            {
                result.ConnectTimeout = ConnectTimeout.Value;
            }
            result.AllowAdmin = AllowAdmin;
            result.Password = Password;

            return result;
        }

        public ConnectionMultiplexer GetConnection()
        {
            var connection = _connection;

            if (connection == null)
            {
                lock (_connectionLock)
                {
                    if (_connection == null)
                    {
                        var options = BuildConfigurationOptions();
                        _connection = ConnectionMultiplexer.Connect(options);
                    }
                    connection = _connection;
                }
            }

            return connection;  
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