using Net.KCPLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Net.UDP
{
    public class KCPConnection : NetSocket
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

            InitKCP(setting);
        }

        public void Start()
        {
            ReceiveFromAsync(remoterPoint);
        }

        public void Send(byte[] data)
        {
            if (null != _kcpObject)
            {
                _kcpObject.Send(data, 0, data.Length);
            }
        }

        public void Update(UInt32 current)
        {
            if (null != _kcpObject)
            {
                _kcpObject.Update(current);
            }
        }

        protected override void OnClose()
        {
            base.OnClose();
            if (null != _kcpObject)
            {
                _kcpObject.Dispose();
                _kcpObject = null;
            }
        }

        protected override bool OnDecodeReceive(byte[] buffer, int offset, int count, EndPoint endPoint, out int error)
        {
            error = 0;
            if (null != _kcpObject)
            {
                _kcpObject.Receive(buffer, offset, count);
            }

            return true;
        }

        private void InitKCP(UDPSetting setting)
        {
            _kcpObject = new KCPStateObject();
            _kcpObject.Initialize(this);
            _kcpObject.remote = remoterPoint;

            _kcpObject.sendHandle = OnKCPSend;
            _kcpObject.receiveHandle = OnKCPSend;
        }

        private void OnKCPSend(byte[] buffer, int offset, int count, EndPoint endPoint)
        {
            SendToAsync(endPoint, buffer, offset, count);
        }

        private void OnKCPReceive(byte[] buffer, int offset, int count, EndPoint endPoint)
        {
            NotifyReceiveMessage(buffer, offset, count, endPoint);
        }

        public UDPSetting setting { get; private set; }
        public IPEndPoint remoterPoint { get; private set; }

        private KCPStateObject _kcpObject;
    }
}
