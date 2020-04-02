using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
    public struct Login2Client : INetSerializable
    {
        public uint code;
        public string playerID;
        public AppData appData;
        public void Deserialize(NetDataReader reader)
        {
            code = reader.GetUInt();
            reader.TryGetString(out playerID);
            appData = new AppData();
            appData.Deserialize(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(code);
            writer.Put(playerID);
            appData.Serialize(writer);
        }
    }

    public class AppData : INetSerializable
    {
        public string serverAppName="";
        public string serverAppVersion="";
        public string bundleIdentifier="";

       
        public void Deserialize(NetDataReader reader)
        {
            reader.TryGetString(out serverAppName);
            reader.TryGetString(out serverAppVersion);
            reader.TryGetString(out bundleIdentifier);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(serverAppName);
            writer.Put(serverAppVersion);
            writer.Put(bundleIdentifier);
        }
    }
}
