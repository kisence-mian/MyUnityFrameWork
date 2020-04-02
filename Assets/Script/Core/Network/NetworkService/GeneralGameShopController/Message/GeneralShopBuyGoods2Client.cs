using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 购买返回消息
/// </summary>
public class GeneralShopBuyGoods2Client : CodeMessageBase
{
    /// <summary>
    /// 商店Type
    /// </summary>
    public String shopType;
    /// <summary>
    /// 物品ID
    /// </summary>
    public String goodsID;
    /// <summary>
    /// 购买数量
    /// </summary>
    public int buyNum;

    public override void DispatchMessage()
    {
        GlobalEvent.DispatchTypeEvent(this);
    }


}

