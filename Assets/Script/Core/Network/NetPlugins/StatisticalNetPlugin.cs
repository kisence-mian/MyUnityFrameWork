using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public sealed class StatisticalNetPlugin : NetPluginBase
{
    public NetStatistics statistics = new NetStatistics();

    public override void OnSendMsg(string messageType, Dictionary<string, object> data)
    {
        statistics.PacketsSent++;
    }
    public override void OnReceiveMsg(NetWorkMessage message)
    {
        statistics.PacketsReceived++;
    }
}

public sealed class NetStatistics
{
    public ulong PacketsSent;
    public ulong PacketsReceived;
    public ulong BytesSent;
    public ulong BytesReceived;
    public ulong PacketLoss;
    public ulong PacketLossPercent
    {
        get { return PacketsSent == 0 ? 0 : PacketLoss * 100 / PacketsSent; }
    }

    public ulong SequencedPacketLoss;

    public void Reset()
    {
        PacketsSent = 0;
        PacketsReceived = 0;
        BytesSent = 0;
        BytesReceived = 0;
        PacketLoss = 0;
    }

    public override string ToString()
    {
        return
            string.Format(
                "BytesReceived: {0}\nPacketsReceived: {1}\nBytesSent: {2}\nPacketsSent: {3}\nPacketLoss: {4}\nPacketLossPercent: {5}\n",
                BytesReceived,
                PacketsReceived,
                BytesSent,
                PacketsSent,
                PacketLoss,
                PacketLossPercent);
    }
}