namespace TagCache.Redis.ProtoBuf
{
    /// <summary>
    /// An interface for Protobuf serializer builders.
    /// </summary>
    public interface ISerializerBuilder
    {
        /// <summary>
        /// Builds the serializer for the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        void Build<T>();
    }
}
