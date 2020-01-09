using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuaWeiPayClass : PayInterface
{
    string goodsName;
    string goodsID;
    float price;
    string mch_orderID;
    string userID;
    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.Android, RuntimePlatform.WindowsEditor };
    }
    public override StoreName GetStoreName()
    {
        return StoreName.HuaWei;
    }

    public override void Init()
    {
        GlobalEvent.AddTypeEvent<PrePay2Client>(OnPrePay);
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
        userID = payInfo.userID;
        this.goodsID = payInfo.goodsID;
        this.goodsName = payInfo.goodsName;
        price = payInfo.price;
        Debug.Log("send HuaWeiPay----message-----" + goodsID);
        //给服务器发消息
        PrePay2Service.SendPrePayMsg(StoreName.HuaWei, goodsID);
    }

    /// <summary>
    /// 获得预支付订单
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnPrePay(PrePay2Client e, object[] args)
    {
        if (e.storeName != StoreName.HuaWei)
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
        IndentListener(e.goodsID, e.mch_orderID, e.prepay_id, price);
    }

    /// <summary>
    /// 消息1 的监听， 获得订单信息，然后调支付sdk
    /// </summary>
    private void IndentListener(string goodID, string mch_orderID, string prepay_id, float price)
    {
        PayInfo payInfo = new PayInfo(
            goodID,
            GetGoodsInfo(goodsID).localizedTitle, 
            prepay_id, 
            FrameWork.SDKManager.GoodsType.NORMAL,
            mch_orderID,
            price, 
            GetGoodsInfo(goodsID).isoCurrencyCode,GetUserID());

        SDKManagerNew.Pay(StoreName.HuaWei.ToString(), payInfo);
    }

    public override string GetUserID()
    {
        return userID;
    }

}
