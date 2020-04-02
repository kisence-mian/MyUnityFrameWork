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
    public static CallBack<PayResult> onVerificationResultCallBack;

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
            //if(string.IsNullOrEmpty(info.receipt))
            //{
            //    info.receipt = Guid.NewGuid().ToString();
            //}
            verificationInterface.CheckRecipe(info);
        }
        else
        {
            Debug.Log("PaymentVerificationManager info.goodsId " + info.goodsId);
            int code = info.isSuccess ? 0 : -1;
            OnVerificationResult(code, info.goodsId,false,info.receipt,info.error,info.storeName);
        }
    }
    /// <summary>
    /// 验证结果调用
    /// </summary>
    /// <param name="code">是否成功</param>
    /// <param name="goodsID">物品ID</param>
    /// <param name="repeatReceipt">是否是重复的订单凭据</param>
    /// <param name="receipt">回执，商户订单号等</param>
    public static void OnVerificationResult(int code,string goodsID, bool repeatReceipt,string receipt,string error,StoreName storeName)
    {
        try
        {
            if (onVerificationResultCallBack != null)
            {
                PayResult result = new PayResult(code,goodsID,error,storeName);
                Debug.Log("验证回调 code " + code + " goodsID " + goodsID);
                onVerificationResultCallBack(result);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
       

        if (code == 0 || code == ErrorCodeDefine.StorePay_RepeatReceipt || repeatReceipt)
        {
            Debug.Log("订单确认"+ goodsID);
            SDKManager.ConfirmPay(storeName.ToString(), goodsID, receipt );
        }

        //验证成功
        if (code!=0)
        {
            Debug.LogError("凭据验证失败！ goodID:" + goodsID);
        }
    }
}


