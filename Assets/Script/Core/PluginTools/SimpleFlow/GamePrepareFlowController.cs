using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using FrameWork.SDKManager;

public static class GamePrepareFlowController
{
    //客户端IP详细数据
    public static bool IsChinaIP;
    public static IPGeolocationDetail iPGeolocationDetail;

   

    private static SimpleFlowManager m_FlowManager;

    public static SimpleFlowManager FlowManager
    {
        get
        {
            if (m_FlowManager == null)
                m_FlowManager = new SimpleFlowManager();
            return m_FlowManager;
        }
    }

    /// <summary>
    /// 是否启动测试模式（用于测试热更新服务器等）
    /// </summary>
    public const string UseTestMode = "GamePrepareFlow_UseTestMode";
    /// <summary>
    /// 保存一个测试大区列表配置的
    /// </summary>
    public const string TestDownloadRegionURL = "GamePrepareFlow_TestDownloadRegionURL";

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="pathArr">默认大区列表下载地址（重打包后将不会使用）</param>
    public static void Init(string[] pathArr)
    {
        Debug.Log("GamePrepareFlowController初始化");

        bool isTestMode = PlayerPrefs.GetInt(UseTestMode, 0) == 0 ? false : true;
        if (isTestMode)
        {
           
            string url = PlayerPrefs.GetString(TestDownloadRegionURL, "");
            Debug.Log("进入GamePrepareFlowController test mode！\n URL:"+url);
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("GamePrepareFlowController test mode 获取的测试URL 为 null");
            }
            pathArr = new string[] { url };
        }
        else
        {
            string selectNetworkPath = SDKManager.GetProperties(SDKInterfaceDefine.PropertiesKey_SelectNetworkPath,
                   "");
            if (!string.IsNullOrEmpty(selectNetworkPath))
            {
                pathArr = selectNetworkPath.Split('|');
            }
        }

        FlowManager.AddFlowItems(new FlowItemBase[]{
        new DownloadRegionServerListFlowItem(),
        new HotupdateFlowItem(),
        new PreLoadResFlowItem(),
        new SelectServerFlowItem()});

        FlowManager.GetFlowItem<DownloadRegionServerListFlowItem>().SetURLs(pathArr);

        FlowManager.GetFlowItem<DownloadRegionServerListFlowItem>().OnFinished += OnDownloadRegionServerListFlowItemFinish;
    }

    private static void OnDownloadRegionServerListFlowItemFinish(FlowItemBase arg1, string arg2)
    {
        iPGeolocationDetail = FlowManager.GetFlowItem<DownloadRegionServerListFlowItem>().iPGeolocationDetail;
        IsChinaIP = FlowManager.GetFlowItem<DownloadRegionServerListFlowItem>().IsChinaIP;
    }



    // Use this for initialization
    public static void Start(Type flowItemType)
    {
        FlowManager.RunFlowItem(flowItemType);
    }

    public static void RetryCurrenStatus()
    {
        FlowManager.RunFlowItem(FlowManager.CurrentRunFlowItem.Name, true);
    }
   /// <summary>
   /// 设置启动还是关闭测试模式
   /// </summary>
   /// <param name="isTestMode"></param>
   /// <param name="testURL"></param>
    public static void SetServerTestMode(bool isTestMode,string testURL=null)
    {
        int intState = isTestMode ? 1 : 0;
        PlayerPrefs.SetInt(UseTestMode, intState);
        if (!string.IsNullOrEmpty(testURL))
            PlayerPrefs.SetString(TestDownloadRegionURL, testURL);
    }
}

