using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 支付商店控制器
/// </summary>
public static class StorePayController
{
    public const int c_PayCode_repeate = 20203; //重复订单

    private static User user;
    /// <summary>
    /// 支付完成回调（code码，物品ID）
    /// </summary>
    public static CallBack<int, string> OnPayCallBack;

    private static List<LocalizedGoodsInfo> productDefinitions;


    public static void Init(List<LocalizedGoodsInfo> productDefinitions)
    {
        Debug.Log("商店初始化：" + JsonUtils.ToJson(productDefinitions));
        //初始化支付凭据验证管理器
        NetworkVerificationImplement implement = new NetworkVerificationImplement();
        PaymentVerificationManager.Init(implement);
        PaymentVerificationManager.onVerificationResultCallBack += OnVerificationResultCallBack;

        LoginGameController.OnUserLogin += OnUserLogin;

        StorePayController.productDefinitions = productDefinitions;
        Debug.Log("支付商店初始化");
    }

    private static void OnVerificationResultCallBack(int t, string t1)
    {
        Debug.Log("支付返回："+t);
        if (OnPayCallBack != null)
            OnPayCallBack(t, t1);
    }

    private static void OnUserLogin(UserLogin2Client t)
    {
        if (t.code != 0)
            return;

        user = t.user;
        PayReSend.Instance.ReSendPay();
        //第一次登陆，非重连
        if (!t.reloginState)
        {
            SDKManager.ExtraInit(SDKType.Pay, null, JsonUtils.ToJson(productDefinitions));
            //Debug.Log("支付SDK初始化");

            //for (int i = 0; i < productDefinitions.Count; i++)
            //{
            //    Debug.Log("ID " + productDefinitions[i].goodsID + " ->" + productDefinitions[i].localizedPrice);
            //}

            //Debug.Log("small ->" + SDKManager.GetGoodsInfo("small").localizedPrice + " json " + JsonUtils.ToJson(productDefinitions));
        }
    }
    public static LocalizedGoodsInfo GetGoodsInfo(string goodID)
    {
        LocalizedGoodsInfo info = SDKManager.GetGoodsInfo(goodID);
        if (info == null)
        {
            foreach (var item in productDefinitions)
            {
                if(item.goodsID == goodID)
                {
                    info = item;
                    break;
                }
            }
        }
        return info;
    }
    public static void Pay(string goodID)
    {
        if (user == null)
        {
            Debug.LogError("未登录，不能支付！");
            if (OnPayCallBack != null)
            {
                OnPayCallBack(ErrorCodeDefine.StroePay_NoLogin, goodID);
            }
            return;
        }
       LocalizedGoodsInfo info=  SDKManager.GetGoodsInfo(goodID);
        Pay(goodID, info.localizedPrice, info.localizedTitle, info.isoCurrencyCode, user.userID);
    }
    public static void Pay(string goodID,float price,string goodsName,string currency,string userID)
    {
        PayInfo payInfo = new PayInfo(goodID, goodsName, "", FrameWork.SDKManager.GoodsType.NORMAL, "", price, currency, userID);
        NetworkVerificationImplement.SetBuyResendMessage(new StoreBuyGoods2Server(), true);
        if (Application.platform == RuntimePlatform.Android)
        {
           
            string sdkName = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_StoreName, null);
            SDKManager.Pay(sdkName, payInfo );
        }
        else
        {
            SDKManager.Pay(payInfo);
        }
        
    }
}



