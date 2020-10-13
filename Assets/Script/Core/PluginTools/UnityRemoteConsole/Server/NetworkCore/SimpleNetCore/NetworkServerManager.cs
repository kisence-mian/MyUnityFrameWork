using System;

namespace SimpleNetCore
{
    public  class NetworkServerManager : NetworkCommon
    {
        public NetworkServerManager(ServerConfiguration configuration) : base(configuration)
        {
        }

        #region Network Events Callbacks

        protected override void OnDisconnectedEvent(Session session, EDisconnectInfo disconnectInfo)
        {
            if (OnPeerDisconnected != null)
                OnPeerDisconnected(session, disconnectInfo);
        }
        protected override void PeerConnectedEvent(Session session)
        {

            if (OnPeerConnected != null)
                OnPeerConnected(session);
        }
        /// <summary>
        /// This event will be called at server when any client connected
        /// </summary>
        /// <param name="connectionId"></param>
        public Action<Session> OnPeerConnected;
        /// <summary>
        /// This event will be called at server when any client disconnected
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="disconnectInfo"></param>
        public Action<Session, EDisconnectInfo> OnPeerDisconnected;
        #endregion

        public Action OnStopServer;

        public void Start(int port)
        {
            NetStart(port);
        }

        protected override void OnStopEvent()
        {
             NetDebug.Log(" SimpleNetManager::OnStopServer");
            if (OnStopServer != null)
                OnStopServer();
        }

        #region Packets send / read
     

        public void Send<T>(Session session,T messageData) 
        {
            //NetDebug.Log("Server Send :" + typeof(T));
            SendData(session,null, messageData);
        }
        #endregion



    }
}
