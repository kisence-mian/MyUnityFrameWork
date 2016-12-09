using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;
using System.Text;

public class TCPService : INetworkInterface 
{
    private Socket m_Socket;
    string m_stringData = "";
    private byte[] m_readData = new byte[1024];

    public string m_IPaddress = "";
    public int m_port = 0; 
    private Thread m_connThread;


    public override void GetIPAddress()
    {
        m_IPaddress = "192.168.0.105";
        m_port = 23333; 

    }

    //连接服务器
    public override void Connect()
    {
        Close();

        m_connThread = null;
        m_connThread = new Thread(new ThreadStart(requestConnect));
        m_connThread.Start();
    }

    //关闭连接
    public override void Close()
    {
        isConnect = false;
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

    //请求数据服务连接线程
    void requestConnect()
    {
        try
        {
            m_ConnectStatusCallback(NetworkState.Connecting);

            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(m_IPaddress);
            IPEndPoint ipe = new IPEndPoint(ip, m_port);
            //mSocket.
            m_Socket.Connect(ipe);
            isConnect = true;
            StartReceive();

            m_ConnectStatusCallback(NetworkState.Connected);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            isConnect = false;
            m_ConnectStatusCallback(NetworkState.FaildToConnect);
        }

    }
    void StartReceive()
    {
        m_stringData = "";
        m_Socket.BeginReceive(m_readData, 0, m_readData.Length, SocketFlags.None, new AsyncCallback(EndReceive), m_Socket);
    }
    void EndReceive(IAsyncResult iar) //接收数据
    {
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);
        if (recv > 0)
        {
            m_stringData = Encoding.UTF8.GetString(m_readData, 0, recv);
            DealMessage(m_stringData);
        }

        StartReceive();
    }
    //发送消息
    public override void SendMessage(String str)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str + "&");
            m_Socket.Send(bytes);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            //m_netWorkCallBack(e.ToString());
        }
    }

    StringBuilder m_buffer = new StringBuilder();
    public override void DealMessage(string s)
    {
        bool isEnd = false;

        if(s.Substring(s.Length-1,1) == NetworkManager.c_endChar.ToString())
        { 
            isEnd = true;
        }

        m_buffer.Append(s);

        string buffer = m_buffer.ToString();

        m_buffer.Remove(0,m_buffer.Length);

        string[] str = buffer.Split(NetworkManager.c_endChar);
        for (int i = 0; i < str.Length; i++)
        {
            if (i != str.Length - 1)
            {
                m_messageCallBack(str[i]);
            }
            else
            {
                if (isEnd)
                {
                    m_messageCallBack(str[i]);
                }
                else
                {
                    m_buffer.Append(str[i]);
                }
            }
        }
        
    }
}
