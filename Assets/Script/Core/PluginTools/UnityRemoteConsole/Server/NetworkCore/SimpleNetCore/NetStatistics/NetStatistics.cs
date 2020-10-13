
using System;
using System.Text;

namespace SimpleNetCore
{
    public class NetStatistics 
    {
        public DateTime ConnectTime { get; internal set; }
        public DateTime DisconnectTime { get; internal set; }
        public long ReceiveAllPackets { get; internal set; }
        public long ReceiveHeatBeatPackets { get; internal set; }
        public long ReceivePingPackets { get; internal set; }
        public long ReceiveDataPackets { get; internal set; }

        public long ReceiveAllBytes { get; internal set; }
        public long ReceiveDataBytes { get; internal set; }
        public long ReceivePingBytes { get; internal set; }
        public long ReceiveHeatBeatBytes { get; internal set; }
  
        public long SendAllPackets { get; internal set; }
        public long SendPingPackets { get; internal set; }
        public long SendHeatBeatPackets { get; internal set; }
        public long SendDataPackets { get; internal set; }

        public long SendAllBytes { get; internal set; }
        public long SendHeatBeatBytes { get; internal set; }
        public long SendPingBytes { get; internal set; }
        public long SendDataBytes { get; internal set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ConnectTime:" + ConnectTime.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Append("\n");
            builder.Append("DisconnectTime:" + DisconnectTime.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Append("\n");
            builder.Append("ReceiveAllPackets:" + ReceiveAllPackets);
            builder.Append("\n");

            builder.Append("ReceiveDataPackets:" + ReceiveDataPackets);
            builder.Append("\n");

            builder.Append("ReceiveAllBytes:" + ReceiveAllBytes);
            builder.Append("\n");

            builder.Append("ReceiveDataBytes:" + ReceiveDataBytes);
            builder.Append("\n");

            builder.Append("SendAllPackets:" + SendAllPackets);
            builder.Append("\n");

            builder.Append("SendDataPackets:" + SendDataPackets);
            builder.Append("\n");

            builder.Append("SendAllBytes:" + SendAllBytes);
            builder.Append("\n");

            builder.Append("SendDataBytes:" + SendDataBytes);

            return builder.ToString();
        }
    }
}
