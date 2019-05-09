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
        if (info.isSuccess)
        {
            verificationInterface.CheckRecipe(info);
        }
        else
        {
            OnVerificationResult(info.isSuccess, info.goodsId,false);
        }
    }
    /// <summary>
    /// 验证结果调用
    /// </summary>
    /// <param name="isSucess">是否成功</param>
    /// <param name="goodID">物品ID</param>
    /// <param name="repeatReceipt">是否是重复的订单凭据</param>
    public static void OnVerificationResult(bool isSucess,string goodID, bool repeatReceipt)
    {
        if (onVerificationResultCallBack != null)
        {

            onVerificationResultCallBack(isSucess, goodID);
        }
       
        if (isSucess || repeatReceipt)
            SDKManager.ConfirmPay(goodID);
        //验证成功
        if (!isSucess)
        {
            Debug.LogError("凭据验证失败！ goodID:" + goodID);
        }
    }
}


