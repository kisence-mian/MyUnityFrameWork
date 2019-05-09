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
        ResendMessageManager.AddResendMessage(msg, typeof(StoreBuyGoods2Client).Name,(resMsg)=>
        {
            StoreBuyGoods2Client e = (StoreBuyGoods2Client)resMsg;
            PaymentVerificationManager.OnVerificationResult(e.code == 0, e.id, e.repeatReceipt);
        });
       // JsonMessageProcessingController.SendMessage(msg);
        Debug.Log(" 当前游戏服务器验证");
    }

    public void Init()
    {
       // GlobalEvent.AddTypeEvent<StoreBuyGoods2Client>(OnUserPaymentVerification);
        Debug.Log(" 当前游戏服务器验证 .Init");
    }

    //private void OnUserPaymentVerification(StoreBuyGoods2Client e, object[] args)
    //{
    //    PaymentVerificationManager.OnVerificationResult(e.code==0, e.id,e.repeatReceipt);
    //}
}

