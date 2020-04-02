using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace GameConsoleController
{
    public class NetDataWriterExtend 
    {

        public static void PutListData<T>(NetDataWriter writer, List<T> data) where T : INetSerializable, new()
        {
            int count = 0;

            if (data != null)
            {
                count = data.Count;
            }
            writer.Put(count);
            foreach (var item in data)
            {
                item.Serialize(writer);
            }
        }
    }
}
