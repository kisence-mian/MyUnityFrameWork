using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiteNetLib.Utils;
using UnityEngine;

namespace LiteNetLibManager
{
    public class LiteNetLibSerializer : INetSerializer
    {
        //NetSerializer netSerializer = new NetSerializer();
        public T Deserialize<T>(NetDataReader reader)where T:new()
        {
            T t = new T();
            INetSerializable serializable = (INetSerializable)t;

            serializable.Deserialize(reader);
            //Debug.Log("Deserialize:" + JsonUtility.ToJson(serializable));
            //Debug.Log("Deserialize:" + JsonUtility.ToJson(t));
            t = (T)serializable;

            
            //netSerializer.RegisterNestedType<T>();
            //Debug.Log("Deserialize:" + JsonUtility.ToJson(t));
            return t;
        }

        public void Serialize(NetDataWriter writer, object messageData)
        {
            INetSerializable serializable = messageData as INetSerializable;
            if (serializable != null)
            {
                serializable.Serialize(writer);
            }
            else
            {
                Debug.LogError("cant change INetSerializable");
            }
        }
    }
}
