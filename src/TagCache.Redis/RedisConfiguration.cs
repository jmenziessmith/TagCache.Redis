namespace TagCache.Redis
{
    public static class RedisConfiguration
    {
        public static class Client
        {
            public static string Host = "localhost";
            public static int DbNo = 12;
            public static int TimeoutMilliseconds = 1000;
        }

        public static class Expiry
        {
            public static int MinutesToRemoveAfterExpiry = 15; 
        }

    }
}
