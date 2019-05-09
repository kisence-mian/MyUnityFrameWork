using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WXPayClass : PayInterface
{

    GameObject androidListener;
    public override RuntimePlatform GetPlatform()
    {
        return  RuntimePlatform.Android;
    }
    public override StoreName GetStoreName()
    {
        return StoreName.WX;
    }
    public override void Init()
    {
        Debug.LogWarning("=========WXPayClass Init===========");
        WXListener.Instance.GetComponent<WXListener>().wXPayClass = this;
        GlobalEvent.AddTypeEvent<PrePay2Client>(OnPrePay);
    }

    /// <summary>
    /// 获得预支付订单
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnPrePay(PrePay2Client e, object[] args)
    {

        WXListener.Instance.GetComponent<WXListener>().GoodId = e.goodsID ;

        Debug.LogWarning("OnPrePay=========：" + e.prepay_id + "=partnerId==");
        DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        string nonceStr = "Random" +UnityEngine.Random.Range(10000, 99999).ToString();
        string timeStamp = (new DateTime(DateTime.UtcNow.Ticks - dt1970.Ticks).AddHours(8).Ticks / 10000000).ToString();
        string stringA = "appid=wx3ce30b3054987098&" + "nonceStr=" + nonceStr + "&packageValue=Sign=WXPay&" + "partnerId=1526756671&" + "prepayId=" + e.prepay_id + "&timeStamp=" + timeStamp;
        string stringSignTemp = stringA + "&key=a8f73a2a5ecfafab1ea80515ef0efbad";
        string sign = MD5Tool.GetMD5FromString(stringSignTemp);

        WXListener.Instance.GetComponent<WXListener>().Mch_orderID = e.mch_orderID;

        IndentListener(e.prepay_id,nonceStr, timeStamp, sign);
    }

    /// <summary>
    /// 统一支付入口
    /// </summary>
    /// <param name="goodsID"></param>
    /// <param name="tag"></param>
    /// <param name="goodsType"></param>
    /// <param name="orderID"></param>
    public override void Pay(string goodsID, string tag, FrameWork.SDKManager.GoodsType goodsType = FrameWork.SDKManager.GoodsType.NORMAL, string orderID = null)
    {
        Debug.LogWarning("send WXpay----message-----" + goodsID);
        //给服务器发消息1
        PrePay2Service.SendPrePayMsg(StoreName.WX, goodsID);
    }


    /// <summary>
    /// 消息1 的监听， 获得订单信息，然后调支付sdk
    /// </summary>
    private void IndentListener(string prepayid, string nonceStr ,string timeStamp , string sign)
    {
        //Debug.LogWarning("get WXpay----message-----" );
        //Debug.LogWarning("prepayid====" + prepayid);
        //Debug.LogWarning("nonceStr====" + nonceStr);
        //Debug.LogWarning("timeStamp====" + timeStamp);
        //Debug.LogWarning("sign====" + sign);

        //WXPay  参数： prepayid(交易会话ID) ， nonceStr(随机字符串)， timeStamp(北京时间戳)， sign (签名)
        WXLoginSDKClass.GetCurrentAndroidJavaObject().Call("WXPay", prepayid, nonceStr, timeStamp,sign);
        //GetCurrentAndroidJavaObject().Call("WXPay2", prepayid);
    }



    /// <summary>
    /// 从sdk返回支付结果
    /// </summary>
    /// <param name="result"></param>
    /// <param name="goodID"></param>
    /// <param name="Mch_orderID"></param>
    public void SetPayResult(string result,string goodID,string Mch_orderID)
    {
        Debug.LogWarning("wxPay Result======" + result);

        OnPayInfo payInfo = new OnPayInfo();
        payInfo.isSuccess = (result == "0");
        payInfo.goodsId = goodID;
        //payInfo.goodsType = DataGenerateManager<StoreDataGenerate>.GetData(goodID).m_ProductType;
        payInfo.receipt = Mch_orderID;
        payInfo.storeName = StoreName.WX;

        PayCallBack(payInfo);
        
    }
}
