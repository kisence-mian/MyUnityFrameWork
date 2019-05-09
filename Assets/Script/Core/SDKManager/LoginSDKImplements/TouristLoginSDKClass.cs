using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouristLoginSDKClass : LoginInterface
{
    public override RuntimePlatform GetPlatform()
    {
        return  Application.platform;
    }
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
        info.accountId = SystemInfo.deviceUniqueIdentifier;
        info.isSuccess = true;
        LoginCallBack(info);
    }




}
