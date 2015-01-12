using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis
{
    public class RedisTagManager
    { 
        private readonly IRedisCacheItemFactory _cacheItemFactory;

        public RedisTagManager(IRedisCacheItemFactory cacheItemFactory)
        {
            _cacheItemFactory = cacheItemFactory;
        }

        /// <summary>
        /// Retrieves all keys from the tag lists for the given tag 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public string[] GetKeysForTag(RedisClient client, string tag)
        {
            var keys = client.GetKeysForTag(tag);
            if (keys != null)
            {
                return keys.ToArray();
            }
            return null;
        }
        
        private string[] GetTagsForKey(RedisClient client, string key)
        {
            var keys = client.GetTagsForKey(key);
            if (keys != null)
            {
                return keys.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Removes the tags from the key list and the key from the tags list
        /// </summary>
        /// <param name="client"></param>
        /// <param name="key"></param>
        public void UpdateTags(RedisClient client, IRedisCacheItem cacheItem)
        {
            SetTagsForKey(client, cacheItem);
            AddKeyToTags(client, cacheItem);
        }

        public void UpdateTags(RedisClient client, string key, IEnumerable<string> tags)
        {
            var cacheItem = _cacheItemFactory.Create(
                key : key,
                tags : tags == null ? null :  tags.ToList()
            );
            UpdateTags(client, cacheItem);
        }

        /// <summary>
        /// Stores all tags in a list for the given key
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public void SetTagsForKey(RedisClient client, IRedisCacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                if (cacheItem.Tags != null && cacheItem.Tags.Any())
                {
                    client.SetTagsForKey(cacheItem.Key, cacheItem.Tags);
                }
            }
        }


        /// <summary>
        /// Stores the given items key in the tag list for each tag
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public void AddKeyToTags(RedisClient client, IRedisCacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                if (cacheItem.Tags != null && cacheItem.Tags.Any())
                {
                    RemoveKeyFromTags(client, cacheItem);
                    client.AddKeyToTags(cacheItem.Key, cacheItem.Tags);
                }
            }
        }


        /// <summary>
        /// Removes the tags from the key list and the key from the tags list
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cacheItem">The cache item.</param>
        public void RemoveTags(RedisClient client, IRedisCacheItem cacheItem)
        {
            RemoveKeyFromTags(client, cacheItem);
            RemoveTagsForKey(client, cacheItem);
        }

        /// <summary>
        /// Removes the tags from the key list and the key from the tags list
        /// </summary>
        /// <param name="client"></param>
        /// <param name="key"></param>
        public void RemoveTags(RedisClient client, string key)
        {
            RemoveTags(client, new[]{key});
        }


        /// <summary>
        /// Removes the tags.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="keys">The keys.</param>
        public void RemoveTags(RedisClient client, string[] keys)
        {
            foreach (var key in keys)
            {
                var tags = GetTagsForKey(client, key);
                var cacheItem = _cacheItemFactory.Create(
                    key: key,
                    tags: tags.ToList()
                    );
                RemoveKeyFromTags(client, cacheItem);
                RemoveTagsForKey(client, cacheItem);
            }
        }


        /// <summary>
        /// Removes the given items key from the tag list for each tag
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public void RemoveKeyFromTags(RedisClient client, IRedisCacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                if (cacheItem.Tags != null && cacheItem.Tags.Any())
                {
                    client.RemoveKeyFromTags(cacheItem.Key, cacheItem.Tags);
                }
            }
        }

        /// <summary>
        /// Removes the given items key from the tag list for each tag
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public async Task<bool> RemoveKeyFromTagsAsync(RedisClient client, IRedisCacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                if (cacheItem.Tags != null && cacheItem.Tags.Any())
                {
                    return await client.RemoveKeyFromTagsAsync(cacheItem.Key, cacheItem.Tags);
                }
            }

            return true;
        }

        /// <summary>
        /// Clears the list of tags for the given key
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public void RemoveTagsForKey(RedisClient client, IRedisCacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                if (cacheItem.Tags != null && cacheItem.Tags.Any())
                {
                    client.SetTagsForKey(cacheItem.Key, null);
                }
            }
        }

        /// <summary>
        /// Removes the tags asynchronous.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="cacheItem">The cache item.</param>
        /// <returns></returns>
        public async Task<bool> RemoveTagsAsync(RedisClient client, IRedisCacheItem cacheItem)
        {
            var remoteKeyTask = RemoveKeyFromTagsAsync(client, cacheItem);
            var removeTagTask = RemoveTagsForKeyAsync(client, cacheItem);

            return await remoteKeyTask && await removeTagTask;
        }

        /// <summary>
        /// Clears the list of tags for the given key
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public async Task<bool> RemoveTagsForKeyAsync(RedisClient client, IRedisCacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                if (cacheItem.Tags != null && cacheItem.Tags.Any())
                {
                    return await client.SetTagsForKeyAsync(cacheItem.Key, null);
                }
            }

            return true;
        }
    }
}
