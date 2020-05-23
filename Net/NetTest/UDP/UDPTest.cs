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
        static UDPServer server;
        static KCPConnection client;


        public static void Run()
        {
            UDPSetting setting = new UDPSetting("127.0.0.1", 50666, KCPSetting.Defaut);

            server = new UDPServer();
            server.Listen(setting);
            server.onReceiveCallback = (message) =>
            {
                byte[] rev = message.GetRawMessage();
                string s = System.Text.Encoding.Default.GetString(rev);
                Util.Debug.ServerLog("REV {0}：{1}", message.remote, s);

                server.Send(message.remote, rev);
            };


            int counter = 0;

            client = new KCPConnection();
            client.Connect(setting);
            client.Start();
            client.onReceiveCallback = (message) =>
            {
                byte[] rev = message.GetRawMessage();
                string s = System.Text.Encoding.Default.GetString(rev);
                Util.Debug.ClientLog("REV {0}：{1}", message.remote, s);

                Thread.Sleep(500);

                counter++;
                byte[] sd = System.Text.Encoding.Default.GetBytes(counter.ToString());
                client.Send(sd);
            };


            byte[] data = System.Text.Encoding.Default.GetBytes(counter.ToString());
            client.Send(data);

            Thread runThread = new Thread(RunThread);
            runThread.Start();

        }

        private static void RunThread()
        {
            while (true)
            {
                Thread.Sleep(3);

                try
                {
                    server.Update();
                    client.Update();
                }
                catch (Exception ex)
                {
                    server.Close();
                    Console.WriteLine("Exception: {0}", ex);
                    break;
                }
            }
        }

    }
}
