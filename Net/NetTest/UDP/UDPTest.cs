using Net;
using Net.KCPLib;
using Net.UDP;
using System;
using System.Text;
using System.Threading;

namespace NetTest
{
    public class UDPTest
    {
        public static void Run()
        {
            UDPSetting setting = new UDPSetting("127.0.0.1", 50666);

            UDPServer s = new UDPServer();
            s.Listen(setting);

            s.onReceiveCallback += (message) => 
            {
                Util.Debug.ServerLog("Receive from:{0}, {1}", message.remote.ToString(), Encoding.ASCII.GetString(message.GetRawMessage()));
                s.Send(message.remote, data: message.GetRawMessage());
            };

            //UDPConnection c = new UDPConnection();
            //c.Connect(setting);
            //c.onReceiveCallback += (message) =>
            //{
            //    Util.Debug.ClientLog("Receive from:{0}, {1}", message.remote, Encoding.ASCII.GetString(message.GetRawMessage()));
            //};

            //byte[] data = System.Text.Encoding.Default.GetBytes("hello udp.");
            //c.Send(data);

            TestSocket();
        }

        public static void TestSocket()
        {

            KCPSocket k1 = new KCPSocket();


            UInt32 conv = 0x12345678;
            var counter = 1;
            var originText = "counter:";
            var rawbytes = Encoding.UTF8.GetBytes(String.Format("{0} {1}", originText, counter));

            KCPSocket sock = new KCPSocket();
            sock.SetHandler((byte[] data, int size) =>
            {
                Console.WriteLine(Encoding.UTF8.GetString(data, 0, size));

                Thread.Sleep(500);
                //rawbytes = Encoding.UTF8.GetBytes(counter.ToString());
                //sock.Send(rawbytes, 0, rawbytes.Length);
            });

            sock.Connect(conv, "127.0.0.1", 50666);
            sock.StartRead();


            rawbytes = new byte[2000];
            for (int i = 0; i < 10; i++)
            {
                rawbytes[i] = 9;
            }

            sock.Send(rawbytes, 0, rawbytes.Length);

            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    sock.Update(Utils.iclock());
                }
                catch (Exception ex)
                {
                    sock.Close();
                    Console.WriteLine("Exception: {0}", ex);
                    break;
                }
            }
        }
    }
}
