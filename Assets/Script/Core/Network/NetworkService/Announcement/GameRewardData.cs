using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GameRewardData
{
    /// <summary>
    /// 分类奖励的类型（如：金币，钻石，卡包）
    /// </summary>
    public string rewardType;
    /// <summary>
    /// 用于二级分类或具体的东西（如卡牌类型的挥砍卡牌），不需要则不填写
    /// </summary>
    public string key;
    /// <summary>
    /// 数量
    /// </summary>
    public int number;
  
}

