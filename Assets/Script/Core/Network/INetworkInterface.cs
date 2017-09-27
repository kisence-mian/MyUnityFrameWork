using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class INetworkInterface 
{
    //消息集合
    // public List<string> mesStr = new List<string>();
    public bool isConnect = false;

    public string m_IPaddress = "";
    public int m_port = 0; 

    public ConnectStatusCallBack m_ConnectStatusCallback;
    public MessageCallBack m_messageCallBack;

    public ProtocolType m_protocolType;

    public virtual void Init()
    {

    }

    public virtual void GetIPAddress()
    {

    }

    public virtual void SetIPAddress(string IP, int port)
    {

    }

    public virtual void Connect()
    {
    }

    public virtual void Close()
    {

    }

    public virtual void SendMessage(string MessageType, Dictionary<string, object> data)
    {

    }

    /// <summary>
    ///回收资源
    /// </summary>
    /// <param name="data"></param>
    public virtual void RecycleData(Dictionary<string, object> data)
    {

    }


    protected NetWorkMessage GetMessageByPool()
    {
        NetWorkMessage result = HeapObjectPool<NetWorkMessage>.GetObject();

        return result;
    }

}


