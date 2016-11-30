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

        ApplicationManager.s_OnApplicationUpdate += Update;
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

    public static void SendMessage(Dictionary<string,object> data)
    {
        string mes = Json.Serialize(data);
        mes = mes.Replace("&", "<FCP:AND>");

        Debug.Log("SendMessage: " + mes);

        s_network.SendMessage(mes);
    }

    static void ReceviceMeaasge(string message)
    {
        Debug.Log("SendMessage: " + message);
        s_messageList.Add(message);
    }

    static void Dispatch(string message)
    {
        try
        {
            message = WWW.UnEscapeURL(message);
            message = message.Replace("<FCP:AND>", "&");
            Dictionary<string, object> data = Json.Deserialize(message) as Dictionary<string, object>;
            InputNetworkEventProxy.DispatchMessageEvent(data["MT"].ToString(), message, data);
        }
        catch (Exception e)
        {
            Debug.LogError("Message Error:" + e.ToString());
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
    static List<string> s_messageList      = new List<string>();

    //将消息的处理并入主线程
    static void Update()
    {
        if (s_messageList.Count >0)
        {
            for (int i = 0; i < s_messageList.Count; i++)
            {
                Dispatch(s_messageList[i]);
            }
            s_messageList.Clear();
        }

        if (s_statusList.Count > 0)
        {
            for (int i = 0; i < s_statusList.Count; i++)
            {
                Dispatch(s_statusList[i]);
            }
            s_statusList.Clear();
        }
    }

    #endregion
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

