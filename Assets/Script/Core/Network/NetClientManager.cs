using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetClientManager
{
   private  INetworkInterface s_network;
   
    public IPEndPoint RemoteIPEndPort
    {
        get
        {
            if (s_network != null)
            {
                return s_network.GetIPAddress();
            }
            return null;
        }
    }
    public  bool IsConnect
    {
        get
        {
            if (s_network == null)
            {
                return false;
            }

            return s_network.m_socketService.isConnect;
        }
    }

    #region 初始化

    /// <summary>
   
    /// <summary>
    /// 网络初始化
    /// </summary>
    /// <typeparam name="TProtocol">协议处理类</typeparam>
    /// <typeparam name="TSocket">Socket类</typeparam>
    /// <param name="protocolType">通讯协议</param>
    public  void Init<TProtocol, TSocket>(ProtocolType protocolType = ProtocolType.Tcp) where TProtocol : INetworkInterface, new() where TSocket : SocketBase, new()
    {

        s_network = new TProtocol();
        s_network.m_socketService = new TSocket();

        Debug.Log("protocolType " + s_network.m_socketService.m_protocolType);

        s_network.m_socketService.m_protocolType = protocolType;

        NetInit();
    }


    private  void NetInit()
    {
        s_network.m_socketService.m_byteCallBack = s_network.SpiltMessage;
        s_network.m_socketService.m_connectStatusCallback = ConnectStatusChange;
        s_network.m_socketService.Init();

        s_network.Init();
        s_network.m_messageCallBack = ReceviceMeaasge;

        foreach (var item in plugins.Values)
        {
            item.SetNetwork(this);
        }
    }

    private  Dictionary<Type, NetPluginBase> plugins = new Dictionary<Type, NetPluginBase>();
    public  T AddPlugin<T>(params object[] paramArray) where T : NetPluginBase, new()
    {
        Type type = typeof(T);
        if (plugins.ContainsKey(type))
            return (T)plugins[type];
        else
        {
            T t = new T();
            t.SetNetwork(this);
            t.Init(paramArray);
            plugins.Add(type, t);
            return t;
        }
    }
    public T GetPlugin<T>() where T : NetPluginBase, new()
    {
        Type type = typeof(T);
        if (plugins.ContainsKey(type))
            return (T)plugins[type];
        return default(T);
    }
    #endregion

    #region API

    public  void Dispose()
    {
        foreach (var item in plugins.Values)
        {
            try
            {
                item.OnDispose();
            }
            catch (Exception e)
            {
                Debug.LogError(item.GetType() + ".OnDispose \n" + e);
            }
        }
       
        if (s_network != null)
        {
            s_network.Dispose();
            s_network = null;
        }

    }

    public  void SetServer(string IP, int port)
    {
        Debug.Log("Set IP=>" + IP + ":" + port);
        IPAddress address;
        if (IPAddress.TryParse(IP, out address))
        {
            s_network.SetIPAddress(IP, port);
        }
        else
        {
            SetDomain(IP, port);
        }
    }


    private  void SetDomain(string url, int port)
    {
        IPHostEntry IPinfo = Dns.GetHostEntry(url);
        IPAddress[] ipList = IPinfo.AddressList;
        Debug.Log("解析域名：" + ipList[0].ToString());
        s_network.SetIPAddress(ipList[0].ToString(), port);
    }

    public  void Connect()
    {
        s_network.Connect();
    }

    public  void DisConnect()
    {
        Debug.Log("断开连接");
        s_network.Close();

    }

    public  void SendMessage(byte[] msg)
    {
        s_network.m_socketService.Send(msg);
    }
    public  void SendMessage(string messageType, Dictionary<string, object> data)
    {
        if (IsConnect)
        {
            foreach (var item in plugins.Values)
            {
                try
                {
                    item.OnSendMsg(messageType, data);
                }
                catch (Exception e)
                {
                    Debug.LogError(item.GetType() + ".OnSendMsg \n" + e);
                }
            }
            s_network.SendMessage(messageType, data);
        }
        else
        {
            Debug.LogError("socket 未连接！");
        }
    }

    public  void SendMessage(Dictionary<string, object> data)
    {
        try
        {
            if (IsConnect)
            {
                if (!data.ContainsKey("MT"))
                {
                    Debug.LogError("NetworkManager SendMessage Error ：消息没有加 MT 字段！");
                    return;
                }

                s_network.SendMessage(data["MT"].ToString(), data);
            }
            else
            {
                Debug.LogError("socket 未连接！");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SendMessage Error " + e.ToString());
        }
    }

    #endregion

    #region 事件派发

     void ReceviceMeaasge(NetWorkMessage message)
    {
        if (message.m_MessageType != null)
        {
            foreach (var item in plugins.Values)
            {
                try
                {
                    item.OnReceiveMsg(message);
                }
                catch (Exception e)
                {
                    Debug.LogError(item.GetType() + ".OnReceiveMsg \n" + e);
                }
            }
        }
        else
        {
            Debug.LogError("ReceviceMeaasge m_MessageType is null !");
        }
    }



     void ConnectStatusChange(NetworkState status)
    {
        foreach (var item in plugins.Values)
        {
            try
            {
                item.OnConnectStateChange(status);
            }
            catch (Exception e)
            {
                Debug.LogError(item.GetType() + "=>" + status + " \n" + e);
            }
        }
    }

  

    #endregion

    #region Update



    //将消息的处理并入主线程
   public  void Update(float deltaTime)
    {
        if (s_network != null)
        {
            s_network.Update();
        }

        foreach (var item in plugins.Values)
        {
            try
            {
                item.Update();
            }
            catch (Exception e)
            {
                Debug.LogError(item.GetType() + ".Update \n" + e);
            }
        }
    }


    #endregion
}

