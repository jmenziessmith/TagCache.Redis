using System.IO;
using System.Xml.Serialization;

namespace TagCache.Redis.Serialization
{
    public class XmlSerializationProvider : Serialization.ISerializationProvider
    {

        public T Deserialize<T>(string value) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(value);

            var obj = xmlSerializer.Deserialize(textReader);
        
            return obj as T;
        }

        public string Serialize<T>(T value) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(value.GetType());
            StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, value);
            return textWriter.ToString();
        }
    }
}
