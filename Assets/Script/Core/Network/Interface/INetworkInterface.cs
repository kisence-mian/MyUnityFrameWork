using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

public abstract class INetworkInterface 
{
    public MessageCallBack m_messageCallBack;
    public SocketBase m_socketService;

    public virtual void Init()
    {
    }

    public virtual void GetIPAddress()
    {

    }

    public virtual void SetIPAddress(string IP, int port)
    {
        m_socketService.SetIPAddress(IP, port);
    }

    public virtual void Connect()
    {
        m_socketService.Connect();
    }

    public virtual void Close()
    {
        m_socketService.Close();
    }
    public virtual void Update()
    {
        m_socketService.Update();
    }
    public virtual void SendMessage(string MessageType, Dictionary<string, object> data)
    {

    }

    public abstract void SpiltMessage(byte[] data, ref int offset, int length);

}


