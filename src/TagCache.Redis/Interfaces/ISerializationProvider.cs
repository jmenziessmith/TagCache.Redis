using StackExchange.Redis;

namespace TagCache.Redis.Interfaces
{
    public interface ISerializationProvider
    {
        T Deserialize<T>(RedisValue value) where T : class;
        RedisValue Serialize<T>(T value) where T : class;
    }
}
