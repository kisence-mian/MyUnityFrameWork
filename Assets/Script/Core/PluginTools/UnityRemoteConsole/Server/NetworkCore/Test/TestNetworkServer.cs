using LiteNetLib;
using LiteNetLibManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TestNetworkServer : MonoBehaviour
{
    private int port = 9132;

    public void Awake()
    {
        string name = Application.productName + "[" + Application.version + "]";

        LitNetServer.SetNetworkServerManager(name, port);


        LitNetServer.ServiceManager.Add<LoginService>();

        LitNetServer.Start();
    }



    private void Update()
    {
        LitNetServer.Update(Time.deltaTime);
    }
    private void OnPeerNetworkError(IPEndPoint arg1, SocketError arg2)
    {
        
    }

    private void OnPeerDisconnected(long arg1, DisconnectInfo arg2)
    {
       
    }

    private void OnPeerConnected(long obj)
    {
        
    }
}

