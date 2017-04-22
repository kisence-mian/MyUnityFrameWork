using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManager 
{
    public const string c_ConfigName = "SDKConfig";
    public const string c_KeyName = "SDKconfig";

    //static LoginCallBack LoginCallBack = null;
    //static PayCallBack   PayCallBack   = null; 

    static LoginInterface s_loginService = null;
    static PayInterface s_payService = null;
    static ADInterface s_ADService = null;

    static List<LogInterface> s_logServiceList = null;
    static List<OtherSDKInterface> s_otherServiceList = null;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        try
        {
            if (ConfigManager.GetIsExistConfig(c_ConfigName))
            {
                Dictionary<string, SingleField> configData = ConfigManager.GetData(c_ConfigName);
                SchemeData tmp = JsonUtility.FromJson<SchemeData>(configData[c_KeyName].GetString());

                LoadService(tmp);

                InitSDK();
            }
        }
        catch(Exception e)
        {
            Debug.LogError("SDKManager Init Exception: " + e.ToString());
        }
    }

    #region 外部调用

    /// <summary>
    /// 登陆
    /// </summary>
    public static void Login()
    {
        try
        {
            s_loginService.Login();
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Login Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// 支付
    /// </summary>
    public static void Pay(string goodsID)
    {
        try
        {
            s_payService.Pay(goodsID);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager Pay Exception: " + e.ToString());
        }
    }

    #region AD

    /// <summary>
    /// 加载广告
    /// </summary>
    public static void LoadAD(ADType adType)
    {
        try
        {
            s_ADService.LoadAD(adType);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager LoadAD Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// 显示广告
    /// </summary>
    public static void PlayAD(ADType adType)
    {
        try
        {
            s_ADService.PlayAD(adType);
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
    public static void CloseAD(ADType adType)
    {
        try
        {
            s_ADService.CloseAD(adType);
        }
        catch (Exception e)
        {
            Debug.LogError("SDKManager CloseAD Exception: " + e.ToString());
        }
    }

    #endregion

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

    #region 加载Service

    static void LoadService(SchemeData data)
    {
        s_loginService = (LoginInterface)AnalysisConfig(data.LoginScheme);
        s_ADService = (ADInterface)AnalysisConfig(data.ADScheme);
        s_payService = (PayInterface)AnalysisConfig(data.PayScheme);

        s_logServiceList = new List<LogInterface>();
        for (int i = 0; i < data.LogScheme.Count; i++)
        {
            s_logServiceList.Add((LogInterface)AnalysisConfig(data.LogScheme[i]));
        }

        s_otherServiceList = new List<OtherSDKInterface>();
        for (int i = 0; i < data.OtherScheme.Count; i++)
        {
            s_otherServiceList.Add((OtherSDKInterface)AnalysisConfig(data.OtherScheme[i]));
        }
    }

    static void InitSDK()
    {
        s_loginService.Init();
        s_ADService.Init();
        s_payService.Init();

        for (int i = 0; i < s_logServiceList.Count; i++)
        {
            s_logServiceList[i].Init();
        }

        for (int i = 0; i < s_otherServiceList.Count; i++)
        {
            s_otherServiceList[i].Init();
        }
    }

    public static SDKInterfaceBase AnalysisConfig(SDKConfigData data)
    {
        return (SDKInterfaceBase)JsonUtility.FromJson(data.SDKContent, Type.GetType(data.SDKName));
    }

    #endregion
}

public delegate void LoginCallBack(string ID, Dictionary<string,object> data);
public delegate void PayCallBack(string goodsID, Dictionary<string, object> data);

public class SchemeData
{
    public string SchemeName;

    public List<SDKConfigData> LogScheme = new List<SDKConfigData>();
    public SDKConfigData LoginScheme;
    public SDKConfigData ADScheme;
    public SDKConfigData PayScheme;
    public List<SDKConfigData> OtherScheme = new List<SDKConfigData>();
}
[System.Serializable]
public class SDKConfigData
{
    public string SDKName;
    public string SDKContent;
}