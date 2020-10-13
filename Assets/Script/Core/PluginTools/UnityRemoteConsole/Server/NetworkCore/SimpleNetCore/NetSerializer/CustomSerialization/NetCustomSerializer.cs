using System;
using System.Collections.Generic;

namespace SimpleNetCore
{
    /// <summary>
    /// 这是一个以类名作为消息头，实现INetSerializable的自定义消息序列化器
    /// </summary>
    public class NetCustomSerializer : INetMsgSerializer
    {
        private Dictionary<string, Type> typeDic = new Dictionary<string, Type>();
        private ByteOrder byteOrder;
        public void Init(NetConfiguration configuration)
        {
            byteOrder = configuration.byteOrder;
            //NetDebug.Log("NetCustomSerializer: init");
            Type[] types = ReflectionTool.FastGetChildTypes(typeof(INetSerializable));
            foreach (var t in types)
            {
                //NetDebug.Log("Add Msg Type:" + t.Name);
                typeDic.Add(t.Name, t);
            }

            //reader = new NetDataReader(byteOrder);
            //writer = new NetDataWriter(byteOrder);
        }
        //private NetDataReader reader = null;
        public object Deserialize(byte[] datas, out object msgType)
        {
            //NetDebug.Log("Deserialize收到消息：" + datas.Length);
            NetDataReader reader = new NetDataReader(byteOrder);
            reader.SetSource(datas, 0);
            msgType = reader.GetString();
            string msgT = msgType.ToString();
            if (!typeDic.ContainsKey(msgT))
            {
                NetDebug.LogError("No msgType:" + msgType);
                return null;
            }
            Type type = typeDic[msgT];
            INetSerializable serializable = (INetSerializable)ReflectionTool.CreateDefultInstance(type);
            serializable.Deserialize(reader);
            return serializable;
        }

        public object GetMsgType(object data)
        {
            return data.GetType().Name;
        }


        //private NetDataWriter writer = null;
        public byte[] Serialize(object msgType, object data)
        {
            NetDataWriter writer = new NetDataWriter(byteOrder);
            writer.Reset();
            writer.PutValue(msgType);
            INetSerializable serializable = data as INetSerializable;
        
            if (serializable != null)
            {
                serializable.Serialize(writer);
            }
            else
            {
                NetDebug.LogError("cant change INetSerializable");
            }
            return writer.CopyData();
        }
     
    }
}
