using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MiniJSON;

public class NetworkManager 
{
    static INetworkInterface s_network;

    public static bool s_isConnect;

    public static void Init()
    {
        //提前加载网络事件派发器，避免异步冲突
        InputManager.LoadDispatcher<InputNetworkConnectStatusEvent>();
        InputManager.LoadDispatcher<InputNetworkMessageEvent>();

        s_network = new TCPService();
        s_network.m_messageCallBack = ReceviceMeaasge;
        s_network.m_ConnectStatusCallback = ConnectStatusChange;
    }

    public static void Connect()
    {
        s_network.GetIPAddress();
        s_network.Connect();
    }

    public static void DisConnect()
    {
        s_network.Close();
    }

    public static void SendMessage(string message)
    {
        s_network.SendMessage(message);
    }

    static void ReceviceMeaasge(string message)
    {
        try
        {
            Dictionary<string, object> data = Json.Deserialize(message) as Dictionary<string, object>;
            InputNetworkEventProxy.DispatchMessageEvent(data["MT"].ToString(), message, data);
        }
        catch(Exception e)
        {
            Debug.LogError("Message Error:" + e.ToString());
        }
    }

    static void ConnectStatusChange(NetworkState status)
    {
        InputNetworkEventProxy.DispatchStatusEvent(status);
    }
}

public delegate void MessageCallBack(string receStr);
public delegate void ConnectStatusCallBack(NetworkState connectStstus);

public enum NetworkState
{
    Connected,
    Connecting,
    NoConnected,
    FaildToConnect,
}

public enum NetworkEvent
{
    ReconnectNetworkEvent,//重新连接
    ConnectSuccess,//网络连接成功
    // ShowLoadingDialog,//网络连接已断开；
}

