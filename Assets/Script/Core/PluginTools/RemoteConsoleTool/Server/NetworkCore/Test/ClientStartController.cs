using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

class ClientStartController : MonoBehaviour
{
    NetManager client = null;
    ClientListener clientListener = null;
    public void Start()
    {
        Debug.Log("客户端");
         clientListener = new ClientListener();
        client = new NetManager(clientListener);

        if (!client.Start())
        {
            Debug.Log("Client1 start failed");

            return;
        }
        NetDataWriter writer = new NetDataWriter();
        writer.Put(0);
        client.SendBroadcast(writer, 6666);
    }
    string port = "6666";
    public void Update()
    {
        client.PollEvents();
        if (clientListener != null && clientListener.peer == null)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(0);
            client.SendBroadcast(writer, 6666);
        }
           
    }
    public void OnGUI()
    {
        if(clientListener!=null&&clientListener.peer!=null)
        GUILayout.Label("Ping:"+ clientListener.peer.Ping);

        port = GUILayout.TextField(port);
        if (GUILayout.Button("Start"))
        {
            client.Connect("localhost", int.Parse(port), "gamekey");
        }
        if (GUILayout.Button("Start"))
        {
            client.Connect(clientListener.remoteEndPoint, "gamekey");
        }

    }
    private class ClientListener : INetEventListener
    {
        public NetPeer peer;
        public IPEndPoint remoteEndPoint;
        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[Client] connected to: {0}:{1}"+ peer.EndPoint);
            this.peer = peer;
            
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[Client] disconnected: " + disconnectInfo.Reason);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Debug.Log("[Client] error! " + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Debug.Log("客户端OnNetworkReceiveUnconnected=》" + messageType);
            if (messageType == UnconnectedMessageType.Broadcast)
            {
                Debug.Log("客户端发现有服务端=》" + remoteEndPoint);
                this.remoteEndPoint = remoteEndPoint;
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {

        }
    }
}

