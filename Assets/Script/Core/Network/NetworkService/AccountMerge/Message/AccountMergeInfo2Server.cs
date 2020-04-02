using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AccountMergeInfo2Server
{
    /// <summary>
    /// 登录平台
    /// </summary>
    public LoginPlatform loginType;
    /// <summary>
    /// 该平台返回的key，如游客登录使用设备号
    /// </summary>
    public String typeKey;
    /// <summary>
    /// password
    /// </summary>
    public String pw;
    public RuntimePlatform platform;
    public String deviceUniqueIdentifier;
    public SystemLanguage deviceSystemLanguage = SystemLanguage.Unknown;

    public static AccountMergeInfo2Server GetMessage(LoginPlatform loginType, String typeKey,string pw)
    {
        AccountMergeInfo2Server msg = new AccountMergeInfo2Server();
        msg.loginType = loginType;
        msg.typeKey = typeKey;
        msg.pw = pw;

        msg.deviceUniqueIdentifier = SystemInfoManager.deviceUniqueIdentifier;
        msg.platform = Application.platform;
        msg.deviceSystemLanguage = Application.systemLanguage;

        return msg;
    }
}

