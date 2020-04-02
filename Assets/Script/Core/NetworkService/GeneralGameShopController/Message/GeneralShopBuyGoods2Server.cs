using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GeneralShopBuyGoods2Server
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

    public String getGoodsID()
    {
        return goodsID;
    }

    public void setGoodsID(String goodsID)
    {
        this.goodsID = goodsID;
    }

    public int getBuyNum()
    {
        return buyNum;
    }

    public void setBuyNum(int buyNum)
    {
        this.buyNum = buyNum;
    }

    public String getShopType()
    {
        return shopType;
    }

    public void setShopType(String shopType)
    {
        this.shopType = shopType;
    }

}
