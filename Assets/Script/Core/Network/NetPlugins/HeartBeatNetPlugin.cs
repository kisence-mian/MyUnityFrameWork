using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class HeartBeatNetPlugin:NetPluginBase
{
    public const string c_HeartBeatMT = "HB";
    /// <summary>
    /// 接收线程循环间隔时间（毫秒）
    /// </summary>
    public const int ReciveThreadSleepTime = 250;
    public const int SendThreadSleepTime = 250;
    #region 属性
    //毫秒
    private float m_heatBeatSendSpaceTime = 3000f;

    private float m_sendHeatBeatTimer;
    private float m_receviceHeatBeatTimer;

    /// <summary>
    /// 接收心跳包消息线程
    /// </summary>
    private Thread reciveHBThread;
    /// <summary>
    /// 发送心跳包线程
    /// </summary>
    private Thread sendHBThread;

    /// <summary>
    /// 设置心跳包发送间隔时间
    /// </summary>
    public float HeatBeatSendSpaceTime
    {
        get
        {
            return m_heatBeatSendSpaceTime;
        }

        set
        {
            m_heatBeatSendSpaceTime = value;
            if (m_heatBeatSendSpaceTime < 0)
                m_heatBeatSendSpaceTime = 3000;
            ResetSendTimer();
        }
    }

    #endregion

    #region 生命周期

    public override void Init(params object[] paramArray)
    {
        if (paramArray.Length > 0)
        {
            HeatBeatSendSpaceTime = (int)paramArray[0];
        }
        ResetReceviceTimer();
        ResetSendTimer();

        reciveHBThread = new Thread(ReciveHBDealThread);
        reciveHBThread.Start();
        sendHBThread = new Thread(SendHBDealThread);
        sendHBThread.Start();
    }

    private void SendHBDealThread()
    {
        while (true)
        {
            if (s_network.IsConnect)
            {
                //定时发送心跳包
                if (m_sendHeatBeatTimer <= 0)
                {
                    ResetSendTimer();
                    SendHeartBeatMessage();
                }
                else
                {
                    m_sendHeatBeatTimer -= SendThreadSleepTime;
                }
            }
            else
            {
                ResetSendTimer();
            }
            Thread.Sleep(SendThreadSleepTime);
        }
    }

    private void ReciveHBDealThread()
    {
        while (true)
        {
            if (s_network.IsConnect)
            {
                if (GetHeartBeatMessage())
                {
                    ResetReceviceTimer();
                }
                else
                {
                    m_receviceHeatBeatTimer -= ReciveThreadSleepTime;
                }
                //长期没收到服务器返回认为断线
                if (m_receviceHeatBeatTimer <= 0)
                {
                    Debug.Log("HeartBeat Break connect");
                    NetworkManager.DisConnect();
                }

            }
            else
            {
                ResetReceviceTimer();
            }
            Thread.Sleep(ReciveThreadSleepTime);

        }
    }

    public override void OnReceiveMsg(NetWorkMessage message)
    {
        if (IsHeartBeatMessage(message))
        {
            lock (s_messageListHeartBeat)
            {
                s_messageListHeartBeat.Add(message);
            }

        }
    }
    /// <summary>
    /// 心跳包消息
    /// </summary>
    List<NetWorkMessage> s_messageListHeartBeat = new List<NetWorkMessage>();

    /// <summary>
    /// 取出心跳消息
    /// </summary>
    /// <returns></returns>
    public bool GetHeartBeatMessage()
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
    public override void OnDispose()
    {
        if (reciveHBThread != null)
        {
            reciveHBThread.Abort();
        }
        if (sendHBThread != null)
        {
            sendHBThread.Abort();
        }
        reciveHBThread = null;
        sendHBThread = null;
    }

    #endregion

    #region 重载方法
    /// <summary>
    /// 判断消息是否是心跳包消息
    /// </summary>
    /// <param name="msg"></param>
    /// <returns></returns>
    public virtual bool IsHeartBeatMessage(NetWorkMessage msg)
    {
        if (msg.m_MessageType == null)
            return false;
        if (msg.m_MessageType == c_HeartBeatMT)
        {
            return true;
        }
        return false;
    }
    protected virtual void SendHeartBeatMessage()
    {
        //Debug.Log("SendHeartBeatMessage");
        s_network.SendMessage(c_HeartBeatMT, new Dictionary<string, object>());
    }


    #endregion

    #region Update

    /// <summary>
    /// 重设心跳包接收Timer
    /// </summary>
    void ResetReceviceTimer()
    {
        m_receviceHeatBeatTimer = HeatBeatSendSpaceTime * 2 + 1000;
    }

    void ResetSendTimer()
    {
        m_sendHeatBeatTimer = HeatBeatSendSpaceTime;
    }

    #endregion
}

