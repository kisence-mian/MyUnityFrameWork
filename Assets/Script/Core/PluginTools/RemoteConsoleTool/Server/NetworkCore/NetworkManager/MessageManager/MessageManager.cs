using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LiteNetLibManager
{
    public delegate void MessageHandlerDelegate(NetMessageHandler msgHandler);
    public class MessageManager
    {
        protected INetSerializer serializer;
        protected readonly Dictionary<string, MessageHandlerDelegate> messageHandlers = new Dictionary<string, MessageHandlerDelegate>();
        protected readonly Dictionary<string, MessageHandlerDelegate> noLoginMessageHandlers = new Dictionary<string, MessageHandlerDelegate>();

        protected readonly NetDataWriter writer = new NetDataWriter();

        private bool isServer;
        public MessageManager(INetSerializer serializer,bool isServer)
        {
            this.serializer = serializer;
            this.isServer = isServer;
        }
        public  byte[] SerializeMsg<T>(T messageData)
        {
            writer.Reset();
            writer.Put(typeof(T).Name);
            if (messageData != null)
            {
                serializer.Serialize(writer, messageData);
            }
            //Debug.Log("Send Byte[] :" + writer.CopyData().Length);

            return writer.CopyData();
        }
        internal virtual void ReadPacket(long connectionId, NetDataReader reader)
        {
            string msgType = reader.GetString();
            MessageHandlerDelegate handlerDelegate;
            bool canInvoke = true;
            if (messageHandlers.TryGetValue(msgType, out handlerDelegate))
            {
               
                if (isServer)
                {
                    if (!LiteNetLibManager.PlayerManager.IsLogin(connectionId))
                    {
                        canInvoke = false;
                        Debug.LogError("No Login cant use msg:" + msgType);
                    }
                }
               
            }
            else if (noLoginMessageHandlers.TryGetValue(msgType, out handlerDelegate))
            {

            }
            else
            {
                canInvoke = false;
            }

            if (canInvoke && handlerDelegate!=null)
            {
                NetMessageHandler messageHandler = new NetMessageHandler(msgType, connectionId, LiteNetLibManager.PlayerManager.GetPlayer(connectionId), reader, serializer);
                handlerDelegate.Invoke(messageHandler);
            }
        }

        public void RegisterMessage<T>(MessageHandlerDelegate handlerDelegate)
        {
            Type type = typeof(T);
            string msgType = type.Name;
            bool isNoLoginMsg = typeof(INoLoginMsg).IsAssignableFrom(type);
            
            Dictionary<string, MessageHandlerDelegate> mapHandler = isNoLoginMsg ? noLoginMessageHandlers : messageHandlers;
            if (mapHandler.ContainsKey(msgType))
                mapHandler[msgType] += handlerDelegate;
            else
            {
                mapHandler.Add(msgType, handlerDelegate);
            }
        }

        public void UnregisterMessage<T>(MessageHandlerDelegate handlerDelegate)
        {
            Type type = typeof(T);
            string msgType = type.Name;
            bool isNoLoginMsg = type.IsAssignableFrom(typeof(INoLoginMsg));

            Dictionary<string, MessageHandlerDelegate> mapHandler = isNoLoginMsg ? noLoginMessageHandlers : messageHandlers;

            if (mapHandler.ContainsKey(msgType))
            {
                mapHandler[msgType] -= handlerDelegate;
            }
            else
            {
                Debug.LogError("No RegisterMessage:" + msgType);
            }
        }
    }
}
