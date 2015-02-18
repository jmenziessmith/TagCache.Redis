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
        
        public IRedisCacheLogger Logger { get; set; }

        // singleton dictionary<host,expirehandler> 
        private static ConcurrentDictionary<string, RedisExpireHandler> _redisExpireHandlersByHost;

        public RedisCacheProvider(RedisConnectionManager connectionManager)
            : this(new CacheConfiguration(connectionManager))
        {

        }

        public RedisCacheProvider(CacheConfiguration configuration)
        {
            _client = new RedisClient(configuration.RedisClientConfiguration.RedisConnectionManagerConnectionManager, configuration.RedisClientConfiguration.DbNo, configuration.RedisClientConfiguration.TimeoutMilliseconds);
            _serializer = configuration.Serializer;
            _tagManager = new RedisTagManager(configuration.CacheItemFactory);
            _expiryManager = new RedisExpiryManager(configuration);
            _cacheItemProvider = new RedisCacheItemProvider(_serializer, configuration.CacheItemFactory);

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
            catch //Itentional exception supression
            {
            }
        }

        public T Get<T>(string key)
        {
            var cacheItem = _cacheItemProvider.Get<T>(_client, key);
            if (cacheItem != null)
            {
                if (CacheItemIsValid(cacheItem))
                {
                    Log("Get", key, "Found");

                    return cacheItem.Value;
                }
            }
            Log("Get", key, "Not Found");
            return default(T);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var cacheItem = await _cacheItemProvider.GetAsync<T>(_client, key);
            if (cacheItem != null)
            {
                if (CacheItemIsValid(cacheItem))
                {
                    Log("Get", key, "Found");

                    return cacheItem.Value;
                }
            }
            Log("Get", key, "Not Found");
            return default(T);
        }

        public List<T> GetByTag<T>(string tag)
        {
            var keys = _tagManager.GetKeysForTag(_client, tag);
            if (keys != null && keys.Length > 0)
            {
                var items = _cacheItemProvider.GetMany<T>(_client, keys);

                var result = new List<T>();

                foreach (var item in items)
                {
                    if (CacheItemIsValid(item))
                    {
                        var value = item.Value;
                        if (value != null)
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

        public async Task<List<T>> GetByTagAsync<T>(string tag)
        {
            var keys = await _tagManager.GetKeysForTagAsync(_client, tag);
            if (keys != null && keys.Length > 0)
            {
                var items = await _cacheItemProvider.GetManyAsync<T>(_client, keys);

                var result = new List<T>();

                foreach (var item in items)
                {
                    if (CacheItemIsValid(item))
                    {
                        var value = item.Value;
                        if (value != null)
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

        private bool CacheItemIsValid(IRedisCacheItem item)
        {
            if (item.Expires < DateTime.Now)
            {
#pragma warning disable 4014
                RemoveAsync(item); // do not wait
#pragma warning restore 4014
                return false;
            }
            return true;
        }

        public void Set<T>(string key, T value, DateTime expires, string tag = null)
        {
            Set<T>(key, value, expires, tag != null ? new[] { tag } : null);
        }

        public async Task<bool> SetAsync<T>(string key, T value, DateTime expires, string tag = null)
        {
            return await SetAsync<T>(key, value, expires, tag != null ? new[] { tag } : null);
        }

        public void Set<T>(string key, T value, DateTime expires, IEnumerable<string> tags)
        {
            Log("Set", key, null);

            var enumeratedTags = tags != null ? tags as string[] ?? tags.ToArray() : null;

            if (_cacheItemProvider.Set<T>(_client, value, key, expires, enumeratedTags))
            {
                _tagManager.UpdateTags(_client, key, enumeratedTags);
            }
        }

        public async Task<bool> SetAsync<T>(string key, T value, DateTime expires, IEnumerable<string> tags)
        {
            Log("Set", key, null);

            var enumeratedTags = tags != null ? tags as string[] ?? tags.ToArray() : null;

            if (await _cacheItemProvider.SetAsync<T>(_client, value, key, expires, enumeratedTags))
            {
                await _tagManager.UpdateTagsAsync(_client, key, enumeratedTags);
            }

            return true;
        }

        public void Remove(string key)
        {
            Log("Remove", key, null);
            Remove(new[] { key });
        }

        public async Task<bool> RemoveAsync(string key)
        {
            Log("Remove", key, null);
            return await RemoveAsync(new[] { key });
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

        public async Task<bool> RemoveByTagAsync(string tag)
        {
            Log("RemoveByTag", tag, null);
            var keys = await _tagManager.GetKeysForTagAsync(_client, tag);
            if (keys != null && keys.Length > 0)
            {
                await RemoveAsync(keys);
            }

            return true;
        }

        public void Remove(IEnumerable<string> keys)
        {
            Remove(keys.ToArray());
        }

        public async Task<bool> RemoveAsync(IEnumerable<string> keys)
        {
            return await RemoveAsync(keys.ToArray());
        }

        public void Remove(string[] keys)
        {
            _client.Remove(keys);
            _tagManager.RemoveTags(_client, keys);
            _expiryManager.RemoveKeyExpiry(_client, keys.ToArray());
        }

        public async Task<bool> RemoveAsync(string[] keys)
        {
            await _client.RemoveAsync(keys);
            await _tagManager.RemoveTagsAsync(_client, keys);
            await _expiryManager.RemoveKeyExpiryAsync(_client, keys.ToArray());

            return true;
        }

        public async Task<bool> RemoveAsync(IRedisCacheItem item)
        {
            var removeTask = _client.RemoveAsync(item.Key);
            var removeTagTask = _tagManager.RemoveTagsAsync(_client, item);

            return await removeTask && await removeTagTask;
        }

        public void Remove(IRedisCacheItem item)
        {
            _client.Remove(item.Key);
            _tagManager.RemoveTags(_client, item);
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

        /// <summary>
        /// This should be called at regular intervals in case the active version of redis does not support subscriptions to expiries
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> RemoveExpiredKeysAsync()
        {
            var maxDate = DateTime.Now.AddMinutes(CacheConfiguration.MinutesToRemoveAfterExpiry);
            var keys = await _expiryManager.GetExpiredKeysAsync(_client, maxDate);
            await RemoveAsync(keys);
            return keys;
        }
    }
}
