using SimpleNetCore;
using System;
using UnityEngine;

namespace SimpleNetManager
{
    public class NetClient
    {
        private static NetworkClientManager netManager;

        public static NetworkClientManager NetManager
        {
            get
            {

                return netManager;
            }
        }
        public static SimpleNetManager.Player Player
        {
            get
            {
                if (NetManager != null)
                {
                    return SimpleNetManager.PlayerManager.GetPlayer(NetManager.Session);
                }
                return null;
            }
        }

        public static Action OnInit;

        private static int m_port;
        public static DiscoveryPeerManager DiscoveryServerManager { get { return discoveryServerManager; } }
        private static DiscoveryPeerManager discoveryServerManager = new DiscoveryPeerManager();
        public static void Init(int port)
        {
            m_port = port;

            NetDebug.Log = Debug.Log;
            NetDebug.LogError = Debug.LogWarning;
            NetDebug.LogError = Debug.LogError;

            ClientConfiguration configuration = (ClientConfiguration)ClientConfiguration.NewDefaultConfiguration(
                new TelepathyTransport(false),
                 new NetCustomSerializer(),
                new MessageDispatcher(false))
                .EnablePing()
                .SetMsgCompress("gzip")
                .EnableEncryption()
                ;
            configuration.EnableReconnect();
            configuration.EnableStatistics();
            netManager = new NetworkClientManager( configuration);
            netManager.Start();

            controllerManager .Init(netManager);

            discoveryServerManager.Start(port);
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
                discoveryServerManager.Update(deltaTime);
            }
        }
        public static void Stop()
        {
            discoveryServerManager.Close();

            if (NetManager != null)
                NetManager.Stop();
            if (controllerManager != null)
                controllerManager.StopAll();

           
        }
    }
}
