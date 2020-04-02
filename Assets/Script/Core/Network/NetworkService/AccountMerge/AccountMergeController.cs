using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// 管理平台账号绑定
/// </summary>
public class AccountMergeController
{
    /// <summary>
    /// 当要绑定的平台已存在回调
    /// </summary>
    public static CallBack<AccountMergeInfo2Client> OnMergeAccountExist;
    /// <summary>
    /// 最终绑定的结果回调
    /// </summary>
    public static CallBack<ConfirmMergeExistAccount2Client> OnConfirmMergeExistAccountCallback;
    /// <summary>
    /// 返回当前账户已经绑定的平台（包含当前登录平台）
    /// </summary>
    public static CallBack<List<LoginPlatform>> OnRequsetAreadyBindPlatformCallBack;
    private static List<LoginPlatform> areadyBindPlatform = new List<LoginPlatform>();
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        GlobalEvent.AddTypeEvent<AccountMergeInfo2Client>(OnAccountMergeInfo);
        GlobalEvent.AddTypeEvent<ConfirmMergeExistAccount2Client>(OnConfirmMergeExistAccount);
        GlobalEvent.AddTypeEvent<RequsetAreadyBindPlatform2Client>(OnRequsetAreadyBindPlatform);
        LoginGameController.OnUserLogin += OnUserLogin;
    }

    private static void OnUserLogin(UserLogin2Client t)
    {
        RequsetAreadyBindPlatform();
    }
    #region 消息接收
    private static void OnRequsetAreadyBindPlatform(RequsetAreadyBindPlatform2Client e, object[] args)
    {
        areadyBindPlatform = e.areadyBindPlatforms;
        if (OnRequsetAreadyBindPlatformCallBack != null)
        {
            OnRequsetAreadyBindPlatformCallBack(e.areadyBindPlatforms);
        }
    }

    private static void OnConfirmMergeExistAccount(ConfirmMergeExistAccount2Client e, object[] args)
    {
        if(e.code==0)
        {
            if (areadyBindPlatform.Contains(e.loginType))
                Debug.LogError("已包含绑定平台：" + e.loginType);
            else
                areadyBindPlatform.Add(e.loginType);
        }
        else
        {
            Debug.LogError("绑定出错");
        }
        if (OnConfirmMergeExistAccountCallback != null)
        {
            OnConfirmMergeExistAccountCallback(e);
        }
    }

    private static void OnAccountMergeInfo(AccountMergeInfo2Client e, object[] args)
    {
        //if (!e.alreadyExistAccount)
        //    return;
        if (e.code == 0)
            Debug.Log("要绑定的账户已存在：" + e.mergeAccount.userID);
        if (OnMergeAccountExist != null)
        {
            OnMergeAccountExist(e);
        }
    }
    #endregion


    /// <summary>
    /// 请求哪些是已绑定的平台信息
    /// </summary>
    private static void RequsetAreadyBindPlatform()
    {
        RequsetAreadyBindPlatform2Server msg = new RequsetAreadyBindPlatform2Server();
        JsonMessageProcessingController.SendMessage(msg);
    }
    /// <summary>
    /// 返回当前账户已经绑定的平台（包含当前登录平台）
    /// </summary>
    /// <returns></returns>
    public static List<LoginPlatform> GetAreadyBindPlatform()
    {
        return areadyBindPlatform;
    }
    /// <summary>
    /// 判断是否能使用商店（规则是当前是游客登录，并且未绑定其他登录方式）
    /// </summary>
    /// <param name="loginType">当前登录方式</param>
    /// <returns></returns>
    public static bool CheckCanUseStore(LoginPlatform loginType)
    {
        if (areadyBindPlatform.Contains(loginType) && areadyBindPlatform.Count == 1 && loginType == LoginPlatform.Tourist)
            return false;
        return true;
    }
    /// <summary>
    /// 当要绑定的账户已存在，确认合并
    /// </summary>
    public static void ConfirmMerge(bool useCurrentAccount)
    {
        ConfirmMergeExistAccount2Server msg = new ConfirmMergeExistAccount2Server();
        msg.useCurrentAccount = useCurrentAccount;
        JsonMessageProcessingController.SendMessage(msg);
    }
    public static bool isWaiting = false;
    /// <summary>
    /// 请求绑定账户
    /// </summary>
    /// <param name="loginPlatform"></param>
    /// <param name="accountID"></param>
    /// <param name="pw"></param>
    public static void MergeLoginPlatform(LoginPlatform loginPlatform, string accountID = "", string pw = "")
    {
        if (isWaiting)
        {
            Debug.LogError("AccountMergeController => 等待sdk返回登录信息");
            return;
        }
        isWaiting = true;

        SDKManager.LoginCallBack += SDKLoginCallBack;
        string tag = "";
        if (loginPlatform == LoginPlatform.AccountLogin)
        {
            accountID = accountID.Trim();
            pw = pw.Trim();
            string pwMd5 = MD5Utils.GetObjectMD5(pw);
            tag = accountID + "|" + pwMd5;
        }
        SDKManager.LoginByPlatform(loginPlatform, tag);
    }

    private static void SDKLoginCallBack(OnLoginInfo info)
    {
        isWaiting = false;
        SDKManager.LoginCallBack -= SDKLoginCallBack;

        if (info.isSuccess)
        {
            AccountMergeInfo2Server msg = AccountMergeInfo2Server.GetMessage(info.loginPlatform, info.accountId, info.password);
            JsonMessageProcessingController.SendMessage(msg);
        }
        else
        {
           //sdk登录失败
            if (OnConfirmMergeExistAccountCallback != null)
            {
                ConfirmMergeExistAccount2Client msg = new ConfirmMergeExistAccount2Client();
                msg.code = -1;
                msg.loginType = info.loginPlatform;
                OnConfirmMergeExistAccountCallback(msg);
            }
        }
    }
}

