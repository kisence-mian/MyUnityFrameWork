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

    //List<RuntimePlatform> platforms = new List<RuntimePlatform>();

    //public override List<RuntimePlatform> GetPlatform()
    //{
    //    return platforms;
    //}

    public override void Init()
    {
        //platforms.Add(RuntimePlatform.Android);
        //platforms.Add(RuntimePlatform.IPhonePlayer);
        //platforms.Add(RuntimePlatform.WindowsPlayer);

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
