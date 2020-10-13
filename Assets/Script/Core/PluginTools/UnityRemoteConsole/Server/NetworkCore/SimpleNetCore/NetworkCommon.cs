using System;
using System.Collections.Generic;
using System.Threading;

namespace SimpleNetCore
{
    public enum NetConnectState
    {
        Connecting,
        Connected,
        DisConnected,
    }

    public abstract class NetworkCommon
    {
        internal string networkAddress = "localhost";
        internal int networkPort = 7770;
        public int NetworkPort { get { return networkPort; } }

        protected NetConfiguration configuration;
        public NetConfiguration Configuration
        {
            get
            {
                return configuration;
            }
        }
        private INetworkTransport transport;



        public IMessageHandler MsgManager
        {

            get
            {
                return configuration.GetMessageHander();
            }
        }

        protected INetworkTransport Transport { get
            {
                return configuration.Transport;
            }
        }

        private Dictionary<long, Session> sessions = new Dictionary<long, Session>();

      
        public NetworkCommon(NetConfiguration configuration)
        {
            if (configuration == null)
                throw new Exception("NetConfiguration is null ! ");

            this.configuration = configuration;

            this.configuration.Init(this);

            if (configuration.UseMultithreading)
            {
                ReceiveEventInit();
                SendEventInit();
            }
        }
        protected void NetStart(int port)
        {
            if (Transport != null)
            {
                Transport.Start(port);
            }
        }
        public  void Stop()
        {
            OnStopEvent();

            ReceiveEventStop();
            SendEventStop();
            configuration.Release();
        }
        protected virtual void OnStopEvent()
        {

        }
        public void Update(float deltaTime)
        {
            if (Transport == null)
                return;

            if (!configuration.UseMultithreading)
            {
                LoopDealwithReceiveData();
                Send2TransportProcess();

            }
            List<NetMsgProcessPluginBase> pluginList= Configuration.GetNetMsgProcessPlugins();
            foreach (var item in pluginList)
            {
                item.Update(deltaTime);
            }

            while (netEventDatas.Count > 0)
            {
                try
                {
                    OnHandleEvent();
                }
                catch (Exception e)
                {
                    NetDebug.LogError(e.ToString());
                }

            }
            try
            {
                OnHandleMsgPackests();
            }
            catch (Exception e)
            {
                NetDebug.LogError(e.ToString());
            }
            OnUpdate(deltaTime);
        }
        protected virtual void OnUpdate(float deltaTime) { }
        #region Receive Update

        private Thread receiveUpdateThread = null;
        private const int deltaTimeThread = 15;

        private void ReceiveEventInit()
        {
            receiveUpdateThread = new Thread(BackGroudReceiveUpdate);
            receiveUpdateThread.Start();
            receiveUpdateThread.IsBackground = true;
        }
        private void ReceiveEventStop()
        {
            if (receiveUpdateThread != null)
            {
                receiveUpdateThread.Abort();
                receiveUpdateThread = null;
            }
        }
        private void BackGroudReceiveUpdate(object obj)
        {
            while (true)
            {
                Thread.Sleep(deltaTimeThread);
                if (Transport == null)
                    break;

                LoopDealwithReceiveData();
                //NetDebug.Log("--------->receiveDatas:"+ receiveDatas.Count);
            }
        }
        private void LoopDealwithReceiveData()
        {
            if (Transport == null)
                return;

            TransportEventData eventData;
            while (Transport.Receive(out eventData))
            {
                Session session = null;

                sessions.TryGetValue(eventData.connectionId, out session);

                if (eventData.type == ENetworkEvent.DataEvent)
                {
                    if (session == null)
                    {
                        NetDebug.LogError("session is null, connectionId :" + session + " is not Connnected");
                        return;
                    }
                    MsgPackest packest;
                    try
                    {
                        packest = new MsgPackest(configuration.byteOrder, session, eventData.data);
                    }
                    catch (Exception e)
                    {
                        NetDebug.LogError("Parse MsgPackest Error :" + e);
                        continue;
                    }
                    if (eventData.data != null)
                        session.StatisticReceivePackets(packest.msgProperty, eventData.data.Length);
                    //NetDebug.Log("接收到消息 connectionId：" + packest.connectionId);
                    NetMsgProcessPluginBase plugin = configuration.GetPlugin(packest.msgProperty);
                    if (plugin == null)
                    {
                        NetDebug.LogError("No NetMsgProcessPluginBase By msgProperty:" + packest.msgProperty);
                    }
                    else
                    {
                        plugin.ReceveProcess(packest);
                    }
                }
                else
                {
                    if (eventData.type == ENetworkEvent.ConnectEvent)
                    {
                        session = new Session(eventData.connectionId);
                        session.OpenNetStatistics(Configuration.UseStatistics);
                        session.SetConnectTimeInStatistic();
                        sessions.Add(session.ConnectionId, session);

                        foreach (var plugin in configuration.GetNetMsgProcessPlugins())
                        {
                            plugin.PeerConnectedEvent(session);
                        }
                    }
                    else if (eventData.type == ENetworkEvent.DisconnectEvent)
                    {
                        if (session != null)
                            session.SetDisconnectTimeInStatistic();
                        sessions.Remove(eventData.connectionId);

                        foreach (var plugin in configuration.GetNetMsgProcessPlugins())
                        {
                            plugin.DisconnectedEvent(session, eventData.disconnectInfo);
                        }
                    }

                    eventData.session = session;
                    //NetDebug.Log("循环接收事件:" + eventData.type);
                    AddNetEvent(eventData);
                }

            }
        }
        #endregion


        #region 事件处理
        private Queue<TransportEventData> netEventDatas = new Queue<TransportEventData>();
        internal void AddNetEvent(TransportEventData eventData)
        {
            netEventDatas.Enqueue(eventData);
        }
   
        private void OnHandleEvent()
        {
            TransportEventData eventData = netEventDatas.Dequeue();
            IMessageHandler messageHander = configuration.GetMessageHander();
            Session session = eventData.session;
            //NetDebug.Log("循环接收事件1:" + eventData.type);
            switch (eventData.type)
            {
                case ENetworkEvent.ConnectEvent:
                    if (messageHander != null)
                    {
                        messageHander.OnConnectedEvent(session);
                    }
                    PeerConnectedEvent(session);
                    break;
               
                case ENetworkEvent.DisconnectEvent:
                    //NetDebug.Log(" ::OnPeerDisconnected peer.ConnectionId: " + eventData.connectionId + " disconnectInfo.Reason: " + eventData.disconnectInfo.Reason);
                    if (messageHander != null)
                    {
                        messageHander.OnDisconnectedEvent(session, eventData.disconnectInfo);
                    }
                    OnDisconnectedEvent(session, eventData.disconnectInfo);
                    break;
            }
        }
      

        protected virtual void PeerConnectedEvent(Session session)
        {

        }
        protected virtual void OnDisconnectedEvent(Session session, EDisconnectInfo disconnectInfo)
        {

        }
        #endregion
        #region 消息包处理
        private SafeQueue<MsgPackest> netMsgPackests = new SafeQueue<MsgPackest>();
        internal void ReceiveMsgPackest(MsgPackest packest)
        {
            //NetDebug.Log("收到处理消息包:"+packest.counter);
            netMsgPackests.Enqueue(packest);
        }

        private void OnHandleMsgPackests()
        {
            if (netMsgPackests.Count == 0)
                return;
            MsgPackest packest;
            while( netMsgPackests.TryDequeue(out packest))
            {
                Session session = packest.session;

                INetMsgSerializer serializer = configuration.GetMsgSerializer();
                object msgType = null;
                object msgData = null;
                try
                {
                    msgData = serializer.Deserialize(packest.contents, out msgType);
                }
                catch (Exception e)
                {
                    NetDebug.LogError("消息序列化失败：" + e);
                    return;
                }


                IMessageHandler messageHander = configuration.GetMessageHander();
                if (messageHander != null)
                {
                    try
                    {
                        messageHander.DispatchMessage(session, msgType, msgData);
                    }
                    catch (Exception e)
                    {
                        NetDebug.LogError("DispatchMessage error:" + e);
                    }

                }
            }
        }
        #endregion

        #region Send Data 发送消息处理

        public struct SendMsgTempData
        {
            public Session session;
            public byte[] datas;

            public SendMsgTempData(Session session, byte[] datas)
            {
                this.session = session;
                this.datas = datas;
            }
        }
        private Thread sendUpdateThread = null;
        private void SendEventInit()
        {
            sendUpdateThread = new Thread(SendDataBackgroudUpdate);
            sendUpdateThread.Start();
            sendUpdateThread.IsBackground = true;
        }
        private void SendEventStop()
        {
            if (sendUpdateThread != null)
            {
                sendUpdateThread.Abort();
                sendUpdateThread = null;
            }
        }
        private void SendDataBackgroudUpdate(object obj)
        {
            while (true)
            {
               
                Send2TransportProcess();
                
                Thread.Sleep(20);
            }
        }
        private SafeQueue<SendMsgTempData> sendMsgLoop = new SafeQueue<SendMsgTempData>();
        protected bool SendData<T>(Session session, object msgType, T messageData) 
        {
            //if (LogInfo)
            //{
            //NetDebug.Log("Send Msg =>::" + typeof(T).Name + "-->" + JsonUtility.ToJson(messageData));
            //}
            if (Transport == null)
                return false;

            INetMsgSerializer serializer = configuration.GetMsgSerializer();
            if(serializer==null)
            {
                NetDebug.LogError("No INetMsgSerializer!");
                return false;
            }
            if (msgType == null)
            {
                msgType = serializer.GetMsgType(messageData);
            }
            byte[] datas = null;
            try
            {
                datas = serializer.Serialize(msgType, messageData);
            }
            catch (Exception e)
            {
                NetDebug.LogError("INetMsgSerializer error:" + e);
                return false;
            }

            sendMsgLoop.Enqueue(new SendMsgTempData(session, datas));
            return true;
        }
        private void Send2TransportProcess()
        {
            SendMsgTempData tempData;
            while (sendMsgLoop.TryDequeue(out tempData))
            {
                try
                {
                    Session session = tempData.session;
                    if (session == null)
                        continue;
                    byte[] datas = tempData.datas;
                    if (datas == null)
                        datas = new byte[0];

                    byte msgProperty =(byte)NetProperty.Data;
                    //必须能msgProperty发送接收一样的才行
                    NetMsgProcessPluginBase plugin = configuration.GetPlugin(msgProperty);
                    if (plugin == null)
                    {
                        NetDebug.LogError("No NetMsgProcessPluginBase:" + msgProperty);
                        continue;
                    }
                    byte[] res = plugin.SendProcess( session, msgProperty , datas);
                    if (res == null)
                    {
                        NetDebug.LogError("No Implement NetMsgProcessPluginBase.SendProcess:" + msgProperty);
                        continue;
                    }
                    //NetDebug.Log("Send byte:" + res.Length );
                    session.StatisticSendPackets(msgProperty, res.Length);
                    Sendbytes(session, res);
                }
                catch (Exception e)
                {
                    NetDebug.LogError("Send error:" + e);
                }
              
            }
        }

        internal bool Sendbytes(Session session, byte[] bytes)
        {
            if (session == null)
                return false;
            return Transport.Send(session.ConnectionId, bytes);
        }
        #endregion

    }
}
