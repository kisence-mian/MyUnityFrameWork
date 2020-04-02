using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace LiteNetLibManager
{
    public class LiteNetLibTransportEventListener : INetEventListener
    {
        private LiteNetLibTransport transport;
        private Queue<TransportEventData> eventQueue;
        private Dictionary<long, NetPeer> peersDict;
        private bool isServer;


        public LiteNetLibTransportEventListener(bool isServer, LiteNetLibTransport transport, Queue<TransportEventData> eventQueue, Dictionary<long, NetPeer> peersDict) 
        {
            this.isServer = isServer;
            this.peersDict = peersDict;
            this.transport = transport;
            this.eventQueue = eventQueue;
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            //Debug.Log("OnConnectionRequest:" + request.RemoteEndPoint);
            if (transport.netManager.ConnectedPeersCount < transport.maxConnections)
                request.AcceptIfKey(transport.connectKey);
            else
                request.Reject();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            //Debug.Log("OnNetworkError:" + endPoint+ " socketError:"+ socketError);
            eventQueue.Enqueue(new TransportEventData()
            {
                type = ENetworkEvent.ErrorEvent,
                endPoint = endPoint,
                socketError = socketError,
            });
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            eventQueue.Enqueue(new TransportEventData()
            {
                type = ENetworkEvent.DataEvent,
                connectionId = peer.Id,
                reader = reader,
            });
        }
        private NetDataWriter _serverWriter = new NetDataWriter();
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (isServer)
            {
                if (messageType == UnconnectedMessageType.Broadcast)
                {
                    // 服务端接收到广播寻找请求
                    if (reader.GetUShort() == transport.broadcastKey)
                    {

                        //Debug.Log("Server 接收到Broadcast");
                        _serverWriter.Reset();
                        _serverWriter.Put(transport.broadcastKey);
                        _serverWriter.Put(transport.broadcastdata);
                        transport.netManager.SendUnconnectedMessage(_serverWriter, remoteEndPoint);

                    }

                }
            }
            else
            {
                if (reader.GetUShort() == transport.broadcastKey)
                {
                    //Debug.Log("Client 接收到Broadcast返回");
                    //客户端接收到来自服务端的寻找请求回复
                    eventQueue.Enqueue(new TransportEventData()
                    {
                        type = ENetworkEvent.DiscoveryEvent,
                        reader = reader,
                        endPoint = remoteEndPoint

                    });
                }
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            //Debug.Log("OnPeerConnected:" + peer.EndPoint);

            if (peersDict != null)
                peersDict[peer.Id] = peer;

            eventQueue.Enqueue(new TransportEventData()
            {
                type = ENetworkEvent.ConnectEvent,
                connectionId = peer.Id,
            });
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("OnPeerDisconnected:" + peer.EndPoint);
            if (peersDict != null)
                peersDict.Remove(peer.Id);

            eventQueue.Enqueue(new TransportEventData()
            {
                type = ENetworkEvent.DisconnectEvent,
                connectionId = peer.Id,
                disconnectInfo = disconnectInfo,
            });
        }
    }
}
