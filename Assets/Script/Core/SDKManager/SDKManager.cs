using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKManager 
{
    const string c_ConfigName = "SDKConfig";

    const string c_LoginConfigName = "Login";
    const string c_LogConfigName = "Log";
    const string c_PayConfigName = "Pay";
    const string c_ADConfigName  = "AD";

    static LoginCallBack LoginCallBack = null;
    static PayCallBack   PayCallBack   = null;

    static LoginInterface     s_loginService;
    static PayInterface       s_payService;
    static ADInterface        s_ADService;
    static List<LogInterface> s_logServiceList;

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

                LoadLogService(
                configData[c_LogConfigName].GetString());

                LoadLoginService(
                configData[c_LoginConfigName].GetString());

                LoadPayService(
                configData[c_PayConfigName].GetString());

                LoadADService(
                configData[c_ADConfigName].GetString());
            }
 
        }
        catch(Exception e)
        {
            Debug.Log("SDKManager Init Exception: " + e.ToString());
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
            Debug.Log("SDKManager Login Exception: " + e.ToString());
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
            Debug.Log("SDKManager Pay Exception: " + e.ToString());
        }
    }

    /// <summary>
    /// 广告
    /// </summary>
    public static void PlayAD()
    {
        try
        {
            s_ADService.PlayAD();
        }
        catch (Exception e)
        {
            Debug.Log("SDKManager AD Exception: " + e.ToString());
        }
    }

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
            Debug.Log("SDKManager Log Exception: " + e.ToString());
        }
    }

    #endregion

    #region 加载Service

    static void LoadLoginService(string serviceName)
    {
        if (serviceName == "null")
        {
            return;
        }

        Type serviceType = Type.GetType(serviceName);
        s_loginService = (LoginInterface)Activator.CreateInstance(serviceType);

        s_loginService.m_callBack = LoginCallBack;
    }

    static void LoadADService(string serviceName)
    {
        if (serviceName == "null")
        {
            return;
        }

        Type serviceType = Type.GetType(serviceName);
        s_ADService = (ADInterface)Activator.CreateInstance(serviceType);
    }

    static void LoadLogService(string serviceName)
    {
        if (serviceName == "null")
        {
            return;
        }

        string[] serviceNameArray = serviceName.Split('|');

        for (int i = 0; i < serviceNameArray.Length; i++)
        {
            if (serviceName != "" && serviceName != null)
            {
                Type serviceType = Type.GetType(serviceNameArray[i]);
                s_logServiceList.Add((LogInterface)Activator.CreateInstance(serviceType));
            }
        }
    }

    static void LoadPayService(string serviceName)
    {
        if (serviceName == "null")
        {
            return;
        }

        Type serviceType = Type.GetType(serviceName);
        s_payService = (PayInterface)Activator.CreateInstance(serviceType);

        s_payService.m_callBack = PayCallBack;
    }

    #endregion
}

public delegate void LoginCallBack(string ID, Dictionary<string,object> data);
public delegate void PayCallBack(string goodsID, Dictionary<string, object> data);