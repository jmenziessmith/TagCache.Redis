using System.IO;
using System.Xml.Serialization;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;

namespace TagCache.Redis.Serialization
{
    public class XmlSerializationProvider : ISerializationProvider
    {
        public T Deserialize<T>(RedisValue value) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(value))
            {
                var obj = xmlSerializer.Deserialize(textReader);
                return obj as T;
            }
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, value);
                return textWriter.ToString();
            }
        }
    }
}
