using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WXLoginSDKClass : LoginInterface
{
    public override LoginPlatform GetLoginPlatform()
    {
        return  LoginPlatform.WeiXin;
    }
    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.Android, RuntimePlatform.WindowsEditor };
    }

    //public override void Init()
    //{
    //    base.Init();

    //    //SDKManagerNew.OnLoginCallBack += ReceiveWXResult;
    //}

    public override void Login(string tage)
    {
        Debug.LogWarning(tage);
        SDKManagerNew.Login("WeiXin", "");
    }

    ////接受微信SDK返回消息
    //public void ReceiveWXResult(OnLoginInfo info)
    //{
    //    info.loginPlatform = LoginPlatform.WeiXin;
    //    LoginCallBack(info);
    //}
}
