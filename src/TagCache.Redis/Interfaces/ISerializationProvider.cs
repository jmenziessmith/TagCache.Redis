namespace TagCache.Redis.Interfaces
{
    public interface ISerializationProvider
    {
        T Deserialize<T>(string value) where T : class;
        string Serialize<T>(T value) where T : class;
    }
}
