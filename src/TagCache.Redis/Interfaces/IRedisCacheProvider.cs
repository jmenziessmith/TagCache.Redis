using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TagCache.Redis.Interfaces
{
    public interface IRedisCacheProvider
    {
        T Get<T>(string key) where T : class;
        List<T> GetByTag<T>(string tag) where T : class;
        void Set<T>(T value, string key, DateTime expires, string tag = null) where T : class;
        void Set<T>(T value, string key, DateTime expires, List<string> tags) where T : class;
        void Remove(string key);
        void Remove(IEnumerable<string> keys);
        void Remove(string[] keys);
        Task RemoveAsync(RedisCacheItem item);
        void Remove(RedisCacheItem item);
        void RemoveByTag(string tag);

        /// <summary>
        /// This should be called at regular intervals if the active version of redis does not support subscriptions to expiries
        /// </summary>
        /// <returns></returns>
        string[] RemoveExpiredKeys();
    }
}