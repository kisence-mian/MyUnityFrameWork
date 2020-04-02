using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

public abstract class INetworkInterface 
{
    public MessageCallBack m_messageCallBack;
    public SocketBase m_socketService;

    public MsgCompressBase msgCompress;
    public virtual void Init()
    {
    }

    public virtual void Dispose()
    {
        m_messageCallBack = null;

        m_socketService.Dispose();
        m_socketService = null;
    }

    public virtual IPEndPoint GetIPAddress()
    {
        return m_socketService.endPoint;
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

    public virtual void SendMessage(string MessageType, string content)
    {

    }

    public abstract void SpiltMessage(byte[] data, ref int offset, int length);

}


