using UnityEngine;
using System.Collections;
using System;

public class INetworkInterface 
{
    //消息集合
    // public List<string> mesStr = new List<string>();
    public bool isConnect = false;

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

    public virtual void SendMessage(string str)
    {

    }

    public virtual void DealMessage(string s)
    {

    }
}


