namespace Net
{
    public class MemoryBufferStream
    {
        public MemoryBufferStream(int capacity)
        {
            length = 0;
            buffer = new byte[capacity];
        }

        public void Copy(byte[] data)
        {
            Copy(data, 0, data.Length);
        }

        public void Copy(byte[] data, int offset, int count)
        {
            if (null == data)
            {
                NetDebug.Error("[MemoryBufferStream] copy, the data can not be null.");
                return;
            }

            int freeLength = GetFreeLength();
            if (freeLength >= count)
            {
                System.Buffer.BlockCopy(data, offset, buffer, length, count);
            }
            else
            {
                int size = buffer.Length + count - freeLength;
                byte[] alloc = new byte[size];
                System.Buffer.BlockCopy(buffer, 0, alloc, 0, length);
                System.Buffer.BlockCopy(data, offset, alloc, length, count);
                buffer = alloc;
            }
            length += count;
        }

        public void Clear(int count = int.MaxValue)
        {
            if (count >= length)
            {
                length = 0;
                return;
            }

            for (int i = 0; i < length - count; ++i)
            {
                buffer[i] = buffer[i + count];
            }

            length -= count;
        }

        public void Dispose()
        {
            length = 0;
            buffer = null;
        }

        private int GetFreeLength()
        {
            if (null == buffer)
            {
                return 0;
            }
            return buffer.Length - length;
        }

        public int length { get; private set; }
        public byte[] buffer { get; private set; }
    }
}