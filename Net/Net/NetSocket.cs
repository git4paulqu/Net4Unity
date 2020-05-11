using System;
using System.Net.Sockets;

namespace Net
{
    public abstract partial class NetSocket
    {
        public void Close()
        {
            OnClose();
        }

        public void Dispose()
        {
            Close();

            if (null != saeaMemory)
            {
                saeaMemory.Dispose();
                saeaMemory = null;
            }

            if (null != receiveBuffer)
            {
                receiveBuffer.Dispose();
                receiveBuffer = null;
            }

            OnDispose();
        }

        protected void Initialize(int ioNum, int bufferSize = NetDefine.DEFAUT_BUFFER_SIZE)
        {
            saeaMemory = new SAEAMemory(ioNum, 
                                         new EventHandler<SocketAsyncEventArgs>(OnSAEACompleted),
                                         bufferSize);

            receiveBuffer = new MemoryBufferStream(ioNum * bufferSize);
        }

        protected virtual void Reset()
        {
            ResetSocket();
        }

        protected virtual void ResetSocket()
        {
            CloseSocket(socket);
            socket = null;
        }

        private void CloseSocket(Socket closeSocket)
        {
            try
            {
                if (null != closeSocket)
                {
                    closeSocket.Shutdown(SocketShutdown.Both);
                    closeSocket.Close();
                }
            }
            catch { }
        }

        public NetRecevieEventCallback onReceiveCallback { get; set; }
        protected virtual bool ready4Send { get; }
        protected Socket socket { get; set; }
        protected SAEAMemory saeaMemory { get; set; }
        protected MemoryBufferStream receiveBuffer { get; set; }
    }
}
