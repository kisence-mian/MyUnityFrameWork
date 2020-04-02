using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



/// <summary>
/// 支付商店控制器
/// </summary>
public static class StorePayController
{
    public const int c_PayCode_repeate = 20203; //重复订单

    private static User user;
    /// <summary>
    /// 支付完成回调（code码，物品ID,error）
    /// </summary>
    public static CallBack<PayResult> OnPayCallBack;

    /// <summary>
    /// 当需要选择支付方式时，框架会回调此CallBack（需要逻辑层监听）
    /// </summary>
    public static CallBack<LocalizedGoodsInfo, List<PayPlatformInfo>> NeedSelectPayPlatformCallBack;

    /// <summary>
    /// 选择完支付方式时回调(逻辑层不需要监听)
    /// </summary>
    public static CallBack<LocalizedGoodsInfo, PayPlatformInfo> OnSelectPayPlatformCallBack;

    private static List<LocalizedGoodsInfo> productDefinitions;


    [RuntimeInitializeOnLoadMethod]
    private static void EventAdd()
    {
        NetworkVerificationImplement implement = new NetworkVerificationImplement();
        PaymentVerificationManager.Init(implement);
        PaymentVerificationManager.onVerificationResultCallBack += OnVerificationResultCallBack;
        LoginGameController.OnUserLogin += OnUserLogin;

        GlobalEvent.AddTypeEvent<CheckPayLimitResultEvent>(OnCheckPayLimitResult);
    }


    public static void Init(List<LocalizedGoodsInfo> productDefinitions)
    {
        Debug.Log("商店初始化：" + JsonUtils.ToJson(productDefinitions));
        //初始化支付凭据验证管理器

        StorePayController.productDefinitions = productDefinitions;

        Debug.Log("支付商店初始化");
        SDKManager.ExtraInit(SDKType.Pay, null, JsonUtils.ToJson(productDefinitions));
        PayReSend.Instance.ReSendPay();
        OnSelectPayPlatformCallBack += OnOnSelectPayPlatform;
    }



    private static void OnVerificationResultCallBack(PayResult result)
    {
        Debug.Log("支付返回："+result. code);
        if (OnPayCallBack != null)
        {
            OnPayCallBack(result);

        }
    }

    private static void OnUserLogin(UserLogin2Client t)
    {
        if (t.code != 0)
            return;

        user = t.user;
       
        ////第一次登陆，非重连
        //if (!t.reloginState)
        //{
        //   // SDKManager.ExtraInit(SDKType.Pay, null, JsonUtils.ToJson(productDefinitions));
        //    //Debug.Log("支付SDK初始化");

        //    //for (int i = 0; i < productDefinitions.Count; i++)
        //    //{
        //    //    Debug.Log("ID " + productDefinitions[i].goodsID + " ->" + productDefinitions[i].localizedPrice);
        //    //}

        //    //Debug.Log("small ->" + SDKManager.GetGoodsInfo("small").localizedPrice + " json " + JsonUtils.ToJson(productDefinitions));
        //}
    }
    public static LocalizedGoodsInfo GetGoodsInfo(string goodID)
    {
        LocalizedGoodsInfo info = SDKManager.GetGoodsInfo(goodID);
        if (info == null)
        {
            foreach (var item in productDefinitions)
            {
                if(item.goodsID == goodID)
                {
                    info = item;
                    break;
                }
            }
        }
        return info;
    }
    public static void Pay(string goodID)
    {
        if (user == null)
        {
            Debug.LogError("未登录，不能支付！");
            if (OnPayCallBack != null)
            {
                OnPayCallBack(new PayResult( ErrorCodeDefine.StroePay_NoLogin, goodID,"No login!"));
            }
            return;
        }
        LocalizedGoodsInfo info=  SDKManager.GetGoodsInfo(goodID);

        SelectPayPlatform(info);
    }

    /// <summary>
    /// 选择支付方式
    /// </summary>
    /// <param name="goodsInfo"></param>
    private static void SelectPayPlatform(LocalizedGoodsInfo goodsInfo)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            List<PayPlatformInfo> allPayPlatformInfos = SDKManager.GetAllPayPlatformInfos();

            //无支付方式- 错误 
            if (allPayPlatformInfos.Count == 0)
            {
                OnVerificationResultCallBack(new PayResult( -9, goodsInfo.goodsID, "No Pay Platform"));
                Debug.LogError("SelectPayPlatform error: no Pay Platform ->" + SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_StoreName, "--"));
            }
            else if (allPayPlatformInfos.Count == 1)
            {
                //单一支付方式，直接调用
                OnOnSelectPayPlatform(goodsInfo, allPayPlatformInfos[0]);
            }
            else
            {
                //多种支付方式，派发事件
                if (NeedSelectPayPlatformCallBack != null)
                {
                    NeedSelectPayPlatformCallBack(goodsInfo, allPayPlatformInfos);
                }
                else
                {
                    Debug.LogError("请监听 StorePayController.NeedSelectPayPlatformCallBack , 并在回调时打开选择支付方式的界面。 玩家选择支付方式后， 再调用StorePayController.OnSelectPayPlatformCallBack 通知框架");
                    Debug.LogError("为了不卡住流程， 暂时默认调用第一个支付方式");
                    if (OnSelectPayPlatformCallBack != null)
                    {
                        OnSelectPayPlatformCallBack(goodsInfo, allPayPlatformInfos[0]);
                    }
                    else
                    {
                        OnVerificationResultCallBack(new PayResult( -11, goodsInfo.goodsID, "Pay Platform CallBack Null"));
                        Debug.LogError("OnSelectPayPlatformCallBack error: null");
                    }
                }
            }
        }
        else
        {
            //ios，暂时没有选择支付方式 这一步骤
            OnOnSelectPayPlatform(goodsInfo, new PayPlatformInfo());
        }
    }

    /// <summary>
    /// 选择支付平台完毕，判断实名制限制
    /// </summary>
    /// <param name="t"></param>
    /// <param name="t1"></param>
    private static void OnOnSelectPayPlatform(LocalizedGoodsInfo goodsInfo, PayPlatformInfo payPlatform)
    {
        if (payPlatform == null) //放弃支付
        {
            OnVerificationResultCallBack(new PayResult( -10, goodsInfo.goodsID, "No Select Pay Platform"));

            return;
        }
        m_goodsInfo = goodsInfo;
        m_payPlatform = payPlatform;

        int price_cent = (int)(goodsInfo.localizedPrice * 100);

        Debug.Log("OnOnSelectPayPlatform SDK： " + payPlatform.SDKName + " tag:" + payPlatform.payPlatformTag);

        //实名制限制判断  （OnCheckPayLimitResult  回调结果）
        RealNameManager.GetInstance().CheckPayLimit(price_cent);



    }

    private static LocalizedGoodsInfo m_goodsInfo;
    private static PayPlatformInfo m_payPlatform;


    /// <summary>
    /// 判断支付 限制的回调
    /// </summary>
    /// <param name="e"></param>
    /// <param name="args"></param>
    private static void OnCheckPayLimitResult(CheckPayLimitResultEvent e, object[] args)
    {
        Debug.Log("OnCheckPayLimitResult SDK： " + e.payLimitType );
        if (e.payLimitType == PayLimitType.None)
        {
            PayInfo payInfo = new PayInfo(m_goodsInfo.goodsID, m_goodsInfo.localizedTitle, m_payPlatform.payPlatformTag, FrameWork.SDKManager.GoodsType.NORMAL, "", m_goodsInfo.localizedPrice, m_goodsInfo.isoCurrencyCode, user.userID, m_payPlatform.SDKName);
            if (Application.platform == RuntimePlatform.Android)
            {
                SDKManager.Pay(m_payPlatform.SDKName, payInfo);
            }
            else
            {
                SDKManager.Pay(payInfo);
            }
        }
        else if (e.payLimitType == PayLimitType.ChildLimit)
        {
            //未成年本日消费超出
            OnVerificationResultCallBack(new PayResult(-21, m_goodsInfo.goodsID, "今日消费已超出未成年限制"));
        }
        else if (e.payLimitType == PayLimitType.NoRealName)
        {
            //未实名制，无法支付
            OnVerificationResultCallBack(new PayResult(-22, m_goodsInfo.goodsID, "请完成实名制认证后重试"));
        }
        else
        {
            //错误，不应该会进来
        }
    }
}



