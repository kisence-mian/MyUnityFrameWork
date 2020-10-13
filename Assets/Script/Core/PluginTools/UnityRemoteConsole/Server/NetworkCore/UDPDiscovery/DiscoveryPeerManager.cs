using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SimpleNetManager
{
    public class DiscoveryPeerManager 
    {
        private List<RemoteTagetInfo> discoverPeerRemoveList = new List<RemoteTagetInfo>();
        private Dictionary<RemoteDeviceInfo, RemoteTagetInfo> discoverPeerTimeoutDic = new Dictionary<RemoteDeviceInfo, RemoteTagetInfo>();
        private float timeOut =3f;

        public Action<RemoteTagetInfo> OnServerDiscover;
        public Action<RemoteTagetInfo> OnServerLoseFind;
        public List<RemoteTagetInfo> GetDiscoverPeers
        {
            get
            {
                return new List<RemoteTagetInfo>(discoverPeerTimeoutDic.Values);
            }
        }
       private NetworkInfo[] networkInfos = NetUtils.GetAllLocalNetworks();
        private List<UDPDiscoverClient> uDPDiscoverClients = new List<UDPDiscoverClient>();

        public void Start(int port)
        {
           
            foreach (var network in networkInfos)
            {
                UDPDiscoverClient client = new UDPDiscoverClient();

                client.Start(network, port);
                uDPDiscoverClients.Add(client);
            }

        }
   
        public void Close()
        {
            foreach (var item in uDPDiscoverClients)
            {
                item.Close();
            }
            uDPDiscoverClients.Clear();
        }
        public void SetTimeOut(float timeOut)
        {
            this.timeOut = timeOut;
        }
       
        private void Add(RemoteDeviceInfo deviceInfo, IPEndPoint remoteEndPoint)
        {
            string ipPortString = remoteEndPoint.ToString();
            //Debug.Log("ipPortString:" + ipPortString);
            if (discoverPeerTimeoutDic.ContainsKey(deviceInfo))
            {
                RemoteTagetInfo info = discoverPeerTimeoutDic[deviceInfo];
                info.timeOut = timeOut;
                if(remoteEndPoint.AddressFamily== AddressFamily.InterNetwork)
                {
                    info.address = remoteEndPoint.Address;
                }
                else
                {
                    info.addressV6 = remoteEndPoint.Address;
                }
            }
            else
            {
                RemoteTagetInfo info = new RemoteTagetInfo(deviceInfo);
                if(remoteEndPoint.AddressFamily == AddressFamily.InterNetwork)
                {
                    info.address = remoteEndPoint.Address;

                }
                else
                {
                    info.addressV6 = remoteEndPoint.Address;
                }
                info.port = remoteEndPoint.Port;
                info.timeOut = timeOut;
                discoverPeerTimeoutDic.Add(deviceInfo, info);

                if (OnServerDiscover != null)
                {
                    OnServerDiscover(info);
                }
            }
        }
      

        // Update is called once per frame
        public void Update(float deltaTime)
        {
            foreach (var client in uDPDiscoverClients)
            {
                UDPPackData uDPPackData;
                if (client.GetMessage(out uDPPackData))
                {
                    RemoteDeviceInfo deviceInfo = null;
                    deviceInfo = SimpleJsonUtils.FromJson<RemoteDeviceInfo>(uDPPackData.data);

                    Add(deviceInfo, uDPPackData.iPEndPoint);
                }
            }
           
            if (discoverPeerTimeoutDic.Count > 0)
            {
                foreach (var item in discoverPeerTimeoutDic.Values)
                {
                    if (item.timeOut <= 0)
                    {
                        discoverPeerRemoveList.Add(item);
                    }
                    else
                    {
                       item.timeOut -= deltaTime;
                    }
                }

                foreach (var item in discoverPeerRemoveList)
                {
                    discoverPeerTimeoutDic.Remove(item.info);
                    if (OnServerLoseFind != null)
                    {
                        OnServerLoseFind(item);
                    }
                }
                discoverPeerRemoveList.Clear();
            }
        }
    }
}

        

