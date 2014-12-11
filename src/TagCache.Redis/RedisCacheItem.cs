using System;
using System.Collections.Generic;

namespace TagCache.Redis
{
   [Serializable]
   public class RedisCacheItem
   {
      public string Key { get; set; }
      public List<string> Tags { get; set; }
      public object Value { get; set; }
      public DateTime Expires { get; set; } 
   }
}
