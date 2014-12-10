using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis
{
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private RedisClient _client;
        private RedisCacheItemProvider _cacheItemProvider;
        private RedisTagManager _tagManager;
        private RedisExpiryManager _expiryManager;
        private ISerializationProvider _serializer;
        private CacheConfiguration _cacheConfiguration;

        public IRedisCacheLogger Logger { get; set; }

        // singleton dictionary<host,expirehandler> 
        private static ConcurrentDictionary<string, RedisExpireHandler> _redisExpireHandlersByHost;
        
        public RedisCacheProvider(RedisConnectionManager connectionManager) : this(new CacheConfiguration(connectionManager))
        {
        
        }

        public RedisCacheProvider(CacheConfiguration configuration)
        {
            _client = new RedisClient(configuration.RedisClientConfiguration.RedisConnectionManagerConnectionManager, configuration.RedisClientConfiguration.DbNo, configuration.RedisClientConfiguration.TimeoutMilliseconds);
            _serializer = configuration.Serializer;
            _tagManager = new RedisTagManager();
            _expiryManager = new RedisExpiryManager(configuration);
            _cacheItemProvider = new RedisCacheItemProvider(_serializer);

            SetupExpireHandler(configuration, this);
        }


        private static void SetupExpireHandler(CacheConfiguration configuration, RedisCacheProvider redisCacheProvider)
        {
            if (_redisExpireHandlersByHost == null)
            {
                _redisExpireHandlersByHost = new ConcurrentDictionary<string, RedisExpireHandler>();
            }
            if (!_redisExpireHandlersByHost.ContainsKey(configuration.RedisClientConfiguration.Host))
            {
                var handler = new RedisExpireHandler(configuration);
                handler.RemoveMethod = redisCacheProvider.Remove;
                handler.LogMethod = redisCacheProvider.Log;
                _redisExpireHandlersByHost.TryAdd(configuration.RedisClientConfiguration.Host, handler);
            }
        }

        private void Log(string method, string arg, string message)
        {
            try
            {
                if (Logger != null)
                {
                    Logger.Log(method, arg, message);
                }
            }
            catch
            {
            }
        }

        public T Get<T>(string key) where T : class
        { 
            var cacheItem = _cacheItemProvider.Get(_client, key);
            if (cacheItem != null)
            {
                if (CacheItemIsValid(cacheItem))
                {
                    Log("Get", key, "Found");
                    return cacheItem.Value as T;
                }
            }
            Log("Get", key, "Not Found");
            return default(T);
        }

        public List<T> GetByTag<T>(string tag) where T : class
        {
            var keys = _tagManager.GetKeysForTag(_client, tag);
            if (keys != null && keys.Length > 0)
            {
                var items = _cacheItemProvider.GetMany(_client, keys);

                var result = new List<T>();

                foreach (var item in items)
                {
                    if (CacheItemIsValid(item))
                    {
                        var value = item.Value as T;
                        if (value!=null)
                        {
                            result.Add(value);
                        }
                    }
                }

                Log("GetByTag", tag, "Found");
                return result;
            }

            Log("GetByTag", tag, "Not Found");
            return null;
        }

        private bool CacheItemIsValid(RedisCacheItem item)
        {
            if (item.Expires < DateTime.Now)
            {
                RemoveAsync(item); // do not wait
                return false;
            }
            return true;
        }


        public void Set<T>(string key, T value, DateTime expires, string tag = null) where T : class
        {  
            var tags = string.IsNullOrEmpty(tag) ? null : new List<string> { tag };
            Set(key, value, expires, tags); 
        }

        public void Set<T>(string key, T value, DateTime expires, List<string> tags) where T : class
        { 
            Log("Set", key, null);
            if (_cacheItemProvider.Set(_client, value, key, expires, tags))
            {
                _tagManager.UpdateTags(_client, key, tags);
            } 
        }


        public void Remove(string key)
        {
            Log("Remove", key, null);
            Remove(new[]{key});
        }

        public void RemoveByTag(string tag)
        {
            Log("RemoveByTag", tag, null);
            var keys = _tagManager.GetKeysForTag(_client, tag);
            if (keys != null && keys.Length > 0)
            {
                Remove(keys);
            }
        }

        public void Remove(IEnumerable<string> keys)
        {
            Remove(keys.ToArray());
        }

        public void Remove(string[] keys) 
        {
            _client.Remove(keys);
            _tagManager.RemoveTags(_client, keys);
            _expiryManager.RemoveKeyExpiry(_client, keys.ToArray());
        }

        public async Task RemoveAsync(RedisCacheItem item)
        {
            _client.Remove(item.Key);
            _tagManager.RemoveTags(_client, item);
        }
        
        public void Remove(RedisCacheItem item)
        {
            var task = Task.Run(() => RemoveAsync(item));
            task.Wait();
            if (task.Exception != null)
            {
                throw task.Exception;
            }
        }

        /// <summary>
        /// This should be called at regular intervals in case the active version of redis does not support subscriptions to expiries
        /// </summary>
        /// <returns></returns>
        public string[] RemoveExpiredKeys()
        {
            var maxDate = DateTime.Now.AddMinutes(CacheConfiguration.MinutesToRemoveAfterExpiry);
            var keys = _expiryManager.GetExpiredKeys(_client, maxDate);
            Remove(keys);
            return keys;
        }

    }
}
