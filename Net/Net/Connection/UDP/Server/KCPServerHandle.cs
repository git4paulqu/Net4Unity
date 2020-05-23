using System.Net;
using System.Collections.Generic;
using System;

namespace Net.UDP
{
    public class KCPServerHandle
    {
        public KCPServerHandle(UDPSetting setting)
        {
            this.setting = setting;
        }

        public void Send(EndPoint remote, byte[] data)
        {
            Send(remote, data, 0, data.Length);
        }

        public void Send(EndPoint remote, byte[] data, int offset, int count)
        {
            KCPStateObject kcp = InitKCPObject(remote);
            kcp.Send(data, offset, count);
        }

        public void Receive(EndPoint remote, byte[] data, int offset, int count)
        {
            KCPStateObject kcp = InitKCPObject(remote);
            kcp.Receive(data, offset, count);
        }

        public void Close()
        {
            lock (lockObject)
            {
                waitRemoveQueque.Clear();
                foreach (var item in map_remote2KCP)
                {
                    waitRemoveQueque.Add(item.Key);
                }
            }
        }

        public void Close(EndPoint remote)
        {
            lock (lockObject)
            {
                string remoteKey = RemotePoint2Key(remote);
                waitRemoveQueque.Add(remoteKey);
            }
        }

        public void Update(UInt32 current)
        {
            lock (lockObject)
            {
                foreach (var item in waitRemoveQueque)
                {
                    CloseKCP(item);
                }
                waitRemoveQueque.Clear();

                foreach (var item in map_remote2KCP)
                {
                    item.Value.Update(current);
                }
            }
        }

        private KCPStateObject InitKCPObject(EndPoint remote)
        {
            lock (lockObject)
            {
                KCPStateObject kcp = null;
                string remoteKey = RemotePoint2Key(remote);
                if (map_remote2KCP.TryGetValue(remoteKey, out kcp))
                {
                    return kcp;
                }

                NetDebug.Log("[S] InitKCPObject, remote:{0}", remote);

                kcp = new KCPStateObject();
                kcp.Initialize(remote, setting.kcp);
                kcp.remote = remote;

                kcp.sendHandle = OnKCPSend;
                kcp.receiveHandle = OnKCPReceive;
                kcp.timeoutCallback = OnTimeoutCallback;
                map_remote2KCP.Add(remoteKey, kcp);

                return kcp;
            }
        }

        private void OnKCPSend(byte[] buffer, int offset, int count, EndPoint endPoint)
        {
            if (null != onReceiveHandle)
            {
                onSendHandle(buffer, offset, count, endPoint);
            }
        }

        private void OnKCPReceive(byte[] buffer, int offset, int count, EndPoint endPoint)
        {
            if (null != onReceiveHandle)
            {
                onReceiveHandle(buffer, offset, count, endPoint);
            }
        }

        private void OnTimeoutCallback(KCPStateObject kcpObject)
        {
            NetDebug.Log("[OnTimeoutCallback] {0}", kcpObject.remote);
            if (null != kcpObject)
            {
                Close(kcpObject.remote);
            }
        }

        private string RemotePoint2Key(EndPoint remote)
        {
            return remote.ToString();
        }

        private void CloseKCP(string remoteKey)
        {
            lock (lockObject)
            {
                KCPStateObject kcp;
                if (map_remote2KCP.TryGetValue(remoteKey, out kcp))
                {
                    if (null != kcp)
                    {
                        kcp.Dispose();
                    }
                    map_remote2KCP.Remove(remoteKey);
                }
            }
        }
        
        private object lockObject = new object();
        private UDPSetting setting { get; set; }
        private Dictionary<string, KCPStateObject> map_remote2KCP = new Dictionary<string, KCPStateObject>();
        private List<string> waitRemoveQueque = new List<string>();

        public OnReceiveHandle onSendHandle;
        public OnReceiveHandle onReceiveHandle;
        public delegate void OnReceiveHandle(byte[] buffer, int offset, int count, EndPoint remote);
    }
}
