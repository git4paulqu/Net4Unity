namespace Net
{
    public class BufferObject
    {
        public void Alloc(int capacity)
        {
            if (capacity > 0)
            {
                buffer = new byte[capacity];
            }
        }

        public void Dispose()
        {
            OnDispose();
            buffer = null;
        }

        public virtual void OnAlloc()
        {

        }

        public virtual void OnRecycle()
        {

        }

        public virtual void OnDispose()
        {

        }

        public byte[] buffer { get; private set; }
    }
}
