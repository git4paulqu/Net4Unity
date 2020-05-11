using System;
using System.Net;
using System.Net.Sockets;

namespace Net
{
    public partial class NetSocket
    {
        protected void ReceiveAsync()
        {
            SocketAsyncEventArgs saea = AllocSAEA();
            ReceiveAsync(saea);
        }

        protected void ReceiveFromAsync(IPEndPoint endPoint)
        {
            SocketAsyncEventArgs saea = AllocSAEA();
            saea.RemoteEndPoint = endPoint;
            ReceiveFromAsync(saea);
        }

        protected void SendAsync(byte[] data)
        {
            if (!ready4Send)
            {
                NetDebug.Log("[NetSocket] SendAsync, not ready for send.");
                return;
            }

            if (null == data || data.Length == 0)
            {
                NetDebug.Log("[NetSocket] SendAsync, the data can not be null.");
                return;
            }

            if (data.Length > NetDefine.MAX_MESSAGE_LENGTH)
            {
                NetDebug.Error("[Netsocket] SendAsync, the data length:{0} is greater message max length:{1}.",
                               data.Length,
                               NetDefine.MAX_MESSAGE_LENGTH);
                return;
            }

            SocketAsyncEventArgs saea = AllocSAEA();
            try
            {
                EncodeSend(saea, data);
                bool willRaiseEvent = socket.SendAsync(saea);
                if (!willRaiseEvent)
                {
                    SendAsyncCallback(saea);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] SendAsync, error:{0}.", ex.Message.ToString());
            }
        }

        protected void SendToAsync(EndPoint endPoint, byte[] data)
        {
            if (null == data || data.Length == 0)
            {
                NetDebug.Log("[NetSocket] SendToAsync, the data can not be null.");
                return;
            }

            if (data.Length > NetDefine.MAX_MESSAGE_LENGTH)
            {
                NetDebug.Error("[Netsocket] SendToAsync, the data length:{0} is greater message max length:{1}.",
                               data.Length,
                               NetDefine.MAX_MESSAGE_LENGTH);
                return;
            }

            SocketAsyncEventArgs saea = AllocSAEA();
            try
            {
                EncodeSend(saea, data);
                saea.RemoteEndPoint = endPoint;
                bool willRaiseEvent = socket.SendToAsync(saea);
                if (!willRaiseEvent)
                {
                    SendToAsyncCallback(saea);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] SendToAsync, error:{0}.", ex.Message.ToString());
            }
        }

        #region SAEA Operating

        private void OnSAEACompleted(object sender, SocketAsyncEventArgs saea)
        {
            switch (saea.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ReceiveAsyncCallback(saea);
                    break;

                case SocketAsyncOperation.ReceiveFrom:
                    ReceiveFromAsyncCallback(saea);
                    break;

                case SocketAsyncOperation.Send:
                    SendAsyncCallback(saea);
                    break;

                case SocketAsyncOperation.SendTo:
                    SendToAsyncCallback(saea);
                    break;

                default:
                    OnSAEACompletedCallback(sender, saea);
                    break;
            }
        }

        private void ReceiveAsync(SocketAsyncEventArgs saea)
        {
            try
            {
                bool willRaiseEvent = socket.ReceiveAsync(saea);
                if (!willRaiseEvent)
                {
                    ReceiveAsyncCallback(saea);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] ReceiveAsync, error:{0}.", ex.Message.ToString());
            }
        }

        private void ReceiveAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                int read = saea.BytesTransferred;
                if (read > 0 && saea.SocketError == SocketError.Success)
                {
                    DecodeReceive(saea);
                    ReceiveAsync(saea);
                }
                else
                {
                    CloseSAEA(saea);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] ReceiveAsyncCallback, error:{0}.", ex.Message.ToString());
            }
        }

        private void ReceiveFromAsync(SocketAsyncEventArgs saea)
        {
            try
            {
                bool willRaiseEvent = socket.ReceiveFromAsync(saea);
                if (!willRaiseEvent)
                {
                    ReceiveFromAsyncCallback(saea);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] ReceiveFromAsync, error:{0}.", ex.Message.ToString());
            }
        }

        private void ReceiveFromAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                int read = saea.BytesTransferred;
                if (read > 0 && saea.SocketError == SocketError.Success)
                {
                    DecodeReceive(saea);
                    ReceiveFromAsync(saea);
                }
                else
                {
                    CloseSAEA(saea);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Log("[NetSocket] ReceiveAsyncCallback, error:{0}.", ex.Message.ToString());
            }
        }

        private void SendAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                int send = saea.BytesTransferred;
                if (send <= 0 || saea.SocketError != SocketError.Success)
                {
                    CloseSAEA(saea);
                }
                else
                {
                    FreeSAEA(saea);
                }
            }
            catch { }
        }

        private void SendToAsyncCallback(SocketAsyncEventArgs saea)
        {
            try
            {
                int send = saea.BytesTransferred;
                if (send <= 0 || saea.SocketError != SocketError.Success)
                {
                    CloseSAEA(saea);
                }
                else
                {
                    FreeSAEA(saea);
                }
            }
            catch { }
        }

        #endregion  #region SAEA operating

        #region Decode/Encode

        private void EncodeSend(SocketAsyncEventArgs saea, byte[] data)
        {
            try
            {
                byte[] buffer = saea.Buffer;
                int offset = saea.Offset;
                int count = data.Length;
                int packCount = count;
                if (OnEncodeSend(buffer, offset, count, data, out packCount))
                {
                    saea.SetBuffer(offset, packCount);
                }
                else
                {
                    saea.SetBuffer(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Error("[NetSocket] OnSend, error: {0}.", ex.Message.ToString());
            }
        }

        private void DecodeReceive(SocketAsyncEventArgs saea)
        {
            try
            {
                byte[] buffer = saea.Buffer;
                int offset = saea.Offset;
                int count = saea.BytesTransferred;

                int error = 0;
                if (!OnDecodeReceive(buffer, offset, count, saea.RemoteEndPoint, out error))
                {
                    if (error > 0)
                    {
                        CloseSAEA(saea);
                        return;
                    }

                    RawMessage message = RawMessage.Clone(buffer, offset, count);
                    message.remote = saea.RemoteEndPoint;
                    OnReceiveAsyncCallback(message);
                }
            }
            catch (Exception ex)
            {
                NetDebug.Error("[NetSocket] OnReceive, error: {0}.", ex.Message.ToString());
            }
        }

        #endregion 

        #region SAEA etc.
        
        private SocketAsyncEventArgs AllocSAEA()
        {
            if (null == saeaMemory)
            {
                NetDebug.Error("[NetSocket] AllocSAEA, the saea memory is null.");
                return null;
            }

            return saeaMemory.Alloc();
        }

        private void FreeSAEA(SocketAsyncEventArgs saea)
        {
            if (null == saeaMemory)
            {
                return;
            }
            saeaMemory.Recycle(saea);
            NetDebug.Log("Recycle Send SAEA count:{0} --{1}", saeaMemory.count, GetType().ToString());
        }

        private void CloseSAEA(SocketAsyncEventArgs saea)
        {
            FreeSAEA(saea);
            OnCloseSAEA(saea);
        }

        #endregion SAEA
    }
}