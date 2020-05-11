using System;
using System.Collections.Generic;

namespace NetTest.TCP
{
    public class TCPTest
    {
        public static void Run()
        {
            RegisterEvent();

            server = new ServerProxy();
            server.Start();

            int count = 2;
            for (int i = 0; i < count; i++)
            {
                ClientProxy c = new ClientProxy();
                c.Start();
            }
        }

        public static void OnClientConnect(ClientProxy client)
        {
            clientMap[client.local] = client;
        }

        public static void OnClientClose(ClientProxy client)
        {
            if (clientMap.ContainsKey(client.local))
            {
                clientMap.Remove(client.local);
            }
        }

        private static void RegisterEvent()
        {
            InputHandle.Register(Commond.Commond.lstc, SelectAllClient);
            InputHandle.Register(Commond.Commond.tcs, ClientSend);
            InputHandle.Register(Commond.Commond.tcstop, ClientStop);
            InputHandle.Register(Commond.Commond.tsstop, ServerStop);
        }

        private static void SelectAllClient(string args)
        {
            string result = "all client:";
            foreach (var item in clientMap)
            {
                result += "\r\n";
                result += item.Value.local;
            }
            Util.Debug.Log(result);
        }

        private static void ClientSend(string args)
        {
            string id;
            string message;
            Util.SplitMessage(args, out id, out message);

            if (null == id)
            {
                Util.Debug.Log("Client send message, but the client id is null.");
                return;
            }

            ClientProxy clientProxy;
            if (!GetClient(id, out clientProxy))
            {
                Util.Debug.Log("Client send message, but the client:{0} is null.", id);
                return;
            }

            byte[] data = System.Text.Encoding.Default.GetBytes(message);
            clientProxy.client.Send(data);
        }

        private static void ClientStop(string args)
        {
            string id;
            string message;
            Util.SplitMessage(args, out id, out message);

            if (null == id)
            {
                Util.Debug.Log("Client stop, but the client id is null.");
                return;
            }

            ClientProxy clientProxy;
            if (!GetClient(id, out clientProxy))
            {
                Util.Debug.Log("Client stop, but the client:{0} is null.", id);
                return;
            }

            clientProxy.Stop();
        }

        private static void ServerStop(string args)
        {
            server.Stop();
        }

        private static bool GetClient(string key, out ClientProxy clientProxy)
        {
            return clientMap.TryGetValue(key, out clientProxy);
        }

        private static ServerProxy server;
        private static Dictionary<string, ClientProxy> clientMap = new Dictionary<string, ClientProxy>();
    }
}