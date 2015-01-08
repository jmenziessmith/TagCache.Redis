namespace TagCache.Redis.ProtoBuf
{
    public class ProtobufSerializationConfiguration
    {
        public ProtobufSerializationConfiguration()
        {
            BuildSerializers = false;
        }

        public bool BuildSerializers { get; set; }
    }
}