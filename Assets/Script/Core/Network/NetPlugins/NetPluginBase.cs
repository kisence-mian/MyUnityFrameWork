using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;


public abstract class NetPluginBase
{
    protected NetClientManager s_network;
    public void SetNetwork(NetClientManager s_network)
    {
        this.s_network = s_network;
    }
    public virtual void Init(params object[] paramArray) { }
    public virtual void Update() { }

    public virtual void OnConnectStateChange(NetworkState status) { }

    public virtual void OnReceiveMsg(NetWorkMessage message) { }

    public virtual void OnSendMsg(string messageType, Dictionary<string, object> data) { }

    public virtual void OnDispose() { }
}

