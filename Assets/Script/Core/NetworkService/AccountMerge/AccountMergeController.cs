using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AccountMergeController
{
    public static CallBack<AccountMergeInfo2Client> OnMergeAccountExist;
    public static CallBack<ConfirmMergeExistAccount2Client> OnConfirmMergeExistAccountCallback;
    private static bool isInit = false;
    private static void Init()
    {
        if (isInit)
            return;
        isInit = true;
        GlobalEvent.AddTypeEvent<AccountMergeInfo2Client>(OnAccountMergeInfo);
        GlobalEvent.AddTypeEvent<ConfirmMergeExistAccount2Client>(OnConfirmMergeExistAccount);
    }

    private static void OnConfirmMergeExistAccount(ConfirmMergeExistAccount2Client e, object[] args)
    {

        if (OnConfirmMergeExistAccountCallback != null)
        {
            OnConfirmMergeExistAccountCallback(e);
        }
    }

    private static void OnAccountMergeInfo(AccountMergeInfo2Client e, object[] args)
    {
        if (!e.alreadyExistAccount)
            return;
        Debug.Log("要绑定的账户已存在：" + e.mergeAccount.userID);
        if (OnMergeAccountExist != null)
        {
            OnMergeAccountExist(e);
        }
    }
    /// <summary>
    /// 当要绑定的账户已存在，确认合并
    /// </summary>
    public static void ConfirmMerge(User mergeAccount,bool useCurrentAccount)
    {
        ConfirmMergeExistAccount2Server msg = new ConfirmMergeExistAccount2Server();
        JsonMessageProcessingController.SendMessage(msg);
    }
    /// <summary>
    /// 请求绑定账户
    /// </summary>
    /// <param name="loginPlatform"></param>
    /// <param name="accountID"></param>
    /// <param name="pw"></param>
    public static void MergeLoginPlatform(LoginPlatform loginPlatform, string accountID = "", string pw = "")
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

    private static void SDKLoginCallBack(OnLoginInfo info)
    {
        SDKManager.LoginCallBack -= SDKLoginCallBack;

        if (info.isSuccess)
        {
            AccountMergeInfo2Server msg = AccountMergeInfo2Server.GetMessage(info.loginPlatform, info.accountId, info.pw);
            JsonMessageProcessingController.SendMessage(msg);
        }
    }
}

