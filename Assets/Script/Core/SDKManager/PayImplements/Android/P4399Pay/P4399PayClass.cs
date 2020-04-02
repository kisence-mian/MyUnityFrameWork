using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P4399PayClass : PayInterface
{
    PayInfo payInfo;
    /// <summary>  
    /// 是否得到订单的支付响应
    /// </summary>
    bool payResponse = false;

        /// <summary>
    /// 长时间未响应
    /// </summary>
    private void StartLongTimeNoResponse()
    {
        payResponse = false;

        Debug.LogWarning("======StartLongTimeNoResponse=====  start  ===" + Time.timeSinceLevelLoad);

        Timer.DelayCallBack(5, (o) =>
         {
             Debug.LogWarning("======StartLongTimeNoResponse=====  end  ===" + payResponse+"=============" + Time.timeSinceLevelLoad);

             if (!payResponse)
             {
                 PayCallBack(new OnPayInfo(payInfo,false, StoreName.m4399));
             }
         });
    }


    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.Android, RuntimePlatform.WindowsEditor };
    }
    public override StoreName GetStoreName()
    {
        return StoreName.m4399;
    }

    public override void Init()
    {
        if (SDKManager.IncludeThePayPlatform(StoreName.m4399))
        {
            Debug.Log("=========4399 PayClass Init===========");
            StorePayController.OnPayCallBack += OnPayResultCallBack;
        }
    }


    public override void Pay(PayInfo l_payInfo)
    {
        StartLongTimeNoResponse();

        payInfo = l_payInfo;

        Debug.Log("DefaultIAPImplement.Pay :" + l_payInfo.goodsID);
        SDKManagerNew.Pay( l_payInfo);
    }

    //正常订单回调
    private void OnPayResultCallBack(PayResult result)
    {
        payResponse = true;
    }
}