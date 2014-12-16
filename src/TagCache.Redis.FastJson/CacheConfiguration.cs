using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCache.Redis.FastJson
{
    public class CacheConfiguration : TagCache.Redis.CacheConfiguration
    {
        public CacheConfiguration(RedisConnectionManager connectionManager) : base(connectionManager)
        {
            this.Serializer = new FastJsonSerializationProvider();
        } 
    }
}
