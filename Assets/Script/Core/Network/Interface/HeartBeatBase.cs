using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HeartBeatBase
{
    #region 属性
    private float m_heatBeatSendSpaceTime = 15f;

    private float m_sendHeatBeatTimer;
    private float m_receviceHeatBeatTimer;

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
            m_heatBeatSendSpaceTime = Mathf.Clamp(value, 2f, 100);
            ResetSendTimer();
        }
    }

    #endregion

    #region 生命周期

    public virtual void Init(int spaceTime)
    {
        HeatBeatSendSpaceTime = spaceTime;

        InputManager.AddListener<InputNetworkMessageEvent>("HB",ReceviceMessage);
        InputManager.AddListener<InputNetworkConnectStatusEvent>(ReceviceConnectStatus);
    }

    public virtual void Dispose()
    {
        InputManager.RemoveListener<InputNetworkMessageEvent>("HB",ReceviceMessage);
        InputManager.RemoveListener<InputNetworkConnectStatusEvent>(ReceviceConnectStatus);
    }

    #endregion

    #region 重载方法

    protected virtual void SendHeartBeatMessage()
    {
        //Debug.Log("SendHeartBeatMessage");
        NetworkManager.SendMessage("HB", new Dictionary<string, object>());
    }

    protected virtual void ReceviceMessage(InputNetworkMessageEvent e)
    {
        if(e.m_MessgaeType == "HB")
        {
            ResetReceviceTimer();
        }
    }

    protected virtual void ReceviceConnectStatus(InputNetworkConnectStatusEvent e)
    {
        if(e.m_status == NetworkState.Connected)
        {
            ResetSendTimer();
            ResetReceviceTimer();
        }
    }

    #endregion

    #region Update

    public void Update()
    {
        m_sendHeatBeatTimer -= Time.unscaledDeltaTime;
        m_receviceHeatBeatTimer -= Time.unscaledDeltaTime;

        //定时发送心跳包
        if (m_sendHeatBeatTimer <= 0)
        {
            ResetSendTimer();
            SendHeartBeatMessage();
        }

        //长期没收到服务器返回认为断线
        if (m_receviceHeatBeatTimer <= 0)
        {
            NetworkManager.DisConnect();
        }
    }

    /// <summary>
    /// 重设心跳包接收Timer
    /// </summary>
    void ResetReceviceTimer()
    {
        m_receviceHeatBeatTimer = HeatBeatSendSpaceTime * 2 + 1;
    }

    void ResetSendTimer()
    {
        m_sendHeatBeatTimer = HeatBeatSendSpaceTime;
    }

    #endregion
}
