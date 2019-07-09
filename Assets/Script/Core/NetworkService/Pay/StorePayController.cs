using FrameWork.SDKManager;
using HDJ.Framework.Utils;
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


    /// <summary>
    /// 支付完成回调（code码，物品ID）
    /// </summary>
    public static CallBack<int, string> OnPayCallBack;

    private static List<LocalizedGoodsInfo> productDefinitions;


    public static void Init(List<LocalizedGoodsInfo> productDefinitions)
    {
        //初始化支付凭据验证管理器
        NetworkVerificationImplement implement = new NetworkVerificationImplement();
        PaymentVerificationManager.Init(implement);
        PaymentVerificationManager.onVerificationResultCallBack += OnVerificationResultCallBack;

        LoginGameController.OnUserLogin += OnUserLogin;

        StorePayController.productDefinitions = productDefinitions;
        Debug.Log("支付商店初始化");

        LoginGameController.OnUserLogin += UserLogin;
        ApplicationManager.s_OnApplicationFocus += OnGameFocus;
    }

    private static void UserLogin(UserLogin2Client t)
    {
        PayReSend.Instance.ReSendPay();
    }

    private static void OnGameFocus(bool focus)
    {
        if (focus) //唤醒时
        {
            //PayReSend.Instance.ReSendPay();
        }

    }

    private static void OnVerificationResultCallBack(int t, string t1)
    {
        Debug.Log("支付返回："+t);
        if (OnPayCallBack != null)
            OnPayCallBack(t, t1);
    }

    private static void OnUserLogin(UserLogin2Client t)
    {
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

    public static void Pay(string goodID,float price,string goodsName,string currency)
    {
        PayInfo payInfo = new PayInfo(goodID, goodsName, "", FrameWork.SDKManager.GoodsType.NORMAL, "", price, currency);
        if (Application.platform == RuntimePlatform.Android)
        {
           
            string sdkName = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_StoreName, null);
            SDKManager.Pay(sdkName, payInfo );
        }
        else
        {
            SDKManager.Pay(payInfo);
        }
        NetworkVerificationImplement.SetBuyResendMessage(new StoreBuyGoods2Server(), true);
    }
}



