using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace GameConsoleController
{
    public class NetDataReaderExtend 
    {

       public static List<T> GetListData<T>(NetDataReader reader) where T: INetSerializable,new()
        {
            List<T> list = new List<T>();
            int count = reader.GetInt();
            for (int i = 0; i < count; i++)
            {
                T data = new T();
                 data.Deserialize(reader);
                list.Add(data);
            }
            return list;
        }
    }
}
