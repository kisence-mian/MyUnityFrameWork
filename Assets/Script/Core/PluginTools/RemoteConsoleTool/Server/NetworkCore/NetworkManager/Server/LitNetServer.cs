using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiteNetLibManager
{
    public class LitNetServer
    {
        private static NetworkServerManager netManager;

        public static NetworkServerManager NetManager
        {
            get
            {
                return netManager;
            }
        }

        public static void SetNetworkServerManager(string discoveryName,int port)
        {
            LiteNetLibTransport transport = new LiteNetLibTransport(port);
            transport.SetBroadcastData(discoveryName);
            LiteNetLibSerializer serializer = new LiteNetLibSerializer();
            netManager = new NetworkServerManager(transport, serializer);

            ServiceManager.Init(netManager);
        }

        private static ServiceManager serviceManager = new ServiceManager();
        public static ServiceManager ServiceManager
        {
            get
            {
                return serviceManager;
            }
        }

        public static void Start()
        {
            if (NetManager == null)
            {
                throw new Exception("no SetNetworkServerManager!");
            }
            serviceManager.StartAll();
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
           
            if (serviceManager != null)
                serviceManager.StopAll();
            if (NetManager != null)
                NetManager.Stop();
        }
    }
}
