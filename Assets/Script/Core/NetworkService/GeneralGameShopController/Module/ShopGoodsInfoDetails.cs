using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ShopGoodsInfoDetails
{
    public bool canBuy = true;
    /// <summary>
    /// 物品返回是否能够购买的状态具体数值 见 ErrorCodeDefine
    /// </summary>
    public int goodsCanBuyStateCode;


    public String goodID;
    public String name;
    public String explain;
    public String iconName;
    /***
	 * 原价
	 */
    public float price;
    /// <summary>
    /// 最终价格，比如可能打折，那么这里就是打折后的价格
    /// </summary>
    public float nowPrice = -1;
    public int goodNum;
    /***
	 * 支付购买的货币类型
	 */
    public String payCoinType;

    public GoodsDiscountInfo discountInfo;


    //public GoodBuyRewardInfo rewardInfo;

    public GoodsBuyTimeLimitInfo timeLimitInfo;

    public GoodsBuyNumOfTimeLimitInfo numOfTimeLimitInfo;

    public bool getCanBuy()
    {
        return canBuy;
    }

    /// <summary>
    /// 获得取整的价格
    /// </summary>
    /// <returns></returns>
    public int GetNowPriceInt()
    {
        return (int)nowPrice;
    }

    public void setCanBuy(bool canBuy)
    {
        this.canBuy = canBuy;
    }

    public String getGoodID()
    {
        return goodID;
    }

    public void setGoodID(String goodID)
    {
        this.goodID = goodID;
    }

    public String getName()
    {
        return name;
    }

    public void setName(String name)
    {
        this.name = name;
    }

    public String getExplain()
    {
        return explain;
    }

    public void setExplain(String explain)
    {
        this.explain = explain;
    }

    public String getIconName()
    {
        return iconName;
    }

    public void setIconName(String iconName)
    {
        this.iconName = iconName;
    }

    public float getPrice()
    {
        return price;
    }

    public void setPrice(float price)
    {
        this.price = price;
    }

    public float getNowPrice()
    {
        return nowPrice;
    }

    public void setNowPrice(float nowPrice)
    {
        this.nowPrice = nowPrice;
    }

    public int getGoodNum()
    {
        return goodNum;
    }

    public void setGoodNum(int goodNum)
    {
        this.goodNum = goodNum;
    }

    public String getPayCoinType()
    {
        return payCoinType;
    }

    public void setPayCoinType(String payCoinType)
    {
        this.payCoinType = payCoinType;
    }

    public GoodsDiscountInfo getDiscountInfo()
    {
        return discountInfo;
    }

    public void setDiscountInfo(GoodsDiscountInfo discountInfo)
    {
        this.discountInfo = discountInfo;
    }

    public GoodsBuyTimeLimitInfo getTimeLimitInfo()
    {
        return timeLimitInfo;
    }

    public void setTimeLimitInfo(GoodsBuyTimeLimitInfo timeLimitInfo)
    {
        this.timeLimitInfo = timeLimitInfo;
    }

    public GoodsBuyNumOfTimeLimitInfo getNumOfTimeLimitInfo()
    {
        return numOfTimeLimitInfo;
    }

    public void setNumOfTimeLimitInfo(GoodsBuyNumOfTimeLimitInfo numOfTimeLimitInfo)
    {
        this.numOfTimeLimitInfo = numOfTimeLimitInfo;
    }
}
