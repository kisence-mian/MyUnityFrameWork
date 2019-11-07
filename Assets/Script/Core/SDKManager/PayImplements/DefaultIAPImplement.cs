using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DefaultIAPImplement : PayInterface
{

    public override void Pay(string goodsID, string tag, FrameWork.SDKManager.GoodsType goodsType = FrameWork.SDKManager.GoodsType.NORMAL, string orderID = null)
    {

        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = true;
        payInfo.goodsId = goodsID;
        payInfo.goodsType = goodsType;
        payInfo.receipt = "";
        payInfo.storeName =  StoreName.None;

        Debug.Log("DefaultIAPImplement.Pay :" + goodsID);
        PayCallBack(payInfo);
    }
}

