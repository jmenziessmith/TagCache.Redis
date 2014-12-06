using System;
using System.Diagnostics;
using BookSleeve;

namespace TagCache.Redis
{
    public class RedisSubscriberConnectionManager : IDisposable
    {
        private volatile RedisSubscriberConnection _connection;
        private readonly object _connectionLock = new object();

        public string Host { get; set; }
        public int Port { get; set; }
        public int IOTimeout { get; set; }
        public string Password { get; set; }
        public int MaxUnsent { get; set; }
        public bool AllowAdmin { get; set; }
        public int SyncTimeout { get; set; }

        public RedisSubscriberConnectionManager(string host, int port = 6379, int ioTimeout = -1, string password = null, int maxUnsent = 2147483647, bool allowAdmin = false, int syncTimeout = 10000)
        {
            Host = host;
            Port = port;
            IOTimeout = ioTimeout;
            Password = password;
            MaxUnsent = maxUnsent;
            AllowAdmin = allowAdmin;
            SyncTimeout = syncTimeout;
        }

        public RedisSubscriberConnection GetConnection(bool waitOnOpen = false)
        {
            var connection = _connection;

            if (connection == null)
            {
                lock (_connectionLock)
                {
                    if (_connection == null)
                    {
                        _connection = new RedisSubscriberConnection(Host, Port, IOTimeout, Password, MaxUnsent); 

                        _connection.Shutdown += ConnectionOnShutdown;
                        var openTask = _connection.Open();

                        if (waitOnOpen) { _connection.Wait(openTask); }
                    }

                    connection = _connection;
                }
            }

            return connection;
        }

        private void ConnectionOnShutdown(object sender, ErrorEventArgs errorEventArgs)
        {
            lock (_connectionLock)
            {
                _connection.Shutdown -= ConnectionOnShutdown;
                _connection = null;
            }
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