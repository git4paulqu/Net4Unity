namespace Net
{
    public class UDPSetting : INetSetting
    {
        public UDPSetting(string host,
                          int port,
                          KCPSetting kcp,
                          int ioNum = NetDefine.DEFAUT_IONUM,
                          int defautBufferSize = NetDefine.DEFAUT_BUFFER_SIZE)
        {
            this.host = host;
            this.port = port;
            this.ioNum = ioNum;
            this.defautBufferSize = defautBufferSize;
            this.kcp = kcp;
        }

        public string host { get; private set; }
        public int port { get; private set; }
        public int ioNum { get; private set; }
        public int defautBufferSize { get; private set; }
        public KCPSetting kcp { get; private set; }
    }

    public struct KCPSetting
    {
        public uint conv { get; set; }
        public int nodelay { get; set; }
        public int interval { get; set; }
        public int resend { get; set; }
        public int nc { get; set; }
        public int sndwnd { get; set; }
        public int rcvwnd { get; set; }
        public int timeoutSec { get; set; }

        public static KCPSetting Defaut
        {
            get {
                KCPSetting setting = new KCPSetting();
                setting.conv = NetDefine.DEF_KCP_CONV;
                setting.nodelay = NetDefine.DEF_KCP_NODELAY;
                setting.interval = NetDefine.DEF_KCP_INTERVAL;
                setting.resend = NetDefine.DEF_KCP_RESEND;
                setting.nc = NetDefine.DEF_KCP_NC;
                setting.sndwnd = NetDefine.DEF_KCP_SNDWND;
                setting.rcvwnd = NetDefine.DEF_KCP_RCVWND;
                setting.timeoutSec = NetDefine.DEF_KCP_TIMEOUT;
                return setting;
            }
        }
    }
}
