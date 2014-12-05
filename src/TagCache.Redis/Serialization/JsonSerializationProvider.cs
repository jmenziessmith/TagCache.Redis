using Newtonsoft.Json;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.Serialization
{
    public class JsonSerializationProvider : ISerializationProvider
    {

        public T Deserialize<T>(string value) where T : class
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string Serialize<T>(T value) where T : class
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
