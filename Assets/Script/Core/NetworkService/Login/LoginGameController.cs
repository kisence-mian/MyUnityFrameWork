using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LoginGameController
{
    /// <summary>
    /// 调用sdk登录返回
    /// </summary>
    public static CallBack<OnLoginInfo> OnSDKLoginCallBack;
    /// <summary>
    /// 服务器登录回调
    /// </summary>
    public static CallBack<UserLogin2Client> OnUserLogin;
    /// <summary>
    /// 退出登录回调
    /// </summary>
    public static CallBack<UserLogout2Client> OnUserLogout;

    private static bool isInit = false;
    private static void Init()
    {
        if (isInit)
            return;
        isInit = true;
        GlobalEvent.AddTypeEvent<UserLogin2Client>(OnUserLoginEvent);
        GlobalEvent.AddTypeEvent<UserLogout2Client>(OnUserLogoutEvent);
        ResendMessageManager.Init();
        AutoReconnectController.EndReconnect += OnEndReconnect;
        AutoReconnectController.Init();
    }
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginPlatform"></param>
    /// <param name="accountID"></param>
    /// <param name="pw"></param>
    public static void Login(LoginPlatform loginPlatform, string accountID = "", string pw = "")
    {
        Init();
        SDKManager.LoginCallBack += SDKLoginCallBack;
        string tag = "";
        if (loginPlatform == LoginPlatform.AccountLogin)
        {
            accountID = accountID.Trim();
            pw = pw.Trim();
            string pwMd5 = HDJ.Framework.Utils.MD5Utils.GetObjectMD5(pw);
            tag = accountID + "|" + pwMd5;
        }
        SDKManager.LoginByPlatform(loginPlatform, tag);
    }
    /// <summary>
    /// 退出登录
    /// </summary>
    public static void Logout()
    {
        Init();
        UserLogout2Server msg = new UserLogout2Server();
        JsonMessageProcessingController.SendMessage(msg);
    }

    #region 消息返回
    private static void OnUserLogoutEvent(UserLogout2Client e, object[] args)
    {
        if(OnUserLogout!=null)
        {
            OnUserLogout(e);
        }
    }

    private static void OnEndReconnect()
    {
        SendLoginMsg();
    }

    private static void OnUserLoginEvent(UserLogin2Client e, object[] args)
    {
        if (OnUserLogin != null)
        {
            OnUserLogin(e);
        }
        if (e.reloginState)
            return;
        ResendMessageManager.startResend = true;
        loginMsg.typeKey = e.typeKey;
    }



    private static void SDKLoginCallBack(OnLoginInfo info)
    {
        SDKManager.LoginCallBack -= SDKLoginCallBack;

        if (OnSDKLoginCallBack != null)
        {
            OnSDKLoginCallBack(info);
        }
        if (info.isSuccess)
        {
            UserLogin2Server msg = UserLogin2Server.GetLoginMessage(info.loginPlatform, info.accountId, info.pw);
            SendLoginMsg(msg);
        }
    }
    #endregion

    private static UserLogin2Server loginMsg;
    /// <summary>
    /// 用于自动重连后自动登录
    /// </summary>
    /// <param name="msg"></param>
    private static void SendLoginMsg(UserLogin2Server msg = null)
    {
       bool reLoginState = false;
        if (msg != null)
            loginMsg = msg;
        else
        {
            if (loginMsg == null)
                return;
            else
            {
                reLoginState = true;
            }
        }
        loginMsg.reloginState = reLoginState;

        Debug.Log("SendLoginMsg -->" + reLoginState);
        JsonMessageProcessingController.SendMessage(loginMsg);
    }

}

