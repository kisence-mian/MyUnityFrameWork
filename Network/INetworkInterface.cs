using UnityEngine;
using System.Collections;
using System;

public class INetworkInterface 
{
    //消息集合
    // public List<string> mesStr = new List<string>();
    static public bool isConnect = false;
    public MessageCallBack m_netWorkCallBack = null;

    public virtual void connect(string ipAdress, int port)
    {
    }

    public virtual void Close()
    {

    }

    public virtual void sendMessage(string str)
    {

    }
    public virtual void dealMessage(string s)
    {

    }
}


