using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class vivoPayClass : PayInterface
{
    public string appid;
    public string mchID;
    public string appSecret;
    string goodsID;
    float price;
    //string mch_orderID;
    GameObject androidListener;
    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.Android, RuntimePlatform.WindowsEditor};
    }
    public override StoreName GetStoreName()
    {
        return StoreName.VIVO;
    }
    public override void Init()
    {
        if (SDKManager.IncludeThePayPlatform(StoreName.VIVO))
        {
            Debug.LogWarning("=========vivoPayClass Init===========");
            //SDKManagerNew.OnPayCallBack += SetPayResult;
            GlobalEvent.AddTypeEvent<PrePay2Client>(OnPrePay);

            appid = SDKManager.GetProperties("vivo", "AppID", appid);
            mchID = SDKManager.GetProperties("vivo", "MchID", mchID);
            appSecret = SDKManager.GetProperties("vivo", "AppSecret", appSecret);
        }
    }

    /// <summary>
    /// 获得预支付订单
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnPrePay(PrePay2Client e, object[] args)
    {

        if (e.storeName != StoreName.VIVO)
        {
            return;
        }

        Debug.LogWarning("OnPrePay=========：" + e.prepay_id + "=partnerId==");

        OnPayInfo onPayInfo = new OnPayInfo();
        onPayInfo.isSuccess = true;
        onPayInfo.goodsId = e.goodsID;
        onPayInfo.storeName = StoreName.VIVO;
        onPayInfo.receipt = e.mch_orderID;
        onPayInfo.price = price;
        PayReSend.Instance.AddPrePayID(onPayInfo);
        //IndentListener(e.goodsID,e.mch_orderID, e.prepay_id, onPayInfo.price);


        PayInfo payInfo = new PayInfo(e.goodsID, GetGoodsInfo(e.goodsID).localizedTitle, "", FrameWork.SDKManager.GoodsType.NORMAL, e.mch_orderID, price, GetGoodsInfo(goodsID).isoCurrencyCode, GetUserID(), e.storeName.ToString());
        payInfo.prepay_id = e.prepay_id;
        SDKManagerNew.Pay(payInfo);


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
        this.goodsID = payInfo.goodsID;
        price = payInfo.price;
        Debug.LogWarning("send vivopay----message-----" + goodsID);
        //给服务器发消息1
        PrePay2Service.SendPrePayMsg(StoreName.VIVO, goodsID);
    }


    ///// <summary>
    ///// 消息1 的监听， 获得订单信息，然后调支付sdk
    ///// </summary>
    //private void IndentListener(string goodID,string mch_orderID,string prepay_id,float price)
    //{
    //    this.mch_orderID = mch_orderID;

    //    PayInfo payInfo = new PayInfo(goodID, GetGoodsInfo(goodID).localizedTitle, prepay_id, FrameWork.SDKManager.GoodsType.NORMAL, mch_orderID, price, GetGoodsInfo(goodsID).isoCurrencyCode,GetUserID());
    //    SDKManagerNew.Pay(StoreName.VIVO.ToString(), payInfo);
    //}

    /// <summary>
    /// 从sdk返回支付结果
    /// </summary>
    /// <param name="result"></param>
    /// <param name="goodID"></param>
    /// <param name="Mch_orderID"></param>
    public void SetPayResult(OnPayInfo info)
    {

        if (info.storeName != StoreName.VIVO)
        {
            return;
        }

        Debug.LogWarning("vivoPay Result======" + info.isSuccess);


        var goodsInfo =  GetGoodsInfo(goodsID);

        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = info.isSuccess;
        payInfo.goodsId = goodsID;
        payInfo.orderID = info.orderID;
        payInfo.goodsType = GetGoodType(goodsID);
        payInfo.storeName = StoreName.VIVO;

        payInfo.currency = goodsInfo.isoCurrencyCode;
        payInfo.price = (float)goodsInfo.localizedPrice;

        Debug.Log("SetPayResult " + payInfo.price + " goodsID " + goodsID);

        payInfo.receipt = info.orderID;
        
        PayCallBack(payInfo);
    }

    public override void ConfirmPay(string goodsID, string mch_orderID, string StoreName)
    {
        PayReSend.Instance.ClearPrePayID(mch_orderID);
    }

}
