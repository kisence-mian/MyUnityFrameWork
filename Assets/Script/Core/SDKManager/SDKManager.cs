using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SDKManager 
{
    public const string c_ConfigName = "SDKConfig";
    public const string c_KeyName = "SDKInfo";

    static List<LoginInterface> s_loginServiceList = null;
    static List<PayInterface> s_payServiceList = null;
    static List<ADInterface> s_ADServiceList = null;
    static List<LogInterface> s_logServiceList = null;
    static List<OtherSDKInterface> s_otherServiceList = null;

    #region 外部调用

    #region 初始化

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        //Debug.Log("SDKManager Init");

        try
        {
            if (ConfigManager.GetIsExistConfig(c_ConfigName))
            {
                LoadService();

                InitSDK();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Init Exception: " + e.ToString());
        }
    }

    #endregion

    #region 获取SDKInterface

    public static LoginInterface GetLoginService<T>() where T: LoginInterface
    {
        return GetLoginService(typeof(T).Name);
    }

    public static LoginInterface GetLoginService(string SDKName)
    {
        return GetSDKService(s_loginServiceList, SDKName);
    }

    public static LoginInterface GetLoginService(int index)
    {
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
        return s_otherServiceList[index];
    }


    #endregion

    #region 登录

    /// <summary>
    /// 登陆,默认访问第一个接口
    /// </summary>
    public static void Login()
    {
        try
        {
            GetLoginService(0).Login();
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Login Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// 登陆
    /// </summary>
    public static void Login(string SDKName)
    {
        try
        {
            GetLoginService(SDKName).Login();
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Login Exception: " + e.ToString());
        }
    }

    /// <summary>
    ///  登陆
    ///  考虑到有些游戏可能接多个游戏平台，建议使用宏指令调用不同的Login<>接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Login<T>() where T: LoginInterface
    {
        try
        {
            GetLoginService<T>().Login();
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Login Exception: " + e.ToString());
        }
    }

    #endregion

    #region 支付

    /// <summary>
    /// 支付,默认访问第一个接口
    /// </summary>
    public static void Pay(string goodsID)
    {
        try
        {
            GetPayService(0).Pay(goodsID);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Pay Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// 支付
    /// </summary>
    public static void Pay(string SDKName,string goodsID)
    {
        try
        {
            GetPayService(SDKName).Pay(goodsID);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Pay Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// 支付
    /// </summary>
    public static void Pay<T>(string goodsID) where T: PayInterface
    {
        try
        {
            GetPayService<T>().Pay(goodsID);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Pay Exception: " + e.ToString());
        }
    }

    #endregion

    #region 广告

    /// <summary>
    /// 加载广告,默认访问第一个接口
    /// </summary>
    public static void LoadAD(ADType adType, string tag = "")
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

    /// <summary>
    /// 加载广告
    /// </summary>
    public static void LoadAD(string SDKName, ADType adType,string tag = "")
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

    /// <summary>
    /// 加载广告
    /// </summary>
    public static void LoadAD<T>( ADType adType, string tag = "") where T:ADInterface
    {
        try
        {
            GetADService<T>().LoadAD(adType, tag);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager LoadAD Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// 显示广告
    /// </summary>
    public static void PlayAD( ADType adType, string tag = "")
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

    /// <summary>
    /// 显示广告
    /// </summary>
    public static void PlayAD(string SDKName, ADType adType, string tag = "")
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

    /// <summary>
    /// 显示广告
    /// </summary>
    public static void PlayAD<T>( ADType adType, string tag = "") where T:ADInterface
    {
        try
        {
            GetADService<T>().PlayAD(adType, tag);
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
    public static void CloseAD( ADType adType, string tag = "")
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

    /// <summary>
    /// 隐藏广告
    /// </summary>
    /// <param name="adType"></param>
    public static void CloseAD(string SDKName, ADType adType, string tag = "")
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

    /// <summary>
    /// 隐藏广告
    /// </summary>
    /// <param name="adType"></param>
    public static void CloseAD<T>(string SDKName, ADType adType, string tag = "") where T:ADInterface
    {
        try
        {
            GetADService<T>().CloseAD(adType, tag);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager CloseAD Exception: " + e.ToString());
        }
    }

    #endregion

    #region 数据上报

    /// <summary>
    /// 数据上报
    /// </summary>
    /// <param name="data"></param>
    public static void Log(string eventID,Dictionary<string,object> data)
    {
        try
        {
            for (int i = 0; i < s_logServiceList.Count; i++)
            {
                s_logServiceList[i].Log(eventID, data);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Log Exception: " + e.ToString());
        }
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
        if (ConfigManager.GetIsExistConfig(SDKManager.c_ConfigName))
        {
            Dictionary<string, SingleField> configData = ConfigManager.GetData(SDKManager.c_ConfigName);
            return JsonUtility.FromJson<SchemeData>(configData[SDKManager.c_KeyName].GetString());
        }
        else
        {
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
        if(schemeData != null)
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

    static void LoadService()
    {
        AnalyzeSchemeData(
            LoadGameSchemeConfig(),
            out s_loginServiceList,
            out s_ADServiceList,
            out s_payServiceList,
            out s_logServiceList,
            out s_otherServiceList
            );
    }

    static void InitSDK()
    {
        InitSDKList(s_loginServiceList);
        InitSDKList(s_ADServiceList);
        InitSDKList(s_payServiceList);
        InitSDKList(s_logServiceList);
        InitSDKList(s_otherServiceList);

        //Debug.Log("s_loginServiceList: " + s_loginServiceList.Count);
    }

    #endregion

    #region 功能函数

    static T GetSDKService<T>(List<T> list,string name) where T:SDKInterfaceBase
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].m_SDKName == name)
            {
                return list[i];
            }
        }

        throw new Exception("GetSDKService "+typeof(T).Name+" Exception dont find ->"+ name + "<-");
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

public delegate void LoginCallBack(string ID, Dictionary<string,object> data);
public delegate void PayCallBack(string goodsID, Dictionary<string, object> data);

public class SchemeData
{
    public string SchemeName;

    public List<SDKConfigData> LogScheme   = new List<SDKConfigData>();
    public List<SDKConfigData> LoginScheme = new List<SDKConfigData>();
    public List<SDKConfigData> ADScheme    = new List<SDKConfigData>();
    public List<SDKConfigData> PayScheme   = new List<SDKConfigData>();
    public List<SDKConfigData> OtherScheme = new List<SDKConfigData>();

    public string AndroidkeystoreName; //Android 签名路径
    public string AndroidkeystorePass; //Android 密钥密码
    public string AndroidkeyaliasName; //Android 密钥别名
    public string AndroidkeyaliasPass; //Android 别名密码

    //图标

}
[System.Serializable]
public class SDKConfigData
{
    public string SDKName;
    public string SDKContent;
}