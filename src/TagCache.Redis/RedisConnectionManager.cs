using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace TagCache.Redis
{
    public class RedisConnectionManager : IDisposable
    {
        private readonly object _connectionLock = new object();
        private static ConnectionMultiplexer _connection;

        public string ConnectionString { get; set; }
        public int? ConnectTimeout { get; set; }
        public string Password { get; set; }
        public int MaxUnsent { get; set; }
        public bool AllowAdmin { get; set; }
        public int? SyncTimeout { get; set; }

        public RedisConnectionManager(string connectionString = "127.0.0.1", int? connectTimeout = null, string password = null, int maxUnsent = 2147483647, bool allowAdmin = false, int? syncTimeout = null)
        {
            ConnectionString = connectionString;
            ConnectTimeout = connectTimeout;
            Password = password;
            MaxUnsent = maxUnsent;
            AllowAdmin = allowAdmin;
            SyncTimeout = syncTimeout;
        }

        private ConfigurationOptions BuildConfigurationOptions()
        {
            var result = ConfigurationOptions.Parse(ConnectionString);

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

        public async Task<ConnectionMultiplexer> GetConnectionAsync()
        {
            var connection = _connection;

            if (connection == null)
            {
                if (_connection == null)
                {
                    var options = BuildConfigurationOptions();
                    _connection = await ConnectionMultiplexer.ConnectAsync(options);
                }

                connection = _connection;
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