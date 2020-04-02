using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ServerStartController :MonoBehaviour
{
    NetManager server=null;
    public void Awake()
    {
        Application.logMessageReceived += logMessageReceived;
       // Application.quitting += OnQuit;

        ServerListener serverListener = new ServerListener();
         server = new NetManager(serverListener);
        server.BroadcastReceiveEnabled = true;
        serverListener.Server = server;
     
        Debug.Log("port:" + server.LocalPort);
    }

    void OnApplicationQuit()
    {
        
        server.Stop();
    }

    string logs = "";
    private void logMessageReceived(string condition, string stackTrace, LogType type)
    {
        logs += condition+"\n";
    }

    public void Update()
    {
        server.PollEvents();
        
    }
    string port = "6666";
    void OnGUI()
    {
        port = GUILayout.TextField(port);
        if (GUILayout.Button("Start"))
        {
            if (!server.Start(int.Parse( port)))
            {
                Debug.Log("启动失败！");
                return;
            }
            Debug.Log("启动服务器");
        }
        if (GUILayout.Button("Stop"))
        {
            server.Stop();
        }
        GUILayout.Label(logs);
    }
    private class ServerListener : INetEventListener
    {
        public NetManager Server;

        public void OnPeerConnected(NetPeer peer)
        {
           
            Debug.Log("客户端连接 id:" + peer.Id + " ip=>" + peer.EndPoint);
            var peers = Server.ConnectedPeerList;
            foreach (var netPeer in peers)
            {
                Debug.Log("已连接的 客户端连接 id:" + netPeer.Id + " ip=>" + netPeer.EndPoint);
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
            //echo
            peer.Send(reader.GetRemainingBytes(), deliveryMethod);

            ////fragment log
            //if (reader.AvailableBytes == 13218)
            //{
            //    Debug.Log("[Server] TestFrag: {0}, {1}",
            //        reader.RawData[reader.UserDataOffset],
            //        reader.RawData[reader.UserDataOffset + 13217]);
            //}
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Debug.Log("[Server] ReceiveUnconnected: {0}"+ reader.RawDataSize);
            NetDataWriter wrtier = new NetDataWriter();
            wrtier.Put("SERVER DISCOVERY RESPONSE :)");
            Server.SendUnconnectedMessage(wrtier, remoteEndPoint);
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var acceptedPeer = request.AcceptIfKey("gamekey");
            Debug.Log("[Server] ConnectionRequest. Ep: {0}, Accepted: {1}"+
                request.RemoteEndPoint+" "+(
                acceptedPeer != null));
        }
    }
}

