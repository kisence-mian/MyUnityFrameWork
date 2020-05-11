using UnityEngine;
using System.Collections;
using System;

public class SMSController 
{
    public static Action<ReplySendSMS2Client> SendResult;

    private static bool isInit = false;
    private static void Init()
    {
        if (isInit)
            return;
        isInit = true;

        GlobalEvent.AddTypeEvent<ReplySendSMS2Client>(OnReplySendSMS);
    }

    private static void OnReplySendSMS(ReplySendSMS2Client e, object[] args)
    {
        if (SendResult != null)
        {
            SendResult(e);
        }
    }

    /// <summary>
    /// 使用模版发送
    /// </summary>
    /// <param name="internationalCode">国际电话区号,中国：86</param>
    /// <param name="phoneNumber">电话</param>
    /// <param name="templateID">短信模版ID</param>
    /// <param name="parameters">短信模版变量</param>
    public static  void SendByTemplate(string internationalTelephoneCode, string phoneNumber,string templateID,string[] parameters)
    {
        Init();
        if (LoginGameController.IsLogin)
        {
            SendSMSData2Server msg = new SendSMSData2Server(internationalTelephoneCode, phoneNumber, templateID, parameters);
            JsonMessageProcessingController.SendMessage(msg);
        }
        else
        {
            Debug.LogError("请先登录");
        }
    }
    /// <summary>
    /// 国内模版发送
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="templateID"></param>
    /// <param name="parameters"></param>
    public static void SendByTemplate(string phoneNumber, string templateID, string[] parameters)
    {
        SendByTemplate("86", phoneNumber, templateID, parameters);
    }

}
