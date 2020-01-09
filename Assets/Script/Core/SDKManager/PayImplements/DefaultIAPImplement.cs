using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DefaultIAPImplement : PayInterface
{

    public override void Pay(PayInfo l_payInfo)
    {

        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = true;
        payInfo.goodsId = l_payInfo.goodsID;
        payInfo.goodsType = l_payInfo.goodsType;
        payInfo.receipt = "";
        payInfo.storeName = StoreName.None;

        Debug.Log("DefaultIAPImplement.Pay :" + payInfo.goodsId);
        PayReSend.Instance.AddPrePayID(payInfo);
        PayCallBack(payInfo);
    }
}

