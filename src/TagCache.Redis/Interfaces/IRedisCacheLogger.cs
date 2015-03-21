namespace TagCache.Redis.Interfaces
{
    public interface IRedisCacheLogger
    {
        void Log(string method, string arg, string message);
    }
}
