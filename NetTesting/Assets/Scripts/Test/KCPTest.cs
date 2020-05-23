//=====================================================
// - FileName:      KCPTest.cs
// - UserName:      #AuthorName#
// - Created:       #CreateTime#
// - Description:   
//======================================================
using KCPLib;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace NetTest
{
    public class KCPTest
    {

        public static void Run()
        {
            kcp1 = new KCPObject(1);
            kcp2 = new KCPObject(2);

            kcp1.sendCallback = OnSendCallback;
            kcp2.sendCallback = OnSendCallback;

            kcp1.rcvCallback = OnReceiveCallback;
            kcp2.rcvCallback = OnReceiveCallback;

            System.Threading.Thread thread = new System.Threading.Thread(Update);
            thread.Start();

            byte[] sb = Encoding.UTF8.GetBytes("0");
            kcp1.Send(sb, 0, sb.Length);
        }

        private static void OnSendCallback(byte[] data, int count, int user)
        {
            KCPObject rcvKCP;
            if (user == 1)
            {
                rcvKCP = kcp2;
            }
            else
            {
                rcvKCP = kcp1;
            }

            Logger.Log("[SEND-{0}] count:{1}, data:{2}", user, count, BitConverter.ToString(data, 0, count));

            byte[] message = new byte[count];
            Buffer.BlockCopy(data, 0, message, 0, count);
            rcvKCP.Receive(message);
        }

        private static void OnReceiveCallback(byte[] data, int count, int user)
        {
            KCPObject sendKCP;
            if (user == 1)
            {
                sendKCP = kcp1;
            }
            else
            {
                sendKCP = kcp2;
            }

            string sd = Encoding.UTF8.GetString(data, 0, count);
            int id = int.Parse(sd);

            id++;
            byte[] sb = Encoding.UTF8.GetBytes(id.ToString());
            Logger.Log("[REV-{0}] {1}", user, sd);

            Thread.Sleep(300);
            sendKCP.Send(sb, 0, sb.Length);
        }

        private static void Update()
        {
            while (true)
            {
                UInt32 iclock = Utils.iclock();

                kcp1.Update(iclock);
                kcp2.Update(iclock);

                Thread.Sleep(3);
            }
        }

   
        private static KCPObject kcp1;
        private static KCPObject kcp2;

    }

    public class Logger
    {
        public static void Log(string format, params object[] args)
        {
            string message = string.Format(format, args);
            UnityEngine.Debug.Log(message);
        }

    }

    public class KCPObject
    {

        public KCPObject(int user)
        {
            this.user = user;

            kcp = KCP.KcpCreate(0xAABBCCDD, new IntPtr(user));

            kcpOutput = KCPOutput;
            KCP.KcpSetoutput(this.kcp, kcpOutput);

            // fast mode
            KCP.KcpNodelay(kcp, 1, 10, 2, 1);
            KCP.KcpWndsize(kcp, 128, 128);
        }

        public void Send(byte[] data, int offset, int count)
        {
            KCP.KcpSend(kcp, data, count);
        }

        public void Receive(byte[] data)
        {
            rcvQueue.Enqueue(data);
        }

        public void Update(UInt32 current)
        {
            ProcessReceive(current);
            KCP.KcpUpdate(kcp, current);
        }

        private void ProcessReceive(UInt32 current)
        {
            while (rcvQueue.Count > 0)
            {
                var data = rcvQueue.Dequeue();
                int r = KCP.KcpInput(kcp, data, 0, data.Length);

                if (r < 0)
                {
                    Logger.Log("[user-{0}] input error, r:{1}", user, r);
                    break;
                }

                var size = KCP.KcpPeeksize(kcp);
                if (size > 0)
                {
                    r = KCP.KcpRecv(kcp, kcpRcvBuf, kcpRcvBuf.Length);
                    if (r <= 0)
                    {
                        break;
                    }
                    rcvCallback(kcpRcvBuf, size, user);
                }
                else
                {
                    Logger.Log("[user-{0}] peek error, r:{1}", user, size);
                    break;
                }
            }
        }

        private int KCPOutput(IntPtr buf, int len, IntPtr kcp, IntPtr user)
        {
            Marshal.Copy(buf, kcpOutputBuff, 0, len);
            OutputKCP(kcpOutputBuff, len, user);
            return len;
        }

        void OutputKCP(byte[] data, int size, object ud)
        {
            sendCallback(data, size, user);
        }

        private KcpOutput kcpOutput;

        public int user;
        public IntPtr kcp;

        private byte[] kcpOutputBuff = new byte[64000];
        public byte[] kcpRcvBuf = new byte[6400];
        public Queue<byte[]> rcvQueue = new Queue<byte[]>();

        public Action<byte[], int, int> rcvCallback;
        public Action<byte[], int, int> sendCallback;
    }

}
