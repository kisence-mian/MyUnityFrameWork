using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LiteNetLibManager
{
    public class LiteNetLibTransport : INetworkTransport
    {
        public NetManager netManager { get; private set; }
        public string connectKey { get; private set; }
        public int maxConnections { get; private set; }
        private readonly Dictionary<long, NetPeer> connectPeers=new Dictionary<long, NetPeer>();

        private readonly Queue<TransportEventData> qEventQueue=new Queue<TransportEventData>();

        public ushort broadcastKey { get; private set; }
        public string broadcastdata { get; private set; }
        public bool discoveryServer { get; private set; }
        public int discoveryPort { get; private set; }

        public LiteNetLibTransport()
        {
           
            netManager = new NetManager(new LiteNetLibTransportEventListener(false, this, qEventQueue, connectPeers));

            netManager.UnconnectedMessagesEnabled = true;
            SetConnectKey("GameKey");
            broadcastKey = 11111;

            List<string> ipv4List = NetUtils.GetLocalIpList(LocalAddrType.IPv4);
            List<string> ipv6List = NetUtils.GetLocalIpList(LocalAddrType.IPv6);


            for (int i = 0; i < ipv4List.Count; i++)
            {
                try
                {
                    string ipv4 = ipv4List[i];
                    string ipv6 = ipv6List[i];
                    netManager.Start(ipv4, ipv6, 0);
                    Debug.Log("NetManager Client start! :" + ipv4 + " => " + ipv6);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
              
            }
            //if (netManager.Start())
            //{
            //    Debug.Log("NetManager start!");
            //}
        }
        public LiteNetLibTransport(int port , int maxConnections = 16)
        {

            netManager = new NetManager(new LiteNetLibTransportEventListener(true, this, qEventQueue, connectPeers));
            this.maxConnections = maxConnections;
            netManager.UnconnectedMessagesEnabled = true;
            netManager.BroadcastReceiveEnabled = true;
            netManager.NatPunchEnabled = true;
#if UNITY_2018_3_OR_NEWER
            netManager.IPv6Enabled = true;
#else
            netManager.IPv6Enabled = false;
#endif
            if (port < 0)
                port = 0;

            SetConnectKey("GameKey");
            broadcastKey = 11111;

            //List<string> ipv4List = NetUtils.GetLocalIpList(LocalAddrType.IPv4);
            //List<string> ipv6List = NetUtils.GetLocalIpList(LocalAddrType.IPv6);


            //for (int i = 0; i < ipv4List.Count; i++)
            //{
            //    try
            //    {
            //        string ipv4 = ipv4List[i];
            //        string ipv6 = ipv6List[i];
            //        netManager.Start(ipv4, ipv6, port);
            //        Debug.Log("NetManager Server start! :" + ipv4 + " => " + ipv6);
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.LogError(e);
            //    }

            //}
            if (netManager.Start(port))
            {
                Debug.Log("NetManager start!");
            }
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
        public LiteNetLibTransport SetDiscoveryServer(bool discoveryServer,int discoveryPort)
        {
            this.discoveryServer = discoveryServer;
            this.discoveryPort = discoveryPort;
            return this;
        }
        public bool Connect(string address, int port)
        {
            return netManager.Connect(address, port, connectKey) != null;
        }

        public void Destroy()
        {
            connectPeers.Clear();
            qEventQueue.Clear();
            if (netManager != null)
                netManager.Stop();
        }

        public bool Disconnect(long connectionId)
        {
            if (connectPeers.ContainsKey(connectionId))
            {
                netManager.DisconnectPeer(connectPeers[connectionId]);
                return true;
            }
            return false;
        }

        public int GetPeersCount()
        {
            if (netManager != null)
                return netManager.ConnectedPeersCount;
            return 0;
        }

        public bool Receive(out TransportEventData eventData)
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
        private NetDataWriter disciverWriter = new NetDataWriter();
        /// <summary>
        /// 发送消息以发现局域网内的服务器
        /// </summary>
        public void SendDiscoveryRequest()
        {
            if (discoveryServer)
            {
                disciverWriter.Reset();
                disciverWriter.Put(broadcastKey);
                disciverWriter.Put(broadcastdata);
                netManager.SendBroadcast(disciverWriter, discoveryPort);
            }
        }

        public bool Send(long connectionId, byte[] data)
        {
            if (connectPeers.ContainsKey(connectionId))
            {
                connectPeers[connectionId].Send(data, DeliveryMethod.ReliableOrdered);
                return true;
            }
            return false;
        }

        public int GetPing(long connectionId)
        {
            if (connectPeers.ContainsKey(connectionId))
            {
               return  connectPeers[connectionId].Ping;
            }

            return -1;
        }
    }
}
