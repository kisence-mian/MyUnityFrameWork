using FrameWork;
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

        private static LoginCallBack s_loginCallBack;
        private static PayCallBack s_payCallBack;

        static bool s_useNewSDKManager = false; //是否使用新版本SDKManager

#region 属性

        public static LoginCallBack LoginCallBack
        {
            get
            {
                return s_loginCallBack;
            }

            set
            {
                s_loginCallBack = value;
            }
        }

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

#endregion

#region 外部调用

#region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
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
                    else
                    {
                        Debug.Log("SDKManager Init");

                        LoadService(data);

                        InitSDK();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SDKManager Init Exception: " + e.ToString());
            }
        }

        /// <summary>
        /// 额外初始化，SDKName == null时初始化全体
        /// </summary>
        public static void ExtraInit(SDKType type,string sdkName = null,string tag = null)
        {
            if(sdkName != null)
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

        static void AllExtraInit<T>(List<T> list,string tag = null) where T : SDKInterfaceBase
        {
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    list[i].ExtraInit(tag);
                }
                catch(Exception e)
                {
                    Debug.LogError("AllExtraInit Exception " + list[i].m_SDKName + " " + e.ToString());
                }
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

        public static PayInterface GetPayService(int index)
        {
            if(s_payServiceList.Count <= index)
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
                    list[i].m_SDKName = list[i].GetType().Name;
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
            if(s_useNewSDKManager)
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

        /// <summary>
        /// 登陆
        /// </summary>
        public static void LoginBySDKName(string SDKName,string tag = "")
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.Login(SDKName,tag);
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
        public static void LoginByPlatform(LoginPlatform  loginPlatform, string tag = "")
        {
            if (s_useNewSDKManager)
            {
                //SDKManagerNew.Login(SDKName, tag);
                Debug.LogError("SDKManager Login Exception: no implement in NewSDKManager");
            }
            else
            {
                try
                {
                    bool isHvae = false;
                    foreach(var item in s_loginServiceList)
                    {
                        if(item.GetPlatform()== Application.platform && item.GetLoginPlatform() == loginPlatform)
                        {
                            item.Login(tag);
                            isHvae = true;

                        }
                    }
                    if (!isHvae)
                    {
                        Debug.LogError("SDKManager Login dont find class by platform:" + Application.platform + " loginPlatform:" + loginPlatform);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Login Exception: " + e.ToString());
                }
            }
        }

#endregion

#region 支付

        static void InitPay(List<PayInterface> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    list[i].m_SDKName = list[i].GetType().Name;
                    list[i].Init();
                      //list[i].m_PayResultCallBack= s_payCallBack;
                    Debug.Log("初始化支付回调");
                }
                catch (Exception e)
                {
                    Debug.LogError("Init PayInterface SDK Exception:\n" + e.ToString());
                }
            }
        }

        /// <summary>
        /// 支付,默认访问第一个接口
        /// </summary>
        public static void Pay(string goodsID, string tag = "",GoodsType goodsType = GoodsType.NORMAL, string orderID = null)
        {
            if(s_useNewSDKManager)
            {
                SDKManagerNew.Pay(goodsID, tag, goodsType, orderID);
            }
            else
            {
                try
                {
                    GetPayService(0).Pay(goodsID, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Pay Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 支付
        /// </summary>
        public static void Pay(string SDKName, string goodsID,string tag ,GoodsType goodsType = GoodsType.NORMAL, string orderID = null)
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.Pay(SDKName,goodsID, tag, goodsType, orderID);
            }
            else
            {
                try
                {
                    GetPayService(SDKName).Pay(goodsID, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager Pay Exception: " + e.ToString());
                }
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

        public static List<LocalizedGoodsInfo> GetAllGoodsInfo( string tag = "")
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

#endregion

#region 广告

        /// <summary>
        /// 加载广告,默认访问第一个接口
        /// </summary>
        public static void LoadAD(ADType adType, string tag = "")
        {
            if(s_useNewSDKManager)
            {
                SDKManagerNew.LoadAD(adType, tag);
            }
            else
            {
                try
                {
                    GetADService(0).LoadAD(adType, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager LoadAD Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 加载广告
        /// </summary>
        public static void LoadAD(string SDKName, ADType adType, string tag = "")
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.LoadAD(SDKName, adType, tag);
            }
            else
            {
                try
                {
                    GetADService(SDKName).LoadAD(adType, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager LoadAD Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 显示广告
        /// </summary>
        public static void PlayAD(ADType adType, string tag = "")
        {
            if(s_useNewSDKManager)
            {
                SDKManagerNew.PlayAD(adType, tag);
            }
            else
            {
                try
                {
                    GetADService(0).PlayAD(adType, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager PlayAD Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 显示广告
        /// </summary>
        public static void PlayAD(string SDKName, ADType adType, string tag = "")
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.PlayAD(SDKName, adType, tag);
            }
            else
            {
                try
                {
                    GetADService(SDKName).PlayAD(adType, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager PlayAD Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 隐藏广告
        /// </summary>
        /// <param name="adType"></param>
        public static void CloseAD(ADType adType, string tag = "")
        {
            if(s_useNewSDKManager)
            {
                SDKManagerNew.CloseAD(adType, tag);
            }
            else
            {
                try
                {
                    GetADService(0).CloseAD(adType, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager CloseAD Exception: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// 隐藏广告
        /// </summary>
        /// <param name="adType"></param>
        public static void CloseAD(string SDKName, ADType adType, string tag = "")
        {
            if (s_useNewSDKManager)
            {
                SDKManagerNew.CloseAD(SDKName,adType, tag);
            }
            else
            {
                try
                {
                    GetADService(SDKName).CloseAD(adType, tag);
                }
                catch (Exception e)
                {
                    Debug.LogError("SDKManager CloseAD Exception: " + e.ToString());
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
            if(s_useNewSDKManager)
            {
                SDKManagerNew.Log(eventID, data);
            }
            else
            {
                if (s_logServiceList == null)
                {
                    throw new Exception("logServiceList is null ,please check SDKManager Init");
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
        }

        public static void LogString(string eventID, string ev_json)
        {
            Dictionary<string, object> en_dic = (Dictionary<string, object>)Json.Deserialize(ev_json);

            Dictionary<string, string> send_info = new Dictionary<string, string>();

            foreach (string item in en_dic.Keys)
            {
                send_info[item] = en_dic[item].ToString();
            }

            Log(eventID, send_info);
        }
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
                Debug.Log("LoadGameSchemeConfig");

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

        static void InitSDKList<T>(List<T> list) where T : SDKInterfaceBase
        {
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    list[i].m_SDKName = list[i].GetType().Name;
                    //Debug.Log("list[i].m_SDKName " + list[i].GetType().Name);
                    list[i].Init();
                }
                catch (Exception e)
                {
                    Debug.LogError("Init " + typeof(T).Name + " SDK Exception:\n" + e.ToString());
                }
            }
        }

#endregion
    }

#region 声明

    public delegate void LoginCallBack(OnLoginInfo info);
    public delegate void PayCallBack(OnPayInfo info);

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