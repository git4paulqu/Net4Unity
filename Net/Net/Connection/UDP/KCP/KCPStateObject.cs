using System.Net;
using System;
using Net.KCPLib;

namespace Net.UDP
{
    public class KCPStateObject
    {
        public void Initialize(object user)
        {
            _kcp = new KCP(0xAABBCCDD, user);
            _kcp.SetOutput(OutputKCP);

            // fast mode
            _kcp.NoDelay(1, 10, 2, 1);
            _kcp.WndSize(128, 128);

            kcpRcvBuf = new byte[(KCP.IKCP_MTU_DEF + KCP.IKCP_OVERHEAD) * 3];
        }

        public void Dispose()
        {
            if (null != _kcp)
            {
                _kcp.Release();
            }
            _kcp = null;

            int messageCount = revQueue.Count;
            for (int i = 0; i < messageCount; i++)
            {
                RawMessage message = null;
                if (revQueue.TryDequeue(out message))
                {
                    RawMessage.Clear(message);
                }
            }
            revQueue.Clear();
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            if (null != _kcp)
            {
                _kcp.Send(buffer, offset, count);
            }

            _needUpdate = true;
        }

        public void Receive(byte[] buffer, int offset, int count)
        {
            RawMessage message = RawMessage.Clone(buffer, offset, count);
            revQueue.Enqueue(message);
        }

        public void Update(UInt32 current)
        {
            if (null == _kcp)
            {
                return;
            }

            ProcessReceive(current);
            if (_needUpdate || current > _nextUpdateTime)
            {
                _kcp.Update(current);
                _nextUpdateTime = _kcp.Check(current);
                _needUpdate = false;
            }
            CheckTimeout(current);
        }

        private void OutputKCP(byte[] data, int size, object ud)
        {
            sendHandle.SafeInvoke(data, 0, size, remote);
        }

        private void ProcessReceive(UInt32 current)
        {
            while (revQueue.Count > 0)
            {
                _lastRecvTime = current;

                RawMessage data = null;
                if (!revQueue.TryDequeue(out data))
                {
                    continue;
                }

                int r = _kcp.Input(data.buffer, 0, data.size);
                RawMessage.Clear(data);

                if (r < 0)
                {
                    continue;
                }
            
                _needUpdate = true;
                while (true)
                {
                    var size = _kcp.PeekSize();
                    if (size > 0)
                    {
                        r = _kcp.Recv(kcpRcvBuf, 0, kcpRcvBuf.Length);
                        if (r <= 0)
                        {
                            break;
                        }
                        receiveHandle.SafeInvoke(kcpRcvBuf, 0 , size, remote);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void CheckTimeout(UInt32 current)
        {
            if (_lastRecvTime == 0)
            {
                _lastRecvTime = current;
            }
            if (current - _lastRecvTime > _recvTimeoutSec * 1000)
            {
                if (null != timeoutCallback)
                {
                    timeoutCallback(this);
                }
            }
        }

        public EndPoint remote;

        public KCPHandleCallback sendHandle;
        public KCPHandleCallback receiveHandle;
        public Action<KCPStateObject> timeoutCallback;

        private KCP _kcp;

        private Int64 _lastRecvTime = 0;
        private int _recvTimeoutSec = 0;

        private bool _needUpdate = false;
        private UInt32 _nextUpdateTime = 0;

        private byte[] kcpRcvBuf;
        private ThreadSafedQueue<RawMessage> revQueue = new ThreadSafedQueue<RawMessage>();
    }

    public delegate void KCPHandleCallback(byte[] buffer, int offset, int count, EndPoint remote);
}
