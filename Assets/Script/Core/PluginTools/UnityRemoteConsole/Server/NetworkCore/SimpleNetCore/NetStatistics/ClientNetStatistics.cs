using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleNetCore
{
    /// <summary>
    /// 客户端网路数据收集
    /// </summary>
    public class ClientNetStatistics
    {
        /// <summary>
        /// 连接成功次数
        /// </summary>
        public int ConnectTimes { get; private set; }
        public int DisconnectTimes { get; private set; }

        public List<NetStatistics> details = new List<NetStatistics>();

        private long lastConnectTime = -1;

        private long lastDisconnectTime = -1;

        public long AllNoConnnectTime { get; private set; }

        public void MarkDisconnect()
        {
            lastDisconnectTime = DateTime.Now.Ticks;
            DisconnectTimes++;
        }
        public void MarkConnected()
        {
            lastConnectTime = DateTime.Now.Ticks;
            if (lastDisconnectTime != -1&& lastConnectTime!=-1)
            {
                AllNoConnnectTime += lastConnectTime - lastDisconnectTime;
            }
            ConnectTimes++;
        }
        public long GetReceiveDataPackets()
        {
            long num = 0;
            foreach (var st in details)
            {
                num += st.ReceiveDataPackets;
            }
            return num;
        }
        public long GetSendDataPackets()
        {
            long num = 0;
            foreach (var st in details)
            {
                num += st.SendDataPackets;
            }
            return num;
        }
        public long GetReceiveHeatBeatPackets()
        {
            long num = 0;
            foreach (var st in details)
            {
                num += st.ReceiveHeatBeatPackets;
            }
            return num;
        }
        public long GetSendHeatBeatPackets()
        {
            long num = 0;
            foreach (var st in details)
            {
                num += st.SendHeatBeatPackets;
            }
            return num;
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ConnectTimes:" + ConnectTimes);
            builder.Append("\n");
            builder.Append("DisconnectTimes:" + DisconnectTimes);
            builder.Append("\n");

            builder.Append("ReceiveDataPackets:" + GetReceiveDataPackets());
            builder.Append("\n");

            builder.Append("SendDataPackets:" + GetSendDataPackets());
            builder.Append("\n");
            builder.Append("ReceiveHeatBeatPackets:" + GetReceiveHeatBeatPackets());
            builder.Append("\n");
            builder.Append("SendHeatBeatPackets:" + GetSendHeatBeatPackets());
            builder.Append("\n");
            builder.Append("AllNoConnnectTime(s):" + (AllNoConnnectTime / TimeSpan.TicksPerSecond));
            return builder.ToString();
        }
    }
}
