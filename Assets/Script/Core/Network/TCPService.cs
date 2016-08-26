using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Net;
using System.Text;

public class TCPService : INetworkInterface 
{
    private Socket mSocket;
    string stringData = "";
    private byte[] readData = new byte[1024];

    public string mIPaddress = "";
    public int port = 0; 
    private Thread connThread;

    //连接服务器
    public override void connect(string ipAdress, int port)
    {
        Close();

        mIPaddress = ipAdress;
        this.port = port;

        connThread = null;
        connThread = new Thread(new ThreadStart(requestConnect));
        connThread.Start();
    }
    //关闭连接
    public override void Close()
    {
        isConnect = false;
        if (mSocket != null)
        {
            mSocket.Close(0);
            mSocket = null;
        }
        if (connThread != null)
        {
            connThread.Join();
            connThread.Abort();
        }
        connThread = null;
        //mesStr.Clear();

        //TcpClient tcp = new TcpClient();
        //tcp.GetStream().

    }
    //请求数据服务连接线程
    void requestConnect()
    {
        try
        {
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(mIPaddress);
            IPEndPoint ipe = new IPEndPoint(ip, port);
            //mSocket.
            mSocket.Connect(ipe);
            isConnect = true;
            startReceive();


        }
        catch (Exception e)
        {
            isConnect = false;
            m_netWorkCallBack(e.ToString());
        }

    }
    void startReceive()
    {
        stringData = "";
        mSocket.BeginReceive(readData, 0, readData.Length, SocketFlags.None, new AsyncCallback(endReceive), mSocket);
    }
    void endReceive(IAsyncResult iar) //接收数据
    {
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);
        if (recv > 0)
        {
            stringData = Encoding.UTF8.GetString(readData, 0, recv);
            //lock (mesStr)
            {
                // mesStr.Add(stringData);
                if (m_netWorkCallBack != null)
                {
                    dealMessage(stringData);
                }
            }
        }
        startReceive();
    }
    //发送消息
    public override void sendMessage(String str)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str + "&");
            mSocket.Send(bytes);
        }
        catch (Exception e)
        {
            m_netWorkCallBack(e.ToString());
        }
    }
    public override void dealMessage(string s)
    {
        string[] str = s.Split('&');
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != "")
            {
                m_netWorkCallBack(str[i]);
            }
        }
    }
}
