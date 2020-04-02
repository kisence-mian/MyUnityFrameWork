using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 自动重连网络
/// </summary>
public class AutoReconnectController
{
    private static bool m_Open = true;
    /// <summary>
    /// 开启，关闭自动重连
    /// </summary>
    public static bool Open
    {
        get { return m_Open; }
        set
        {
            m_Open = value;
        }
    }
    /// <summary>
    /// 间隔多长时间重新连接
    /// </summary>
    public static float DelayTime
    {
        get
        {
            return delayTime;
        }

        set
        {
            delayTime = value;
            if (delayTime < 1)
                delayTime = 1;
        }
    }

    private static float delayTime = 3;
    /// <summary>
    /// 开始重连
    /// </summary>
    public static CallBack StartReconnect;
    /// <summary>
    /// 结束重连
    /// </summary>
    public static CallBack EndReconnect;

    private static bool startReconenct = false;
    private static float tempTimer;
    private static bool isInit = false;
    public static void Init()
    {
        if (isInit)
            return;
        isInit = true;

        InputManager.AddListener<InputNetworkConnectStatusEvent>(OnNetworkConenctStatus);
        ApplicationManager.s_OnApplicationUpdate += Update;
    }

    private static void Update()
    {
        if (!Open)
            return;
        if (startReconenct)
        {
            if (tempTimer <= 0)
            {
                tempTimer = delayTime;
                NetworkManager.Connect();
            }
            else
            {
                tempTimer -= Time.unscaledDeltaTime;
              
            }
        }
    }

    //是否是断线状态
    private static bool isBreakConenct = false;
    private static void OnNetworkConenctStatus(InputNetworkConnectStatusEvent msg)
    {
        Debug.Log("OnNetworkConenctStatus :" + msg.m_status);
        if (msg.m_status == NetworkState.FaildToConnect || msg.m_status == NetworkState.ConnectBreak)
        {
            if (!isBreakConenct)
            {
                isBreakConenct = true;

                Debug.LogWarning("OnNetworkConenctStatus :" + msg.m_status + " " + isBreakConenct);
                startReconenct = true;

                if(Open && StartReconnect != null)
                {
                    StartReconnect();
                }
            }
            else
            {
                Debug.LogWarning("OnNetworkConenctStatus :" + msg.m_status + " " + isBreakConenct);
            }

            //断线
        }
        else if (msg.m_status == NetworkState.Connected)
        {
            isBreakConenct = false;

           
            startReconenct = false;
            if (Open && EndReconnect != null)
            {
                EndReconnect();
            }
            //连接成功
        }
    }
}

