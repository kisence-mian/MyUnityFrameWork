using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;

namespace LiteNetLibManager
{
    public struct TransportEventData
    {
        public ENetworkEvent type;
        public long connectionId;
        public NetDataReader reader;
        public DisconnectInfo disconnectInfo;
        public IPEndPoint endPoint;
        public SocketError socketError;
    }
}
