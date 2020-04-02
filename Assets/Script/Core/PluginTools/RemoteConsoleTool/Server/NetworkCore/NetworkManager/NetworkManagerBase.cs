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
    public enum LogLevel : byte
    {
        Developer = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5,
    }

    public enum NetConnectState
    {
        Connecting,
        Connected,
        DisConnected,
    }
   
    public abstract class NetworkManagerBase
    {
        public string name;

        protected string networkAddress = "localhost";
        protected int networkPort = 7770;
        public int NetworkPort { get { return networkPort; } }
        public bool LogDev { get { return currentLogLevel <= LogLevel.Developer; } }
        public bool LogDebug { get { return currentLogLevel <= LogLevel.Debug; } }
        public bool LogInfo { get { return currentLogLevel <= LogLevel.Info; } }
        public bool LogWarn { get { return currentLogLevel <= LogLevel.Warn; } }
        public bool LogError { get { return currentLogLevel <= LogLevel.Error; } }
        public bool LogFatal { get { return currentLogLevel <= LogLevel.Fatal; } }


        protected LogLevel currentLogLevel = LogLevel.Info;
        public LogLevel LogLevel
        {
            get
            {
                return currentLogLevel;
            }
            set
            {
                currentLogLevel = value;
            }
        }

        protected INetworkTransport transport;

        protected TransportEventData tempEventData;
        protected MessageManager msgManager;
        public MessageManager MsgManager {

            get
            {
                return msgManager;
            }
        }


        public NetworkManagerBase(INetworkTransport transport)
        {
            this.transport = transport;
           
        }

        private float sendDiscoveryRequestDelay = 0.6f;
        private float tempDiscoveryRequestTime = 0;
        public  void Update(float deltaTime)
        {
            if (transport == null)
                return;
            int count = 100;
            while (count>0 && transport.Receive(out tempEventData))
            {
                count--;
                try
                {
                    OnReceiveMessage(tempEventData);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
               
            }

           
            if (tempDiscoveryRequestTime <= 0)
            {
                tempDiscoveryRequestTime = sendDiscoveryRequestDelay;
                transport.SendDiscoveryRequest();
            }
            else
            {
                tempDiscoveryRequestTime -= deltaTime;
            }

            OnUpdate(deltaTime);
        }
        private void OnReceiveMessage(TransportEventData eventData)
        {
            switch (eventData.type)
            {
                case ENetworkEvent.ConnectEvent:
                    //if (LogInfo) Debug.Log("[" + name + "] ::OnPeerConnected peer.ConnectionId: " + eventData.connectionId);
                        PeerConnectedEvent(eventData.connectionId);
                    break;
                case ENetworkEvent.DataEvent:
                    msgManager.ReadPacket(eventData.connectionId, eventData.reader);
                    break;
                case ENetworkEvent.DisconnectEvent:
                    if (LogInfo) Debug.Log("[" + name + "] ::OnPeerDisconnected peer.ConnectionId: " + eventData.connectionId + " disconnectInfo.Reason: " + eventData.disconnectInfo.Reason);
                        DisconnectedEvent(eventData.connectionId, eventData.disconnectInfo);
                    break;
                case ENetworkEvent.ErrorEvent:
                    if (LogError) Debug.LogError("[" + name + "] ::OnNetworkError endPoint: " + eventData.endPoint + " socketErrorCode " + eventData.socketError);
                        PeerNetworkErrorEvent(eventData.endPoint, eventData.socketError);
                    break;
                case ENetworkEvent.DiscoveryEvent:
                    //if (LogInfo) Debug.Log("[" + name + "] LiteNetLibServer::DiscoveryEvent endPoint: " + eventData.endPoint);
                    DiscoverServer(eventData.endPoint, eventData.reader.GetString());
                    break;
            }
        }
        protected virtual void OnUpdate(float deltaTime) { }
        protected virtual void DiscoverServer(IPEndPoint endPoint, string centent) { }
        protected virtual void PeerConnectedEvent(long connectionId)
        {

        }
        protected virtual void DisconnectedEvent(long connectionId, DisconnectInfo disconnectInfo)
        {

        }
        protected virtual void PeerNetworkErrorEvent(IPEndPoint endPoint, SocketError socketError)
        {

        }
        public virtual void Stop()
        {
            if (transport != null)
            {
               
                transport.Destroy();
                transport = null;
            }
        }
        protected bool SendData<T>(long connectionId, T messageData) where T : new()
        {
            //if (LogInfo)
            //{
            //    Debug.Log("Send Msg =>::" +typeof(T).Name+"-->"+ JsonUtility.ToJson(messageData));
            //}
            if (transport == null|| msgManager==null)
                return false;
            byte[] datas = msgManager.SerializeMsg(messageData);
            return transport.Send(connectionId, datas);
        }

    }
}
