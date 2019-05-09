using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WXLoginSDKClass : LoginInterface
{
    GameObject go;

    public GameObject Go
    {
        get
        {
            WXListener.Instance.GetComponent<WXListener>().wXLoginSDKClass = this;
            //if (go == null)
            //{
            //    go = new GameObject(typeof(WXLoginSDKClass).ToString());
            //    go.AddComponent<WXListener>().wXLoginSDKClass = this;

            //}
            go = WXListener.Instance;
            return go;
        }
    }
    public override LoginPlatform GetLoginPlatform()
    {
        return  LoginPlatform.WeiXin;
    }
    public override RuntimePlatform GetPlatform()
    {
        return RuntimePlatform.Android;
    }
    public override void Init()
    {
        base.Init();
        if (Go == null)
        {
            Debug.LogWarning("WXLoginSDKClass init failed");
        }

    }

    public static AndroidJavaObject GetCurrentAndroidJavaObject()                //要有这个AndroidJavaObject才能Call
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.catplanethistory.com.wxapi.WXEntryActivity");
        jc.CallStatic("Text");
        //return jc.GetStatic<AndroidJavaObject>("currentActivity");
        return jc.CallStatic<AndroidJavaObject>("GetInstance");
    }


    public override void Login(string tage)
    {
        Debug.LogWarning("WX login");
        if (Go != null)
        {
            if (!WXListener.HaveWXApp())
            {
                
                OnLoginInfo info = new OnLoginInfo();
                info.isSuccess = false;
                info.error = LoginErrorEnum.NoInstallApp;
                LoginCallBack(info);
            }
            else
            {
                GetCurrentAndroidJavaObject().Call("SdkLogin");
            }


        }
        else
        {
            Debug.LogError("WXLoginSDK go is null");
        }
    }

    //接受微信SDK返回消息
    public void ReceiveWXResult(string token)
    {
        Debug.LogWarning("WX getresult == " + token);
        OnLoginInfo info = new OnLoginInfo();
        info.accountId = token;
        info.isSuccess = true;

        LoginCallBack(info);
        
    }

    //测试SDK 返回
    public void DebugWXResult(string value)
    {
        Debug.LogWarning("WX Debug == " + value);
    }


}
