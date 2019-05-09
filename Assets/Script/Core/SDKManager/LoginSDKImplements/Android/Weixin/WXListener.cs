using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WXListener : MonoBehaviour {

    private static GameObject instance;



    public WXLoginSDKClass wXLoginSDKClass;
    public WXPayClass wXPayClass;

    public static GameObject Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(WXListener).ToString());
                instance.AddComponent<WXListener>();
            }
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    /// <summary>
    /// 是否安装了微信客户端
    /// </summary>
    /// <returns></returns>
    public static bool HaveWXApp()
    {
        return WXLoginSDKClass.GetCurrentAndroidJavaObject().Call<bool>("IsWXAppInstalle");
    }

    #region 分享到朋友圈
    /// <summary>
    /// 分享到朋友圈
    /// </summary>
    public void WXShareToFriends(string url,string title,string des )
    {
        if (!HaveWXApp())
        {
           //MiniHintWindow.ShowHint("UI/HotUpdateWindow/NoWX");
            return;
        }

        WXLoginSDKClass.GetCurrentAndroidJavaObject().Call("SdkScenetimeline", "https://www.taptap.com/app/163003", "test", "测试");
    }
    //分享的结果(也是截屏的监听)
    public void ShareWXResult(string value)
    {
        //分享成功(必然)
        if (value == "0")
        {

        }

    }

    #endregion

    #region 分享截屏到朋友圈

    /// <summary>
    /// 分享截屏
    /// </summary>
    public void SharePhoto()
    {
        if (!HaveWXApp())
        {
           // MiniHintWindow.ShowHint("UI/HotUpdateWindow/NoWX");
            return;
        }

        StartCoroutine(RecordFrame());
    }


    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        // do something with texture

        byte[] bytes = ScreenCapture.CaptureScreenshotAsTexture(0).EncodeToPNG();
        WXLoginSDKClass.GetCurrentAndroidJavaObject().Call("SdkScreenshotToCircle", bytes);

        // cleanup
        Object.Destroy(texture);
    }

    #endregion

    //接受微信SDK返回消息
    public void ReceiveWXResult(string token)
    {
        Debug.LogWarning("WX getresult == " + token);
        wXLoginSDKClass.ReceiveWXResult(token);
    }

    //测试SDK 返回
    public void DebugWXResult(string value)
    {
        Debug.LogWarning("WX Debug == " + value);
        wXLoginSDKClass.DebugWXResult(value);
    }


    #region 支付

    /// <summary>
    /// 商品ID
    /// </summary>
    private string goodId;

    public string GoodId
    {
        get
        {
            return goodId;
        }

        set
        {
            goodId = value;
        }
    }



    /// <summary>
    /// 商户订单号
    /// </summary>
    private string mch_orderID;
    public string Mch_orderID
    {
        get
        {
            return mch_orderID;
        }

        set
        {
            mch_orderID = value;
        }
    }


    /// <summary>
    /// 支付回调
    /// </summary>
    public void ReceiveWXPay(string result)
    {
        Debug.LogWarning("WXpay----result-----" + result);

        string[] payInfo = result.Split('_');

        if (payInfo[0] == "0")
        {
            Debug.LogWarning("=======GoodId=======" + GoodId);
           // StoreBuyGoods2Server.SenBuyMsg(GoodId, 1,StoreName.WX, Mch_orderID);
        }

        wXPayClass.SetPayResult(payInfo[0],goodId, Mch_orderID);


    }
    #endregion
}
