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
    static Dictionary<string, StoreBuyGoods2Server> goodsPayInfo= new Dictionary<string, StoreBuyGoods2Server>(); //所有发送服务器的订单详情 key = receipt


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
        SaveGoodsPayInfo(msg.receipt, msg);
        ResendMessageManager.AddResendMessage(msg, typeof(StoreBuyGoods2Client).Name, (resMsg) =>
        {
            StoreBuyGoods2Client e = (StoreBuyGoods2Client)resMsg;

            StoreName storeName = GetGoodsPayInfo(e.receipt).storeName;

            Debug.LogWarning("NetworkVerificationImplement   StoreBuyGoods2Client=========" + e.id + " storeName:" + storeName);
            
            PaymentVerificationManager.OnVerificationResult(e.code, e.id, e.repeatReceipt, e.receipt,null, storeName);
        },noSend);
    }


    /// <summary>
    /// 存储发送的消息
    /// </summary>
    /// <param name="receipt"></param>
    /// <param name="msg"></param>
    static private void SaveGoodsPayInfo(string receipt, StoreBuyGoods2Server msg)
    {
        if (goodsPayInfo.ContainsKey(receipt))
        {
            Debug.LogError("Repeat GoodsPayInfo:" + receipt);
        }
        else
        {
            goodsPayInfo.Add(receipt, msg);
        }
    }

    /// <summary>
    /// 查询之前保存的订单信息
    /// </summary>
    /// <param name="receipt"></param>
    /// <returns></returns>
    static private StoreBuyGoods2Server GetGoodsPayInfo(string receipt)
    {
        if (goodsPayInfo.ContainsKey(receipt))
        {
            return goodsPayInfo[receipt];
        }
        else
        {
            Debug.LogError("No Found GoodsPayInfo:" + receipt);
            return new StoreBuyGoods2Server();
        }
    }
}

