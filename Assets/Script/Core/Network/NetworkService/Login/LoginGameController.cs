using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class LoginGameController
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
    /// 当服务器要求玩家使用兑换码
    /// </summary>
    public static CallBack<AskUserUseActivationCode2Client> OnAskUseActivationCode;
    /// <summary>
    /// 是否已登录
    /// </summary>
    private static bool isLogin = false;
    /// <summary>
    /// 是否已点击登录按钮
    /// </summary>
    public static bool isClickLogin = false;
    /// <summary>
    /// 玩家登陆的SDk平台
    /// </summary>
    public static LoginPlatform PlayerLoginPlatform
    {
        get;
        private set;
    }

    /// <summary>
    /// 激活码
    /// </summary>
    private static string activationCode;
    public static string ActivationCode
    {
        get { return activationCode; }
        set
        {
            activationCode = value;
            if (loginMsg != null)
            {
                loginMsg.activationCode = activationCode;
            }
        }
    }

    public static bool IsLogin
    {
        get
        {
            return isLogin;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        GlobalEvent.AddTypeEvent<UserLogin2Client>(OnUserLoginEvent);
        GlobalEvent.AddTypeEvent<UserLogout2Client>(OnUserLogoutEvent);
        GlobalEvent.AddTypeEvent<AskUserUseActivationCode2Client>(AskUserUseActivationCode);
        ResendMessageManager.Init();
        AutoReconnectController.EndReconnect += ReLogin;
        AutoReconnectController.Init();
        ApplicationManager.s_OnApplicationUpdate += LogonUpdate;
    }

    /// <summary>
    /// 检查是否要直接登录，如果是那就直接登录（默认选择筛选后的第0个登录）
    /// </summary>
    /// <param name="datas">返回可登录选项</param>
    /// <returns></returns>
    public static bool CheckAutoLoginOrGetLoginPlatforms(out List<LoginConfigData> datas)
    {
        datas = new List<LoginConfigData>();
        List<LoginConfigData> allConfigs = DataGenerateManager<LoginConfigData>.GetAllDataList();

        string sdkStr = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_LoginPlatform, "");
        if (!string.IsNullOrEmpty(sdkStr))
        {
            string[] arrStr = sdkStr.Split('|');
            foreach (var item in arrStr)
            {
                LoginConfigData con = DataGenerateManager<LoginConfigData>.GetData(item);
                if (con != null)
                {
                    datas.Add(con);
                }
                else
                {
                    Debug.LogError("获取登录配置失败：" + item);
                }

            }
        }
        else
        {
            //Debug.LogError("获取登录配置失败：" + item);

            foreach (var d in allConfigs)
            {
                List<string> platforms = null;
                if (d.m_SupportPlatform == null)
                    platforms = new List<string>();
                else
                    platforms = new List<string>(d.m_SupportPlatform);

                if (d.m_UseItem || platforms.Contains(Application.platform.ToString()))
                {
                    datas.Add(d);
                }
            }
        }

        string directlyLoginStr = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_DirectlyLogin, "false");
        bool directlyLogin = bool.Parse(directlyLoginStr);
        if (directlyLogin)
        {
            LoginConfigData d = datas[0];
            Login(d.m_loginName, "", "", d.m_CustomInfo);
            return true;
        }
        return false;
    }


    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginPlatform"></param>
    /// <param name="accountID"></param>
    /// <param name="pw"></param>
    public static void Login(LoginPlatform loginPlatform, string accountID = "", string pw = "", string custom = "")
    {
        SDKManager.LoginCallBack += SDKLoginCallBack;

        string tag = "";
        accountID = accountID.Trim();
        pw = pw.Trim();
        string pwMd5 = MD5Utils.GetObjectMD5(pw);
        tag = accountID + "|" + pwMd5 + "|" + custom;

        SDKManager.LoginByPlatform(loginPlatform, tag);
    }
    public static void ReLogin()
    {
        if (loginMsg == null)
        {
            //Debug.LogError("没有登录记录！不能调用ReLogin");
            return;
        }
        SendLoginMsg();
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
        if (IsLogin)
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
    private static void AskUserUseActivationCode(AskUserUseActivationCode2Client e, object[] args)
    {
        ResendMessageManager.startResend = false;
        isClickLogin = false;
        if (OnAskUseActivationCode != null)
        {
            OnAskUseActivationCode(e);
        }
    }
    private static void OnUserLogoutEvent(UserLogout2Client e, object[] args)
    {
        isLogin = false;
        isClickLogin = false;
        ResendMessageManager.startResend = false;
        loginMsg = null;

        SDKManager.LoginOut(PlayerLoginPlatform.ToString());
        GameInfoCollecter.AddPlayerInfoValue("IsLogin", IsLogin);

        if (OnUserLogout!=null)
        {
            OnUserLogout(e);
        }
    }

    private static void OnUserLoginEvent(UserLogin2Client e, object[] args)
    {
        activationCode = "";
        if (e.code == 0)
        {
            isLogin = true;
            GameDataMonitor.PushData("User", e.user, "玩家数据");
            GameInfoCollecter.AddPlayerInfoValue("IsLogin", IsLogin);
            GameInfoCollecter.AddPlayerInfoValue("UserID", e.user.userID);
            GameInfoCollecter.AddPlayerInfoValue("LoginType", e.user.loginType);
            GameInfoCollecter.AddPlayerInfoValue("NickName", e.user.nickName);
            GameInfoCollecter.AddPlayerInfoValue("TypeKey", e.user.typeKey);
            GameInfoCollecter.AddPlayerInfoValue("PlayTime", e.user.playTime);
            GameInfoCollecter.AddPlayerInfoValue("TotalLoginDays", e.user.totalLoginDays);

            SDKManager.UserID = e.user.userID;
        }

        if (OnUserLogin != null)
        {
            OnUserLogin(e);
        }
        if (e.reloginState)
            return;
        isClickLogin = false;
        if (e.code != 0)
        {
            
            Debug.LogError("Login error code:" +e.code);
            return;
        }
        ResendMessageManager.startResend = true;
        loginMsg.typeKey = e.user.typeKey;

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
            PlayerLoginPlatform = info.loginPlatform;
            UserLogin2Server msg = UserLogin2Server.GetLoginMessage(info.loginPlatform, info.accountId, info.password,activationCode);
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
                if (IsLogin)
                    reLoginState = true;
            }
        }
        loginMsg.reloginState = reLoginState;

        Debug.Log("SendLoginMsg -->" + reLoginState);
        JsonMessageProcessingController.SendMessage(loginMsg);
    }
}

