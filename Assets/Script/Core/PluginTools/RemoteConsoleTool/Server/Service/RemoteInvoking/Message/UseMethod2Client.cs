using UnityEngine;
using System.Collections;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace GameConsoleController
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
