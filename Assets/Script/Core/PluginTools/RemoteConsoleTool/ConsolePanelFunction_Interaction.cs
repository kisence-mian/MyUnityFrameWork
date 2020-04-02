using UnityEngine;
using System.Collections;
using GameConsoleController;
using System;

public static class ConsolePanelFunction_Interaction
{
    private const string MethodType = "Frame";

    [RemoteInvoking(name ="设置程序运行模式", methodType = MethodType)]
    [ParamsDescription(paramName = "m_AppMode",selectItemValues =new string[] {" Developing","QA","Release" })]
    private static void SetAppMode(string m_AppMode)
    {
         PlayerPrefs.SetString("AppMode", m_AppMode);
        ApplicationManager.Instance.m_AppMode = (AppMode)Enum.Parse(typeof(AppMode), m_AppMode);
    }
    [RemoteInvoking(name = "大区测试模式开关",methodType = MethodType)]
    [ParamsDescription(paramName = "testNetAreaURL",paramsDescriptionName ="测试大区地址")]
    private static void SetServerTestMode(bool isTestMode, string testNetAreaURL)
    {
        GamePrepareFlowController.SetServerTestMode(isTestMode, testNetAreaURL);
    }

    #region 设置热更新测试地址
    [RemoteInvoking(name = "设置热更新测试地址", methodType = MethodType)]
    [ParamsDescription(paramName = "testPath", paramsDescriptionName = "热更新地址",getDefaultValueMethodName = "GetNowHotUpdateTestPath")]
    private static void SetHotUpdateTestPath(string testPath)
    {
        PlayerPrefs.SetString(HotupdateFlowItem.P_SelectHotUpdateTestPath, testPath);
    }

    private static string GetNowHotUpdateTestPath()
    {
        return PlayerPrefs.GetString(HotupdateFlowItem.P_SelectHotUpdateTestPath, "");
    }
    #endregion

    #region 设置登录地区Country Code
    [RemoteInvoking(name = "设置登录地区Country Code(IOS-3166)", methodType = MethodType)]
    [ParamsDescription(paramName = "countryCode",selectItemValues =new string[] {"CN","TW","US","DE" },  paramsDescriptionName = "国家码", getDefaultValueMethodName = "GetCurrentCountryCode")]
    private static void SetDeviceLoginCountryCode(string countryCode)
    {
        PlayerPrefs.SetString(DownloadRegionServerListFlowItem.P_Country_Code, countryCode);
    }

    private static string GetCurrentCountryCode()
    {
        return PlayerPrefs.GetString(DownloadRegionServerListFlowItem.P_Country_Code, "");
    }
    #endregion

}
