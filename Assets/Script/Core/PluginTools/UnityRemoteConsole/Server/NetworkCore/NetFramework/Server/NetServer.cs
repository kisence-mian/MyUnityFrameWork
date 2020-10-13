using SimpleNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SimpleNetManager
{
    public class NetServer
    {
        private static NetworkServerManager netManager;

        public static NetworkServerManager NetManager
        {
            get
            {
                return netManager;
            }
        }
        private static ServiceManager serviceManager = new ServiceManager();
        public static ServiceManager ServiceManager
        {
            get
            {
                return serviceManager;
            }
        }

        public static UDPDiscoverServerManager DiscoverServer
        {
            get
            {
                return discoverServer;
            }
        }

        private static UDPDiscoverServerManager discoverServer = new UDPDiscoverServerManager();
        public static void Start(int port)
        {

            ServerConfiguration configuration = (ServerConfiguration)ServerConfiguration.NewDefaultConfiguration(
                new TelepathyTransport(true) ,
                 new NetCustomSerializer(), 
                new MessageDispatcher(true)).SetMsgCompress("gzip") .EnableEncryption();

            NetDebug.Log = Debug.Log;
            NetDebug.LogError = Debug.LogWarning;
            NetDebug.LogError = Debug.LogError;

            netManager = new NetworkServerManager(configuration);

            ServiceManager.Init(netManager);
            ServiceManager.StartAll();
            netManager.Start(port);
           
        }

        public static void Update(float deltaTime)
        {
            if (netManager != null)
            {
                netManager.Update(deltaTime);
                ServiceManager.Update(deltaTime);
            }
        }

        public static void Stop()
        {
           
            if (ServiceManager != null)
                ServiceManager.StopAll();
            if (NetManager != null)
                NetManager.Stop();

            discoverServer.Close();
        }
    }
}
