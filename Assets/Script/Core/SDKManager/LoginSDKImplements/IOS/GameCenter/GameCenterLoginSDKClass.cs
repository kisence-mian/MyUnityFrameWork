using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCenterLoginSDKClass : LoginInterface
{
    static bool isAuthenticated = false; //已经调用过

    public override List<RuntimePlatform> GetPlatform()
    {
        return new List<RuntimePlatform>() { RuntimePlatform.IPhonePlayer, RuntimePlatform.WindowsEditor, RuntimePlatform.OSXEditor };
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
        if (isAuthenticated)
        {
            OnLoginInfo info = new OnLoginInfo();
            if (UnityEngine.Social.localUser.authenticated)
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
        
        UnityEngine.Social.localUser.Authenticate(AccessGameCenterCallback);
        Debug.LogWarning("=========GameCenter authenticated==========" + UnityEngine.Social.localUser.authenticated);

        isAuthenticated = true;
    }

    private void AccessGameCenterCallback(bool success)
    {
        Debug.LogWarning("=========GameCenter Callback==========" + success);
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
            info.error = LoginErrorEnum.GameCenterNotOpen;
        }
        LoginCallBack(info);
    }

}
