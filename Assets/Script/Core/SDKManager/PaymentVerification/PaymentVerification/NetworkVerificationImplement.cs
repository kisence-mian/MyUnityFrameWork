using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 当前游戏服务器验证的具体实现
/// </summary>
public class NetworkVerificationImplement : PaymentVerificationInterface
{
    public void CheckRecipe(OnPayInfo info)
    {
        StoreBuyGoods2Server msg = new StoreBuyGoods2Server();
        msg.storeName = info.storeName;
        msg.receipt = info.receipt;
        msg.id = info.goodsId;
        SetBuyResendMessage(msg, false);
        Debug.LogWarning(info.storeName);
       // JsonMessageProcessingController.SendMessage(msg);
        Debug.Log(" 当前游戏服务器验证");
    }

    public void Init()
    {
        // GlobalEvent.AddTypeEvent<StoreBuyGoods2Client>(OnUserPaymentVerification);
        //Debug.Log(" 当前游戏服务器验证 .Init");
    }

   public static void SetBuyResendMessage(StoreBuyGoods2Server msg, bool noSend)
    {
        ResendMessageManager.AddResendMessage(msg, typeof(StoreBuyGoods2Client).Name, (resMsg) =>
        {

            StoreBuyGoods2Client e = (StoreBuyGoods2Client)resMsg;
            Debug.LogWarning("NetworkVerificationImplement   StoreBuyGoods2Client=========" + e.id);
            PaymentVerificationManager.OnVerificationResult(e.code, e.id, e.repeatReceipt, e.receipt);
        },noSend);
    }
}

