using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 前端间隔1min询问服务器，根据规则触发实名认证，逻辑层应监听 RealNameLimitEvent 事件以处理实名制规则触发后的逻辑（需要游戏逻辑层处理）
// 通过调用 CheckPayLimit 方法，启动购买限制认证， 监听CheckPayLimitResultEvent 消息，获取结果 （本步骤已与库的购买逻辑结合，游戏逻辑层不需要处理）
public class RealNameManager  {

    static private RealNameManager instance;

    static public RealNameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new RealNameManager();
        }
        return instance;
    }

    #region 变量
    private bool openRealName = false;//是否开启实名认证 ，总开关
    private RealNameStatus realNameStatus = RealNameStatus.NotNeed;//实名状态

    public RealNameStatus RealNameStatus
    {
        get
        {
            return realNameStatus;
        }
        set
        {
            Debug.Log("Set RealNameStatus:" + realNameStatus);
            realNameStatus = value;
        }
    }
    public bool isAdult = false; //是成年人
    #endregion

    #region 外部调用

    /// <summary>
    /// 是否需要实名认证
    /// </summary>
    /// <returns></returns>
    public bool IsOpenRealName()
    {
        return openRealName;
    }

    /// <summary>
    /// 是成年人
    /// </summary>
    /// <returns></returns>
    public bool IsAdult()
    {
        return isAdult;
    }

    /// <summary>
    /// 开始实名认证
    /// </summary>
    /// <returns></returns>
    public void StartRealNameAttestation()
    {
        if (openRealName && RealNameStatus == RealNameStatus.NotRealName)
        {
            SDKManager.StartRealNameAttestation(); // 调用sdk 开始实名认证 
            TestRealNameStatus(); //将认证结果上报服务器
        }
        else
        {
            Debug.LogWarning("StartRealNameVerify useless: openRealName is" + openRealName + " realNameStatus is " + RealNameStatus);
        }
    }

    /// <summary>
    /// 检测是否有支付限制  需要监听 CheckPayLimitResultEvent 消息，获取结果
    /// </summary>
    public void CheckPayLimit(int payAmount)
    {
        CheckPayLimitEvent.Dispatch(payAmount);
    }

    #endregion  

    #region 生命周期
    public void Init()
    {
        //检测总开关
        TestRealNameSwitch();
        if(openRealName)
        {
            ApplicationManager.s_OnApplicationUpdate += OnUpdate;
            SDKManager.RealNameCallBack += OnRealNameCallBack;

            AddNetEvent();
            GlobalEvent.AddTypeEvent<CheckPayLimitEvent>(OnCheckPayLimit);

            //检测实名状态（但不触发实名）
            TestRealNameStatus();
            InitOnlineTimer();
        }
    }

    /// <summary>
    /// SDK 实名制认证回调
    /// </summary>
    /// <param name="info"></param>
    private void OnRealNameCallBack(RealNameData info)
    {
        Debug.Log("OnRealNameCallBack" + info.realNameStatus + " isAdult:" + info.isAdult);

        RealNameStatus = info.realNameStatus;
        isAdult = info.isAdult;
    }

    /// <summary>
    /// 接收到询问支付限制的事件
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnCheckPayLimit(CheckPayLimitEvent e, object[] args)
    {
        PayLimitType payLimitType = PayLimitType.None;//默认不需要实名认证，无限制
        if (openRealName)
        {
            if (RealNameStatus == RealNameStatus.NotRealName)
            {
                StartRealNameAttestation(); //自动开始实名制认证
                payLimitType = PayLimitType.NoRealName;
            }
            else if (RealNameStatus == RealNameStatus.IsRealName)
            {
                if (isAdult) //成年
                {
                    payLimitType = PayLimitType.None;
                }
                else //未成年
                {
                    if (CheckPayLimitBySDK(e.payAmount))
                    {
                        payLimitType = PayLimitType.ChildLimit;
                    }
                    else
                    {
                        payLimitType = PayLimitType.None;
                    }
                }
            }
            else if (RealNameStatus == RealNameStatus.NotNeed)
            {
                payLimitType = PayLimitType.None;//默认不需要实名认证，无限制
            }
        }

        Debug.LogWarning("OnCheckPayLimit====payLimitType==" + payLimitType);

        CheckPayLimitResultEvent.Dispatch(e.payAmount, payLimitType);
    }

    private void OnUpdate()
    {
        OnlineTimer();
    }

    #endregion

    #region 在线计时器
    float c_onlineTimer = 1 * 60;//当日游戏时间检测间隔    1分钟 * 60秒/分钟  

    float onlineTimer = 0;

    private void InitOnlineTimer()
    {
        ResetOnlineTimer();
    }

    /// <summary>
    /// 重置计时器
    /// </summary>
    private void ResetOnlineTimer()
    {
        onlineTimer = c_onlineTimer;
    }

    private void OnlineTimer()
    {
        if (RealNameStatus == RealNameStatus.NotNeed)
        {
            return;
        }

        if (RealNameStatus == RealNameStatus.IsRealName && isAdult == true)
        {
            return;
        }

        if (onlineTimer >= 0)
        {

            onlineTimer -= Time.deltaTime;
            if (onlineTimer < 0)
            {
                //询问服务器 是否超出体验时间或未成年是否超时
                AskServerOnlineTime();

                //重置计时器
                ResetOnlineTimer();
            }
        }
    }


    #endregion

    #region 实名制和未成年检测

    /// <summary>
    /// 检测实名总开关
    /// </summary>
    private void TestRealNameSwitch()
    {
        string l_openRealName = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_OpenRealName, "false");
        openRealName = (l_openRealName == "true");  //重打包工具控制总开关
        //上报服务器
    }

    /// <summary>
    /// 检测实名制状态（不触发实名制认证）
    /// </summary>
    private void TestRealNameStatus()
    {
        RealNameStatus = GetRealNameStatusFromSDK();

        isAdult = SDKManager.IsAdult();

        //上报服务器
        AskServerOnlineTime();
    }

    /// <summary>
    /// 询问服务器在线时间，触发检测
    /// </summary>
    private void AskServerOnlineTime()
    {
        Debug.LogWarning("AskServerOnlineTime" + RealNameStatus + isAdult);
        RequestRealNameState2Server.RequestRealName(RealNameStatus, isAdult);
    }

    #endregion

    #region 与服务器通信

    /// <summary>
    /// 添加服务器消息监听
    /// </summary>
    private void AddNetEvent()
    {
        GlobalEvent.AddTypeEvent<RequestRealNameState2Client>(OnRequestRealNameResult);
    }

    /// <summary>
    /// 询问实名制检测的回调
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnRequestRealNameResult(RequestRealNameState2Client e, object[] args)
    {
        RealNameLimitEvent.Dispatch(e.onlineTime,e.night,e.canPlay,e.realNameStatus,e.adult);
    }

    #endregion

    #region 未成年支付限制

    /// <summary>
    /// 检测是否支付限制
    /// </summary>
    /// <param 本次支付金额（分）="payAmont"></param>
    /// <returns></returns>
    public bool CheckPayLimitBySDK(int payAmont)
    {
        if (!openRealName)
        {
            return false;
        }

        return SDKManager.CheckPayLimit(payAmont);
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 获得当前实名制状态 （从缓存中）
    /// </summary>
    /// <returns></returns>
    private RealNameStatus GetRealNameStatus()
    {
        if (!openRealName)
        {
            return RealNameStatus.NotNeed;
        }
        else
        {
            return RealNameStatus;
        }
    }

    /// <summary>
    /// 从SDK 获取实名制状态，并缓存
    /// </summary>
    /// <returns></returns>
    private RealNameStatus GetRealNameStatusFromSDK()
    {
        if (!openRealName)
        {
            RealNameStatus = RealNameStatus.NotNeed;
        }
        else
        {
            RealNameStatus = SDKManager.GetRealNameType();
        }

        Debug.Log("GetRealNameStatusFromSDK :openRealName " + openRealName + " realNameStatus:" + RealNameStatus);

        return RealNameStatus;
    }
    #endregion
}
