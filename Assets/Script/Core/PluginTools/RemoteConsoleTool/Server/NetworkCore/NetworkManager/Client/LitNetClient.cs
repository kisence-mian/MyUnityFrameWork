using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
    public class LitNetClient
    {
        private static NetworkClientManager netManager;

        public static NetworkClientManager NetManager
        {
            get
            {

                return netManager;
            }
        }
        public static LiteNetLibManager.Player Player
        {
            get
            {
                if (NetManager != null)
                {
                    return LiteNetLibManager.PlayerManager.GetPlayer(NetManager.ConnectionId);
                }
                return null;
            }
        }

        public static Action OnInit;

        private static int m_port; 
        public static void Init(int port)
        {
            m_port = port;

            LiteNetLibTransport transport = new LiteNetLibTransport();
            transport.SetDiscoveryServer(true, port);
            LiteNetLibSerializer serializer = new LiteNetLibSerializer();
            netManager = new NetworkClientManager(transport, serializer);

            controllerManager .Init(netManager);

            if (OnInit != null)
            {
                OnInit();
            }
        }

        private static ClientControllerManager controllerManager = new ClientControllerManager() ;
        public static ClientControllerManager ControllerManager
        {
            get
            {
                return controllerManager;
            }
        }

        public static void Start(string ip)
        {
            if (NetManager == null)
            {
                throw new Exception("no SetNetworkServerManager!");
            }
           if( NetManager.Connect(ip, m_port))
            {
                controllerManager.StartAll();
            }
        }
        public static void Update(float deltaTime)
        {
            if (netManager != null)
            {
                netManager.Update(deltaTime);
                controllerManager.Update(deltaTime);
            }
        }
        public static void Stop()
        {
            if (NetManager != null)
                NetManager.Stop();
            if (controllerManager != null)
                controllerManager.StopAll();
        }
    }
}
