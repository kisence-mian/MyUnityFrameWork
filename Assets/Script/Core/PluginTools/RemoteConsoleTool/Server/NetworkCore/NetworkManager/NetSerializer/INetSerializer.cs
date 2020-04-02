using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
  public  interface INetSerializer
    {
        void Serialize(NetDataWriter writer, object messageData);

        T Deserialize<T>(NetDataReader reader) where T : new();
    }
}
