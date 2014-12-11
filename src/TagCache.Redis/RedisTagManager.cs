using System.Collections.Generic;
using System.Linq;

namespace TagCache.Redis
{
    public class RedisTagManager
    {

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
        public void UpdateTags(RedisClient client, RedisCacheItem cacheItem)
        {
            SetTagsForKey(client, cacheItem);
            AddKeyToTags(client, cacheItem);
        }

        public void UpdateTags(RedisClient client, string key, IEnumerable<string> tags)
        {
            var cacheItem = new RedisCacheItem
            {
                Key = key,
                Tags = tags
            };
            UpdateTags(client, cacheItem);
        }

        /// <summary>
        /// Stores all tags in a list for the given key
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public void SetTagsForKey(RedisClient client, RedisCacheItem cacheItem)
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
        public void AddKeyToTags(RedisClient client, RedisCacheItem cacheItem)
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
        /// <param name="client"></param>
        /// <param name="key"></param>
        public void RemoveTags(RedisClient client, RedisCacheItem cacheItem)
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


        public void RemoveTags(RedisClient client, string[] keys)
        {
            foreach (var key in keys)
            {
                var tags = GetTagsForKey(client, key);
                var cacheItem = new RedisCacheItem()
                {
                    Key = key,
                    Tags = tags.ToList()
                };
                RemoveKeyFromTags(client, cacheItem);
                RemoveTagsForKey(client, cacheItem);
            }
        }


        /// <summary>
        /// Removes the given items key from the tag list for each tag
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public void RemoveKeyFromTags(RedisClient client, RedisCacheItem cacheItem)
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
        /// Clears the list of tags for the given key
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cacheItem"></param>
        public void RemoveTagsForKey(RedisClient client, RedisCacheItem cacheItem)
        {
            if (cacheItem != null)
            {
                if (cacheItem.Tags != null && cacheItem.Tags.Any())
                {
                    client.SetTagsForKey(cacheItem.Key, null);
                }
            }
        } 


    }
}
