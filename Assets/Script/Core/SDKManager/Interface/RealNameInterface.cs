using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RealNameInterface : SDKInterfaceBase
{

    public override void Init()
    {
        base.Init();
    }

    /// <summary>
    /// 实名制状态
    /// </summary>
    public virtual RealNameStatus GetRealNameType()
    {
        return RealNameStatus.NotNeed;
    }

    /// <summary>
    /// 是否成年
    /// </summary>
    /// <returns></returns>
    public virtual bool IsAdult()
    {
        return true;
    }

    /// <summary>
    /// 今日在线时长
    /// </summary>
    /// <returns></returns>
    public virtual int GetTodayOnlineTime()
    {
        return -1;
    }

    /// <summary>
    /// 开始实名制
    /// </summary>
    public virtual void StartRealNameAttestation()
    {

    }

    /// <summary>
    /// 检测支付是否受限
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckPayLimit(int payAmount)
    {
        return false;
    }

    /// <summary>
    /// 上报支付金额
    /// </summary>
    /// <param 支付金额="payAmount"></param>
    public virtual void LogPayAmount(int payAmount)
    {

    }

}


/// <summary>
/// 实名制状态
/// </summary>
public enum RealNameStatus
{
    IsRealName, //已经实名制
    NotRealName, //未实名制
    NotNeed,//不需要实名制
}