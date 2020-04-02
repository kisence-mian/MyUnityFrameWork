using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameConsoleController
{
    public struct AppInfoData2Client : INetSerializable
    {
        public ShowInfoData data;
        public void Deserialize(NetDataReader reader)
        {
            data = new ShowInfoData();
            data.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            data.Serialize(writer);
        }
    }
}
