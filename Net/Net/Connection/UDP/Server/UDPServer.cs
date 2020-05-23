using System;
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

            kcpHandle = new KCPServerHandle(setting);
            kcpHandle.onSendHandle = OnSend;
            kcpHandle.onReceiveHandle = OnReceive;

            Start();
        }

        private void Start()
        {
            ReceiveFromAsync(new IPEndPoint(IPAddress.Any, setting.port));
        }

        public void Send(EndPoint endPoint, byte[] data)
        {
            Send(endPoint, data, 0, data.Length);
        }

        public void Send(EndPoint endPoint, byte[] data, int offset, int count)
        {
            if (null != kcpHandle)
            {
                kcpHandle.Send(endPoint, data, offset, count);
            }
        }

        public void Update()
        {
            if (null != kcpHandle)
            {
                UInt32 current = NetUtility.Time.GetIclock();
                kcpHandle.Update(current);
            }
        }

        protected override bool OnDecodeReceive(byte[] buffer, int offset, int count, EndPoint endPoint, out int error)
        {
            error = 0;
            if (null != kcpHandle)
            {
                kcpHandle.Receive(endPoint, buffer, offset, count);
            }

            return true;
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        private void OnSend(byte[] buffer, int offset, int count, EndPoint remote)
        {
            SendToAsync(remote, buffer, offset, count);
        }

        private void OnReceive(byte[] buffer, int offset, int count, EndPoint remote)
        {
            NotifyReceiveMessage(buffer, offset, count, remote);
        }

        public UDPSetting setting { get; private set; }
        public IPEndPoint localEndPoint;
        private KCPServerHandle kcpHandle { get; set; }
    }
}
