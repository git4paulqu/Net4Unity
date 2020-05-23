using System;
using System.Net;

namespace Net
{
    public class RawMessage : BufferObject, INetEventObject
    {
        public static RawMessage Clone(byte[] data, int offset, int length, int capacity)
        {
            RawMessage message = BufferObjectCache.Alloc<RawMessage>(capacity);
            if (null == data)
            {
                message.size = 0;
                return message;
            }

            message.size = length;
            Buffer.BlockCopy(data, offset, message.buffer, 0, length);
            return message;
        }

        public static void Clear(RawMessage rawMessage)
        {
            BufferObjectCache.Free(rawMessage);
        }

        public override void OnRecycle()
        {
            size = 0;
            userData = null;
            remote = null;
            base.OnRecycle();
        }

        public byte[] GetRawMessage()
        {
            if (size == 0)
            {
                return null;
            }

            byte[] raw = new byte[size];
            Buffer.BlockCopy(buffer, 0, raw, 0, size);
            return raw;
        }

        public int size { get; private set; }
        public object userData { get; set; }
        public EndPoint remote { get; set; }
    }
}