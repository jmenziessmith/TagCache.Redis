using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TagCache.Redis.Interfaces
{
    public interface IRedisCacheLogger
    {
        void Log(string method, string arg, string message);
    }
}
