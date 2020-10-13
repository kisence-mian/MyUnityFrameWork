using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleNetCore
{
    /// <summary>
    /// 网络连接会话类，一个连接一个实例
    /// </summary>
    public class Session 
    {
        public long ConnectionId
        {
            get
            {
                return connectionId;
            }
        }

        private long connectionId;
        public Session(long connectionId)
        {
            this.connectionId = connectionId;
        }

        #region Counter

        //public uint SendMsgCounter { get => sendMsgCounter; }
        //public uint ReceiveMsgCounter { get => receiveMsgCounter;  }


        private uint sendMsgCounter = 0;
        private uint receiveMsgCounter = 0;

    

        public bool CheckReceiveMsgCounter(uint counter)
        {
            if (counter> receiveMsgCounter )
                return true;
            else
            {
                return false;
            }
        }

        public uint AddSendCounter()
        {
            sendMsgCounter++;
            return sendMsgCounter;
        }
        public uint SetReceiveCounter(uint counter)
        {
            receiveMsgCounter =  counter;
            return receiveMsgCounter;
        }
        #endregion

        #region Attribute
        private Dictionary<string, object> attributeDatas = new Dictionary<string, object>();

        public IEnumerable<string> GetAttributeKeys()
        {
            return attributeDatas.Keys;
        }

        public object GetAttribute(string key)
        {
            object value = null;
            attributeDatas.TryGetValue(key, out value);
            return value;
        }

        public void SetAttribute(string key,object value)
        {
            if (attributeDatas.ContainsKey(key))
            {
                attributeDatas[key] = value;
            }
            else
            {
                attributeDatas.Add(key, value);
            }
        }

        public void RemoveAttribute(string key)
        {
            if (attributeDatas.ContainsKey(key))
            {
                attributeDatas.Remove(key);
            }
        }
        public bool ContainsAttributeKey(string key)
        {
            return attributeDatas.ContainsKey(key);
        }

        #endregion

        #region Statistics
        public NetStatistics statistics = new NetStatistics();
        public bool UseStatistics { get; private set; }
        internal void OpenNetStatistics(bool isOpenStatistics)
        {
            UseStatistics = isOpenStatistics;
        }
        internal void SetConnectTimeInStatistic()
        {
            if (!UseStatistics)
                return;
            statistics.ConnectTime = new System.DateTime();
        }
        internal void SetDisconnectTimeInStatistic()
        {
            if (!UseStatistics)
                return;
            statistics.DisconnectTime = new System.DateTime();
        }
        internal void StatisticReceivePackets(byte property,int byteSize)
        {

            if (!UseStatistics)
                return;
            try
            {
                statistics.ReceiveAllPackets++;
                statistics.ReceiveAllBytes += byteSize;
                switch ((NetProperty)property)
                {
                    case NetProperty.Data:
                        statistics.ReceiveDataPackets++;
                        statistics.ReceiveDataBytes += byteSize;
                        break;
                    case NetProperty.HeartBeatServerSend:
                        statistics.ReceiveHeatBeatPackets++;
                        statistics.ReceiveHeatBeatBytes += byteSize;
                        break;
                    case NetProperty.Pong:
                        statistics.ReceivePingPackets++;
                        statistics.ReceivePingBytes += byteSize;
                        break;
                }
                
            }
            catch (System.Exception e)
            {
                NetDebug.LogError(e.ToString());
            }
        }
        internal void StatisticSendPackets(byte property, int byteSize)
        {

            if (!UseStatistics)
                return;
            try
            {
                statistics.SendAllPackets++;
                statistics.SendAllBytes += byteSize;
                switch ((NetProperty)property)
                {
                    case NetProperty.Data:
                        statistics.SendDataPackets++;
                        statistics.SendDataBytes += byteSize;
                        break;
                    case NetProperty.HeartBeatClinetSend:
                        statistics.SendHeatBeatPackets++;
                        statistics.SendHeatBeatBytes += byteSize;
                        break;
                    case NetProperty.Ping:
                        statistics.SendPingPackets++;
                        statistics.SendPingBytes += byteSize;
                        break;
                }

            }
            catch (System.Exception e)
            {
                NetDebug.LogError(e.ToString());
            }
        }
        #endregion
        public override string ToString()
        {
            return "ConnectionId:" + ConnectionId;
        }
    }
}
