using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PublicPayClass : PayInterface
{
    PayInfo payInfo;
    //string goodsID;
    //string mch_orderID;
    GameObject androidListener;
    StoreName storeName = StoreName.None;
    string userID;

    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.Android, RuntimePlatform.WindowsEditor};
    }
    public override StoreName GetStoreName()
    {
        return storeName;
    }

    public override void Init()
    {
        m_SDKName = "PublicPayClass";
        //storeName =(StoreName)Enum.Parse( typeof(StoreName),SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_StoreName, "None"));
        
        //有其他的payClass符合就不启动public pay
        if(!SDKManager.GetHasPayService(storeName.ToString()))
        {
            GlobalEvent.AddTypeEvent<PrePay2Client>(OnPrePay);

            //Debug.Log("PublicPayClass Init m_SDKName:>" + m_SDKName + "<");
        }

        //GlobalEvent.DispatchEvent("Fight",)

        GlobalEvent.DispatchTypeEvent<InputUIOnClickEvent>(null);

        GlobalEvent.AddTypeEvent<InputUIOnClickEvent>((e,objs)=> {

        });

        SDKManager.GoodsInfoCallBack += OnGoodsInfoCallBack;
        StorePayController.OnPayCallBack += OnPayResultCallBack;
    }

    /// <summary>
    /// 从SDK获取商品信息的回调
    /// </summary>
    /// <param name="info"></param>
    private void OnGoodsInfoCallBack(GoodsInfoFromSDK info)
    {
        for (int i = 0; i < productDefinitions.Count; i++)
        {
            if (productDefinitions[i].goodsID == info.goodsId)
            {
                Debug.LogWarning("GetGoodsInfoFromSDK =====id:" + info.goodsId + "=====price:" + info.localizedPriceString);
                productDefinitions[i].localizedPriceString = info.localizedPriceString;
                return;
            }
        }
    }

    public override void ExtraInit(string tag)
    {
        base.ExtraInit(tag);

        //获取sdk 商品信息

        for (int i = 0; i < productDefinitions.Count; i++)
        {
            string goodsID = productDefinitions[i].goodsID;
            Debug.LogWarning("==============从sdk 获取商品信息：" + goodsID);
            SDKManagerNew.GetGoodsInfoFromSDK(null, goodsID);
        }

    }

    /// <summary>
    /// 获得预支付订单
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private void OnPrePay(PrePay2Client e, object[] args)
    {
        Debug.LogWarning("OnPrePay=========：" + e.prepay_id + "=partnerId==");

        //判断是否需要重发支付
        if(SDKManager.GetReSendPay(e.storeName.ToString()))
        {
            OnPayInfo onPayInfo = new OnPayInfo();
            onPayInfo.isSuccess = true;
            onPayInfo.goodsId = e.goodsID;
            onPayInfo.storeName = e.storeName;
            onPayInfo.receipt = e.mch_orderID;
            onPayInfo.price = payInfo.price;
            PayReSend.Instance.AddPrePayID(onPayInfo);
        }

        payInfo.orderID = e.mch_orderID;
        payInfo.prepay_id = e.prepay_id;

        SDKManagerNew.Pay(payInfo);
        StartLongTimeNoResponse();
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
        //mch_orderID = payInfo.orderID;
        //this.goodsID = this.payInfo.goodsID;
        Debug.Log("send publicPay message storeName" + payInfo.storeName + " goodsID " + payInfo.goodsID);
        //给服务器发y预支付消息
        PrePay2Service.SendPrePayMsg((StoreName)Enum.Parse(typeof(StoreName), payInfo.storeName), payInfo.goodsID);
    }


    ///// <summary>
    ///// 消息1 的监听， 获得订单信息，然后调支付sdk
    ///// </summary>
    //private void IndentListener(string goodID,string mch_orderID,string prepay_id,float price)
    //{
    //    PayInfo payInfo = new PayInfo(
    //        goodID,
    //        GetGoodsInfo(goodsID).localizedTitle, 
    //        prepay_id, 
    //        FrameWork.SDKManager.GoodsType.NORMAL,
    //        mch_orderID,
    //        price, 
    //        GetGoodsInfo(goodsID).isoCurrencyCode,GetUserID(),
    //        storeName);

    //    SDKManagerNew.Pay( payInfo);
    //}

    public override LocalizedGoodsInfo GetGoodsInfo(string goodsID)
    {
         return base.GetGoodsInfo(goodsID);
    }

    /// <summary>
    /// 确认支付成功
    /// </summary>
    /// <param name="goodsID"></param>
    /// <param name="mch_orderID"></param>
    public override void ConfirmPay(string goodsID, string mch_orderID,string SDKName)
    {
        PayReSend.Instance.ClearPrePayID(mch_orderID);
        Debug.Log("ConfirmPay  : " + goodsID);
        //擦除sdk记录
        SDKManagerNew.ClearPurchaseBySDK(SDKName, goodsID, mch_orderID);
    }

    public override string GetUserID()
    {
        return userID;
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
            Debug.LogWarning("======StartLongTimeNoResponse=====  end  ===" + payResponse + "=============" + Time.timeSinceLevelLoad);

            if (!payResponse)
            {
                PayCallBack(new OnPayInfo(payInfo, false, (StoreName)Enum.Parse(typeof(StoreName), payInfo.storeName)));
            }
        });
    }


    //正常订单回调
    private void OnPayResultCallBack(PayResult result)
    {
        payResponse = true;
    }
}
