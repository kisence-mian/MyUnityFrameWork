namespace SimpleNetCore
{
    public struct TransportEventData
    {
        public ENetworkEvent type;
        public long connectionId;
        public byte[] data;
        public EDisconnectInfo disconnectInfo;

        /// <summary>
        /// 临时变量
        /// </summary>
        internal Session session;
    }

    public enum NetProperty:byte
    {
        Data=0,
        /// <summary>
        /// 心跳包客户端发送
        /// </summary>
        HeartBeatClinetSend=1,
        /// <summary>
        /// 心跳包服务端返回
        /// </summary>
        HeartBeatServerSend = 2,
        /// <summary>
        /// ping发送
        /// </summary>
        Ping=3,
        /// <summary>
        /// 服务端ping返回
        /// </summary>
        Pong=4,

    }
}
