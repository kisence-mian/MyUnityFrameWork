using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketService : SocketBase
{
    protected Socket m_Socket;
    private Thread m_connThread;

    protected int m_offset = 0;

    AsyncCallback m_acb = null;

    public override void Init()
    {
        m_acb = new AsyncCallback(EndReceive);
    }

    public override void Close()
    {
        isConnect = false;
        m_connectStatusCallback(NetworkState.ConnectBreak);

        if (m_Socket != null)
        {
            m_Socket.Close(0);
            m_Socket = null;
        }
        if (m_connThread != null)
        {
            m_connThread.Join();
            m_connThread.Abort();
        }
        m_connThread = null;
    }

    public override void Connect()
    {
        if(isConnect)
        {
            Close();
        }

        m_connThread = null;
        m_connThread = new Thread(new ThreadStart(RequestConnect));
        m_connThread.Start();
    }

    public override void Send(byte[] sendbytes)
    {
        try
        {
            m_Socket.Send(sendbytes);
        }
        catch (Exception e)
        {
            Debug.LogError("Send Error: " + e.ToString());
            isConnect = false;
            m_connectStatusCallback(NetworkState.ConnectBreak);
        }
    }

    void RequestConnect()
    {
        try
        {
            m_connectStatusCallback(NetworkState.Connecting);

            SocketType socketType = SocketType.Stream;

            if (m_protocolType == ProtocolType.Udp)
            {
                socketType = SocketType.Dgram;
            }

            m_Socket = new Socket(AddressFamily.InterNetwork, socketType, m_protocolType);
            IPAddress ip = IPAddress.Parse(m_IPaddress);

            IPEndPoint ipe = new IPEndPoint(ip, m_port);

            m_Socket.Connect(ipe);

            m_offset = 0;
            StartReceive();

            isConnect = true;
            m_connectStatusCallback(NetworkState.Connected);
        }
        catch (Exception e)
        {
            Debug.LogError("IP :" + m_IPaddress + " Port: " + m_port + " :" + e.ToString());
            isConnect = false;
            m_connectStatusCallback(NetworkState.FaildToConnect);
        }
    }

    void StartReceive()
    {
        m_Socket.BeginReceive(m_readData, m_offset, m_readData.Length, SocketFlags.None, m_acb, m_Socket);
    }

    void EndReceive(IAsyncResult iar) //接收数据
    {
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);
        if (recv > 0)
        {
            DealByte(m_readData, ref m_offset, recv);
        }

        StartReceive();
    }

    protected virtual void DealByte(byte[] data, ref int offset, int length)
    {
        if (m_byteCallBack != null)
        {
            //Debug.Log("DealByte " + Encoding.UTF8.GetString(data, offset, length));

            m_byteCallBack(m_readData, ref m_offset, length);
        }
    }

    public override void Update()
    {
       
    }
}
