
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SimpleNetManager
{
    public class UDPDiscoverServerManager
    {
        //private NetworkInfo[] networkInfos = NetUtils.GetAllLocalNetworks();
        private List<UDPDiscoverServer> udpDisServer = new List<UDPDiscoverServer>();
        public void Start(int port, string content)
        {
            //foreach (var network in networkInfos)
            //{
            //    UDPDiscoverServer server = new UDPDiscoverServer();

            //    server.Start(network, port,content);
            //    udpDisServer.Add(server);
            //}

            UDPDiscoverServer server = new UDPDiscoverServer();
            server.Start( port, content);
            udpDisServer.Add(server);
        }

        public void Close()
        {
            foreach (var item in udpDisServer)
            {
                item.Close();
            }
            udpDisServer.Clear();
        }
    }
    public class UDPDiscoverServer 
    {
        private UdpClient udpServer;
        //private  Thread updateThread = null;
        byte[] ResponseData;
        private int port;
        public void Start( int port, string content)
        {
            ResponseData = Encoding.UTF8.GetBytes(content);
            this.port = port;
            //var localEndpoint = new IPEndPoint(IPAddress.Parse(network.IPAddress), port);
            udpServer = new UdpClient(port);
            udpServer.EnableBroadcast = true;
            udpServer.BeginReceive(Received, udpServer);

        }
        /// <summary>
        /// 异步接收UDP数据
        /// </summary>
        /// <param name="iar"></param>
        void Received(IAsyncResult iar)
        {
            udpServer = iar.AsyncState as UdpClient;
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port); ;
            byte[] buffer = udpServer.EndReceive(iar, ref ipEndPoint);

            var ClientRequest = Encoding.UTF8.GetString(buffer);

            //NetDebug.Write("Recived {0} from {1}, sending response", ClientRequest, ipEndPoint.Address.ToString());
            if (ClientRequest == UDPDiscoverClient.UDPKey)
            {
                udpServer.Send(ResponseData, ResponseData.Length, ipEndPoint);
            }
            //继续异步接收数据
            udpServer.BeginReceive(Received, udpServer);
        }
        public void Close()
        {
            if (udpServer != null)
            {
                udpServer.Close();
                udpServer.Dispose();
                udpServer = null;
            }
        }
    }
}