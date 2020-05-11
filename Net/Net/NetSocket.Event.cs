using System.Net;
using System.Net.Sockets;

namespace Net
{
    public partial class NetSocket
    {
        protected virtual void OnClose()
        {
            ResetSocket();
        }

        protected virtual void OnDispose()
        {

        }

        protected virtual void OnSAEACompletedCallback(object sender, SocketAsyncEventArgs saea)
        {

        }

        protected virtual void OnCloseSAEA(SocketAsyncEventArgs saea)
        {

        }

        protected virtual void OnReceiveAsyncCallback(RawMessage message)
        {
            onReceiveCallback.SafeInvoke(message);
        }

        protected virtual bool OnEncodeSend(byte[] buffer, int offset, int count, byte[] data, out int packCount)
        {
            packCount = count;
            return false;
        }

        protected virtual bool OnDecodeReceive(byte[] buffer, int offset, int count, EndPoint endPoint, out int error)
        {
            error = 0;
            return false;
        }
    }
}
