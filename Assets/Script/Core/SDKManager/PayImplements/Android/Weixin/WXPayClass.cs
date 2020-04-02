using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WXPayClass : PayInterface
{
    PayInfo payInfo;
    public string appid;
    public string mchID;
    public string appSecret;
    string goodsID;
    string mch_orderID;
    GameObject androidListener;
    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.Android, RuntimePlatform.WindowsEditor};
    }
    public override StoreName GetStoreName()
    {
        return StoreName.WX;
    }
    public override void Init()
    {
        if (SDKManager.IncludeThePayPlatform(StoreName.WX))
        {
            Debug.LogWarning("=========WXPayClass Init===========");
            //SDKManagerNew.OnPayCallBack += SetPayResult;
            GlobalEvent.AddTypeEvent<PrePay2Client>(OnPrePay);

            appid = SDKManager.GetProperties("WeiXin", "AppID", appid);
            mchID = SDKManager.GetProperties("WeiXin", "MchID", mchID);
            appSecret = SDKManager.GetProperties("WeiXin", "AppSecret", appSecret);
        }


        //if ((StoreName)Enum.Parse(typeof(StoreName), SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_StoreName, "None"))
        //    == StoreName.WX)
        //{

        //}
    }

    /// <summary>
    /// 获得预支付订单
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnPrePay(PrePay2Client e, object[] args)
    {

        if (e.storeName != StoreName.WX)
        {
            return;
        }

        Debug.LogWarning("OnPrePay=========：" + e.prepay_id + "=partnerId==");
        DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        string nonceStr = "Random" +UnityEngine.Random.Range(10000, 99999).ToString();
        string timeStamp = (new DateTime(DateTime.UtcNow.Ticks - dt1970.Ticks).AddHours(8).Ticks / 10000000).ToString();
        string stringA = "appid="+ appid + "&" + "nonceStr=" + nonceStr + "&packageValue=Sign=WXPay&" + "partnerId=" + mchID + "&" + "prepayId=" + e.prepay_id + "&timeStamp=" + timeStamp;
        string stringSignTemp = stringA + "&key=" + appSecret;
        string sign = MD5Tool.GetMD5FromString(stringSignTemp);

        OnPayInfo onPayInfo = new OnPayInfo();
        onPayInfo.isSuccess = true;
        onPayInfo.goodsId = e.goodsID;
        onPayInfo.storeName = StoreName.WX;
        onPayInfo.receipt = e.mch_orderID;
        onPayInfo.price = payInfo.price;
        onPayInfo.goodsName = payInfo.goodsName;
        PayReSend.Instance.AddPrePayID(onPayInfo);
        IndentListener(e.goodsID, e.mch_orderID, e.prepay_id, nonceStr, timeStamp, sign, payInfo.price);
    }

    /// <summary>
    /// 统一支付入口
    /// </summary>
    /// <param name="goodsID"></param>
    /// <param name="tag"></param>
    /// <param name="goodsType"></param>
    /// <param name="orderID"></param>
    public override void Pay(PayInfo l_payInfo)
    {
        payInfo = l_payInfo;
        this.goodsID = payInfo.goodsID;
        //this.price = payInfo.price;
        Debug.LogWarning("send WXpay----message-----" + goodsID + "price" + l_payInfo.price);
        //给服务器发消息1
        PrePay2Service.SendPrePayMsg(StoreName.WX, l_payInfo.goodsID);
    }


    /// <summary>
    /// 消息1 的监听， 获得订单信息，然后调支付sdk
    /// </summary>
    private void IndentListener(string goodID,string mch_orderID,string prepay_id, string nonceStr ,string timeStamp , string sign,float price)
    {
        this.mch_orderID = mch_orderID;

        string tag = mch_orderID;

        PayInfo l_payInfo = new PayInfo(goodID, payInfo.goodsName, tag, FrameWork.SDKManager.GoodsType.NORMAL, prepay_id, price, GetGoodsInfo(goodsID).isoCurrencyCode,GetUserID(), "WeiXin");

        SDKManagerNew.Pay(l_payInfo);
    }

    /// <summary>
    /// 从sdk返回支付结果
    /// </summary>
    /// <param name="result"></param>
    /// <param name="goodID"></param>
    /// <param name="Mch_orderID"></param>
    public void SetPayResult(OnPayInfo info)
    {

        if (info.storeName != StoreName.WX)
        {
            return;
        }

        Debug.LogWarning("wxPay Result======" + info.isSuccess);


        var goodsInfo =  GetGoodsInfo(goodsID);

        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = info.isSuccess;
        payInfo.goodsId = goodsID;
        payInfo.orderID = mch_orderID;
        payInfo.goodsType = GetGoodType(goodsID);
        payInfo.storeName = StoreName.WX;

        payInfo.currency = goodsInfo.isoCurrencyCode;
        payInfo.price = (float)goodsInfo.localizedPrice;

        Debug.Log("SetPayResult " + payInfo.price + " goodsID " + goodsID);

        payInfo.receipt = mch_orderID;
        
        PayCallBack(payInfo);
    }

    public override void ConfirmPay(string goodsID, string mch_orderID, string StoreName)
    {
        PayReSend.Instance.ClearPrePayID(mch_orderID);
    }

}
