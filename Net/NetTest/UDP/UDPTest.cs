using Net;
using Net.UDP;
using System.Text;

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
                Util.Debug.ServerLog("Receive from:{0}, {1}", message.remote, Encoding.ASCII.GetString(message.buffer));
                s.Send(message.remote, data: message.buffer);
            };

            UDPConnection c = new UDPConnection();
            c.Connect(setting);
            c.onReceiveCallback += (message) =>
            {
                Util.Debug.ClientLog("Receive from:{0}, {1}", message.remote, Encoding.ASCII.GetString(message.buffer));
            };

            byte[] data = System.Text.Encoding.Default.GetBytes("hello udp.");
            c.Send(data);
        }
    }
}
