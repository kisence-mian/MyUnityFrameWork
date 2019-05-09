using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCenterLoginSDKClass : LoginInterface
{
    public override RuntimePlatform GetPlatform()
    {
        return  RuntimePlatform.IPhonePlayer;
    }
    public override LoginPlatform GetLoginPlatform()
    {
        return  LoginPlatform.Apple;
    }
    public override void Init()
    {

    }

    public override void Login(string tage)
    {
        Debug.LogWarning("======GameCenterLoginSDKClass==========登录");
        AccessGameCenter();
    }

    private void AccessGameCenter()
    {
        UnityEngine.Social.localUser.Authenticate(AccessGameCenterCallback);
        //if (UnityEngine.Social.localUser.authenticated)
        //{
            
        //}
        //else
        //{
        //    OnLoginInfo info = new OnLoginInfo();
        //    info.isSuccess = false;
        //    info.error = LoginErrorEnum.GameCenterNotOpen;

        //    LoginCallBack(info);
        //}
    }

    private void AccessGameCenterCallback(bool success)
    {
        OnLoginInfo info = new OnLoginInfo();
        if (success)
        {
            info.isSuccess = true;
            info.accountId = Social.localUser.id;
            info.nickName = Social.localUser.userName;
        }
        else
        {
            info.isSuccess = false;
        }
        LoginCallBack(info);
    }

}
