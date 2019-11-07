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

    /// <summary>
    /// 是否已登录
    /// </summary>
    public static bool isLogin = false;
    public static bool isClickLogin = false;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        GlobalEvent.AddTypeEvent<UserLogin2Client>(OnUserLoginEvent);
        GlobalEvent.AddTypeEvent<UserLogout2Client>(OnUserLogoutEvent);
        ResendMessageManager.Init();
        AutoReconnectController.EndReconnect += OnEndReconnect;
        AutoReconnectController.Init();
        ApplicationManager.s_OnApplicationUpdate += LogonUpdate;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginPlatform"></param>
    /// <param name="accountID"></param>
    /// <param name="pw"></param>
    public static void Login(LoginPlatform loginPlatform, string accountID = "", string pw = "")
    {
        
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
        UserLogout2Server msg = new UserLogout2Server();
        JsonMessageProcessingController.SendMessage(msg);
    }

    #region 消息重发
    const float c_reSendTimer = 3; //重发间隔

    static float reSendTimer = 0;

    /// <summary>
    /// 按时重发登录消息
    /// </summary>
    private static void LogonUpdate()
    {
        if (isLogin)
        {
            return;
        }

        if(!isClickLogin)
        {
            return;
        }

        if (reSendTimer > 0)
        {
            reSendTimer -= Time.deltaTime;

            if (reSendTimer < 0)
            {
                NetworkManager.DisConnect();
            }
        }
    }


    #endregion


    #region 消息返回
    private static void OnUserLogoutEvent(UserLogout2Client e, object[] args)
    {
        isLogin = false;
        isClickLogin = false;
        ResendMessageManager.startResend = false;
        loginMsg = null;
        if (OnUserLogout!=null)
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
        if (e.code == 0)
            isLogin = true;

        if (OnUserLogin != null)
        {
            OnUserLogin(e);
        }
        if (e.reloginState)
            return;
        ResendMessageManager.startResend = true;
        loginMsg.typeKey = e.typeKey;

        SDKManager.LogLogin(e.user.userID);
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
            isClickLogin = true;
            UserLogin2Server msg = UserLogin2Server.GetLoginMessage(info.loginPlatform, info.accountId, info.password);
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
        reSendTimer = c_reSendTimer;
        bool reLoginState = false;
        if (msg != null)
            loginMsg = msg;
        else
        {
            if (loginMsg == null)
                return;
            else
            {
                if (isLogin)
                    reLoginState = true;
            }
        }
        loginMsg.reloginState = reLoginState;

        Debug.Log("SendLoginMsg -->" + reLoginState);
        JsonMessageProcessingController.SendMessage(loginMsg);
    }
}

