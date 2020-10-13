using System;
using UnityEngine;
namespace SimpleNetCore

{
    public class SimpleTCPTransport : INetworkTransport
    {
        SimpleTCP.Server server;
        SimpleTCP.Client client ;
        public SimpleTCPTransport(bool isServer):base(isServer)
        {

            SimpleTCP.Logger.Log = Debug.Log;
            SimpleTCP.Logger.LogWarning = Debug.LogWarning;
            SimpleTCP.Logger.LogError = Debug.LogError;
            if (isServer)
            {
                server = new SimpleTCP.Server();
            }
            else
            {
                client = new SimpleTCP.Client();
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
        public override bool Disconnect(long connectionId, EDisconnectReason disconnectReason)
        {
            try
            {
                if (!IsServer)
                {
                    client.Disconnect();
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
            SimpleTCP.Message msg;
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
                    case SimpleTCP.EventType.Connected:
                        msgType = ENetworkEvent.ConnectEvent;
                        break;
                    case SimpleTCP.EventType.Data:
                        msgType = ENetworkEvent.DataEvent;
                        break;
                    case SimpleTCP.EventType.Disconnected:
                        msgType = ENetworkEvent.DisconnectEvent;
                        break;
                    default:
                        break;
                }
                EDisconnectReason disReason = EDisconnectReason.ConnectionFailed;
                if(msg.disconnectReason== SimpleTCP.DisconnectReason.ConnectionFailed)
                {
                    disReason = EDisconnectReason.ConnectionFailed;
                }else if(msg.disconnectReason == SimpleTCP.DisconnectReason.DisconnectPeerCalled)
                {
                    disReason = EDisconnectReason.DisconnectPeerCalled;
                }

                eventData = new TransportEventData()
                {
                    type = msgType,
                    connectionId = msg.connectionId,
                    data = msg.data,
                    disconnectInfo= new EDisconnectInfo() {
                     Reason= disReason,
                     }
                };
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
