using System;
using System.Threading.Tasks;

namespace TagCache.Redis
{
    public class RedisExpiryManager
    {
        public readonly string _setKey;

        public RedisExpiryManager(CacheConfiguration configuration)
        {
            _setKey = string.Format("{0}:_cacheExpireyKeys", configuration.RootNameSpace);
        }

        public void SetKeyExpiry(RedisClient client, string key, DateTime expiryDate)
        {
            client.SetTimeSet(_setKey, key, expiryDate);
        }

        public void RemoveKeyExpiry(RedisClient client, string[] keys)
        {
            client.RemoveTimeSet(_setKey, keys);
        }

        public async Task<bool> RemoveKeyExpiryAsync(RedisClient client, string[] keys)
        {
            await client.RemoveTimeSetAsync(_setKey, keys);
            return true;
        }

        public string[] GetExpiredKeys(RedisClient client, DateTime maxDate)
        {
            return client.GetFromTimeSet(_setKey, maxDate);
        }

        public async Task<string[]> GetExpiredKeysAsync(RedisClient client, DateTime maxDate)
        {
            return await client.GetFromTimeSetAsync(_setKey, maxDate);
        }
    }
}