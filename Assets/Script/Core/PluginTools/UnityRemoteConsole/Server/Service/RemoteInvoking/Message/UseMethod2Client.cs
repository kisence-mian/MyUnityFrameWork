using System.Collections.Generic;
using SimpleNetCore;

namespace UnityRemoteConsole
{
    public class UseMethod2Client : INetSerializable
    {
        public int code;
        public string error;
        public Dictionary<string, string> paramNameValues;
        public void Deserialize(NetDataReader reader)
        {

            code = reader.GetInt();
            error = reader.GetString();
           
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(code);
            writer.Put(error);
            
        }
    }
}
