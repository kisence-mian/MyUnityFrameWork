

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 比对文件版本，下载最新文件
/// 更新Update配置
/// </summary>
public class HotUpdateManager 
{
    public const string c_HotUpdateRecordName = "HotUpdateRecord";
    public const string c_HotUpdateConfigName = "HotUpdateConfig";
    public static string c_versionFileName = "Version";
    public static string c_largeVersionKey = "LargeVersion";
    public static string c_smallVersonKey  = "SmallVerson";

    public static string c_downLoadPathKey     = "DownLoadPath";
    public static string c_UseTestDownLoadPathKey = "UseTestDownLoadPath";
    public static string c_testDownLoadPathKey = "TestDownLoadPath";

    public const string c_useHotUpdateRecordKey = "UseHotUpdate";

#if !UNITY_WEBGL

    static Dictionary<string, object> m_versionConfig;
    static Dictionary<string, SingleField> m_hotUpdateConfig;

    static string s_versionFileDownLoadPath;
    static string s_Md5FileDownLoadPath;
    static string s_resourcesFileDownLoadPath;

    static HotUpdateCallBack s_UpdateCallBack;

    static string m_versionFileCatch;
    static string m_Md5FileCatch;

    public static void StartHotUpdate(HotUpdateCallBack CallBack)
    {
        s_UpdateCallBack = CallBack;

        Init();

        //检查Streaming版本和Persistent版本哪一个更新
        CheckLocalVersion();
        
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
    }

    static void CheckLocalVersion()
    {
        AssetBundle ab = AssetBundle.LoadFromFile(PathTool.GetAbsolutePath(ResLoadLocation.Streaming,
            c_versionFileName + "." + AssetsBundleManager.c_AssetsBundlesExpandName));
        TextAsset text = (TextAsset)ab.mainAsset;
        string StreamVersionContent = text.text;
        ab.Unload(true);

        //stream版本
        Dictionary<string, object> StreamVersion = (Dictionary<string, object>)MiniJSON.Json.Deserialize(StreamVersionContent);

        //Streaming版本如果比Persistent版本还要新，则更新Persistent版本
        if ((GetInt(StreamVersion[c_largeVersionKey]) > GetInt(m_versionConfig[c_largeVersionKey]))||
            (GetInt(StreamVersion[c_smallVersonKey]) > GetInt(m_versionConfig[c_smallVersonKey]))
            )
        {
            RecordManager.CleanRecord(c_HotUpdateRecordName);
            Init();
        }
    }

    static IEnumerator CheckVersion()
    {
        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingVersionFile, 0);
        //取得服务器版本文件
        WWW www = new WWW(s_versionFileDownLoadPath);

        //Debug.Log("服务器获取版本文件 ：" + s_versionFileDownLoadPath);
        //yield return www;

        while (!www.isDone)
        {
            UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingVersionFile, GetHotUpdateProgress(false, false, www.progress));
            yield return new WaitForEndOfFrame();
        } 

        if (www.error != null && www.error != "")
        {
            //下载失败
            Debug.LogError("Version File DownLoad Error URL:" + s_versionFileDownLoadPath + " error:" + www.error);

            UpdateDateCallBack(HotUpdateStatusEnum.VersionFileDownLoadFail, 0);
            yield break;
        }

        m_versionFileCatch = ((TextAsset)www.assetBundle.mainAsset).text;

        www.assetBundle.Unload(true);

        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingVersionFile, GetHotUpdateProgress(false, false, 1));

        //Debug.Log("Version File :text: " + m_versionFileCatch);

        Dictionary<string, object> ServiceVersion = (Dictionary<string, object>)MiniJSON.Json.Deserialize(m_versionFileCatch);

        //服务器大版本比较大，需要整包更新
        if ( GetInt(m_versionConfig[c_largeVersionKey])
            < GetInt(ServiceVersion[c_largeVersionKey]))
        {
            Debug.Log("需要更新整包");
            UpdateDateCallBack(HotUpdateStatusEnum.NeedUpdateApplication, GetHotUpdateProgress(true, false, 0));
        }
        //服务器小版本比较大，更新文件
        else if (GetInt(m_versionConfig[c_smallVersonKey]) 
            < GetInt(ServiceVersion[c_smallVersonKey]) )
        {
            Debug.Log("服务器小版本比较大，更新文件");

            UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, false, 0));

            yield return DownLoadFile();
        }
        //服务器小版本比较小，无需更新
        else
        {
            //Debug.Log("无需更新，直接进入游戏");
            UpdateDateCallBack(HotUpdateStatusEnum.NoUpdate, 1);
            yield break;
        }
    }

    static int GetInt(object obj)
    {
        return int.Parse(obj.ToString());
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

        m_Md5FileCatch = ((TextAsset)www.assetBundle.mainAsset).text;

        www.assetBundle.Unload(true);

        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingMd5File, GetHotUpdateProgress(true, false, 1));

        ResourcesConfigStruct serviceFileConfig = ResourcesConfigManager.AnalysisResourcesConfig2Struct(m_Md5FileCatch);
        ResourcesConfigStruct localFileConfig   = ResourcesConfigManager.AnalysisResourcesConfig2Struct(ResourcesConfigManager.ReadResourceConfigContent());

        s_downLoadList = new List<ResourcesConfig>();

        CheckBundleList(serviceFileConfig.relyList, localFileConfig.relyList);
        CheckBundleList(serviceFileConfig.bundleList, localFileConfig.bundleList);

        yield return StartDownLoad();
    }

    static void CheckBundleList(Dictionary<string, ResourcesConfig> serviceDict, Dictionary<string, ResourcesConfig> localDict)
    {
        foreach(ResourcesConfig item in serviceDict.Values )
        {
            ResourcesConfig Tmp = item;

            if (localDict.ContainsKey(Tmp.name))
            {
                ResourcesConfig localTmp = localDict[Tmp.name];

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

        UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, true,  GetDownLoadFileProgress(0)));

        RecordTable hotupdateData = RecordManager.GetData(c_HotUpdateRecordName);
        
        for (int i = 0; i < s_downLoadList.Count; i++)
        {
            string md5Tmp = hotupdateData.GetRecord(s_downLoadList[i].name, "null");

            if (md5Tmp == s_downLoadList[i].md5)
            {
                //该文件已更新完毕
                UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, true, GetDownLoadFileProgress(i)));
            }
            else
            {
                string downloadPath = s_resourcesFileDownLoadPath + s_downLoadList[i].path + "." + AssetsBundleManager.c_AssetsBundlesExpandName;

                WWW www = new WWW(downloadPath);
                yield return www;

                if (www.error != null && www.error != "")
                {
                    Debug.LogError("下载出错！ " + downloadPath + " " + www.error);
                    UpdateDateCallBack(HotUpdateStatusEnum.UpdateFail, GetHotUpdateProgress(true, true, GetDownLoadFileProgress(i)));

                    yield break;
                }
                else
                {
                    Debug.Log("下载成功！ " + downloadPath);

                    ResourceIOTool.CreateFile(Application.persistentDataPath + "/" + s_downLoadList[i].path +"." + AssetsBundleManager.c_AssetsBundlesExpandName, www.bytes);
                    RecordManager.SaveRecord(c_HotUpdateRecordName, s_downLoadList[i].name, s_downLoadList[i].md5);

                    UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, true, GetDownLoadFileProgress(i)));
                }
            }
        }

        //保存版本信息
        //保存文件信息
        ResourceIOTool.WriteStringByFile(PathTool.GetAbsolutePath(ResLoadLocation.Persistent, HotUpdateManager.c_versionFileName + "." + ConfigManager.c_expandName), m_versionFileCatch);
        ResourceIOTool.WriteStringByFile(PathTool.GetAbsolutePath(ResLoadLocation.Persistent, ResourcesConfigManager.c_ManifestFileName + "." + ConfigManager.c_expandName), m_Md5FileCatch);

        //从stream读取配置
        RecordManager.SaveRecord(c_HotUpdateRecordName, c_useHotUpdateRecordKey, true);

        UpdateDateCallBack(HotUpdateStatusEnum.UpdateSuccess, 1);

        //重新生成资源配置
        ResourcesConfigManager.Initialize();
    }

    static void Init()
    {
        m_versionConfig   = (Dictionary<string,object>) MiniJSON.Json.Deserialize(ReadVersionContent());
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

        s_versionFileDownLoadPath   = downLoadPath + HotUpdateManager.c_versionFileName + "." + AssetsBundleManager.c_AssetsBundlesExpandName;
        s_Md5FileDownLoadPath       = downLoadPath + ResourcesConfigManager.c_ManifestFileName + "." + AssetsBundleManager.c_AssetsBundlesExpandName;
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
            return 0.1f + (0.1f * progress);
        }
        else
        {
            return 0.2f + (0.8f * progress);
        }
    }

    static float GetDownLoadFileProgress(int index)
    {
        if (s_downLoadList.Count ==0)
        {
            Debug.Log("更新列表为 0");
            return 0.95f;
        }

        return ((float)(index + 1) / (float)(s_downLoadList.Count + 1));
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

    public static string ReadVersionContent()
    {
        string dataJson = "";

        if (ResourceManager.m_gameLoadType == ResLoadLocation.Resource)
        {
            dataJson = ResourceIOTool.ReadStringByResource(
                c_versionFileName + "." + ConfigManager.c_expandName);
        }
        else
        {
            ResLoadLocation type = ResLoadLocation.Streaming;

            if (RecordManager.GetData(c_HotUpdateRecordName).GetRecord(c_useHotUpdateRecordKey, false))
            {
                type = ResLoadLocation.Persistent;
                dataJson = ResourceIOTool.ReadStringByFile(
                    PathTool.GetAbsolutePath(
                         type,
                         c_versionFileName + "." + ConfigManager.c_expandName));
            }
            else
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathTool.GetAbsolutePath(
                  type,
                  c_versionFileName + "." + AssetsBundleManager.c_AssetsBundlesExpandName));
                    TextAsset text = (TextAsset)ab.mainAsset;
                    dataJson = text.text;
                    ab.Unload(true);
            }


        }

        return dataJson;
    }
}

public delegate void HotUpdateCallBack(HotUpdateStatusInfo info);

public struct HotUpdateStatusInfo
{
    public HotUpdateStatusEnum m_status;
    public LoadState m_loadState;
    public bool isFailed;

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

        if (status == HotUpdateStatusEnum.Md5FileDownLoadFail ||
            status == HotUpdateStatusEnum.UpdateFail || 
            status == HotUpdateStatusEnum.VersionFileDownLoadFail)
        {
            s_info.isFailed = true;
        }
        else
        {
            s_info.isFailed = false;
        }


        s_info.m_loadState.progress = progress;

        return s_info;
    }
    
#endif
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

