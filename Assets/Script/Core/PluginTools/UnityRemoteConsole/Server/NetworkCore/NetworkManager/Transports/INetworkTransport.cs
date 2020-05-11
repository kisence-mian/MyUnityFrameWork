using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;

namespace LiteNetLibManager
{
    public interface INetworkTransport
    {

        bool Start();
        int GetPeersCount();
        bool Connect(string address, int port);

        bool Receive(out TransportEventData eventData);
        void Destroy();
       
       
        bool Send(long connectionId, byte[] data);
        bool Disconnect(long connectionId);

        void SendDiscoveryRequest();

        int GetPing(long connectionId);
    }
}
