using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class INetworkInterface 
{
    //消息集合
    // public List<string> mesStr = new List<string>();
    public bool isConnect = false;

    public string m_IPaddress = "";
    public int m_port = 0; 

    public ConnectStatusCallBack m_ConnectStatusCallback;
    public MessageCallBack m_messageCallBack;

    public virtual void GetIPAddress()
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
}


