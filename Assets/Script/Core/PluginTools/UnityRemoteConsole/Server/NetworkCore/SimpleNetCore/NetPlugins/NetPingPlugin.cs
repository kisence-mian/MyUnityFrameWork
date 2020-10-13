using System.Threading;
using System;

namespace SimpleNetCore
{
    /// <summary>
    /// Ping功能(客户端)
    /// </summary>
    public class NetPingPlugin : NetMsgProcessPackestPluginBase
    {
        internal int Ping
        {
            get
            {
                return ping;
            }
        }
        #region 开关Ping
        /// <summary>
        /// 开关
        /// </summary>
        internal bool Enable
        {
            get
            {
                return enable;
            }
            set
            {
                enable = value;
                if (isInit && enable)
                {
                    EnableFunc();
                }
                else
                {
                    DisableFunc();
                }
                //NetDebug.Log("Enable Ping:" + enable);
            }
        }
        private bool enable = true;
        private void EnableFunc()
        {
            if (pingThread == null)
            {
                pingThread = new Thread(ClientPingUpdate);
                pingThread.Start();
                pingThread.IsBackground = true;
            }
        }
        private void DisableFunc()
        {
            if (pingThread != null)
            {
                pingThread.Abort();
                pingThread = null;
            }
        }

        #endregion

        private EndianBitConverter bitConverter;

        public override byte GetNetProperty()
        {
            return (byte)NetProperty.Pong;
        }
        private bool isInit = false;
        protected override void OnInit()
        {
            bitConverter = EndianBitConverter.GetBitConverter(networkCommon.Configuration.byteOrder);
            isInit = true;
            //Debug.Log("NetPingPlugin Init!!");
            if (enable)
            {
                EnableFunc();
            }
        }

    
        public override void Release()
        {
            isInit = false;
            DisableFunc();
        }
        private bool isConnect = false;
        private Session session;
        public override void PeerConnectedEvent(Session session)
        {
            isConnect = true;
            this.session = session;
        }

        public override void DisconnectedEvent(Session session, EDisconnectInfo disconnectInfo)
        {
            isConnect = false;
        }
        /// <summary>
        /// ping发送间隔时间 毫秒
        /// </summary>
        /// <param name="time"></param>
        public void SetPingDelayTime(int time)
        {
            if (time <= 0.1f)
                return;
            pingDelayTime = time;
        }
        private Thread pingThread = null;

        private int pingDelayTime = 1000;
        private int ping=-1;

        private byte property = (byte)NetProperty.Ping;
        private void ClientPingUpdate(object obj)
        {
            int tempTime = pingDelayTime;
            while (true)
            {
                if (enable && isConnect)
                {
                    if (msgQueue.Count > 0)
                    {
                        MsgPackest eventData = msgQueue.Dequeue();
                        long lastTime = bitConverter.ToInt64(eventData.contents, 0);
                        //NetDebug.Log("接收到时间:" + lastTime + " eventData.contents"+eventData.contents.Length);
                        ping = (int)((DateTime.Now.Ticks - lastTime) / 20000);
                        msgQueue.Clear();
                    }

                    if (tempTime <= 0)
                    {
                        tempTime = pingDelayTime;
                        //Debug.Log("发送时间：" + DateTime.Now.Ticks);
                        byte[] contents = bitConverter.GetBytes(DateTime.Now.Ticks);
                        //long temp = bitConverter.ToInt64(contents, 0);
                        //NetDebug.Log("发送Ping:" + temp);

                        byte[] sendBytes = MsgPackest.Write2Bytes(networkCommon.Configuration.byteOrder, 0,0,0, property, contents);
                        session.StatisticSendPackets(property, sendBytes.Length);
                        networkCommon.Sendbytes( session,sendBytes);
                    }
                    else
                    {
                        tempTime -= 50;
                    }
                }
                Thread.Sleep(50);
            }
        }
    }
    /// <summary>
    /// Ping功能(服务端)
    /// </summary>
    public class NetPongPlugin : NetMsgProcessPluginBase
    {
        public override byte GetNetProperty()
        {
            return (byte)NetProperty.Ping;
        }

        protected override void OnInit()
        {

        }
        public override void ReceveProcess( MsgPackest packest)
        {
            byte[] sendBytes = MsgPackest.Write2Bytes(networkCommon.Configuration.byteOrder, 0,0,0, (byte)NetProperty.Pong, packest.contents);
            networkCommon.Sendbytes(packest.session,sendBytes);
        }
        public override void Release()
        {

        }
    }
}

