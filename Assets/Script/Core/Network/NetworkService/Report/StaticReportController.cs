using FrameWork.SDKManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 将一些固定数据上报服务器
/// </summary>
public class StaticReportController
{
    
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        LoginGameController.OnUserLogin += OnUserLogin;
    }

    private static void OnUserLogin(UserLogin2Client t)
    {
        if (t.reloginState)
            return;
        if (t.code != 0)
            return;
        SendDeviceInfo(t.user.userID);
    }

    private const string ReportUserData = "ReportUserData";
    private static void SendDeviceInfo(string userID)
    {
        Dictionary<string, string> datas = new Dictionary<string, string>();
        datas.Add("uuid", userID);
        string channel = "Windows";

#if UNITY_ANDROID && !UNITY_EDITOR
        channel = "Android";
#endif

#if UNITY_IOS && !UNITY_EDITOR
        channel = "IOS";
#endif

        //Debug.Log("OnLoginEvent : " + e.state + "  Error: " + e.error);
        string[] deviceInfo = SystemInfo.deviceModel.Split(' ');

       string cc=  SDKManager.GetProperties(SDKInterfaceDefine.FileName_ChannelProperties, channel);

        datas.Add("channel", cc);
        datas.Add("brand", deviceInfo[0]);
        datas.Add("deviceName", SystemInfo.deviceModel);
        datas.Add("version", ApplicationManager.Version);
        datas.Add("processorType", SystemInfo.processorType.ToString());
        datas.Add("processorCount", SystemInfo.processorCount.ToString());

        string net = "移动网络";

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            net = "无网络";
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            net = "wifi";
        }

        datas.Add("net", net);

        datas.Add("systemLanguage", Application.systemLanguage.ToString());

        datas.Add("memorySize", SystemInfo.systemMemorySize.ToString());
        datas.Add("graphicMemorySize", SystemInfo.graphicsMemorySize.ToString());
        datas.Add("shaderLevel", SystemInfo.graphicsShaderLevel.ToString());
        datas.Add("graphicDeviceType", SystemInfo.graphicsDeviceType.ToString());

        string[] os = SystemInfo.operatingSystem.Split(' ');

        datas.Add("os", os[0]);
        datas.Add("ov", SystemInfo.operatingSystem);

        int w = Screen.width;
        int h = Screen.height;

        if (w < h)
        {
            w = Screen.height;
            h = Screen.width;
        }

        datas.Add("resolution", w + "x" + h);

        SDKManager.Log(ReportUserData, datas);
    }
}

