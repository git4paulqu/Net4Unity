namespace Net
{
    public class NetDefine
    {
        /// <summary>
        /// 默认的IO数量
        /// </summary>
        public const int DEFAUT_IONUM = 2;

        /// <summary>
        /// 最大收发包字节数TCP
        /// </summary>
        public const int MAX_TCP_MESSAGE_LENGTH = 64000;


        /// <summary>
        /// 消息包长度字节数
        /// </summary>
        public const int MESSAGE_LENGTH_SIZE = 4;

        /// <summary>
        /// 默认的buffer长度字节数
        /// </summary>
        public const int DEFAUT_BUFFER_SIZE = MAX_TCP_MESSAGE_LENGTH + MESSAGE_LENGTH_SIZE;


        #region kcp

        /// <summary>
        /// 最大收发包字节数KCP
        /// </summary>
        public const int MAX_KCP_MESSAGE_LENGTH = DEF_KCP_MTU + DEF_KCP_OVERHEAD;

        public const int DEF_KCP_MTU = 1400;
        public const int DEF_KCP_OVERHEAD = 24;

        public const uint DEF_KCP_CONV = 0xAABBCCDD;
        public const int DEF_KCP_NODELAY = 1;
        public const int DEF_KCP_INTERVAL = 10;
        public const int DEF_KCP_RESEND = 2;
        public const int DEF_KCP_NC = 1;
        public const int DEF_KCP_SNDWND = 256;
        public const int DEF_KCP_RCVWND = 256;
        public const int DEF_KCP_TIMEOUT = 30;

        #endregion
    }
}
