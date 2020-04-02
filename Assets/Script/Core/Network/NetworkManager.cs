using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FrameWork;
using System.Net.Sockets;
using System.Net;

public class NetworkManager
{
    static INetworkInterface s_network;
    public static HeartBeatBase s_heatBeat;

    public static bool IsConnect
    {
        get {
            if (s_network == null)
            {
                return false;
            }

            return s_network.m_socketService.isConnect;
        }
    }

    #region 初始化

    /// <summary>
    /// 对旧代码的兼容
    /// </summary>
    /// <typeparam name="T">协议处理类</typeparam>
    /// <param name="protocolType"></param>
    [Obsolete("New method is Init<TProtocol,TSocket>")]
    public static void Init<T>(ProtocolType protocolType = ProtocolType.Tcp) where T : INetworkInterface, new()
    {
        Init<T, SocketService>();
    }

    [Obsolete("New method is Init(string networkInterfaceName,string socketName)")]
    public static void Init(string networkInterfaceName)
    {
        Init(networkInterfaceName, "SocketService");
    }
    /// <summary>
    /// 网络初始化
    /// </summary>
    /// <typeparam name="TProtocol">协议处理类</typeparam>
    /// <typeparam name="TSocket">Socket类</typeparam>
    /// <param name="protocolType">通讯协议</param>
    public static void Init<TProtocol, TSocket>( ProtocolType protocolType = ProtocolType.Tcp) where TProtocol : INetworkInterface, new() where TSocket : SocketBase, new()
    {

        Init<TProtocol, TSocket>(null, protocolType);
    }
    /// <summary>
    /// 网络初始化
    /// </summary>
    /// <typeparam name="TProtocol">协议处理类</typeparam>
    /// <typeparam name="TSocket">Socket类</typeparam>
    /// <param name="protocolType">通讯协议</param>
    public static void Init<TProtocol, TSocket>(MsgCompressBase msgCompress, ProtocolType protocolType = ProtocolType.Tcp) where TProtocol : INetworkInterface, new() where TSocket : SocketBase, new()
    {

        s_network = new TProtocol();
        s_network.m_socketService = new TSocket();
        s_network.msgCompress = msgCompress;
        Debug.Log("protocolType " + s_network.m_socketService.m_protocolType);

        s_network.m_socketService.m_protocolType = protocolType;

        NetInit();
    }

    public static void Init(string networkInterfaceName, string socketName)
    {
        Type type = Type.GetType(networkInterfaceName);

        s_network = Activator.CreateInstance(type) as INetworkInterface;

        Type socketType = Type.GetType(networkInterfaceName);
        s_network.m_socketService = Activator.CreateInstance(socketType) as SocketBase;
        s_network.m_socketService.m_protocolType = ProtocolType.Tcp;

        NetInit();
    }

    private static void NetInit()
    {
        //提前加载网络事件派发器，避免异步冲突
        InputManager.LoadDispatcher<InputNetworkConnectStatusEvent>();
        InputManager.LoadDispatcher<InputNetworkMessageEvent>();
        s_network.m_socketService.m_byteCallBack = s_network.SpiltMessage;
        s_network.m_socketService.m_connectStatusCallback = ConnectStatusChange;
        s_network.m_socketService.Init();

        s_network.Init();
        s_network.m_messageCallBack = ReceviceMeaasge;

        ApplicationManager.s_OnApplicationUpdate += Update;
        ApplicationManager.s_OnApplicationQuit += DisConnect;
        ApplicationManager.s_OnApplicationQuit += Dispose;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spaceTime">发送间隔时间，毫秒</param>
    public static void InitHeartBeat<T>(int spaceTime = 2500) where T : HeartBeatBase, new()
    {
        s_heatBeat = new T();
        s_heatBeat.Init(spaceTime);
    }

    #endregion

    #region API

    public static void Dispose()
    {
        InputManager.UnLoadDispatcher<InputNetworkConnectStatusEvent>();
        InputManager.UnLoadDispatcher<InputNetworkMessageEvent>();
        if (s_network != null)
        {
            s_network.Dispose();
            s_network = null;
        }
        if (s_heatBeat != null)
        {
            s_heatBeat.Dispose();
            s_heatBeat = null;
        }

        ApplicationManager.s_OnApplicationUpdate -= Update;
    }

    public static void SetServer(string IP, int port)
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


    public static void SetDomain(string url,int port)
    {
        IPHostEntry IPinfo = Dns.GetHostEntry(url);
        IPAddress[] ipList = IPinfo.AddressList;
        Debug.Log("解析域名：" + ipList[0].ToString());
        s_network.SetIPAddress(ipList[0].ToString(), port);
    }

    public static void Connect()
    {
        s_network.Connect();
    }

    public static void DisConnect()
    {
        Debug.Log("断开连接");
        s_network.Close();
       
    }

    public static void SendMessage(byte[] msg)
    {
        s_network.m_socketService.Send(msg);
    }
    public static void SendMessage(string messageType ,Dictionary<string,object> data)
    {
        if(IsConnect)
        {
            s_network.SendMessage(messageType,data);
        }
        else
        {
            Debug.LogError("socket 未连接！");
        }
    }

    public static void SendMessage(Dictionary<string, object> data)
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
        catch(Exception e)
        {
            Debug.LogError("SendMessage Error " + e.ToString());
        }
    }

    #endregion

    #region 事件派发

    static int msgCount = 0;

    static void ReceviceMeaasge(NetWorkMessage message)
    {
        if(message.m_MessageType != null)
        {
            if (s_heatBeat.IsHeartBeatMessage(message))
            {
                lock (s_messageListHeartBeat)
                {
                    s_messageListHeartBeat.Add(message);
                }
               
            }
            else
            {
                lock (s_messageList)
                {
                    s_messageList.Add(message);
                }
            }

            msgCount++;
        }
        else
        {
            Debug.LogError("ReceviceMeaasge m_MessageType is null !");
        }
    }

    static void Dispatch(NetWorkMessage msg)
    {
        try
        {

            InputNetworkEventProxy.DispatchMessageEvent(msg.m_MessageType, msg.m_data);
        }
        catch (Exception e)
        {
            string messageContent = "";
            if (msg.m_data != null)
            {
                messageContent = Json.Serialize(msg.m_data);
            }
            Debug.LogError("Message Error: MessageType is ->" + msg.m_MessageType + "<- MessageContent is ->" + messageContent + "<-\n" + e.ToString());
        }
    }

    static void ConnectStatusChange(NetworkState status)
    {
        lock (s_statusList)
        {
            s_statusList.Add(status);
        }
    }

    static void Dispatch(NetworkState status)
    {
        InputNetworkEventProxy.DispatchStatusEvent(status);
    }

    #endregion

    #region Update

    static List<NetworkState> s_statusList = new List<NetworkState>();
    static List<NetWorkMessage> s_messageList = new List<NetWorkMessage>();
    /// <summary>
    /// 心跳包消息
    /// </summary>
    static List<NetWorkMessage> s_messageListHeartBeat = new List<NetWorkMessage>();
    const int MaxDealCount = 2000;
    /// <summary>
    /// 取出心跳消息
    /// </summary>
    /// <returns></returns>
    public static bool GetHeartBeatMessage()
    {
        NetWorkMessage msg = default(NetWorkMessage);
        lock (s_messageListHeartBeat)
        {
            if (s_messageListHeartBeat.Count > 0)
            {
                msg = s_messageListHeartBeat[0];
                s_messageListHeartBeat.RemoveAt(0);
                return true;
            }
        }
        return false;
    }
    //将消息的处理并入主线程
    static void Update()
    {
        if (s_messageList.Count > 0)
        {
            lock (s_messageList)
            {
                for (int i = 0; i < s_messageList.Count; i++)
                {
                    Dispatch(s_messageList[i]);

                    s_messageList.RemoveAt(i);
                    i--;
                }
            }
        }

        lock (s_statusList)
        {
            if (s_statusList.Count > 0)
            {
                for (int i = 0; i < s_statusList.Count; i++)
                {
                    Dispatch(s_statusList[i]);
                }
                s_statusList.Clear();
            }
        }

        if (s_network != null)
        {
            s_network.Update();
        }

        //if(s_heatBeat != null && IsConnect)
        //{
        //    s_heatBeat.Update();
        //}
    }

   
    #endregion
}

public delegate void ByteCallBack(byte[] data, ref int offset, int length);
public delegate void MessageCallBack(NetWorkMessage receStr);
public delegate void ConnectStatusCallBack(NetworkState connectStstus);

public enum NetworkState
{
    Connected,
    Connecting,
    ConnectBreak,
    FaildToConnect,
    NetworkError,
}

public struct NetWorkMessage
{
   public string m_MessageType;
   public int m_MsgCode;

   public Dictionary<string, object> m_data;
}

