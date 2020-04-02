using System;
using System.Collections.Generic;

public class AnnouncementContent2Client : MessageClassInterface
{

    public string id;
    public string titleName;
    public string content;
    public List<GameRewardData> rewardDatas = new List<GameRewardData>();
    /// <summary>
    /// 用来分辨消息用途是公告，还是排名奖励等
    /// </summary>
    public string useTag;
    public void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}



