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
  public  class NetworkServerManager : NetworkManagerBase
    {

        protected readonly HashSet<long> ConnectionIds = new HashSet<long>();

        public NetworkServerManager(INetworkTransport transport, INetSerializer serializer) : base(transport)
        {
            msgManager = new MessageManager(serializer,true);
        }

        #region Network Events Callbacks

        protected override void DisconnectedEvent(long connectionId, DisconnectInfo disconnectInfo)
        {
            RemoveConnectionId(connectionId);
            if (OnPeerDisconnected != null)
                OnPeerDisconnected(connectionId, disconnectInfo);
        }
        protected override void PeerConnectedEvent(long connectionId)
        {
            AddConnectionId(connectionId);
            if (OnPeerConnected != null)
                OnPeerConnected(connectionId);
        }
        protected override void PeerNetworkErrorEvent(IPEndPoint endPoint, SocketError socketError)
        {
            if (OnPeerNetworkError != null)
                OnPeerNetworkError(endPoint, socketError);
        }
        /// <summary>
        /// This event will be called at server when any client connected
        /// </summary>
        /// <param name="connectionId"></param>
        public Action<long> OnPeerConnected;
        /// <summary>
        /// This event will be called at server when any client disconnected
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="disconnectInfo"></param>
        public Action<long, DisconnectInfo> OnPeerDisconnected;
           /// <summary>
        /// This event will be called at server when there are any network error
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="socketError"></param>
        public Action<IPEndPoint, SocketError> OnPeerNetworkError;
        #endregion

        #region Start / Stop Callbacks
        public Action OnStopServer;

        #endregion

        public override void Stop()
        {
            base.Stop();
            if (LogInfo) Debug.Log("[" + name + "] LiteNetLibManager::OnStopServer");
            if (OnStopServer != null)
                OnStopServer();
        }

        public void AddConnectionId(long connectionId)
        {
            ConnectionIds.Add(connectionId);
        }

        public bool RemoveConnectionId(long connectionId)
        {
            return ConnectionIds.Remove(connectionId);
        }

        public bool ContainsConnectionId(long connectionId)
        {
            return ConnectionIds.Contains(connectionId);
        }

        public IEnumerable<long> GetConnectionIds()
        {
            return ConnectionIds;
        }

        #region Packets send / read
     

        public void Send<T>(long connectionId,T messageData) where T : new()
        {
            //Debug.Log("Server Send :" + typeof(T));
            SendData(connectionId, messageData);
        }
        public void Send<T>(Player player, T messageData) where T : new()
        {
            SendData(player.connectionId, messageData);
        }
        #endregion



    }
}
