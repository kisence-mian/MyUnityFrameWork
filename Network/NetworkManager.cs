using UnityEngine;
using System.Collections;
using System;

public class NetworkManager 
{
    public static INetworkInterface s_network;
    public static MessageCallBack s_onMessageCallBack;

    public static bool s_isConnect;

    public static void Init()
    {

      

        s_network = new TCPService();

        s_network.m_netWorkCallBack = ReceviceMeaasge;
    }

    public static void Connect()
    {
        s_network.connect("", 123);
    }

    public static void DisConnect()
    {
        s_network.Close();
    }

    public static void SendMessage(string l_message)
    {
        s_network.sendMessage(l_message);
    }

    public static void ReceviceMeaasge(string l_message)
    {
        try
        {
            if (s_onMessageCallBack != null)
                s_onMessageCallBack(l_message);
        }
        catch(Exception e)
        {
            Debug.LogError("Message Error:" + e.ToString());
        }
    }
}

public delegate void MessageCallBack(string receStr);
