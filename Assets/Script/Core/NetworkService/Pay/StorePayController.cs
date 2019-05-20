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
    /// <summary>
    /// 支付完成回调（是否成功，物品ID）
    /// </summary>
    public static CallBack<bool, string> OnPayCallBack;

    private static List<PayProductDefinition> productDefinitions;
    public static void Init(List<PayProductDefinition> productDefinitions)
    {
        //初始化支付凭据验证管理器
        NetworkVerificationImplement implement = new NetworkVerificationImplement();
        PaymentVerificationManager.Init(implement);
        PaymentVerificationManager.onVerificationResultCallBack += OnVerificationResultCallBack;

        LoginGameController.OnUserLogin += OnUserLogin;

        StorePayController.productDefinitions = productDefinitions;
        Debug.Log("支付商店初始化");
    }

    private static void OnVerificationResultCallBack(bool t, string t1)
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
            Debug.Log("支付SDK初始化");
        }
    }

    public static void Pay(string goodID)
    {
        SDKManager.Pay(goodID);
    }
}

