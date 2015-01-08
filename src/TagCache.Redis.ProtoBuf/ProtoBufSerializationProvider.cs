using System.IO;
using System.Linq;
using AutoProtobuf;
using ProtoBuf;
using ProtoBuf.Data;
using StackExchange.Redis;
using TagCache.Redis.Interfaces;
using System.Data;

namespace TagCache.Redis.ProtoBuf
{
    public class ProtoBufSerializationProvider : ISerializationProvider
    {
        private readonly ProtobufSerializationConfiguration configuration;

        public ProtoBufSerializationProvider(ProtobufSerializationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ProtoBufSerializationProvider()
        {
            configuration = new ProtobufSerializationConfiguration { BuildSerializers = false };
        }

        public T Deserialize<T>(RedisValue value) where T : class
        {
            using (var memoryStream = new MemoryStream(value))
            {
                var type = typeof(T);
                var valueType = type.BaseType != null && type.IsAssignableFrom(typeof(RedisCacheItem<>)) ? type.GetGenericArguments().FirstOrDefault() : null;

                var dataTableType = typeof(DataTable);

                if (valueType != null && (valueType == dataTableType || valueType.IsSubclassOf(dataTableType)))
                {
                    return new RedisCacheItem<DataTable>
                    {
                        Value = DataSerializer.DeserializeDataTable(memoryStream)
                    } as T;
                }

                var dataSetType = typeof(DataSet);

                if (valueType != null && (valueType == dataSetType || valueType.IsSubclassOf(dataSetType)))
                {
                    return new RedisCacheItem<DataSet>
                    {
                        Value = DataSerializer.DeserializeDataSet(memoryStream)
                    } as T;
                }

                var dataReaderType = typeof(IDataReader);

                if (valueType != null && (valueType == dataReaderType || valueType.IsSubclassOf(dataReaderType)))
                {
                    return new RedisCacheItem<IDataReader>
                    {
                        Value = DataSerializer.Deserialize(memoryStream)
                    } as T;
                }

                if (configuration.BuildSerializers)
                {
                    SerializerBuilder.Build<T>();
                }

                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        public RedisValue Serialize<T>(T value) where T : class
        {
            bool useStandardSerializer = true;
            var type = typeof(T);
            var valueType = type.BaseType != null && type.BaseType.IsAssignableFrom(typeof(RedisCacheItem<>)) ? type.GetGenericArguments().FirstOrDefault() : null;

            using (var memoryStream = new MemoryStream())
            {
                if (valueType != null)
                {
                    if (valueType == typeof(DataTable))
                    {
                        DataSerializer.Serialize(memoryStream, (value as RedisCacheItem<DataTable>).Value);
                        useStandardSerializer = false;
                    }

                    if (valueType == typeof(DataSet))
                    {
                        DataSerializer.Serialize(memoryStream, (value as RedisCacheItem<DataSet>).Value);
                        useStandardSerializer = false;
                    }

                    if (valueType == typeof(IDataReader))
                    {
                        DataSerializer.Serialize(memoryStream, (value as RedisCacheItem<IDataReader>).Value);
                        useStandardSerializer = false;
                    }
                }

                if (useStandardSerializer)
                {
                    if (configuration.BuildSerializers || type.BaseType.IsAssignableFrom(typeof(RedisCacheItem<>)))
                    {
                        SerializerBuilder.Build(value);
                    }
                    Serializer.Serialize(memoryStream, value);
                }

                var bytes = memoryStream.ToArray();
                return bytes;
            }
        }
    }
}
