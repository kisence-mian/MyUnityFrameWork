using UnityEngine;
using System.Collections;

public class RequestRealNameState2Client : MessageClassInterface
{
    public int code;
    /// <summary>
    /// 玩家游戏总时长 （分）
    /// </summary>
    public long allPlayTime = 0;
    /// <summary>
    /// 今日在线时长(秒)
    /// </summary>
    public int onlineTime = 0;

    /// <summary>
    /// 是否可以继续游玩
    /// </summary>
    public bool canPlay = true;

    /// <summary>
    /// 是否成年
    /// </summary>
    public bool adult = true;

    /// <summary>
    /// 是否是深夜（22时至次日8时 不得为未成年人提供游戏服务）
    /// </summary>
    public bool night = false;

    /// <summary>
    /// 实名制状态
    /// </summary>
    public RealNameStatus realNameStatus = RealNameStatus.NotNeed;


    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}
