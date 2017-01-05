using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 比对文件版本，下载最新文件
/// 更新Update配置
/// </summary>
public class HotUpdateManager 
{
    public const string c_HotUpdateRecordName = "HotUpdateRecord";
    public const string c_HotUpdateConfigName = "HotUpdateConfig";
    public static string c_versionConfigName = "version";
    public static string c_largeVersionKey = "largeVersion";
    public static string c_smallVersonKey  = "smallVerson";

    public static string c_downLoadPathKey     = "DownLoadPath";
    public static string c_UseTestDownLoadPathKey = "UseTestDownLoadPath";
    public static string c_testDownLoadPathKey = "TestDownLoadPath";

    static Dictionary<string, SingleField> m_versionConfig;
    static Dictionary<string, SingleField> m_hotUpdateConfig;

    static string s_versionFileDownLoadPath;
    static string s_Md5FileDownLoadPath;
    static string s_resourcesFileDownLoadPath;

    static HotUpdateCallBack s_UpdateCallBack;

    public static void StartHotUpdate(HotUpdateCallBack CallBack)
    {
        s_UpdateCallBack = CallBack;

        Init();
        
        //开始热更新
        ApplicationManager.Instance.StartCoroutine(HotUpdateProgress());
    }

    /// <summary>
    /// 热更新流程
    /// </summary>
    static IEnumerator HotUpdateProgress()
    {
        //先检查文件版本
        yield return CheckVersion();

        Debug.Log("需要更新文件");

        //更新文件
        yield return DownLoadFile();
    }

    static IEnumerator CheckVersion()
    {
        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingVersionFile, 0);
        //取得服务器版本文件
        WWW www = new WWW(s_versionFileDownLoadPath);

        Debug.Log("服务器获取版本文件 ：" + s_versionFileDownLoadPath);
        //yield return www;

        while (!www.isDone)
        {
            UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingVersionFile, GetHotUpdateProgress(false, false, www.progress));
            yield return new WaitForEndOfFrame();
        } 

        if (www.error != null && www.error != "")
        {
            //下载失败
            Debug.LogError("Version File DownLoad Error " + www.error);

            UpdateDateCallBack(HotUpdateStatusEnum.VersionFileDownLoadFail, 0);
            yield break;
        }

        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingVersionFile, GetHotUpdateProgress(false, false, 1));

        Debug.Log("www.text: " + www.text);

        Dictionary<string, SingleField> ServiceVersion = JsonTool.Json2Dictionary<SingleField>(www.text);

        //服务器大版本比较大，需要整包更新
        if (m_versionConfig[c_largeVersionKey].GetInt()
            < ServiceVersion[c_largeVersionKey].GetInt())
        {
            UpdateDateCallBack(HotUpdateStatusEnum.NeedUpdateApplication, 0);
            //ui.ShowDialog(true);
        }
        //服务器小版本比较大，更新文件
        else if (m_versionConfig[c_smallVersonKey].GetInt() 
            < ServiceVersion[c_smallVersonKey].GetInt())
        {
            Debug.Log("服务器小版本比较大，更新文件");

            UpdateDateCallBack(HotUpdateStatusEnum.Updating, 0);

            yield return DownLoadFile();
        }

        //服务器小版本比较小，无需更新
        else
        {
            Debug.Log("无需更新，直接进入游戏");
            UpdateDateCallBack(HotUpdateStatusEnum.NoUpdate, 1);
            yield break;
        }
    }

    /// <summary>
    /// 更新文件
    /// </summary>
    /// <returns></returns>
    static IEnumerator DownLoadFile()
    {
        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingMd5File, GetHotUpdateProgress(true, false, 0));
        //取得服务器版本文件
        WWW www = new WWW(s_Md5FileDownLoadPath);
        Debug.Log("服务器获取MD5文件 ：" + s_Md5FileDownLoadPath);
        //yield return www;

        while (!www.isDone)
        {
            UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingMd5File, GetHotUpdateProgress(true, false, www.progress));
            yield return new WaitForEndOfFrame();
        } 

        if (www.error != null && www.error != "")
        {
            //下载失败
            Debug.LogError("MD5 DownLoad Error " + www.error);

            UpdateDateCallBack(HotUpdateStatusEnum.Md5FileDownLoadFail, GetHotUpdateProgress(true,false,0));
            yield break;
        }

        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingMd5File, GetHotUpdateProgress(true, false, 1));

        Dictionary<string, SingleField> serviceFileConfig = JsonTool.Json2Dictionary<SingleField>(www.text);
        Dictionary<string, SingleField> localFileConfig = ResourcesConfigManager.GetResourcesConfig();


        List<ResourcesConfig> m_ServiceRelyList = JsonTool.Json2List < ResourcesConfig > (serviceFileConfig[ResourcesConfigManager.c_relyBundleKey].GetString());
        List<ResourcesConfig> m_ServiceBundleList = JsonTool.Json2List<ResourcesConfig>(serviceFileConfig[ResourcesConfigManager.c_bundlesKey].GetString());

        List<ResourcesConfig> m_LocalRelyList = JsonTool.Json2List<ResourcesConfig>(localFileConfig[ResourcesConfigManager.c_relyBundleKey].GetString());
        List<ResourcesConfig> m_LocalBundleList = JsonTool.Json2List<ResourcesConfig>(localFileConfig[ResourcesConfigManager.c_bundlesKey].GetString());

        s_downLoadList = new List<ResourcesConfig>();

        CheckBundleList(m_ServiceRelyList, m_LocalRelyList);
        CheckBundleList(m_ServiceBundleList, m_LocalBundleList);

        yield return StartDownLoad();
    }

    static void CheckBundleList(List<ResourcesConfig> serviceList,List<ResourcesConfig> localList)
    {
        Dictionary<string, ResourcesConfig> localConfigDict = new Dictionary<string,ResourcesConfig>();

        for (int i = 0; i < localList.Count; i++)
        {
            localConfigDict.Add(localList[i].name, localList[i]);
        }

        for (int i = 0; i < serviceList.Count; i++)
        {
            ResourcesConfig Tmp = serviceList[i];

            if (localConfigDict.ContainsKey(Tmp.name))
            {
                ResourcesConfig localTmp = localConfigDict[Tmp.name];
                if (!Tmp.md5.Equals(localTmp.md5))
                {
                    s_downLoadList.Add(Tmp);
                }
            }
            else
            {
                s_downLoadList.Add(Tmp);
            }
        }
    }

    static List<ResourcesConfig> s_downLoadList = new List<ResourcesConfig>();
    //static List<ResourcesConfig> s_deleteList = new List<ResourcesConfig>();

    static IEnumerator StartDownLoad()
    {
        Debug.Log("下载服务器文件到本地");

        UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, true, GetHotUpdateProgress(true, true, GetDownLoadFileProgress(0))));
        
        for (int i = 0; i < s_downLoadList.Count; i++)
        {
            string downloadPath = s_resourcesFileDownLoadPath + "/" + s_downLoadList[i].path + "." + AssetsBundleManager.c_AssetsBundlesExpandName;

            WWW www = new WWW(downloadPath);
            yield return www;

            if (www.error != null && www.error != "")
            {
                Debug.LogError("下载出错！ " + downloadPath + " " + www.error);

                UpdateDateCallBack(HotUpdateStatusEnum.UpdateFail, GetHotUpdateProgress(true, true, GetDownLoadFileProgress(i)));
            }
            else
            {
                Debug.Log("下载成功！ " + downloadPath);

                ResourceIOTool.CreateFile(Application.persistentDataPath + "/" + s_downLoadList[i].path, www.bytes);
                RecordManager.SaveRecord(c_HotUpdateRecordName, s_downLoadList[i].name, true);

                UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true,true,GetDownLoadFileProgress(i)));
            }
        }

        UpdateDateCallBack(HotUpdateStatusEnum.UpdateSuccess, 1);
    }

    static void Init()
    {
        m_versionConfig   = ConfigManager.GetData(c_versionConfigName);
        m_hotUpdateConfig = ConfigManager.GetData(c_HotUpdateConfigName);

        string downLoadServicePath = null;
        bool isTest = m_hotUpdateConfig[c_UseTestDownLoadPathKey].GetBool();

        //使用测试下载地址
        if(isTest)
        {
            downLoadServicePath = m_hotUpdateConfig[c_testDownLoadPathKey].GetString();
        }
        else
        {
            downLoadServicePath = m_hotUpdateConfig[c_downLoadPathKey].GetString();
        }

        string downLoadPath = downLoadServicePath + "/" + platform + "/" + Application.version + "/";

        s_versionFileDownLoadPath = downLoadPath + ConfigManager.c_directoryName + "/" + HotUpdateManager.c_versionConfigName + "." + ConfigManager.c_expandName;
        s_Md5FileDownLoadPath     = downLoadPath + ConfigManager.c_directoryName + "/" + ResourcesConfigManager.c_configFileName + "." + ConfigManager.c_expandName;
        s_resourcesFileDownLoadPath = downLoadPath;
    }

    static void UpdateDateCallBack(HotUpdateStatusEnum status, float progress)
    {
        try
        {
            s_UpdateCallBack(HotUpdateStatusInfo.GetUpdateInfo(status, progress));
        }
        catch(Exception e)
        {
            Debug.LogError("UpdateDateCallBack Error :" + e.ToString());
        }
    }

    static float GetHotUpdateProgress(bool isDownLoadVersion,bool isDownLoadMd5,float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (!isDownLoadVersion)
        {
            return 0.1f * progress;
        }
        else if (!isDownLoadMd5)
        {
            return 0.1f + 0.1f * progress;
        }
        else
        {
            return 0.2f + 0.8f * progress;
        }
    }

    static float GetDownLoadFileProgress(int index)
    {
        if (s_downLoadList.Count ==0)
        {
            Debug.Log("更新列表为 0");
            return 1;
        }
        return (float)(index / s_downLoadList.Count);
    }

    static string platform
    {
        get
        {
            string Platform = "Win";

#if UNITY_ANDROID //安卓
            Platform = "Android";
#elif UNITY_IOS //iPhone
                Platform = "IOS";
#endif
            return Platform;
        }
    }
}

public delegate void HotUpdateCallBack(HotUpdateStatusInfo info);

public struct HotUpdateStatusInfo
{
    public HotUpdateStatusEnum m_status;
    public LoadState m_loadState;

    static HotUpdateStatusInfo s_info = new HotUpdateStatusInfo();
    public static HotUpdateStatusInfo GetUpdateInfo(HotUpdateStatusEnum status,float progress)
    {
        s_info.m_status = status;

        if (s_info.m_loadState == null)
        {
            s_info.m_loadState = new LoadState();
        }

        if (progress == 1)
        {
            s_info.m_loadState.isDone = true;
        }
        else
        {
            s_info.m_loadState.isDone = false;
        }

        s_info.m_loadState.progress = progress;

        return s_info;
    }
}

public enum HotUpdateStatusEnum
{
    NoUpdate,                //无需更新
    NeedUpdateApplication,   //需要整包更新

    VersionFileDownLoadFail, //版本文件下载失败
    Md5FileDownLoadFail,     //Md5文件下载失败
    UpdateFail,              //更新失败
    UpdateSuccess,           //更新成功

    DownLoadingVersionFile,  //下载版本文件中
    DownLoadingMd5File,      //下载Md5文件中
    Updating,                //更新中
}
