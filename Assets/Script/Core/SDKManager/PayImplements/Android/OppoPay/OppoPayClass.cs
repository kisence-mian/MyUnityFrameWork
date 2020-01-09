using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppoPayClass : PayInterface
{
    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() {  RuntimePlatform.Android, RuntimePlatform.WindowsEditor};
    }
    public override StoreName GetStoreName()
    {
        return StoreName.OPPO;
    }
    public override void Pay(PayInfo l_payInfo)
    {

        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = true;
        payInfo.goodsId = l_payInfo.goodsID;
        payInfo.goodsType = l_payInfo.goodsType;
        payInfo.price = l_payInfo.price;
        payInfo.receipt = "";
        payInfo.storeName = StoreName.None;
        payInfo.goodsName = l_payInfo.goodsName;

        Debug.Log("DefaultIAPImplement.Pay :" + l_payInfo.goodsID);
        PayCallBack(payInfo);
    }
}
