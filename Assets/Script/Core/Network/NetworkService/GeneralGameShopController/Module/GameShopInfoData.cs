using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GameShopInfoData
{

    public String shopType;

    public List<ShopGoodsInfoDetails> shopGoodsInfoDetails = new List<ShopGoodsInfoDetails>();

    public String getShopType()
    {
        return shopType;
    }

    public void setShopType(String shopType)
    {
        this.shopType = shopType;
    }

    public List<ShopGoodsInfoDetails> getShopGoodsInfoDetails()
    {
        return shopGoodsInfoDetails;
    }

    public void setShopGoodsInfoDetails(List<ShopGoodsInfoDetails> shopGoodsInfoDetails)
    {
        this.shopGoodsInfoDetails = shopGoodsInfoDetails;
    }
    //============

    public ShopGoodsInfoDetails GetGoodsInfo(String goodID)
    {
        foreach (ShopGoodsInfoDetails info in shopGoodsInfoDetails)
        {
            if (info.goodID==(goodID))
            {
                return info;
            }
        }
        return null;
    }
}
