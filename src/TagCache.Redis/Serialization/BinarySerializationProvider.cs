using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

namespace TagCache.Redis.Serialization
{
    public class BinarySerializationProvider : Serialization.ISerializationProvider
    {
        BinaryFormatter formatter = new BinaryFormatter();

        public T Deserialize<T>(string value) where T : class
        {
            var bytes = Convert.FromBase64String(value);
            MemoryStream memoryStream = new MemoryStream(bytes);
            return (T)formatter.Deserialize(memoryStream);
        }

        public string Serialize<T>(T value) where T : class
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                formatter.Serialize(mStream, value);
                var bytes = mStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
