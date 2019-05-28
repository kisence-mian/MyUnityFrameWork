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
            data.Add(SDKInterfaceDefine.Login_ParameterName_Device, Application.identifier);
            data.Add(SDKInterfaceDefine.SDKName, SDKName);
            data.Add(SDKInterfaceDefine.Tag, tag);

            Call(data);
        }

        #endregion

        #region 支付

        /// <summary>
        /// 支付,默认访问第一个接口
        /// </summary>
        public static void Pay(string goodsID, string tag,GoodsType goodsType = GoodsType.NORMAL,string orderID = null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Pay); //支付不传FunctionName

            data.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsID, goodsID);
            data.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsType, goodsType.ToString());
            data.Add(SDKInterfaceDefine.Pay_ParameterName_CpOrderID, orderID);
            data.Add(SDKInterfaceDefine.Tag, tag);

            Call(data);
        }

        /// <summary>
        /// 支付,默认访问第一个接口
        /// </summary>
        public static void Pay(string SDKName,string goodsID,string tag, GoodsType goodsType = GoodsType.NORMAL, string orderID = null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add(SDKInterfaceDefine.SDKName, SDKName);
            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Pay); //支付不传FunctionName

            data.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsID, goodsID);
            data.Add(SDKInterfaceDefine.Pay_ParameterName_GoodsType, goodsType.ToString());
            data.Add(SDKInterfaceDefine.Pay_ParameterName_CpOrderID, orderID);
            data.Add(SDKInterfaceDefine.Tag, tag);

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

        public static void Log_Login(string AccountId, Dictionary<string, string> data = null)
        {
            if(data == null)
            {
                data = new Dictionary<string, string>();
            }

            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_Login);
            data.Add(SDKInterfaceDefine.Log_ParameterName_AccountId, AccountId);

            Call(data);
        }

        public static void Log_Loginout(string AccountId = null, Dictionary<string, string> data = null)
        {
            if (data == null)
            {
                data = new Dictionary<string, string>();
            }

            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_LoginOut);
            data.Add(SDKInterfaceDefine.Log_ParameterName_AccountId, AccountId);

            Call(data);
        }

        public static void Log(string eventID, Dictionary<string, string> data = null)
        {
            Log(eventID, null, data);
        }

        public static void Log(string eventID, string label ,Dictionary<string, string> data = null)
        {
            if (data == null)
            {
                data = new Dictionary<string, string>();
            }

            data.Add(SDKInterfaceDefine.ModuleName, SDKInterfaceDefine.ModuleName_Log);
            data.Add(SDKInterfaceDefine.FunctionName, SDKInterfaceDefine.Log_FunctionName_Event);
            data.Add(SDKInterfaceDefine.Log_ParameterName_EventID, eventID);
            data.Add(SDKInterfaceDefine.Log_ParameterName_EventLabel, label);

            Call(data);
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

        #if UNITY_ANDROID

        static AndroidJavaClass androidInterface = null;

        #elif UNITY_IOS

        #endif

        static void Call(Dictionary<string, string> data)
        {
            string content = Serializer.Serialize(data);

#if UNITY_EDITOR
            Debug.LogWarning("SDKManagerNew Call 目前是在 Editor 下运行 ->" + content);

#elif UNITY_ANDROID

            if (androidInterface == null)
            {
                androidInterface = new AndroidJavaClass("SdkInterface.SdkInterface");
            }
            androidInterface.CallStatic("UnityRequestFunction", content);

#elif UNITY_IOS

#endif
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

        public static CallBack<OnLoginInfo> OnLoginCallBack;

        static void OnLogin(Dictionary<string, string> data)
        {
            bool isSuccess = bool.Parse(data[SDKInterfaceDefine.ParameterName_IsSuccess]);
            string accountId = data[SDKInterfaceDefine.Login_ParameterName_AccountId];

            OnLoginInfo info = new OnLoginInfo();
            info.isSuccess = isSuccess;
            info.accountId = accountId;

            OnLoginCallBack(info);
        }

        #endregion

        #region Pay

        public static CallBack<OnPayInfo> OnPayCallBack;

        static void OnPay(Dictionary<string, string> data)
        {
            bool isSuccess = bool.Parse(data[SDKInterfaceDefine.ParameterName_IsSuccess]);
            string goodsId = data[SDKInterfaceDefine.Pay_ParameterName_GoodsID];

            OnPayInfo info = new OnPayInfo();
            info.isSuccess = isSuccess;
            info.goodsId = goodsId;

            try
            {
                if(OnPayCallBack!= null)
                {
                    OnPayCallBack(info);
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
                    androidInterface = new AndroidJavaClass("SdkInterface.SdkInterface");
                }
                return androidInterface.CallStatic<string>("GetProperties", properties, key, defaultValue);
            }
            catch(Exception e)
            {
                Debug.LogError("GetProperties Error:" + e.ToString());
                return defaultValue;
            }
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
        public bool isSuccess;
        public string goodsId;
        public GoodsType goodsType;
        public StoreName storeName;
        public string receipt;
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

#endregion
}