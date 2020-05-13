using Net.UDP;
using System.Net;

namespace Net
{
    public static class NetExtends
    {
        public static void SafeInvoke(this NetEventCallback callback, INetEventObject message)
        {
            if (callback != null)
            {
                callback.Invoke(message);
            }
        }

        public static void SafeInvoke(this NetRecevieEventCallback callback, RawMessage message)
        {
            if (callback != null)
            {
                callback.Invoke(message);
            }
        }

        public static void SafeInvoke(this KCPHandleCallback callback, byte[] buffer, int offset, int count, EndPoint remote)
        {
            if (callback != null)
            {
                callback.Invoke(buffer, offset, count, remote);
            }
        }
    }
}