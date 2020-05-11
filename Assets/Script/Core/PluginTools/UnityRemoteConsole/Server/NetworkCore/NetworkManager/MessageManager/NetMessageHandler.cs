using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace LiteNetLibManager
{
    public class NetMessageHandler
    {
        public string msgType { get; private set; }
        public long connectionId { get; private set; }
        public NetDataReader message { get; private set; }
        public Player player { get; private set; }

        private INetSerializer serializer;
        public NetMessageHandler(string msgType, long connectionId,Player player, NetDataReader message, INetSerializer serializer)
        {
            this.msgType = msgType;
            this.connectionId = connectionId;
            this.message = message;
            this.serializer = serializer;
            this.player = player;
        }

        public T GetMessage<T>() where T:new()
        {
            int pos = message.Position;
            T t = serializer.Deserialize<T>(message);
            message.SetSource(message.RawData, pos);
            //Debug.Log("ReceveMsg <=::" + msgType + " ==>:" + JsonUtility.ToJson(t));
            return t;
        }
    }
}
