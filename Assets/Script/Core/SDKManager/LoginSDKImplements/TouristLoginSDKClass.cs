using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristLoginSDKClass : LoginInterface
{
    public override LoginPlatform GetLoginPlatform()
    {
        return  LoginPlatform.Tourist;
    }
    public override void Init()
    {
        base.Init();


    }


    public override void Login(string tage)
    {
        OnLoginInfo info = new OnLoginInfo();
        info.accountId = SystemInfoManager.deviceUniqueIdentifier;
        info.isSuccess = true;
        LoginCallBack(info);
    }




}
