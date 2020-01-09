﻿using FrameWork;
using FrameWork.SDKManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace FrameWork.SDKManager
{
    public static class SDKManager
    {
        static bool isInit = false;
        public static string UserID;

#if UNITY_ANDROID
        public const string c_ConfigName = "SDKConfig_Android";
#elif UNITY_IOS
        public const string c_ConfigName = "SDKConfig_IOS";
#else
        public const string c_ConfigName = "SDKConfig";
#endif

        public const string c_KeyName = "SDKInfo";

        static List<LoginInterface> s_loginServiceList = null;
        static List<PayInterface> s_payServiceList = null;
        static List<ADInterface> s_ADServiceList = null;
        static List<LogInterface> s_logServiceList = null;
        static List<OtherSDKInterface> s_otherServiceList = null;
        private static PayCallBack s_payCallBack;
        private static ADCallBack s_adCallBack;
        private static GoodsInfoCallBack s_goodsInfoCallBack;

        static Dictionary<string, OtherCallBack> s_callBackDict = new Dictionary<string, OtherCallBack>();

        static bool s_useNewSDKManager = false; //是否使用新版本SDKManager

        #region 属性

        public static LoginCallBack LoginCallBack { get; set; }

        public static PayCallBack PayCallBack
        {
            get
            {
                return s_payCallBack;
            }

            set
            {
                s_payCallBack = value;
            }
        }
        public static ADCallBack ADCallBack
        {
            get
            {
                return s_adCallBack;
            }

            set
            {
                s_adCallBack = value;
            }
        }

        public static GoodsInfoCallBack GoodsInfoCallBack
        {
            get
            {
                return s_goodsInfoCallBack;
            }

            set
            {
                s_goodsInfoCallBack = value;
            }
        }

        #endregion

        #region 外部调用

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            if (!isInit)
            {
                isInit = true;

                try
                {
                    if (ConfigManager.GetIsExistConfig(c_ConfigName))
                    {
                        SchemeData data = LoadGameSchemeConfig();

                        s_useNewSDKManager = data.UseNewSDKManager;

                        if (s_useNewSDKManager)
                        {
                            SDKManagerNew.Init();
                        }

                        //Debug.Log("SDKManager Init");
                        LoadService(data);
                        InitSDK();
                        AutoListenerInit();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Init Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 额外初始化，SDKName == null时初始化全体
        /// </summary>
        public static void ExtraInit(SDKType type, string sdkName = null, string tag = null)
        {
            if (sdkName != null)
            {
                switch (type)
                {
                    case SDKType.AD:
                        GetADService(sdkName).ExtraInit(tag); break;
                    case SDKType.Log:
                        GetLogService(sdkName).ExtraInit(tag); break;
                    case SDKType.Login:
                        GetLoginService(sdkName).ExtraInit(tag); break;
                    case SDKType.Other:
                        GetOtherService(sdkName).ExtraInit(tag); break;
                    case SDKType.Pay:
                        GetPayService(sdkName).ExtraInit(tag); break;
                }
            }
            else
            {
                switch (type)
                {
                    case SDKType.AD:
                        AllExtraInit(s_ADServiceList, tag); break;
                    case SDKType.Log:
                        AllExtraInit(s_logServiceList, tag); break;
                    case SDKType.Login:
                        AllExtraInit(s_loginServiceList, tag); break;
                    case SDKType.Other:
                        AllExtraInit(s_otherServiceList, tag); break;
                    case SDKType.Pay:
                        AllExtraInit(s_payServiceList, tag); break;
                }
            }
        }

        static void AllExtraInit<T>(List<T> list, string tag = null) where T : SDKInterfaceBase
        {
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    list[i].ExtraInit(tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("AllExtraInit Exception " + list[i].m_SDKName + " " + e.ToString());
                }
            }
        }

        static void CheckInit()
        {
            if(!isInit)
            {
                throw new Exception("SDKManager not init !");
            }
        }

        #endregion

        #region 获取SDKInterface

        public static LoginInterface GetLoginService<T>() where T : LoginInterface
        {
            return GetLoginService(typeof(T).Name);
        }

        public static LoginInterface GetLoginService(string SDKName)
        {
            return GetSDKService(s_loginServiceList, SDKName);
        }

        public static LoginInterface GetLoginService(int index)
        {
            if (s_loginServiceList.Count <= index)
            {
                throw new Exception("GetLoginService error index->" + index + " count->" + s_loginServiceList.Count);
            }

            return s_loginServiceList[index];
        }

        public static PayInterface GetPayService<T>() where T : PayInterface
        {
            return GetPayService(typeof(T).Name);
        }

        public static PayInterface GetPayService(string SDKName)
        {
            return GetSDKService(s_payServiceList, SDKName);
        }

        public static bool GetHasPayService(string SDKName)
        {
            return GetHasSDKService(s_payServiceList, SDKName);
        }

        public static PayInterface GetPayService(int index)
        {
            if (s_payServiceList.Count <= index)
            {
                throw new Exception("GetPayService error index->" + index + " count->" + s_payServiceList.Count);
            }

            return s_payServiceList[index];
        }

        public static ADInterface GetADService<T>() where T : ADInterface
        {
            return GetADService(typeof(T).Name);
        }

        public static ADInterface GetADService(string SDKName)
        {
            return GetSDKService(s_ADServiceList, SDKName);
        }

        public static ADInterface GetADService(int index)
        {
            if (s_ADServiceList.Count <= index)
            {
                throw new Exception("GetADService error index->" + index + " count->" + s_ADServiceList.Count);
            }

            return s_ADServiceList[index];
        }

        public static LogInterface GetLogService<T>() where T : LogInterface
        {
            return GetLogService(typeof(T).Name);
        }

        public static LogInterface GetLogService(string SDKName)
        {
            return GetSDKService(s_logServiceList, SDKName);
        }

        public static LogInterface GetLogService(int index)
        {
            if (s_logServiceList.Count <= index)
            {
                throw new Exception("GetLogService error index->" + index + " count->" + s_logServiceList.Count);
            }

            return s_logServiceList[index];
        }

        public static OtherSDKInterface GetOtherService<T>() where T : OtherSDKInterface
        {
            return GetOtherService(typeof(T).Name);
        }

        public static OtherSDKInterface GetOtherService(string SDKName)
        {
            return GetSDKService(s_otherServiceList, SDKName);
        }

        public static OtherSDKInterface GetOtherService(int index)
        {
            if (s_otherServiceList.Count <= index)
            {
                throw new Exception("GetOtherService error index->" + index + " count->" + s_otherServiceList.Count);
            }

            return s_otherServiceList[index];
        }

        #endregion

        #region 登录

        static void InitLogin(List<LoginInterface> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    //list[i].m_SDKName = list[i].GetType().Name;
                    list[i].Init();
                    //s_loginCallBack += list[i].m_callBack;
                }
                catch (Exception e)
                {
                    Debug.LogError("Init LoginInterface SDK Exception:\n" + e.ToString());
                }
            }
        }

        /// <summary>
        /// 登陆,默认访问第一个接口
        /// </summary>
        public static void Login(string tag = "")
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.Login();
            }
            else
            {
                try
                {
                    GetLoginService(0).Login(tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Login Exception: " + e.ToString());
                }
            }
        }

        public static void LoginOut(string SDKName = null,String tag = null)
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LoginOut(SDKName,tag);
            }
            else
            {
                try
                {
                    GetLoginService(SDKName).LoginOut(tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Login Exception: " + SDKName + "===" + e.ToString());
                }
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        public static void LoginBySDKName(string SDKName, string tag = "")
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.Login(SDKName, tag);
            }
            else
            {
                try
                {
                    GetLoginService(SDKName).Login(tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Login Exception: " + SDKName + "===" + e.ToString());
                }
            }
        }
        /// <summary>
        /// 登陆
        /// </summary>
        public static void LoginByPlatform(LoginPlatform loginPlatform, string tag = "")
        {
            try
            {
                bool isHave = false;
                foreach (var item in s_loginServiceList)
                {
                    if (item.GetPlatform().Contains(Application.platform) && item.GetLoginPlatform() == loginPlatform)
                    {
                        item.Login(tag);
                        isHave = true;
                    }
                }

                if (!isHave)
                {
                    if (s_useNewSDKManager)
                    {
                        Debug.LogWarning(loginPlatform);
                        SDKManagerNew.Login(loginPlatform.ToString(), tag);

                    }
                    else
                    {
                        Debug.LogError("SDKManager Login dont find class by platform:" + Application.platform + " loginPlatform:" + loginPlatform);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager Login Exception: " + e.ToString());
            }
        }
        public static List<LoginPlatform> GetSupportLoginPlatform()
        {
            List<LoginPlatform> platforms = new List<LoginPlatform>();

            try
            {
                foreach (var item in s_loginServiceList)
                {
                    if (item.GetPlatform().Contains(Application.platform))
                    {
                        platforms.Add(item.GetLoginPlatform());
                    }
                }

                if (s_useNewSDKManager)
                {
                    List<LoginPlatform> newList = SDKManagerNew.GetSupportLoginPlatform();

                    for (int i = 0; i < newList.Count; i++)
                    {
                        if (!platforms.Contains(newList[i]))
                        {
                            platforms.Add(newList[i]);
                        }
                    }
                }

                if (platforms.Count == 0)
                {
                    Debug.LogError("SDKManager Login dont find class by platform:" + Application.platform + " please check config");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager Login Exception: " + e.ToString());
            }

            return platforms;
        }

        #endregion

        #region 支付

        static void InitPay(List<PayInterface> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    //list[i].m_SDKName = list[i].GetType().Name;
                    list[i].Init();
                    //list[i].m_PayResultCallBack= s_payCallBack;
                    //Debug.Log("初始化支付回调");
                }
                catch (Exception e)
                {
                    Debug.LogError("Init PayInterface SDK Exception:\n" + e.ToString() + " " + list[i].m_SDKName);
                }
            }

            PublicPayClass publicPayInterface = new PublicPayClass();
            publicPayInterface.Init();
            list.Add(publicPayInterface);
        }

        /// <summary>
        /// 支付,默认访问第一个接口
        /// </summary>
        public static void Pay(PayInfo payInfo)
        {
            //优先使用本地配置的SDK
            if (s_payServiceList.Count > 0)
            {
                try
                {
                    GetPayService(0).Pay(payInfo);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Pay Exception: " + e.ToString());
                }
            }

            else if (s_useNewSDKManager)
            {
                if (GetPrePay(""))
                {
                    try
                    {
                        GetPayService("PublicPayClass").Pay(payInfo);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("SDKManager PublicPayClass Exception: " + e.ToString());
                    }
                }
                else {
                    SDKManagerNew.Pay(payInfo);
                }

            }
            else
            {
                Debug.Log("支付SDK 没有配置！ ");
            }
        }

        /// <summary>
        /// 支付
        /// </summary>
        public static void Pay(string SDKName, PayInfo payInfo )
        {
            Debug.Log("Pay  SDKname " + SDKName + " GetHasSDKService " + GetHasSDKService(s_payServiceList, SDKName));

            //优先使用本地配置的SDK
            if (GetHasSDKService(s_payServiceList, SDKName))
            {
                try
                {
                    GetPayService(SDKName).Pay(payInfo);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Pay Exception: " + e.ToString());
                }
            }

            else if (s_useNewSDKManager)
            {

                if (GetPrePay(SDKName))
                {
                    try
                    {
                        GetPayService("PublicPayClass").Pay(payInfo);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("SDKManager PublicPayClass Exception: " + e.ToString());
                    }
                }
                else
                {
                    SDKManagerNew.Pay(SDKName, payInfo);
                }

            }
            else
            {
                Debug.LogError("支付SDK 没有配置！ ");
            }
        }

        /// <summary>
        /// 支付,默认访问第一个接口
        /// </summary>
        public static void ConfirmPay(string orderID, string tag = "")
        {
            try
            {
                GetPayService(0).ConfirmPay(orderID, tag);
               
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager Pay Exception: " + e.ToString());
            }
        }

        /// <summary>
        /// 支付
        /// </summary>
        public static void ConfirmPay(string SDKName, string orderID, string tag = "")
        {
            try
            {
                GetPayService(SDKName).ConfirmPay(orderID, tag);
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager Pay Exception: " + e.ToString());
            }
        }

        public static LocalizedGoodsInfo GetGoodsInfo(string goodsID, string tag = "")
        {
            try
            {
                return GetPayService(0).GetGoodsInfo(goodsID);
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager GetGoodsInfo Exception: " + e.ToString());
            }
            return null;
        }

        public static LocalizedGoodsInfo GetGoodsInfo(string SDKName, string goodsID, string tag = "")
        {
            try
            {
                return GetPayService(SDKName).GetGoodsInfo(goodsID);
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager GetGoodsInfo Exception: " + e.ToString());
            }

            return null;
        }

        public static List<LocalizedGoodsInfo> GetAllGoodsInfo(string tag = "")
        {
            try
            {
                return GetPayService(0).GetAllGoodsInfo();
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager GetGoodsInfo Exception: " + e.ToString());
            }
            return null;
        }

        public static List<LocalizedGoodsInfo> GetAllGoodsInfo(string SDKName, string tag = "")
        {
            try
            {
                return GetPayService(SDKName).GetAllGoodsInfo();
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager GetGoodsInfo Exception: " + e.ToString());
            }

            return null;
        }

        public static bool GetPrePay(String SDKName)
        {
            return SDKManagerNew.GetPrePay(SDKName);
        }

        public static bool GetReSendPay(String SDKName)
        {
            return SDKManagerNew.GetReSendPay(SDKName);
        }

        #endregion

        #region 广告

        /// <summary>
        /// 加载广告,默认访问第一个接口
        /// </summary>
        public static void LoadAD(ADType adType, string tag = "")
        {
            //读取注入配置
            string sdkName = GetProperties(SDKInterfaceDefine.PropertiesKey_ADPlatform, null);

            LoadAD(sdkName,adType,tag);
        }

        /// <summary>
        /// 加载广告
        /// </summary>
        public static void LoadAD(string SDKName, ADType adType, string tag = "")
        {
            if (GetHasSDKService(s_ADServiceList, SDKName))
            {
                try
                {
                    GetADService(SDKName).LoadAD(adType, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Pay Exception: " + e.ToString());
                }
            }

            else if (s_useNewSDKManager)
            {
                SDKManagerNew.LoadAD(SDKName, adType, tag);
            }
            else
            {
                Debug.LogError("广告SDK 没有配置！ ");
            }
        }

        /// <summary>
        /// 广告已加载成功,默认访问第一个接口
        /// </summary>
        public static bool ADIsLoaded(ADType adType, string tag = "")
        {
            //读取注入配置
            string sdkName = GetProperties(SDKInterfaceDefine.PropertiesKey_ADPlatform, null);

            return ADIsLoaded(sdkName, adType, tag);
        }

        /// <summary>
        /// 加载广告成功
        /// </summary>
        public static bool ADIsLoaded(string SDKName, ADType adType, string tag = "")
        {
            try
            {
                if (GetHasSDKService(s_ADServiceList, SDKName))
                {
                    return GetADService(SDKName).IsLoaded(adType, tag);
                }
                else
                {
                    if (s_useNewSDKManager)
                    {
                        return SDKManagerNew.ADIsLoaded(SDKName,adType, tag); ;
                    }
                    else
                    {
                        Debug.LogError("SDKManager ADIsLoaded Not find: " + SDKName);
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager ADIsLoaded Exception: " + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 显示广告
        /// </summary>
        public static void PlayAD(ADType adType, string tag = "")
        {
            string sdkName = GetProperties(SDKInterfaceDefine.PropertiesKey_ADPlatform, null);
            PlayAD(sdkName, adType, tag);
        }

        /// <summary>
        /// 显示广告
        /// </summary>
        public static void PlayAD(string SDKName, ADType adType, string tag = "")
        {
            try
            {
                if (GetHasSDKService(s_ADServiceList, SDKName))
                {
                    GetADService(SDKName).PlayAD(adType, tag);
                }
                else
                {
                    if (s_useNewSDKManager)
                    {
                        SDKManagerNew.PlayAD(SDKName, adType, tag);
                    }
                    else
                    {
                        Debug.LogError("SDKManager PlayAD Not find: " + SDKName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager PlayAD Exception: " + e.ToString());
            }
        }

        /// <summary>
        /// 隐藏广告
        /// </summary>
        /// <param name="adType"></param>
        public static void CloseAD(ADType adType, string tag = "")
        {
            string sdkName = GetProperties(SDKInterfaceDefine.PropertiesKey_ADPlatform, null);
            CloseAD(sdkName, adType, tag);
        }

        /// <summary>
        /// 隐藏广告
        /// </summary>
        /// <param name="adType"></param>
        public static void CloseAD(string SDKName, ADType adType, string tag = "")
        {
            if (GetHasSDKService(s_ADServiceList, SDKName))
            {
                GetADService(SDKName).CloseAD(adType, tag);
            }
            else
            {
                if (s_useNewSDKManager)
                {
                    SDKManagerNew.CloseAD(SDKName, adType, tag);
                }
                else
                {
                    Debug.LogError("SDKManager CloseAD Not find: " + SDKName);
                }
            }
        }

        #endregion

        #region 数据上报

        /// <summary>
        /// 数据上报
        /// </summary>
        /// <param name="data"></param>
        public static void Log(string eventID, Dictionary<string, string> data)
        {
            CheckInit();

            if (s_useNewSDKManager)
            {
                SDKManagerNew.Log(eventID, data);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].Log(eventID, data);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Log Exception: " + e.ToString());
                }
            }
        }

        public static void LogLogin(string accountID, Dictionary<string, string> data = null)
        {
            CheckInit();
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LogLogin(accountID, data);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].LogLogin(accountID, data);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager LogLogin Exception: " + e.ToString());
                }
            }
        }

        public static void LogLoginOut(string accountID)
        {
            CheckInit();
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LogLoginOut(accountID);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].LogLoginOut(accountID);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager LogLoginOut Exception: " + e.ToString());
                }
            }
        }

        public static void LogPay(string orderID, string goodsID,int count, float price, string currency, string payment)
        {
            CheckInit();
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LogPay(orderID, goodsID, count, price, currency,payment);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].LogPay(orderID, goodsID, count, price, currency, payment);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager LogPay Exception: " + e.ToString());
                }
            }
        }

        public static void LogPaySuccess(string orderID)
        {
            CheckInit();
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LogPaySuccess(orderID);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].LogPaySuccess(orderID);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager LogPaySuccess Exception: " + e.ToString());
                }
            }
        }

        //以下三个配合使用，用于追踪虚拟物品的产出消耗
        public static void LogRewardVirtualCurrency(float count, string reason)
        {
            CheckInit();
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LogRewardVirtualCurrency(count, reason);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].LogRewardVirtualCurrency(count, reason);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager RewardVirtualCurrency Exception: " + e.ToString());
                }
            }
        }

        public static void LogPurchaseVirtualCurrency(string goodsID, int num, float price)
        {
            CheckInit();
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LogPurchaseVirtualCurrency(goodsID,  num,  price);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].LogPurchaseVirtualCurrency(goodsID, num, price);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager PurchaseVirtualCurrency Exception: " + e.ToString());
                }
            }
        }

        public static void LogUseItem(string goodsID, int num)
        {
            CheckInit();
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LogUseItem(goodsID, num);
            }

            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                try
                {
                    s_logServiceList[i].LogUseItem(goodsID, num);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager LogUseItem Exception: " + e.ToString());
                }
            }
        }

        #endregion

        #region 其他SDK

        public static void ToClipboard(string content)
        {
#if UNITY_EDITOR
            GUIUtility.systemCopyBuffer = content;

#elif UNITY_ANDROID

            SDKManagerNew.ToClipboard(content);
#elif UNITY_IOS
            ClipboardManager.ToClipboard(content);
#endif
        }

        public static void DownloadApk(string url = null)
        {
#if UNITY_ANDROID

            SDKManagerNew.DownloadApk(url);
#endif
        }

        public static void GetAPKSize(string url = null)
        {
#if UNITY_ANDROID

            SDKManagerNew.GetAPKSize(url);
#endif
        }

        /// <summary>
        /// 读取注入配置
        /// </summary>
        public static string GetProperties(string key,string defaultValue = "")
        {
           return GetProperties(SDKInterfaceDefine.FileName_ChannelProperties,key, defaultValue);
        }

        /// <summary>
        /// 读取注入配置
        /// </summary>
        public static string GetProperties(string properties, string key, string defaultValue)
        {
            return SDKManagerNew.GetProperties(properties, key, defaultValue);
        }

        /// <summary>
        /// 游戏关闭
        /// </summary>
        public static void QuitApplication()
        {
            SDKManagerNew.QuitApplication();
        }

        #region 事件监听

        public static void AddOtherCallBackListener(string functionName, OtherCallBack callBack)
        {
            if (s_callBackDict.ContainsKey(functionName))
            {
                s_callBackDict[functionName] += callBack;
            }
            else
            {
                s_callBackDict.Add(functionName, callBack);
            }

            if(s_useNewSDKManager)
            {
                SDKManagerNew.AddOtherCallBackListener(functionName, callBack);
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

            if (s_useNewSDKManager)
            {
                SDKManagerNew.RemoveOtherCallBackListener(functionName, callBack);
            }
        }


        #endregion

        #endregion

    #endregion

        #region 加载SDK设置

        /// <summary>
        /// 读取当前游戏内的SDK配置
        /// 找不到或者解析失败会返回Null
        /// </summary>
        /// <returns></returns>
        public static SchemeData LoadGameSchemeConfig()
        {
            if (ConfigManager.GetIsExistConfig(c_ConfigName))
            {
                //Debug.Log("LoadGameSchemeConfig");

                Dictionary<string, SingleField> configData = ConfigManager.GetData(c_ConfigName);
                return JsonUtility.FromJson<SchemeData>(configData[c_KeyName].GetString());
            }
            else
            {
                Debug.Log("LoadGameSchemeConfig null");

                return null;
            }
        }

        public static void AnalyzeSchemeData(
            SchemeData schemeData,

            out List<LoginInterface> loginScheme,
            out List<ADInterface> ADScheme,
            out List<PayInterface> payScheme,
            out List<LogInterface> logScheme,
            out List<OtherSDKInterface> otherScheme
            )
        {
            if (schemeData != null)
            {
                loginScheme = new List<LoginInterface>();
                for (int i = 0; i < schemeData.LoginScheme.Count; i++)
                {
                    loginScheme.Add((LoginInterface)AnalysisConfig(schemeData.LoginScheme[i]));
                }

                ADScheme = new List<ADInterface>();
                for (int i = 0; i < schemeData.ADScheme.Count; i++)
                {
                    ADScheme.Add((ADInterface)AnalysisConfig(schemeData.ADScheme[i]));
                }

                payScheme = new List<PayInterface>();
                for (int i = 0; i < schemeData.PayScheme.Count; i++)
                {
                    payScheme.Add((PayInterface)AnalysisConfig(schemeData.PayScheme[i]));
                }

                logScheme = new List<LogInterface>();
                for (int i = 0; i < schemeData.LogScheme.Count; i++)
                {
                    logScheme.Add((LogInterface)AnalysisConfig(schemeData.LogScheme[i]));
                }

                otherScheme = new List<OtherSDKInterface>();
                for (int i = 0; i < schemeData.OtherScheme.Count; i++)
                {
                    otherScheme.Add((OtherSDKInterface)AnalysisConfig(schemeData.OtherScheme[i]));
                }
            }
            else
            {
                loginScheme = new List<LoginInterface>();
                ADScheme = new List<ADInterface>();
                payScheme = new List<PayInterface>();
                logScheme = new List<LogInterface>();
                otherScheme = new List<OtherSDKInterface>();
            }
        }

        static SDKInterfaceBase AnalysisConfig(SDKConfigData data)
        {
            if (data == null)
            {
                return new NullSDKInterface();
            }
            else
            {
                return (SDKInterfaceBase)JsonUtility.FromJson(data.SDKContent, Assembly.Load("Assembly-CSharp").GetType(data.SDKName));
            }
        }

#endregion

        #region 初始化SDK

        static void LoadService(SchemeData data)
        {
            AnalyzeSchemeData(
                data,
                out s_loginServiceList,
                out s_ADServiceList,
                out s_payServiceList,
                out s_logServiceList,
                out s_otherServiceList
                );
        }

        static void InitSDK()
        {
            InitLogin(s_loginServiceList);
            InitSDKList(s_ADServiceList);
            InitPay(s_payServiceList);
            InitSDKList(s_logServiceList);
            InitSDKList(s_otherServiceList);

            //Debug.Log("s_loginServiceList: " + s_loginServiceList.Count);
        }

#endregion

        #region 功能函数

        static T GetSDKService<T>(List<T> list, string name) where T : SDKInterfaceBase
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].m_SDKName == name)
                {
                    return list[i];
                }
            }

            throw new Exception("GetSDKService " + typeof(T).Name + " Exception dont find ->" + name + "<-");
        }

        static bool GetHasSDKService<T>(List<T> list, string name) where T : SDKInterfaceBase
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].m_SDKName == name)
                {
                    return true;
                }
            }

            return false;
        }

        static void InitSDKList<T>(List<T> list) where T : SDKInterfaceBase
        {
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    list[i].m_SDKName = list[i].GetType().Name;
                    list[i].Init();
                }
                catch (Exception e)
                {
                    Debug.LogError("Init " + typeof(T).Name + " SDK Exception:\n" + e.ToString());
                }
            }
        }

        #endregion

        #region 自动监听上报

        /// <summary>
        /// 自动上报监听初始化
        /// </summary>
        static void AutoListenerInit()
        {
            PayCallBack += OnPayCallBackListener;
        }


        /// <summary>
        /// 自动上报支付
        /// </summary>
        static void OnPayCallBackListener(OnPayInfo info)
        {
            Debug.Log("自动上报支付 success >" + info.isSuccess + "<");
            if (info.isSuccess)
            {
                LogPay(info.orderID, info.goodsId, 1, info.price, info.currency, info.storeName.ToString());
            }
        }

        #endregion
    }

    #region 声明

    public delegate void LoginCallBack(OnLoginInfo info);
    public delegate void PayCallBack(OnPayInfo info);
    public delegate void ADCallBack(OnADInfo info);
    public delegate void OtherCallBack(OnOtherInfo info);
    public delegate void GoodsInfoCallBack(GoodsInfoFromSDK info);
    public delegate void ConsumePurchaseCallBack(ConsumePurchaseInfo info);

    public class SchemeData
    {
        public string SchemeName;

        public bool UseNewSDKManager;

        public List<SDKConfigData> LogScheme = new List<SDKConfigData>();
        public List<SDKConfigData> LoginScheme = new List<SDKConfigData>();
        public List<SDKConfigData> ADScheme = new List<SDKConfigData>();
        public List<SDKConfigData> PayScheme = new List<SDKConfigData>();
        public List<SDKConfigData> OtherScheme = new List<SDKConfigData>();
    }
    [System.Serializable]
    public class SDKConfigData
    {
        public string SDKName;
        public string SDKContent;
    }

    public enum SDKType
    {
        Log,
        Login,
        AD,
        Pay,
        Other,
    }

#endregion
}