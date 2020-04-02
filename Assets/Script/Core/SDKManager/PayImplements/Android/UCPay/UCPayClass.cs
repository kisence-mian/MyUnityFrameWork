using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCPayClass : PayInterface
{
    PayInfo payInfo;
    string goodsID;
    float price;
    //string mch_orderID;
    string userID;

    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.Android, RuntimePlatform.WindowsEditor };
    }
    public override StoreName GetStoreName()
    {
        return StoreName.UC;
    }

    public override void Init()
    {
        if (SDKManager.IncludeThePayPlatform(StoreName.UC))
        {

            Debug.Log("=========UC PayClass Init===========");
            GlobalEvent.AddTypeEvent<PrePay2Client>(OnPrePay);
            StorePayController.OnPayCallBack += OnPayResultCallBack;
        }
    }

    //正常订单回调
    private void OnPayResultCallBack(PayResult result)
    {
        payResponse = true;
    }

    /// <summary>
    /// 统一支付入口
    /// </summary>
    /// <param name="goodsID"></param>
    /// <param name="tag"></param>
    /// <param name="goodsType"></param>
    /// <param name="orderID"></param>
    public override void Pay(PayInfo payInfo)
    {
        this.payInfo = payInfo;
        userID = payInfo.userID;
        this.goodsID = payInfo.goodsID;
        price = payInfo.price;
        Debug.Log("send UC----message-----" + goodsID);
        //给服务器发消息
        PrePay2Service.SendPrePayMsg(StoreName.UC, goodsID);
    }

    /// <summary>
    /// 获得预支付订单
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnPrePay(PrePay2Client e, object[] args)
    {
        if (e.storeName != StoreName.UC)
        {
            return;
        }

        Debug.LogWarning("OnPrePay=========：" + e.prepay_id + "=partnerId==");

        //华为的支付重发意义不大
        //OnPayInfo onPayInfo = new OnPayInfo();
        //onPayInfo.isSuccess = true;
        //onPayInfo.goodsId = e.goodsID;
        //onPayInfo.storeName = StoreName.HuaWei;
        //onPayInfo.receipt = e.mch_orderID;
        //onPayInfo.price = price;
        //PayReSend.Instance.AddPrePayID(onPayInfo);

        StartLongTimeNoResponse();

        //IndentListener(e.goodsID, e.mch_orderID, e.prepay_id, price);

        PayInfo payInfo = new PayInfo(
            e.goodsID,
            GetGoodsInfo(goodsID).localizedTitle,
            "",
            FrameWork.SDKManager.GoodsType.NORMAL,
            e.mch_orderID,
            price,
            GetGoodsInfo(goodsID).isoCurrencyCode, GetUserID(), e.storeName.ToString());
        payInfo.prepay_id = e.prepay_id;


        SDKManagerNew.Pay(payInfo);
    }

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
                 PayCallBack(new OnPayInfo(payInfo,false, StoreName.UC));
             }
         });
    }

    ///// <summary>
    ///// 消息1 的监听， 获得订单信息，然后调支付sdk
    ///// </summary>
    //private void IndentListener(string goodID, string mch_orderID, string prepay_id, float price)
    //{
    //    PayInfo payInfo = new PayInfo(
    //        goodID,
    //        GetGoodsInfo(goodsID).localizedTitle,
    //        prepay_id,
    //        FrameWork.SDKManager.GoodsType.NORMAL,
    //        mch_orderID,
    //        price,
    //        GetGoodsInfo(goodsID).isoCurrencyCode, GetUserID());

    //    SDKManagerNew.Pay(StoreName.UC.ToString(), payInfo);
    //}

    public override string GetUserID()
    {
        return userID;
    }

}
