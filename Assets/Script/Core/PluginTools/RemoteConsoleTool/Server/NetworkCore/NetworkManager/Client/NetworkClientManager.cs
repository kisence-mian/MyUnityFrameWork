using GameConsoleController;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace LiteNetLibManager
{
 public   class NetworkClientManager: NetworkManagerBase
    {
        public string NetworkAddress { get { return networkAddress; } }

        public bool IsConnected { get; private set; }

        private long connectionId = -1;
        public long ConnectionId { get { return connectionId; } private set { connectionId = value; } }

        public int Ping { get
            {
                if (transport != null && ConnectionId!=-1)
                    return transport.GetPing(ConnectionId);
                else
                    return -1;
            }
        }

        private NetConnectState m_connectState = NetConnectState.DisConnected;
        public NetConnectState ConnectState
        {
            get
            {
                return m_connectState;
            }
            private set
            {
                m_connectState = value;
            }
        }

        public DiscoveryPeersHandler discoveryPeersHandler = new DiscoveryPeersHandler();

        public NetworkClientManager(INetworkTransport transport, INetSerializer serializer) : base(transport)
        {
            msgManager = new MessageManager(serializer,false);
        }
        public  bool Connect()
        {
            return Connect(networkAddress, networkPort);
        }

        public  bool Connect(string networkAddress, int networkPort)
        {
            if(IsConnected)
            {
                Debug.LogError("client is connected!");
                return false;
            }
            if (ConnectState == NetConnectState.Connecting)
                return false;
            ConnectState = NetConnectState.Connecting;

            this.networkAddress = networkAddress;
            this.networkPort = networkPort;
            if (LogInfo) Debug.Log("Client connecting to " + networkAddress + ":" + networkPort);
           
          if( transport.Connect(networkAddress, networkPort))
            {
                //if (LogInfo) Debug.Log("[" + name + "] LiteNetLibManager::Client Connect");
                if (OnStartClient != null)
                    OnStartClient();
                return true;
            }
          
            return false;
        }
        public void Disconnect()
        {
            if (IsConnected)
            {
                Debug.Log("Client Disconnect");
                transport.Disconnect(ConnectionId);
            }
        }
        public override void Stop()
        {
            DisconnectInfo info = new DisconnectInfo();
            info.Reason = DisconnectReason.DisconnectPeerCalled;
            DisconnectedEvent(ConnectionId, info);
            base.Stop();

            if (LogInfo) Debug.Log("[" + name + "] ::OnStopClient");
            if (OnStopClient != null)
                OnStopClient();
        }
        public void Send<T>(T messageData,bool isLog =true) where T : new()
        {
            if (isLog)
                Debug.Log("Send Msg:" + SimpleJsonUtils.ToJson(messageData));

            if (IsConnected)
                SendData(ConnectionId, messageData);
            else
            {
                if (LogInfo)
                {
                    Debug.LogError("Client not Connected!");
                }
            }
        }

        protected override void DiscoverServer(IPEndPoint endPoint, string content)
        {
            RemoteDeviceInfo deviceInfo = null;
            deviceInfo = SimpleJsonUtils.FromJson<RemoteDeviceInfo>(content);
            if (deviceInfo == null)
            {
                deviceInfo = new RemoteDeviceInfo();
                deviceInfo.appName = content;
            }
            discoveryPeersHandler.Add(deviceInfo, endPoint);
            //Debug.Log("DiscoverServer:" + endPoint + " Centent:" + content);
        }
        protected override void DisconnectedEvent(long connectionId, DisconnectInfo disconnectInfo)
        {
            Debug.Log("Client DisconnectedEvent :" + connectionId+ " disconnectInfo:"+ disconnectInfo.Reason);
            IsConnected = false;
            ConnectState = NetConnectState.DisConnected;

            if (OnDisconnected != null)
                OnDisconnected(disconnectInfo);
        }
        protected override void PeerConnectedEvent(long connectionId)
        {
            Debug.Log("Client Peer Connected ! connectionId :" + connectionId);
            IsConnected = true;
            ConnectionId = connectionId;
            ConnectState = NetConnectState.Connected;
            if (OnClientConnected != null)
                OnClientConnected();
        }
        protected override void PeerNetworkErrorEvent(IPEndPoint endPoint, SocketError socketError)
        {
            if (OnPeerNetworkError != null)
                OnPeerNetworkError(endPoint, socketError);
        }
        public Action<IPEndPoint, SocketError> OnPeerNetworkError;
        /// <summary>
        /// This event will be called at client when connected to server
        /// </summary>

        public Action OnClientConnected;
  

        public Action<DisconnectInfo> OnDisconnected;
        /// <summary>
        /// This is a hook that is invoked when the client is started.
        /// </summary>
        /// <param name="client"></param>
        public Action OnStartClient;
        /// <summary>
        /// This hook is called when a client is stopped.
        /// </summary>
        public Action OnStopClient;

        protected override void OnUpdate(float deltaTime)
        {
            discoveryPeersHandler.Update(deltaTime);
        }

        
    }
}
