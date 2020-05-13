namespace Net
{
    public class UDPSetting : INetSetting
    {
        public UDPSetting(string host,
                          int port,
                          int ioNum = NetDefine.DEFAUT_IONUM,
                          int defautBufferSize = NetDefine.DEFAUT_BUFFER_SIZE,
                          uint conv = uint.MaxValue / 2)
        {
            this.host = host;
            this.port = port;
            this.ioNum = ioNum;
            this.defautBufferSize = defautBufferSize;
            this.conv = conv;
        }

        public string host { get; private set; }
        public int port { get; private set; }
        public int ioNum { get; private set; }
        public int defautBufferSize { get; private set; }
        public uint conv { get; private set; }
    }
}
