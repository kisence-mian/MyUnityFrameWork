using System.Threading;

namespace SimpleNetCore
{
    public class NetHeartBeatPingPlugin : NetMsgProcessPackestPluginBase
    {
        public override byte GetNetProperty()
        {
            return (byte)NetProperty.HeartBeatServerSend;
        }
        private byte[] sendBytes = null;
        protected override void OnInit()
        {
            if (networkCommon.Configuration.UseMultithreading)
            {
                heardBeatThread = new Thread(HeardBeatThreadFun);
                heardBeatThread.Start();
                heardBeatThread.IsBackground = true;
            }

           sendBytes= MsgPackest.Write2Bytes(networkCommon.Configuration.byteOrder,
               0,0,0, (byte)NetProperty.HeartBeatClinetSend, new byte[0]);
        }

        public override void Release()
        {
            if (heardBeatThread != null)
            {
                heardBeatThread.Abort();
                heardBeatThread = null;
            }
        }
         private  bool isConnect = false;
        private Session session;
        public override void PeerConnectedEvent(Session session)
        {
            isConnect = true;
            this.session = session;

            ResetFlag();
        }

        public override void DisconnectedEvent(Session session, EDisconnectInfo disconnectInfo)
        {
            isConnect = false;
        }
        #region HeartBeat
        /// <summary>
        /// 心跳间隔时间 毫秒
        /// </summary>
        /// <param name="time"></param>
        public void SetheatBeatTime(int time)
        {
            if (time <= 0)
                return;
            heatBeatTime = time;
        }
        private Thread heardBeatThread = null;

        private int heatBeatTime = 7000;
        private const int ThreadUpdateTime = 500;
        private void HeardBeatThreadFun(object obj)
        {
            while (true)
            {
                ClientHeardBeatUpdate(ThreadUpdateTime);
                Thread.Sleep(ThreadUpdateTime);
            }
        }

        public override void Update(float deltaTime)
        {
           
            if (!networkCommon.Configuration.UseMultithreading)
            {
                int dt = (int)(deltaTime * 1000);
                ClientHeardBeatUpdate(dt);
            }
        }
        int tempTime = 1000;
        int lostCount = 0;
        private void ResetFlag()
        {
            msgQueue.Clear();
            lostCount = 0;
            tempTime = heatBeatTime;
        }
        private void ClientHeardBeatUpdate(int deltaTime)
        {

            if (isConnect)
            {
                if (msgQueue.Count > 0)
                {
                    //NetDebug.Log("重置心跳包：");
                    ResetFlag();
                    return;
                }

                if (tempTime <= 0)
                {
                    lostCount++;
                    tempTime = heatBeatTime+ lostCount*2;
                    if (lostCount > 4)
                    {
                        NetDebug.Log("ClientHeardBeatUpdate Disconnect! lostCount:" + lostCount);

                        networkCommon.Configuration.Transport.Disconnect(this.session.ConnectionId, EDisconnectReason.Timeout);

                        isConnect = false;
                        lostCount = 0;
                        return ;
                    }
                    else
                    {
                        //NetDebug.Log("发送心跳：lostCount："+ lostCount);
                        session.StatisticSendPackets((byte)NetProperty.HeartBeatClinetSend, sendBytes.Length);
                        networkCommon.Sendbytes(session, sendBytes);

                    }
                }
                else
                {
                    tempTime -= deltaTime;
                }
            }
           
        }
      

        #endregion
    }
    public class NetHeartBeatPongPlugin : NetMsgProcessPluginBase
    {
        private byte[] sendBytes=null;

        public override byte GetNetProperty()
        {
            return (byte)NetProperty.HeartBeatClinetSend;
        }

        protected override void OnInit()
        {
            sendBytes = MsgPackest.Write2Bytes(networkCommon.Configuration.byteOrder, 0,0,0, (byte)NetProperty.HeartBeatServerSend, new byte[0]);
        }
        public override void ReceveProcess( MsgPackest packest)
        {
           networkCommon.Sendbytes(packest.session,sendBytes);
        }
        public override void Release()
        {
            
        }
    }
}
