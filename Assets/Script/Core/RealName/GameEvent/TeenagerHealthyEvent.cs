using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实名制检测的结果
/// </summary>
public class RealNameLimitEvent
{
    /// <summary>
    /// 限制类型
    /// </summary>
    public RealNameLimitType realNameLimitType;

    /// <summary>
    /// 实名制信息
    /// </summary>
    public RealNameData realNameData;

    /// <summary>
    /// 描述
    /// </summary>
    public string describ = "";

    public RealNameLimitEvent(RealNameData l_realNameData,string l_describ = "")
    {
        realNameData = l_realNameData;
        describ = l_describ;
        realNameLimitType = GetLimitResult(realNameData);
    }

    private RealNameLimitType GetLimitResult(RealNameData l_realNameData)
    {
        if (!l_realNameData.canPlay) //禁止继续游玩
        {
            if (l_realNameData.realNameStatus == RealNameStatus.NotRealName)
            {
                if (string.IsNullOrEmpty(describ))
                {
                    describ = "根据规定，未完成实名制的用户，最多体验本游戏1小时，继续游玩请完成实名制认证";
                }
                //未实名制，游戏体验上限1小时
                return RealNameLimitType.NoRealNameMaxTimeLimit;
            }
            else if (!l_realNameData.isAdult)
            {
                if (l_realNameData.isNight)
                {
                    if (string.IsNullOrEmpty(describ))
                    {
                        describ = "根据规定，22时至次日8时，不得对未成年提供游戏服务，请合理安排作息";
                    }
                    //深夜， 22时至次日8时 不得为未成年人提供游戏服务
                    return RealNameLimitType.ChildNightLimit;
                }
                else
                {
                    if (string.IsNullOrEmpty(describ))
                    {
                        describ = "根据规定，未成年人法定节假日每日在线时长不得超过3小时，其他日期1.5小时，请合理安排作息";
                    }
                    //未成年人，每日在线时长不得超过x小时（法定节假日3小时，其他日期1.5小时）
                    return RealNameLimitType.ChildTimeLimit;

                }
            }
            else
            {
                Debug.LogError("GetLimitResult error： adult:" + l_realNameData.isAdult);
                return RealNameLimitType.NoLimit;
            }
        }
        else
        {
            return RealNameLimitType.NoLimit;//可以玩，表示未受限制
        }
    }

    static public void Dispatch(int l_onlineTime, bool l_isNight, bool l_canPlay, RealNameStatus l_realNameStatus,bool l_isAdult)
    {
        RealNameData realNameData = new RealNameData(l_canPlay, l_realNameStatus, l_isAdult, l_onlineTime, l_isNight);
        GlobalEvent.DispatchTypeEvent(new RealNameLimitEvent(realNameData));
    }

}

/// <summary>
/// 实名制检测结果
/// </summary>
public enum RealNameLimitType
{
    NoLimit,//无限制
    NoRealNameMaxTimeLimit,//未实名制，达到最大时间限制
    ChildNightLimit,//未成年，深夜限制（22时至次日8时 不得为未成年人提供游戏服务）
    ChildTimeLimit,//未成年，每日游戏时长限制
}

/// <summary>
/// 实名制信息
/// </summary>
public class RealNameData
{
    /// <summary>
    /// 是否可以继续游玩
    /// </summary>
    public bool canPlay = true;
    /// <summary>
    /// 实名制状态
    /// </summary>
    public RealNameStatus realNameStatus = RealNameStatus.NotNeed;
    /// <summary>
    /// 是否成年
    /// </summary>
    public bool isAdult = true;
    /// <summary>
    /// 今日在线时长
    /// </summary>
    public int onlineTime = 0;
    /// <summary>
    /// 是否是深夜
    /// </summary>
    public bool isNight = false;

    public RealNameData()
    {
    }

    public RealNameData(bool canPlay, RealNameStatus realNameStatus, bool isAdult, int onlineTime, bool isNight)
    {
        this.realNameStatus = realNameStatus;
        this.onlineTime = onlineTime;
        this.canPlay = canPlay;
        this.isNight = isNight;
        this.isAdult = isAdult;
    }
}
