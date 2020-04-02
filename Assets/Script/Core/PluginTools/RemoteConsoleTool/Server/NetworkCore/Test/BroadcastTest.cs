using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

public class BroadcastTest : MonoBehaviour
{
    private class ClientListener : INetEventListener
    {
        public NetManager Client;

        public void OnPeerConnected(NetPeer peer)
        {
             Debug.LogFormat("[Client {0}] connected to: {1}:{2}", Client.LocalPort, peer.EndPoint.Address, peer.EndPoint.Port);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
           
             Debug.Log("[Client] disconnected: " + disconnectInfo.Reason);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError error)
        {
             Debug.Log("[Client] error! " + error);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            var text = reader.GetString(100);
             Debug.LogFormat("[Client] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, text);
            if (messageType == UnconnectedMessageType.BasicMessage && text == "SERVER DISCOVERY RESPONSE")
            {
                Client.Connect(remoteEndPoint, "key");
            }
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }
    }

    private class ServerListener : INetEventListener
    {
        public NetManager Server;

        public void OnPeerConnected(NetPeer peer)
        {
             Debug.Log("[Server] Peer connected: " + peer.EndPoint);
            var peers = Server.ConnectedPeerList;
            foreach (var netPeer in peers)
            {
                 Debug.LogFormat("ConnectedPeersList: id={0}, ep={1}", netPeer.Id, netPeer.EndPoint);
            }
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
             Debug.Log("[Server] Peer disconnected: " + peer.EndPoint + ", reason: " + disconnectInfo.Reason);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
             Debug.Log("[Server] error: " + socketErrorCode);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {

        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
             Debug.LogFormat("[Server] ReceiveUnconnected {0}. From: {1}. Data: {2}", messageType, remoteEndPoint, reader.GetString(100));
            NetDataWriter wrtier = new NetDataWriter();
            wrtier.Put("SERVER DISCOVERY RESPONSE");
            Server.SendUnconnectedMessage(wrtier, remoteEndPoint);
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("key");
        }
    }

    private ClientListener _clientListener1;
    private ClientListener _clientListener2;
    private ServerListener _serverListener;

    private void Start()
    {
        StartCoroutine(Run());
    }
    public IEnumerator Run()
    {
         Debug.Log("=== Broadcast Test ===");
        //Server
        _serverListener = new ServerListener();

        NetManager server = new NetManager(_serverListener);
        server.BroadcastReceiveEnabled = true;
        if (!server.Start(9050))
        {
             Debug.Log("Server start failed");
            //Console.ReadKey();
            yield return 0;
        }
        _serverListener.Server = server;

        //Client
        _clientListener1 = new ClientListener();

        NetManager client1 = new NetManager(_clientListener1)
        {
            UnconnectedMessagesEnabled = true,
            SimulateLatency = true,
            SimulationMaxLatency = 1500
        };
        _clientListener1.Client = client1;
        if (!client1.Start())
        {
             Debug.Log("Client1 start failed");

            yield return 0;
        }

        _clientListener2 = new ClientListener();
        NetManager client2 = new NetManager(_clientListener2)
        {
            UnconnectedMessagesEnabled = true,
            SimulateLatency = true,
            SimulationMaxLatency = 1500
        };

        _clientListener2.Client = client2;
        client2.Start();

        //Send broadcast
        NetDataWriter writer = new NetDataWriter();

        writer.Put("CLIENT 1 DISCOVERY REQUEST");
        client1.SendBroadcast(writer, 9050);
        writer.Reset();

        writer.Put("CLIENT 2 DISCOVERY REQUEST");
        client2.SendBroadcast(writer, 9050);

        int index = 0;
        while (index<30)
        {
            client1.PollEvents();
            client2.PollEvents();
            server.PollEvents();
            index++;
            yield return  new WaitForSeconds(0.5f);
        }

        client1.Stop();
        client2.Stop();
        server.Stop();
       // Console.ReadKey();
         Debug.Log("Press any key to exit");
       // Console.ReadKey();
    }
}
