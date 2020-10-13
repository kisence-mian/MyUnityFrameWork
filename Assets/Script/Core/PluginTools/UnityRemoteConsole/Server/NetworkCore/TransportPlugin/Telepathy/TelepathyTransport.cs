using System;
using UnityEngine;
namespace SimpleNetCore

{
    public class TelepathyTransport : INetworkTransport
    {
        Telepathy.Server server;
        Telepathy.Client client ;
        public  TelepathyTransport(bool isServer):base(isServer)
        {

            Telepathy.Logger.Log = Debug.Log;
            Telepathy.Logger.LogWarning = Debug.LogWarning;
            Telepathy.Logger.LogError = Debug.LogError;
            if (isServer)
            {
                server = new Telepathy.Server();
            }
            else
            {
                client = new Telepathy.Client();
            }
        }
        public override bool Start(int port)
        {

            if (IsServer)
            {
                return server.Start(port);
            }

            return true;

        }
        public override bool Connect(string address, int port)
        {
            try
            {
                client.Connect(address, port);
            }
            catch (Exception e)
            {
                NetDebug.LogError(e.ToString());
                return false;
            }
            return true;
                

        }

        public override void Destroy()
        {
            if (client != null)
            {
                client.Disconnect();
                client = null;
            }
            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }
        public EDisconnectInfo m_disconnectInfo;
        public override bool Disconnect(long connectionId, EDisconnectReason disconnectReason)
        {
            try
            {
                if (!IsServer)
                {
                    client.Disconnect();
                    m_disconnectInfo = new EDisconnectInfo()
                    {
                        Reason =disconnectReason
                    };
                }
                else
                {
                    server.Disconnect((int)connectionId);
                }
            }
            catch (Exception e)
            {
                NetDebug.LogError(e.ToString());
                return false;
            }
            return true;
           
        }


        public override bool Receive(out TransportEventData eventData)
        {
            Telepathy.Message msg;
            bool state = false;
            if (IsServer)
                state = server.GetNextMessage(out msg);
            else
                state = client.GetNextMessage(out msg);

            if (state)
            {
                //NetDebug.Log("接收到消息：" + msg.eventType);
                ENetworkEvent msgType = ENetworkEvent.DataEvent;
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        msgType = ENetworkEvent.ConnectEvent;
                        break;
                    case Telepathy.EventType.Data:
                        msgType = ENetworkEvent.DataEvent;
                        break;
                    case Telepathy.EventType.Disconnected:
                        msgType = ENetworkEvent.DisconnectEvent;
                        break;
                    default:
                        break;
                }
                eventData = new TransportEventData()
                {
                    type = msgType,
                    connectionId = msg.connectionId,
                    data = msg.data,
                    disconnectInfo=this.m_disconnectInfo,
                };
                if(msgType== ENetworkEvent.DisconnectEvent)
                {
                    m_disconnectInfo = default(EDisconnectInfo);
                }
            }
            else
            {
                eventData = default(TransportEventData);
            }

            return state;
        }

        public override bool Send(long connectionId, byte[] data)
        {
            try
            {
                if (!IsServer)
                {
                    client.Send(data);
                }
                else
                {
                    server.Send((int)connectionId, data);
                }
            }
            catch (Exception e)
            {
                NetDebug.LogError(e.ToString());
                return false;
            }
            return true;
        }

        
    }
}
