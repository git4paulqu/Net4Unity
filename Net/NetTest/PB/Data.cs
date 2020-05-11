using ProtoBuf;

namespace NetTest
{
    [ProtoContract]
    public class Data
    {
        [ProtoMember(1)]
        public int id;
    }
}