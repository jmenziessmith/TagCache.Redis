using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TagCache.Redis.Interfaces
{
    public interface IRedisCacheProvider
    {
        T Get<T>(string key);
        List<T> GetByTag<T>(string tag);
        void Set<T>(string key, T value, DateTime expires, string tag = null);
        void Set<T>(string key, T value, DateTime expires, IEnumerable<string> tags);
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