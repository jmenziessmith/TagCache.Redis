namespace TagCache.Redis.Serialization
{
    public interface ISerializationProvider
    {
        T Deserialize<T>(string value) where T : class;
        string Serialize<T>(T value) where T : class;
    }
}
