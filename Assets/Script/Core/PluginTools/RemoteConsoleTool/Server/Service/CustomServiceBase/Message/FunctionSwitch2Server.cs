using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameConsoleController
{
    [Serializable]
    public struct FunctionSwitch2Server : INetSerializable
    {
        public string functionName;
        public bool isOpenFunction;

        public void Deserialize(NetDataReader reader)
        {
            functionName = reader.GetString();
            isOpenFunction = reader.GetBool();
            
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(functionName);
            writer.Put(isOpenFunction);
           
        }
    }
}
