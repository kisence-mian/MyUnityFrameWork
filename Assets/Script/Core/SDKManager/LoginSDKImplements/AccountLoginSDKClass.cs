using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public   class AccountLoginSDKClass: LoginInterface
{
    public override LoginPlatform GetLoginPlatform()
    {
        return  LoginPlatform.AccountLogin;
    }


    public override void Login(string tag)
    {
        string[] arr = tag.Split('|');
        OnLoginInfo info = new OnLoginInfo();
        info.accountId = arr[0]; ;
        info.password = arr[1];
        info.isSuccess = true;

        LoginCallBack(info);
    }
}

