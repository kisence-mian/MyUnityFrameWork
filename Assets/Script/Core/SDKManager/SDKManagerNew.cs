using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrameWork.SDKManager
{
    public static class SDKManagerNew
    {
        const string c_callBackObjectName = "CallBackObject";

        static bool isInit = false;
        static SDKCallBackListener s_callBackListener;
        static Dictionary<string, OtherCallBack> s_callBackDict = new Dictionary<string, OtherCallBack>();

        #region 外部调用

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if(!isInit)
            {
                GameObject go = new GameObject(c_callBackObjectName);
                s_callBackListener = go.AddComponent<SDKCallBackListener>();
                s_callBackListener.sdkCallBack = OnSDKCallBack;

                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Init);
                data.Add(SDKInterfaceDefine.ListenerName, c_callBackObjectName);

                Call(data);
                isInit = true;

                ApplicationManager.s_OnApplicationQuit += OnAppplicationQuit;
            }
        }

        public static void Dispose()
        {
            if(isInit)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Dispose);

                Call(data);

                s_callBackListener.sdkCallBack = null;
                GameObject.Destroy(s_callBackListener.gameObject);
                s_callBackListener = null;

                isInit = false;
            }
        }

        #endregion

        #region 登录

        /// <summary>
        /// 登陆,默认访问第一个接口
        /// </summary>
        public static void Login()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Login);
            data.Add(SDKInterfaceDefine.Login_ParameterName_Device, Application.identifier);

            Call(data);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        public static void Login(string SDKName,string tag)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Login);
            data.Add(SDKInterfaceDefine.Login_ParameterName_Device, SystemInfo.deviceUniqueIdentifier);
            data.Add(SDKInterfaceDefine.SDKName, SDKName);
            data.Add(SDKInterfaceDefine.Tag, tag);

            Call(data);
        }

        public static List<LoginPlatform> GetSupportLoginPlatform()
        {
#if UNITY_EDITOR
            return new List<LoginPlatform>();
#elif UNITY_ANDROID

            if (androidInterface == null)
            {
                androidInterface = new AndroidJavaClass("sdkInterface.SdkInterface");
            }
            string result =  androidInterface.CallStatic<String>("GetSupportLoginPlatform");

            if(string.IsNullOrEmpty(result))
            {
                return new List<LoginPlatform>();
            }
            else
            {
                string[] strs = result.Split('|');
                List<LoginPlatform> list = new List<LoginPlatform>();
                for (int i = 0; i < strs.Length; i++)
                {
                    try
                    {
                        list.Add((LoginPlatform)Enum.Parse(typeof(LoginPlatform), strs[i]));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("SDKManagerNew GetSupportLoginPlatform error:" + e.ToString());
                    }
                }

                return list;
            }

#elif UNITY_IOS
             return new List<LoginPlatform>();
#endif

        }

        #endregion

        #region 支付

        /// <summary>
        /// 支付,默认访问第一个接口
        /// </summary>
        public static void Pay(PayInfo payInfo)
        {
            Pay(null, payInfo);
        }

        /// <summary>
        /// 支付,默认访问第一个接口
        /// </summary>
        public static void Pay(string SDKName, PayInfo payInfo)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            if (SDKName != null)
            {
                data.Add(SDKInterfaceDefine.SDKName, SDKName);
            }
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Pay); //支付不传FunctionName

            data.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsID, payInfo.goodsID);
            data.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsType, payInfo.goodsType.ToString());
            data.Add(SDKInterfaceDefine.Pay_ParameterName_OrderID, payInfo.orderID);
            data.Add(SDKInterfaceDefine.Pay_ParameterName_Price, payInfo.price.ToString());
            data.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsName, payInfo.goodsName.ToString());
            data.Add(SDKInterfaceDefine.Pay_ParameterName_Currency, payInfo.currency);
            data.Add(SDKInterfaceDefine.Tag, payInfo.tag);

            Call(data);
        }

        #endregion

        #region 广告

        /// <summary>
        /// 加载广告,默认访问第一个接口
        /// </summary>
        public static void LoadAD(ADType adType, string tag = "")
        {

        }

        /// <summary>
        /// 加载广告,默认访问第一个接口
        /// </summary>
        public static void LoadAD(string SDKName,ADType adType, string tag = "")
        {

        }

        /// <summary>
        /// 显示广告
        /// </summary>
        public static void PlayAD(ADType adType, string tag = "")
        {

        }

        /// <summary>
        /// 显示广告
        /// </summary>
        public static void PlayAD(string SDKName, ADType adType, string tag = "")
        {

        }

        /// <summary>
        /// 隐藏广告
        /// </summary>
        /// <param name="adType"></param>
        public static void CloseAD(ADType adType, string tag = "")
        {

        }

        /// <summary>
        /// 隐藏广告
        /// </summary>
        /// <param name="adType"></param>
        public static void CloseAD(string SDKName, ADType adType, string tag = "")
        {

        }
        /// <summary>
        /// 隐藏广告
        /// </summary>
        /// <param name="adType"></param>
        public static void CloseAD<T>(string SDKName, ADType adType, string tag = "") where T : ADInterface
        {

        }

        #endregion

        #region 数据上报
        public static void Log(string eventID, Dictionary<string, string> data = null)
        {
            Log(eventID, null, data);
        }

        public static void Log(string eventID, string label, Dictionary<string, string> data = null)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();

            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_Event);
            msg.Add(SDKInterfaceDefine.Log_ParameterName_EventID, eventID);
            msg.Add(SDKInterfaceDefine.Log_ParameterName_EventLabel, label);

            if (data != null)
            {
                msg.Add(SDKInterfaceDefine.Log_ParameterName_EventMap, Json.Serialize(data));
            }
            Call(msg);
        }

        public static void LogLogin(string accountId, Dictionary<string, string> data = null)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();

            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_Login);
            msg.Add(SDKInterfaceDefine.Log_ParameterName_AccountId, accountId);

            if (data != null)
            {
                msg.Add(SDKInterfaceDefine.Log_ParameterName_EventMap, Json.Serialize(data));
            }

            Call(msg);
        }

        public static void LogLoginOut(string AccountId, Dictionary<string, string> data = null)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();
            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_LoginOut);
            msg.Add(SDKInterfaceDefine.Log_ParameterName_AccountId, AccountId);

            if (data != null)
            {
                msg.Add(SDKInterfaceDefine.Log_ParameterName_EventMap, Json.Serialize(data));
            }

            Call(msg);
        }

        public static void LogPay(string orderID, string goodsID,int count, float price, string currency, string payment)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();
            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_LogPay);

            msg.Add(SDKInterfaceDefine.Pay_ParameterName_OrderID, orderID);
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsID, goodsID);
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Price, price.ToString());
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Currency, currency);
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Payment, payment);
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Count, count.ToString());

            Call(msg);
        }

        public static void LogPaySuccess(string orderID)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();
            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_LogPaySuccess);

            msg.Add(SDKInterfaceDefine.Pay_ParameterName_OrderID, orderID);

            Call(msg);
        }

        //以下三个配合使用，用于追踪虚拟物品的产出消耗
        public static void LogRewardVirtualCurrency(float count, string reason)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();
            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_RewardVirtualCurrency);

            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Count, count.ToString());
            msg.Add(SDKInterfaceDefine.Log_ParameterName_RewardReason, reason);

            Call(msg);
        }

        public static void LogPurchaseVirtualCurrency(string goodsID, int num, float price)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();
            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_PurchaseVirtualCurrency);

            msg.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsID, goodsID);
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Count, num.ToString());
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Price, price.ToString());

            Call(msg);
        }

        public static void LogUseItem(string goodsID, int num)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>();
            msg.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            msg.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_UseItem);

            msg.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsID, goodsID);
            msg.Add(SDKInterfaceDefine.Pay_ParameterName_Count, num.ToString());

            Call(msg);
        }

        #endregion

        #region 其他

        public static void Exit()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Other);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Other_FunctionName_Exit);
            Call(data);
        }

        public static void ToClipboard(String content)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Other);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Other_FunctionName_CopyToClipboard);
            data.Add(SDKInterfaceDefine.Other_ParameterName_Content, content);
            Call(data);
        }

        public static void DownloadApk(string url = null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Other);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Other_FunctionName_DownloadAPK);
            if(url != null)
            {
                data.Add(SDKInterfaceDefine.Other_ParameterName_DownloadURL, url);
            }

            Call(data);
        }

        public static void GetAPKSize(string url = null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Other);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Other_FunctionName_GetAPKSize);
            if (url != null)
            {
                data.Add(SDKInterfaceDefine.Other_ParameterName_DownloadURL, url);
            }

            Call(data);
        }

        #region 事件监听

        public static void AddOtherCallBackListener(string functionName,OtherCallBack callBack)
        {
            if(s_callBackDict.ContainsKey(functionName))
            {
                s_callBackDict[functionName] += callBack;
            }
            else
            {
                s_callBackDict.Add(functionName, callBack);
            }
        }

        public static void RemoveOtherCallBackListener(string functionName, OtherCallBack callBack)
        {
            if (s_callBackDict.ContainsKey(functionName))
            {
                s_callBackDict[functionName] -= callBack;
            }
            else
            {
                Debug.LogError("RemoveOtherCallBackListener 不存在的 function Name ->" + functionName + "<-");
            }
        }

        #endregion

        #endregion 特殊回调

        static void OnAppplicationQuit()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_LifeCycle);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.LifeCycle_FunctionName_OnApplicationQuit);

            Call(data);
        }

        #region 

        #endregion

        #endregion

        #region 回调接口

        static void OnSDKCallBack(Dictionary<string, string> data)
        {
            string mouduleName = data[SDKInterfaceDefine.ModuleName];

            switch (mouduleName)
            {
                case SDKInterfaceDefine.ModuleName_Debug:
                    OnDebug(data);
                    break;
                case SDKInterfaceDefine.ModuleName_Login:
                    OnLogin(data);
                    break;
                case SDKInterfaceDefine.ModuleName_Pay:
                    OnPay(data);
                    break;
                case SDKInterfaceDefine.ModuleName_AD:
                    OnAD(data);
                    break;
                case SDKInterfaceDefine.ModuleName_Log:
                    OnLog(data);
                    break;
                case SDKInterfaceDefine.ModuleName_Other:
                    OnOther(data);
                    break;
            }
        }



        #endregion

        #region SDK接口

#if UNITY_EDITOR

#elif UNITY_ANDROID

        static AndroidJavaClass androidInterface = null;

#elif UNITY_IOS

#endif

        static void Call(Dictionary<string, string> data)
        {
            try
            {
#if UNITY_EDITOR
                //Debug.LogWarning("SDKManagerNew Call 目前是在 Editor 下运行 ->" + content);

#elif UNITY_ANDROID
            string content = Serializer.Serialize(data);
            if (androidInterface == null)
            {
                androidInterface = new AndroidJavaClass("sdkInterface.SdkInterface");
            }

            if (androidInterface != null)
            {
                androidInterface.CallStatic("UnityRequestFunction", content);
            }

#elif UNITY_IOS

#endif
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManagerNew Call Error " + e);
            }
        }

#endregion

        #region 回调处理

#region Debug

        static void OnDebug(Dictionary<string, string> data)
        {
            string functionName = data[SDKInterfaceDefine.FunctionName];
            string content = "SDKManagerNew "+ functionName + " : " + data[SDKInterfaceDefine.ParameterName_Content];

            switch(functionName)
            {
                case SDKInterfaceDefine.FunctionName_OnError:Debug.LogError(content);break;
                case SDKInterfaceDefine.FunctionName_OnLog: Debug.Log(content); break;
            }
        }

#endregion

#region Login

        //public static LoginCallBack OnLoginCallBack;

        static void OnLogin(Dictionary<string, string> data)
        {
            bool isSuccess = bool.Parse(data[SDKInterfaceDefine.ParameterName_IsSuccess]);
            string accountId = data[SDKInterfaceDefine.Login_ParameterName_AccountId];

            OnLoginInfo info = new OnLoginInfo();
            info.isSuccess = isSuccess;
            info.accountId = accountId;

            Debug.Log("SDKManagerNew OnLogin " + accountId);

            if (data.ContainsKey(SDKInterfaceDefine.Login_ParameterName_loginPlatform))
            {
                info.loginPlatform = (LoginPlatform)Enum.Parse(typeof(LoginPlatform), data[SDKInterfaceDefine.Login_ParameterName_loginPlatform]);
            }

            if (SDKManager.LoginCallBack != null)
            {
                try
                {
                    SDKManager.LoginCallBack(info);
                }
                catch (Exception e)
                {
                    Debug.LogError("OnLogin Error " + e.ToString());
                }
            }
        }

#endregion

#region Pay

        static void OnPay(Dictionary<string, string> data)
        {
            bool isSuccess = bool.Parse(data[SDKInterfaceDefine.ParameterName_IsSuccess]);

            StoreName storeName = (StoreName)Enum.Parse(typeof(StoreName), data[SDKInterfaceDefine.Pay_ParameterName_Payment]);
            string goodsId = data[SDKInterfaceDefine.Pay_ParameterName_GoodsID];
            string orderID = data[SDKInterfaceDefine.Pay_ParameterName_OrderID];
            string currency = data[SDKInterfaceDefine.Pay_ParameterName_Currency];
            string goodsName = data[SDKInterfaceDefine.Pay_ParameterName_GoodsName];
            string receipt = data[SDKInterfaceDefine.Pay_ParameterName_Receipt];
            float price = float.Parse(data[SDKInterfaceDefine.Pay_ParameterName_Price]);
            GoodsType goodsType =(GoodsType) Enum.Parse(typeof(GoodsType), data[SDKInterfaceDefine.Pay_ParameterName_GoodsType]);

            OnPayInfo info = new OnPayInfo();
            info.isSuccess = isSuccess;
            info.storeName = storeName;
            info.goodsId = goodsId;
            info.orderID = orderID;
            info.currency = currency;
            info.goodsName = goodsName;
            info.receipt = receipt;
            info.price = price;
            info.goodsType = goodsType;

            try
            {
                if(SDKManager.PayCallBack!= null)
                {
                    SDKManager.PayCallBack(info);
                }
            }
            catch(Exception e)
            {
                Debug.LogError("OnPayCallBack Error" + e.ToString() + e.StackTrace);
            }
        }

#endregion

#region AD

        static void OnAD(Dictionary<string, string> data)
        {

        }

#endregion

#region Log

        static void OnLog(Dictionary<string, string> data)
        {

        }

#endregion

#region Other

        static void OnOther(Dictionary<string, string> data)
        {
            OnOtherInfo info = new OnOtherInfo();
            info.data = data;

            string functionName = data[SDKInterfaceDefine.FunctionName];

            DispatchOtherCallBack(functionName, info);
        }

        static void DispatchOtherCallBack(string functionName, OnOtherInfo info)
        {
            if(s_callBackDict.ContainsKey(functionName))
            {
                try
                {
                    if (s_callBackDict[functionName] != null)
                    {
                        s_callBackDict[functionName](info);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("DispatchOtherCallBack Error " + e.ToString());
                }
            }
        }

        public static string GetProperties(string properties, string key, string defaultValue)
        {
#if UNITY_EDITOR
            return defaultValue;

#elif UNITY_ANDROID
            try
            {
                if (androidInterface == null)
                {
                    androidInterface = new AndroidJavaClass("sdkInterface.SdkInterface");
                }

                if (androidInterface != null)
                {
                    return androidInterface.CallStatic<string>("GetProperties", properties, key, defaultValue);
                }
                else
                {
                    return defaultValue;
                }
            }
            catch(Exception e)
            {
                Debug.LogError("GetProperties Error:" + e.ToString());
                return defaultValue;
            }

#else
             return defaultValue;
#endif

        }
#endregion

#endregion
    }

#region 声明

    public class SDKCallBackListener : MonoBehaviour
    {
        Deserializer deserializer = new Deserializer();

        public CallBack<Dictionary<string, string>> sdkCallBack;

        public void OnSDKCallBack(string str)
        {
            Dictionary<string, string> data = deserializer.Deserialize<Dictionary<string, string>>(str);

            sdkCallBack(data);
        }
    }

    public struct OnPayInfo
    {
        //必填
        public bool isSuccess;
        public string goodsId;
        public string orderID;
        public StoreName storeName;

        public string goodsName;
        public float price;
        public string currency;   //货币类型

        public string receipt; //支付回执

        //选填
        public GoodsType goodsType;
    }

    public struct OnADInfo
    {
        public ADType aDType;
        public string tag;
    }

    public struct OnLoginInfo
    {
        public bool isSuccess;
        public string accountId;
        public string nickName;
        public string headPortrait;
        public string password;
        public LoginPlatform loginPlatform;
        public LoginErrorEnum error;
    }

    public struct OnOtherInfo
    {
        public Dictionary<string, string> data;
    }

    public enum GoodsType
    {
        NORMAL,    //可以反复购买的商品
        ONCE_ONLY, //只能购买一次的商品
        RIGHTS,    //购买持续一段时间的商品，例如会员
    }

    public enum LoginErrorEnum
    {
        None,
        GameCenterNotOpen,
        /// <summary>
        /// 没有安装相应的app
        /// </summary>
        NoInstallApp,
    }

    public struct PayInfo
    {
        public string goodsID;
        public string goodsName;
        public string tag;
        public GoodsType goodsType ;
        public string orderID ;
        public float price ;
        public string currency;   //货币类型

        public PayInfo(string goodsID, string goodsName, string tag, GoodsType goodsType, string orderID, float price,string currency)
        {
            this.goodsID = goodsID;
            this.goodsName = goodsName;
            this.tag = tag;
            this.goodsType = goodsType;
            this.orderID = orderID;
            this.price = price;
            this.currency = currency;
        }
    }

#endregion
}