using System.Net;
using System.Net.Sockets;

namespace Net.UDP
{
    public class UDPServer : NetSocket
    {
        public void Listen(UDPSetting setting)
        {
            if (null == setting)
            {
                NetDebug.Error("[UDPConnection] the setting can not be null.");
                return;
            }

            this.setting = setting;
            Initialize(this.setting.ioNum);

            localEndPoint = NetUtility.GetIPEndPoint(setting.host, setting.port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            socket.Bind(localEndPoint);
            ReceiveFromAsync(new IPEndPoint(IPAddress.Any, setting.port));
        }

        protected override bool OnDecodeReceive(byte[] buffer, int offset, int count, EndPoint endPoint, out int error)
        {
            error = 0;


            return true;
        }

        public UDPSetting setting { get; private set; }
        public IPEndPoint localEndPoint;
    }
}
