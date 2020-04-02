using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameConsoleController
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
