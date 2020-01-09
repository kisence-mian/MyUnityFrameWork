using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public abstract class SocketBase
{
    public string m_IPaddress = "";
    public int m_port = 0;
    public IPEndPoint endPoint;
    public bool isConnect = false;

    public ConnectStatusCallBack m_connectStatusCallback;
    public ByteCallBack m_byteCallBack;

    public ProtocolType m_protocolType;

    public byte[] m_readData = new byte[1024 * 1024];

    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Init()
    {

    }

    public virtual void Dispose()
    {

    }

    /// <summary>
    /// 设置IP和端口
    /// </summary>
    /// <param name="IP"></param>
    /// <param name="port"></param>
    public virtual void SetIPAddress(string IP, int port)
    {
        m_IPaddress = IP;
        m_port = port;
        endPoint = new IPEndPoint(IPAddress.Parse(m_IPaddress), port);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="sendbytes"></param>
    public abstract void Send(byte[] sendbytes);

    /// <summary>
    /// 建立连接
    /// </summary>
    public abstract void Connect();

    /// <summary>
    ///关闭连接
    /// </summary>
    public abstract void Close();

    public abstract void Update();
}
