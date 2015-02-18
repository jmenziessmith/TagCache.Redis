using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TagCache.Redis.Interfaces
{
    public interface IRedisCacheProvider
    {
        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);

        List<T> GetByTag<T>(string tag);
        Task<List<T>> GetByTagAsync<T>(string tag);

        void Set<T>(string key, T value, DateTime expires, string tag = null);
        Task<bool> SetAsync<T>(string key, T value, DateTime expires, string tag = null);

        void Set<T>(string key, T value, DateTime expires, IEnumerable<string> tags);
        Task<bool> SetAsync<T>(string key, T value, DateTime expires, IEnumerable<string> tags);

        void Remove(string key);
        Task<bool> RemoveAsync(string key);

        void Remove(IEnumerable<string> keys);
        Task<bool> RemoveAsync(IEnumerable<string> keys);

        void Remove(string[] keys);
        Task<bool> RemoveAsync(string[] keys);

        void RemoveByTag(string tag);
        Task<bool> RemoveByTagAsync(string tag);

        Task<bool> RemoveAsync(IRedisCacheItem item);
        void Remove(IRedisCacheItem item);

        /// <summary>
        /// This should be called at regular intervals in case the active version of redis does not support subscriptions to expiries
        /// </summary>
        /// <returns></returns>
        string[] RemoveExpiredKeys();

        /// <summary>
        /// This should be called at regular intervals in case the active version of redis does not support subscriptions to expiries
        /// </summary>
        /// <returns></returns>
        Task<string[]> RemoveExpiredKeysAsync();
    }
}