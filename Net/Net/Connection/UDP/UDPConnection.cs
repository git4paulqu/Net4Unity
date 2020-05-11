using System.Net;
using System.Net.Sockets;

namespace Net.UDP
{
    public class UDPConnection : NetSocket
    {
        public void Connect(UDPSetting setting)
        {
            if (null == setting)
            {
                NetDebug.Error("[UDPConnection] the setting can not be null.");
                return;
            }

            this.setting = setting;
            Initialize(this.setting.ioNum);

            remoterPoint = NetUtility.GetIPEndPoint(setting.host, setting.port);
            socket = new Socket(remoterPoint.AddressFamily,
                                SocketType.Dgram,
                                ProtocolType.Udp);
            socket.Connect(remoterPoint);
            ReceiveFromAsync(remoterPoint);
        }

        public void Send(byte[] data)
        {
            SendToAsync(remoterPoint, data);
        }

        public UDPSetting setting { get; private set; }
        public IPEndPoint remoterPoint { get; private set; }
    }
}
