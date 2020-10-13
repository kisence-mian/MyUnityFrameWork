using SimpleNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleNetManager
{
  

    public class UDPDiscoverClient
    {
        private UdpClient client;
        byte[] RequestData;
        //private Thread updateThread = null;
        private IPEndPoint broadcastIP;

        public const string UDPKey = "UDPDiscover";

        private Queue<UDPPackData> packDatas = new Queue<UDPPackData>();
        private Thread sendThread;
        private int port;
        private NetworkInfo network;
        public void Start(NetworkInfo network, int port)
        {
            this.network = network;
            this.port = port;
            var localEndpoint = new IPEndPoint(IPAddress.Parse(network.IPAddress), 0);
            client = new UdpClient(localEndpoint);
            client.EnableBroadcast = true;
            //client.AllowNatTraversal(true);
           
             broadcastIP = new IPEndPoint(IPAddress.Broadcast, port);

            client.BeginReceive(Received, client);

            sendThread = new Thread(BackGroudSend);
            sendThread.Start();
            sendThread.IsBackground = true;
        }
 
   
        public void Close()
        {
            NetDebug.Log("UDPDiscoverClient.Close");
            if (client != null)
            {
                client.Close();
                client.Dispose();
                client = null;
            }
            if (sendThread != null)
            {
                sendThread.Abort();
                sendThread = null;
            }
        }
        /// <summary>
        /// 异步接收UDP数据
        /// </summary>
        /// <param name="iar"></param>
        void Received(IAsyncResult iar)
        {
            client = iar.AsyncState as UdpClient;
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            byte[] buffer = client.EndReceive(iar, ref ipEndPoint);

            //将获取的byte[]数据转换成字符串
           // string m = Encoding.UTF8.GetString(buffer).Trim();
            var ServerResponse = Encoding.UTF8.GetString(buffer);
            //NetDebug.Log("Recived "+ ServerResponse + " from "+ ipEndPoint.Address.ToString());
            packDatas.Enqueue(new UDPPackData(ipEndPoint, ServerResponse));

            if (packDatas.Count > 10000)
            {
                packDatas.Clear();
            }
            //Console.WriteLine("Receive:{0}", m);

            //继续异步接收数据
            client.BeginReceive(Received, client);
        }
        private void BackGroudSend(object obj)
        {
            while (true) {

                Send(UDPKey);

                Thread.Sleep(700);
            }
        }

        public bool GetMessage(out UDPPackData data)
        {
            if (packDatas.Count > 0)
            {
                data = packDatas.Dequeue();
                return true;
            }
            else
            {
                data = default(UDPPackData);
            }
            return false;
        }

        private void Send(string content)
        {
            if (client == null)
                return;
            RequestData = Encoding.UTF8.GetBytes(content);
            //NetDebug.Log(network.IPAddress + " =>UDP client:Send=>" + content);
            client.Send(RequestData, RequestData.Length, broadcastIP);

        }

    }
    public struct UDPPackData
    {
        public IPEndPoint iPEndPoint;
        public string data;

        public UDPPackData(IPEndPoint iPEndPoint, string data)
        {
            this.iPEndPoint = iPEndPoint;
            this.data = data;
        }
    }
}
