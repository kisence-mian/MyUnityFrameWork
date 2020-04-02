using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using Ping = System.Net.NetworkInformation.Ping;

public class PingNetPlugin:NetPluginBase
{
    public long Ping { get; private set; }

    private IPEndPoint remoteIPEndPort;
    //private bool isConnect = false;

   
    public override void Update()
    {
        PingLogic();
    }
    void PingLogic()
    {
        if (s_network == null)
        {
            Debug.Log("s_network is null");
            return;
        }
        remoteIPEndPort = s_network.RemoteIPEndPort;
        var sender = new Ping();

        var reply = sender.Send(remoteIPEndPort.Address);
        if (reply.Status == IPStatus.Success)
        {
            Ping = reply.RoundtripTime;
        }
    }

}

