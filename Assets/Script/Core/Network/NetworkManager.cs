using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FrameWork;
using System.Net.Sockets;

public class NetworkManager 
{
    static INetworkInterface s_network;

    public static bool IsConnect
    {
        get {
            if(s_network == null)
            {
                return false;
            }

            return s_network.m_socketService.isConnect;
        }
    }

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
    public static void Init<TProtocol,TSocket>(ProtocolType protocolType = ProtocolType.Tcp) where TProtocol : INetworkInterface,new () where TSocket : SocketBase,new()
    {
        //提前加载网络事件派发器，避免异步冲突
        InputManager.LoadDispatcher<InputNetworkConnectStatusEvent>();
        InputManager.LoadDispatcher<InputNetworkMessageEvent>();

        s_network = new TProtocol();
        s_network.m_socketService = new TSocket();
        s_network.m_socketService.m_byteCallBack = s_network.SpiltMessage;
        s_network.m_socketService.m_connectStatusCallback = ConnectStatusChange;
        s_network.m_socketService.m_protocolType = protocolType;
        s_network.m_socketService.Init();

        s_network.Init();
        s_network.m_messageCallBack = ReceviceMeaasge;

        ApplicationManager.s_OnApplicationUpdate += Update;
        ApplicationManager.s_OnApplicationQuit += DisConnect;

        ApplicationManager.s_OnApplicationOnGUI += GUI;
    }

    public static void Init(string networkInterfaceName,string socketName)
    {
        //提前加载网络事件派发器，避免异步冲突
        InputManager.LoadDispatcher<InputNetworkConnectStatusEvent>();
        InputManager.LoadDispatcher<InputNetworkMessageEvent>();

        Type type = Type.GetType(networkInterfaceName);

        s_network = Activator.CreateInstance(type) as INetworkInterface;

        Type socketType = Type.GetType(networkInterfaceName);

        s_network.m_socketService = Activator.CreateInstance(socketType) as SocketBase;
        s_network.m_socketService.m_byteCallBack = s_network.SpiltMessage;
        s_network.m_socketService.m_connectStatusCallback = ConnectStatusChange;
        s_network.m_socketService.Init();

        s_network.Init();
        s_network.m_messageCallBack = ReceviceMeaasge;

        ApplicationManager.s_OnApplicationUpdate += Update;
        ApplicationManager.s_OnApplicationQuit += DisConnect;
    }

    public static void Dispose()
    {
        InputManager.UnLoadDispatcher<InputNetworkConnectStatusEvent>();
        InputManager.UnLoadDispatcher<InputNetworkMessageEvent>();

        s_network.m_messageCallBack = null;
        s_network = null;

        ApplicationManager.s_OnApplicationUpdate -= Update;
    }

    public static void SetServer(string IP, int port)
    {
        s_network.SetIPAddress(IP, port);
    }
    public static void Connect()
    {
        //s_network.GetIPAddress();
        s_network.Connect();
    }

    public static void DisConnect()
    {
        Debug.Log("断开连接");
        s_network.Close();
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

    static int msgCount = 0;

    static void ReceviceMeaasge(NetWorkMessage message)
    {
        if(message.m_MessageType != null)
        {
            s_messageList.Add(message);
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
        s_statusList.Add(status);
    }

    static void Dispatch(NetworkState status)
    {
        InputNetworkEventProxy.DispatchStatusEvent(status);
    }

    #region Update

    static List<NetworkState> s_statusList = new List<NetworkState>();
    static List<NetWorkMessage> s_messageList = new List<NetWorkMessage>();
    const int MaxDealCount = 2000;

    //将消息的处理并入主线程
    static void Update()
    {
        if (s_messageList.Count > 0)
        {
            int dealCount = 0;
            for (int i = 0; i < s_messageList.Count; i++)
            {
                dealCount++;
                Dispatch(s_messageList[i]);

                s_messageList.RemoveAt(i);
                i--;

                //if (dealCount >= MaxDealCount)
                //{
                //    Debug.Log("s_messageList.Count " + s_messageList.Count);

                //    break;
                //}
            }
        }


        if (s_statusList.Count > 0)
        {
            for (int i = 0; i < s_statusList.Count; i++)
            {
                Dispatch(s_statusList[i]);
            }
            s_statusList.Clear();
        }
       if(s_network != null)
        {
            s_network.Update();
        }
       
    }
    static float msgCountTimer = 0;
    static int count = 0;
    static void GUI()
    {
        msgCountTimer += Time.deltaTime;
        GUILayout.Label("MPS " + count);

        if (msgCountTimer > 1)
        {
            count = msgCount;
            msgCountTimer = 0;
            msgCount = 0;
        }

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
}

public struct NetWorkMessage
{
   public string m_MessageType;

   public Dictionary<string, object> m_data;
}

