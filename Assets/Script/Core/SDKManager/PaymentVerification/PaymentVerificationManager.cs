using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 管理支付验证
/// </summary>
public class PaymentVerificationManager
{
    public static CallBack<bool, string> onVerificationResultCallBack;

    private static PaymentVerificationInterface verificationInterface;
    public static void Init(PaymentVerificationInterface verificationInterface)
    {
        PaymentVerificationManager.verificationInterface = verificationInterface;
        verificationInterface.Init();
        SDKManager.PayCallBack += PayCallBack;
    }

    private static void PayCallBack(OnPayInfo info)
    {
        verificationInterface.CheckRecipe( info);
    }

    public static void OnVerificationResult(bool t,string goodID)
    {
        //验证成功
        if (!t)
        {
         
            Debug.LogError("凭据验证失败！ goodID:" + goodID);
            return;
        }
        SDKManager.ConfirmPay(goodID);
        if (onVerificationResultCallBack!=null)
        {
            onVerificationResultCallBack(t, goodID);
        }
    }
}


