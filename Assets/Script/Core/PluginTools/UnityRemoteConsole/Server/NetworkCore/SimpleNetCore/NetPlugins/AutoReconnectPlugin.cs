
using System.Collections;
using System.Threading;
using System;

namespace SimpleNetCore
{
    public class AutoReconnectPlugin : NetMsgProcessPluginBase
    {
        public override byte GetNetProperty()
        {
            return 99;
        }

        private bool enable = true;
      
        public bool IsReconnecting { get; private set; }
        public bool Enable { get => enable; set
            {
                enable = value;
                if (enable == false)
                {
                    ForceStopReconnect();
                }
            }
        }

        private Thread reconnectThread;
        private bool isRelease = false;
        public override void Release()
        {
            isRelease = true;
            ForceStopReconnect();
        }

        protected override void OnInit()
        {
            isRelease = false;
        }

        public void ForceStopReconnect()
        {
            IsReconnecting = false;
            if (reconnectThread != null)
            {
                reconnectThread.Abort();
                reconnectThread = null;
            }
        }
        public override void PeerConnectedEvent(Session session)
        {
            ForceStopReconnect();
        }
        public override void DisconnectedEvent(Session session, EDisconnectInfo disconnectInfo)
        {
            if (isRelease)
                return;
            if (IsReconnecting&& disconnectInfo.Reason == EDisconnectReason.DisconnectPeerCalled)
            {
                ForceStopReconnect();
                return;
            }
            if (Enable && !IsReconnecting && disconnectInfo.Reason!= EDisconnectReason.DisconnectPeerCalled)
            {
                NetDebug.Log("DisconnectedEvent 开始重连");
                IsReconnecting = true;

                {
                    tempTime = 0;
                    reconnectThread = new Thread(ReconnectThreadFun);
                    reconnectThread.Start();
                    reconnectThread.IsBackground = true;
                }
            }
        }

        private void ReconnectThreadFun(object obj)
        {
            while (true)
            {
                ReconnectUpdate(500);
                Thread.Sleep(500);
            }
        }

        private int updateTime = 3500;
        private int tempTime = 0;
        private void ReconnectUpdate(int deltaTime)
        {

            if (tempTime < 0)
            {
                tempTime = updateTime;
                NetDebug.Log("开始重新连接:" + networkCommon.networkAddress + ":" + networkCommon.networkPort);
                networkCommon.Configuration.Transport.Connect(networkCommon.networkAddress, networkCommon.networkPort);

            }
            else
            {
                tempTime -= deltaTime;
            }

        }
    }
}