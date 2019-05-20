using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


 public   class RemeedCodeUse2Client : CodeMessageBase
{
    public List<GameRewardData> rewardDatas = new List<GameRewardData>();
    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }
}