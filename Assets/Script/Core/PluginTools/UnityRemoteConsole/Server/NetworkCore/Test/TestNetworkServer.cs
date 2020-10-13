
using SimpleNetManager;
using SimpleNetCore;
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

        //LitNetServer.SetNetworkServerManager(name, port);


        NetServer.ServiceManager.Add<LoginService>();

        NetServer.Start(port);
    }



    private void Update()
    {
        NetServer.Update(Time.deltaTime);
    }
    private void OnPeerNetworkError(IPEndPoint arg1, SocketError arg2)
    {
        
    }

    private void OnPeerDisconnected(long arg1, EDisconnectInfo arg2)
    {
       
    }

    private void OnPeerConnected(long obj)
    {
        
    }
}

