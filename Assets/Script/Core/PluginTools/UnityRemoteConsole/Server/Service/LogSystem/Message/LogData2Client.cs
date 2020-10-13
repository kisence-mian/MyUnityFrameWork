using SimpleNetCore;
using System;

namespace UnityRemoteConsole
{
    [Serializable]
    public class LogData2Client : INetSerializable
    {
        public LogData logData=new LogData();
        public void Deserialize(NetDataReader reader)
        {
            logData.Deserialize(reader);

        }

        public void Serialize(NetDataWriter writer)
        {
            logData.Serialize(writer);
        }
    }
}
