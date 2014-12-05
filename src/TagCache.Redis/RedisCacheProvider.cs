using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Serialization.ISerializationProvider _serializer;
        private CacheConfiguration _cacheConfiguration;

        public RedisCacheProvider() : this(new CacheConfiguration())
        {
            
        }

        public RedisCacheProvider(CacheConfiguration configuration)
        {
            // todo : the redis configuration needs to be injected
            _client = new RedisClient(configuration.RedisClientConfiguration.Host, configuration.RedisClientConfiguration.DbNo, configuration.RedisClientConfiguration.TimeoutMilliseconds);
            _serializer = configuration.Serializer;
            _tagManager = new RedisTagManager();
            _expiryManager = new RedisExpiryManager(configuration);
            _cacheItemProvider = new RedisCacheItemProvider(_serializer);
        }

        public T Get<T>(string key) where T : class
        {
            var cacheItem = _cacheItemProvider.Get(_client, key);
            if (cacheItem != null)
            {
                if (CacheItemIsValid(cacheItem))
                {
                    return cacheItem.Value as T;
                }
            }
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

                return result;
            }
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
         

        public void Set<T>(T value, string key, DateTime expires, string tag = null) where T : class
        {
            var tags = string.IsNullOrEmpty(tag) ? null : new List<string> { tag };
            Set(value, key, expires, tags);
        }

        public void Set<T>(T value, string key, DateTime expires, List<string> tags) where T : class
        {
            if (_cacheItemProvider.Set(_client, value, key, expires, tags))
            {
                _tagManager.UpdateTags(_client, key, tags);
            }
        }


        public void Remove(string key)
        {
            Remove(new[]{key});
        }

        public void RemoveByTag(string tag)
        {
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
        /// This should be called at regular intervals if the active version of redis does not support subscriptions to expiries
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
