using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TagCache.Redis
{
    public class RedisClient
    {
        private const bool _waitOnOpen = true;
        private const bool _IsRedisExpiryEnabled = true;
        private const string _rootName = "_redisCache";


        string _host;
        int _db;
        int _timeout = 100;
        private static RedisConnectionManager _connectionManager = new RedisConnectionManager("localhost");


        public RedisClient(string host, int db, int timeout)
        {
            _host = host;
            _db = db;
            _timeout = timeout;
            _connectionManager.Host = _host; 
        }
         
        
        public void Set(string key, string value, int expirySeconds)
        { 
            var addTask = SetAsync(key, value, expirySeconds);
            addTask.Wait(_timeout);
            if (addTask.Exception != null)
            {
                throw addTask.Exception;
            }
            if (addTask.Result == false)
            {
                throw new Exception("Add Failed");
            }                   
        }

        private async Task<bool> SetAsync(string key, string value, int expirySeconds)
        {
            var conn = _connectionManager.GetConnection(_waitOnOpen);
            if (_IsRedisExpiryEnabled)
            {
                await conn.Strings.Set(_db, key, value, expirySeconds).ConfigureAwait(false);
            }
            else
            {
                await conn.Strings.Set(_db, key, value).ConfigureAwait(false);
            }
            return true;
            
        }

        public string Get(string key)
        { 
            var resultTask = GetAsync(key);
            resultTask.Wait(_timeout);
            if (resultTask.Exception != null)
            {
                throw resultTask.Exception;
            }
            return resultTask.Result;
        }

        private async Task<string> GetAsync(string key)
        { 
            var conn = _connectionManager.GetConnection(_waitOnOpen);
            var value = await conn.Strings.GetString(_db, key).ConfigureAwait(false);
            return value; 
        }
         

        public bool Remove(string key)
        {
            return Remove(new string[] { key });
        }

        public bool Remove(string[] keys)
        {
            var addTask = RemoveAsync(keys);
            addTask.Wait(_timeout);
            if (addTask.Exception != null)
            {
                throw addTask.Exception;
            }
            if (addTask.Result == false)
            {
                throw new Exception("Remove Failed");
            }
            return true;
        }

        private async Task<bool> RemoveAsync(string[] keys)
        { 
            var conn = _connectionManager.GetConnection(_waitOnOpen);
            await conn.Keys.Remove(_db, keys).ConfigureAwait(false);
            return true;  
        }


        #region tags


        public string[] GetKeysForTag(string tag)
        {
            var addTask = GetKeysForTagAsync(tag);
            addTask.Wait(_timeout);
            if (addTask.Exception != null)
            {
                throw addTask.Exception;
            }
            return addTask.Result;
        }

        private async Task<string[]> GetKeysForTagAsync(string  tag)
        { 
            var conn = _connectionManager.GetConnection(_waitOnOpen);
            var result = await conn.Sets.GetAllString(_db, TagKeysListKey(tag)).ConfigureAwait(false);
            return result;  
        }
        

        public void AddKeyToTags(string key, List<string> tags)
        {
            var addTask = AddKeyToTagsAsync(key, tags);
            addTask.Wait(_timeout);
            if (addTask.Exception != null)
            {
                throw addTask.Exception;
            }
            if (addTask.Result == false)
            {
                throw new Exception("Add Tags Failed");
            }
        }

        private async Task<bool> AddKeyToTagsAsync(string key, List<string> tags)
        {
            if (key != null && tags != null && tags.Count > 0)
            {

                var conn = _connectionManager.GetConnection(_waitOnOpen);

                using (var trans = conn.CreateTransaction())
                {
                    foreach (var tag in tags)
                    {
                        trans.Sets.Add(_db, TagKeysListKey(tag), key);
                    }
                    await trans.Execute().ConfigureAwait(false);
                }

            }
            return true;
        }
        

        public void RemoveKeyFromTags(string key, List<string> tags)
        {
            var addTask = RemoveKeyFromTagsAsync(key, tags);
            addTask.Wait(_timeout);
            if (addTask.Exception != null)
            {
                throw addTask.Exception;
            }
            if (addTask.Result == false)
            {
                throw new Exception("Remove Tags Failed");
            }
        }

        private async Task<bool> RemoveKeyFromTagsAsync(string key, List<string> tags)
        {
            if (tags != null && tags.Count > 0)
            {
                var conn = _connectionManager.GetConnection(_waitOnOpen);
                using (var trans = conn.CreateTransaction())
                {
                    foreach (var tag in tags)
                    {
                        trans.Sets.Remove(_db, TagKeysListKey(tag), key);
                    }
                    await trans.Execute().ConfigureAwait(false);
                }
            }
            return true;
        }


        public void SetTagsForKey(string key, List<string> tags)
        {
            var addTask = SetTagsForKeyAsync(key, tags);
            addTask.Wait();
            if (addTask.Exception != null)
            {
                throw addTask.Exception;
            }
            if (addTask.Result == false)
            {
                throw new Exception("Set Tags For Key Failed");
            }
        }

        private async Task<bool> SetTagsForKeyAsync(string key, List<string> tags)
        {
            if (key != null)
            {
                var conn = _connectionManager.GetConnection(_waitOnOpen);
                using (var trans = conn.CreateTransaction())
                {
                    trans.Keys.Remove(_db, KeyTagsListKey(key)); // empty list

                    if (tags != null && tags.Count > 0)
                    {
                        trans.Sets.Add(_db, KeyTagsListKey(key), tags.ToArray()); // add each tag
                    }
                    await trans.Execute().ConfigureAwait(false);
                }
            }
            return true;
        }

        
        public void RemoveTagsForKey(string key)
        {
            var addTask = RemoveTagsForKeyAsync(key);
            addTask.Wait();
            if (addTask.Exception != null)
            {
                throw addTask.Exception;
            }
            if (addTask.Result == false)
            {
                throw new Exception("Set Tags For Key Failed");
            }
        }

        private async Task<bool> RemoveTagsForKeyAsync(string key)
        {
            if (key != null)
            {
                var conn = _connectionManager.GetConnection(_waitOnOpen);

                await conn.Keys.Remove(_db, KeyTagsListKey(key)).ConfigureAwait(false); // empty list
            }
            return true;
        }

 


      public string[] GetTagsForKey(string key)
      {
          var addTask = GetTagsForKeyAsync(key);
          addTask.Wait(_timeout);
          if (addTask.Exception != null)
          {
              throw addTask.Exception;
          }
          return addTask.Result;
      }

      private async Task<string[]> GetTagsForKeyAsync(string key)
      {
          var conn = _connectionManager.GetConnection(_waitOnOpen);
          var result = await conn.Sets.GetAllString(_db, KeyTagsListKey(key)).ConfigureAwait(false);
          return result;
      }


        private string TagKeysListKey(string tag)
        {
            return string.Format("{0}:_cacheKeysByTag:{1}" , _rootName, tag);
        }

        private string KeyTagsListKey(string key)
        {
            return string.Format("{0}:_cacheTagsByKey:{1}", _rootName, key);
        }

        #endregion

        #region TimeSets

        public bool SetTimeSet(string setKey, string value, DateTime date)
        {
            var task = SetTimeSetAsync(setKey, value, date);
            task.Wait(_timeout);
            if (task.Exception != null)
            {
                throw task.Exception;
            }
            if (task.Result == false)
            {
                throw new Exception("Set Failed");
            }
            return true;
        }
        public async Task<bool> SetTimeSetAsync(string setKey, string value, DateTime date)
        {
           var conn = _connectionManager.GetConnection(_waitOnOpen); 
           var result = conn.SortedSets.Add(_db, setKey, value, Helpers.TimeToRank(date));
           return true;
        }
         

        public bool RemoveTimeSet(string setKey, string[] keys)
        {
            var task = RemoveTimeSetAsync(setKey, keys);
            task.Wait(_timeout);
            if (task.Exception != null)
            {
                throw task.Exception;
            }
            if (task.Result == false)
            {
                throw new Exception("Remove Failed");
            }
            return true;
        }

        public async Task<bool> RemoveTimeSetAsync(string setKey, string[] keys)
        {
            var conn = _connectionManager.GetConnection(_waitOnOpen);
            var result = conn.SortedSets.Remove(_db, setKey, keys);
            return true;
        }


        public KeyValuePair<string, DateTime>[] GetFromTimeSet(string setKey, DateTime maxDate)
        {
            var task = GetFromTimeSetAsync(setKey, maxDate);
            task.Wait(_timeout);
            if (task.Exception != null)
            {
                throw task.Exception;
            }
            return task.Result;
        }


        public async Task<KeyValuePair<string, DateTime>[]> GetFromTimeSetAsync(string setKey, DateTime maxDate)
        {
          var conn = _connectionManager.GetConnection(_waitOnOpen);
          var timeAsRank = Helpers.TimeToRank(maxDate);
          var keys = await conn.SortedSets.RangeString(_db, setKey, min: 0, max: timeAsRank, minInclusive: true, maxInclusive: true);
          return keys.Select(Helpers.ToKeyValuePairStringDate).ToArray();
        }


        public void RemoveKey(string key)
        {
           var conn = _connectionManager.GetConnection(_waitOnOpen); 
           var task1 = conn.Open();
           task1.Wait();
           var task2 = conn.Keys.Remove(_db, key);
           task2.Wait();
        }



        #endregion

        public void Expire(string key, int seconds)
        {
            var conn = _connectionManager.GetConnection(_waitOnOpen);
            conn.Keys.Expire(_db, key, seconds);
        }
    }
}
