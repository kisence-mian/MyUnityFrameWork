using LiteNetLib;
using SimpleNetCore;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleNetManager
{
    public class LiteNetLibTransport : INetworkTransport
    {
        public NetManager netManager { get; private set; }
        public string connectKey { get; private set; }
        //public int maxConnections { get; private set; }
        private readonly Dictionary<long, NetPeer> connectPeers = new Dictionary<long, NetPeer>();

        private readonly Queue<TransportEventData> qEventQueue = new Queue<TransportEventData>();

        public ushort broadcastKey { get; private set; }
        public string broadcastdata { get; private set; }
        public bool discoveryServer { get; private set; }
        public int discoveryPort { get; private set; }

        //private int port = 0;

        public  LiteNetLibTransport(bool isServer) :base(isServer)
        {
            netManager = new NetManager(new LiteNetLibTransportEventListener(isServer, this, qEventQueue, connectPeers));

            SetConnectKey("GameKey");
            broadcastKey = 11111;
            //this.maxConnections = maxConnections;
            netManager.UnconnectedMessagesEnabled = true;
            netManager.BroadcastReceiveEnabled = true;
            netManager.NatPunchEnabled = true;
#if UNITY_2018_3_OR_NEWER
            netManager.IPv6Enabled = true;
#else
            netManager.IPv6Enabled = false;
#endif
 
        }

        public override bool Start(int port)
        {
            if (netManager.Start(port))
            {
                Debug.Log("NetManager start!");
                return true;
            }
            return false;
        }
        public LiteNetLibTransport SetConnectKey(string connectKey)
        {
            this.connectKey = connectKey;
            return this;
        }
        public LiteNetLibTransport SetBroadcastData(string broadcastdata)
        {
            this.broadcastdata = broadcastdata;
            return this;
        }
        public LiteNetLibTransport SetDiscoveryServer(bool discoveryServer, int discoveryPort)
        {
            this.discoveryServer = discoveryServer;
            this.discoveryPort = discoveryPort;
            return this;
        }
        public override bool Connect(string address, int port)
        {
            return netManager.Connect(address, port, connectKey) != null;
        }

        public override void Destroy()
        {
            connectPeers.Clear();
            qEventQueue.Clear();
            if (netManager != null)
                netManager.Stop();
        }

        public override bool Disconnect(long connectionId, EDisconnectReason disconnectReason)
        {
            if (connectPeers.ContainsKey(connectionId))
            {
                netManager.DisconnectPeer(connectPeers[connectionId]);
                return true;
            }
            return false;
        }
        public override bool Receive(out TransportEventData eventData)
        {
            eventData = default(TransportEventData);
            if (netManager == null)
                return false;

            netManager.PollEvents();
            if (qEventQueue.Count == 0)
                return false;
            eventData = qEventQueue.Dequeue();



            return true;
        }

        public override bool Send(long connectionId, byte[] data)
        {
            if (connectPeers.ContainsKey(connectionId))
            {
                connectPeers[connectionId].Send(data, DeliveryMethod.ReliableOrdered);
                return true;
            }
            return false;
        }

    }
}
