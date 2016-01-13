using System;
using Newtonsoft.Json;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.Json.Net
{
    public class JsonSerializationProvider : ISerializationProvider
    {
        private readonly JsonSerializerSettings _settings;

        public JsonSerializationProvider()
            : this(new JsonSerializerSettings())
        { }

        public JsonSerializationProvider(JsonSerializerSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            _settings = settings;
            _settings.Converters.Add(new RedisCacheItemConverter());
        }

        public T Deserialize<T>(RedisValue value) where T : class
        {
            return JsonConvert.DeserializeObject<T>(value, _settings);
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            return JsonConvert.SerializeObject(value, _settings);
        }

        class RedisCacheItemConverter : JsonConverter
        {
            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override bool CanConvert(Type objectType)
            {
                // Intercept conversions from interfaces IRedisCacheItem<T> and IRedisCacheItem
                return objectType.IsInterface && typeof(IRedisCacheItem).IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                Type concreteType = typeof(RedisCacheItem);

                if (objectType.IsGenericType)
                {
                    concreteType = typeof(RedisCacheItem<>).MakeGenericType(objectType.GenericTypeArguments);
                }

                return serializer.Deserialize(reader, concreteType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
