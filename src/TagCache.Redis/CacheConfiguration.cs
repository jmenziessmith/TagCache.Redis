using TagCache.Redis.Serialization;

namespace TagCache.Redis
{
    public class CacheConfiguration
    {
        public CacheConfiguration()
        {
            RedisClientConfiguration = new RedisClientConfiguration();
        }

        internal const int MinutesToRemoveAfterExpiry = 15;

        private const string _defaultRootNameSpace = "_redisCache";  

        private string _rootNameSpace;

        public string RootNameSpace {
            get { return _rootNameSpace ?? _defaultRootNameSpace; }
            set { _rootNameSpace = value; }
        }

        private ISerializationProvider _serializer;

        public ISerializationProvider Serializer {
            get
            {
                if (_serializer == null)
                {
                    _serializer = new BinarySerializationProvider();
                }
                return _serializer;

            }
            set { _serializer = value; }
        } 

        public RedisClientConfiguration RedisClientConfiguration { get; set; }
    }

    public class RedisClientConfiguration
    {
        public const string _DefaultHost = "localhost";
        public const int _DefaultDbNo = 0;
        public const int _DefaultTimeoutMilliseconds = 1000;

        private string _host;

        public string Host
        {
            get { return _host ?? _DefaultHost; }
            set { _host = value; }
        }

        private int? _dbNo;
        public int DbNo
        {
            get { return _dbNo ?? _DefaultDbNo; }
            set { _dbNo = value; }
        }


        private int? _timeoutMilliseconds;
        public int TimeoutMilliseconds
        {
            get { return _timeoutMilliseconds ?? _DefaultTimeoutMilliseconds; }
            set { _timeoutMilliseconds = value; }
        }

    }

}
