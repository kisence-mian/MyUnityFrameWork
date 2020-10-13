using System;

namespace SimpleNetCore
{
    public class NetworkClientManager : NetworkCommon
    {
        /// <summary>
        /// This hook is called when a client is stopped.
        /// </summary>
        public Action OnStopClient;

        public string NetworkAddress { get { return networkAddress; } }

        public bool IsConnected { get; private set; }

        private Session session;
        public Session Session { get { return session; } private set { session = value; } }

        public int Ping
        {
            get
            {
                if(pingPlugin!=null)
                return pingPlugin.Ping;

                return -1;
            }
        }
        /// <summary>
        /// 开关ping
        /// </summary>
        public bool EnablePing
        {
            get
            {
                if (pingPlugin != null)
                {
                    return pingPlugin.Enable;
                }
                return false;
            }
            set
            {
                if (pingPlugin != null)
                {
                    pingPlugin.Enable = value;
                }
            }
        }
        private ClientNetStatistics clientNetStatistics = new ClientNetStatistics();
        public ClientNetStatistics Statistics
        {
            get
            {
                return clientNetStatistics;
            }
        }
        /// <summary>
        /// 连接状态变化
        /// </summary>
        public Action<NetConnectState, EDisconnectInfo> OnNetConnectStateChange;
        private NetConnectState m_connectState = NetConnectState.DisConnected;
        public NetConnectState ConnectState
        {
            get
            {
                return m_connectState;
            }
        }
        private void SetNetConnectState(NetConnectState m_connectState)
        {
            SetNetConnectState(m_connectState, default(EDisconnectInfo));
        }
            private void SetNetConnectState(NetConnectState m_connectState, EDisconnectInfo info)
        {
            this.m_connectState = m_connectState;
            if (OnNetConnectStateChange != null)
            {
                OnNetConnectStateChange(m_connectState,info);
            }
        }
        public NetworkClientManager(ClientConfiguration configuration) : base(configuration)
        {
        }
        private NetPingPlugin pingPlugin;
        private AutoReconnectPlugin autoReconnectPlugin;
        public void Start()
        {
            NetStart(0);

            NetMsgProcessPluginBase plugin = Configuration.GetPlugin((byte)NetProperty.Pong);

             autoReconnectPlugin = ((ClientConfiguration)Configuration).GetReconnectPlugin();

            if (plugin != null)
            {
                pingPlugin = (NetPingPlugin)plugin;
            }
        }
        public bool Connect()
        {
            return Connect(networkAddress, networkPort);
        }

        public bool Connect(string networkAddress, int networkPort)
        {
            clientNetStatistics = new ClientNetStatistics();
            if (IsConnected)
            {
                NetDebug.LogError("client is connected!");
                return false;
            }
            if (ConnectState == NetConnectState.Connecting)
                return false;
            SetNetConnectState( NetConnectState.Connecting);

            this.networkAddress = networkAddress;
            this.networkPort = networkPort;
            NetDebug.Log("Client connecting to " + networkAddress + ":" + networkPort);

            if (Transport.Connect(networkAddress, networkPort))
            {
                return true;
            }
            SetNetConnectState(NetConnectState.DisConnected);
            return false;
        }
        public void Disconnect()
        {
            if (IsConnected)
            {
                NetDebug.Log("Client Disconnect");
                Transport.Disconnect(session.ConnectionId, EDisconnectReason.DisconnectPeerCalled);
            }
            else
            {
                if(autoReconnectPlugin != null)
                {
                    autoReconnectPlugin.ForceStopReconnect();
                }
            }

        }
        protected override void OnStopEvent()
        {
            Disconnect();
            //NetDebug.Log(" Client OnStopClient");
            if (OnStopClient != null)
                OnStopClient();
        }
        public bool Send<T>(T messageData) 
        {
            //if (isLog)
                //NetDebug.Log("Send Msg:" + SimpleJsonUtils.ToJson(messageData));

            if (IsConnected)
               return  SendData(session, null, messageData);
            else
            {

                NetDebug.LogError("Client not Connected!");
                return false;
            }
        }
        protected override void OnUpdate(float deltaTime)
        {
           
            if (autoReconnectPlugin != null)
            {
                if (autoReconnectPlugin.IsReconnecting)
                {
                    if (ConnectState == NetConnectState.Connecting)
                    {
                        return;
                    }
                    SetNetConnectState(NetConnectState.Connecting);
                }
            }
        }

        protected override void OnDisconnectedEvent(Session session, EDisconnectInfo disconnectInfo)
        {
            if (Configuration.UseStatistics&& session!=null)
            {
                clientNetStatistics.MarkDisconnect();
                clientNetStatistics.details.Add(session.statistics);
            }
            NetDebug.Log("Client DisconnectedEvent :" + session + " disconnectInfo:" + disconnectInfo.Reason);
            IsConnected = false;
            SetNetConnectState(NetConnectState.DisConnected,disconnectInfo);
        }
        protected override void PeerConnectedEvent(Session session)
        {
            NetDebug.Log("Client Peer Connected ! connectionId :" + session);
            if (Configuration.UseStatistics)
            {
                clientNetStatistics.MarkConnected();
                clientNetStatistics.details.Add(session.statistics);
            }
            IsConnected = true;
            this.session = session;
            SetNetConnectState(NetConnectState.Connected);
        }
    }
}
