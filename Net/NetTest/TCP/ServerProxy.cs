using Net;
using Net.TCP.Server;

namespace NetTest.TCP
{
    public class ServerProxy
    {
        public void Start()
        {
            TCPSetting serverSetting = TestDefine.GetTCPSetting();
            server = new TCPServer(serverSetting);
            server.onConnectionConnect += OnAccepted;
            server.onConnectionDropped += OnConnectionDropped;
            server.onReceiveCallback += OnRevecie;
            server.Listen();
        }

        public void Stop()
        {
            server.Close();
        }

        private void OnAccepted(INetEventObject message)
        {
            Net.TCP.Server.ConnectionMessage connectionMessage = message as Net.TCP.Server.ConnectionMessage;
            Util.Debug.ServerLog("Accepted form:{0}", connectionMessage.remote);
        }

        private void OnConnectionDropped(INetEventObject message)
        {
            Net.TCP.Server.ConnectionMessage connectionMessage = message as Net.TCP.Server.ConnectionMessage;
            Util.Debug.ServerLog("OnConnectionDropped form:{0}", connectionMessage.remote.ToString());
        }

        private void OnRevecie(RawMessage rawMessage)
        {
            string remote = (string)rawMessage.userData;
            string content = System.Text.Encoding.Default.GetString(rawMessage.buffer);

            Util.Debug.ServerLog("OnRevecie from:{0} content:{1}", remote, content);

            server.Send(remote, rawMessage.buffer);
        }

        public TCPServer server;
    }
}
