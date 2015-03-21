using System;
using System.IO;
using System.Linq;
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
        private readonly static Type ProtobutRedisCacheItemType = typeof(ProtobufRedisCacheItem<>);

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtoBufSerializationProvider"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ProtoBufSerializationProvider(ProtobufSerializationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtoBufSerializationProvider"/> class.
        /// </summary>
        public ProtoBufSerializationProvider()
        {
            configuration = new ProtobufSerializationConfiguration();            
        }

        /// <summary>
        /// Deserializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public T Deserialize<T>(RedisValue value) where T : class
        {
            using (var memoryStream = new MemoryStream(value))
            {
                var type = typeof(T);
                var isRedisCacheItemType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRedisCacheItem<>);
                var valueType = isRedisCacheItemType ? type.GetGenericArguments().FirstOrDefault() : null;
                
                if (valueType != null && valueType == typeof(DataTable))
                {
                    return new ProtobufRedisCacheItem<DataTable>
                    {
                        Value = DataSerializer.DeserializeDataTable(memoryStream)
                    } as T;
                }
                
                if (valueType != null && valueType == typeof(DataSet))
                {
                    return new ProtobufRedisCacheItem<DataSet>
                    {
                        Value = DataSerializer.DeserializeDataSet(memoryStream)
                    } as T;
                }
                
                if (valueType != null && valueType == typeof(IDataReader))
                {
                    return new ProtobufRedisCacheItem<IDataReader>
                    {
                        Value = DataSerializer.Deserialize(memoryStream)
                    } as T;
                }

                if (configuration.BuildSerializers != null)
                {
                    configuration.BuildSerializers.Build<T>();
                }

                if (isRedisCacheItemType)
                {
                    var originalType = type;
                    type = ProtobutRedisCacheItemType;
                    type = type.MakeGenericType(originalType.GetGenericArguments());
                }

                return Serializer.NonGeneric.Deserialize(type, memoryStream) as T;
            }
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public RedisValue Serialize<T>(T value) where T : class
        {
            bool useStandardSerializer = true;
            var type = value.GetType();
            var isProtobugCacheItemType = type.IsGenericType && type.GetGenericTypeDefinition() == ProtobutRedisCacheItemType;
            var cacheItemValueType = isProtobugCacheItemType ? type.GetGenericArguments().FirstOrDefault() : null;

            using (var memoryStream = new MemoryStream())
            {
                if (cacheItemValueType != null)
                {
                    if (cacheItemValueType == typeof(DataTable))
                    {
                        DataSerializer.Serialize(memoryStream, (value as ProtobufRedisCacheItem<DataTable>).Value);
                        useStandardSerializer = false;
                    }

                    if (cacheItemValueType == typeof(DataSet))
                    {
                        DataSerializer.Serialize(memoryStream, (value as ProtobufRedisCacheItem<DataSet>).Value);
                        useStandardSerializer = false;
                    }

                    if (cacheItemValueType == typeof(IDataReader))
                    {
                        DataSerializer.Serialize(memoryStream, (value as ProtobufRedisCacheItem<IDataReader>).Value);
                        useStandardSerializer = false;
                    }
                }

                if (useStandardSerializer)
                {
                    if (configuration.BuildSerializers != null)
                    {
                        configuration.BuildSerializers.Build<T>();
                    }
                    Serializer.NonGeneric.Serialize(memoryStream, value);
                }

                var bytes = memoryStream.ToArray();
                return bytes;
            }
        }
    }
}
