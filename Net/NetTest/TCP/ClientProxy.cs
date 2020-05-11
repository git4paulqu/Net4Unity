using Net;
using Net.TCP;

namespace NetTest.TCP
{
    public class ClientProxy
    {
        public void Start()
        {
            TCPSetting clientSetting = TestDefine.GetTCPSetting();
            client = new TCPClient(clientSetting);
            client.onConnectCallback += OnConnected;
            client.onConnectFailedCallback += OnConnectedFail;
            client.onClosedCallback += OnCloseCallback;
            client.onReceiveCallback += OnRevecie;
            client.Connect();
        }

        public void Stop()
        {
            client.Close();
        }

        private void OnConnected(INetEventObject message)
        {
            Debug("Client Connected Success");
            TCPTest.OnClientConnect(this);
        }

        private void OnConnectedFail(INetEventObject message)
        {
            Debug("Client Connected Failed");
        }

        private void OnCloseCallback(INetEventObject message)
        {
            RawMessage msg = message as RawMessage;
            string port = msg.userData as string;

            Debug("Client OnClose, port:{0}", port);
            TCPTest.OnClientClose(this);
        }

        private void OnRevecie(RawMessage rawMessage)
        {
            string remote = client.remote;
            string content = System.Text.Encoding.Default.GetString(rawMessage.buffer);

            Debug("OnRevecie from:{0} content:{1}", remote, content);
        }

        private void Debug(string format, params object[] args)
        {
            format = string.Format("{0}: {1}", local, format);
            Util.Debug.ClientLog(format, args);
        }

        public string local
        {
            get
            {
                if (null == client)
                {
                    return string.Empty;
                }

                return client.local.Replace("127.0.0.1:", "");
            }
        }

        public TCPClient client;
    }
}
