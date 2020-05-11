using Net;

namespace NetTest
{
    public class TestDefine
    {
        public static string host = "127.0.0.1";
        public static int tcp_port = 50500;

        public static char messageSplit = '@';

        public static TCPSetting GetTCPSetting()
        {
            return new TCPSetting(host, tcp_port, backlog: 10);
        }
    }
}
