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

    static Dictionary<string, object> s_versionConfig;
    static Dictionary<string, SingleField> s_hotUpdateConfig;

    static string s_versionFileDownLoadPath;
    static string s_ManifestFileDownLoadPath;
    static string s_resourcesFileDownLoadPath;

    static HotUpdateCallBack s_UpdateCallBack;

    static string s_versionFileCache;

    static AssetBundleManifest s_ManifestCache;
    static byte[] s_ManifestByteCache;

    public static void StartHotUpdate(HotUpdateCallBack CallBack)
    {
        s_UpdateCallBack = CallBack;

        Init();

        //检查Streaming版本和Persistent版本哪一个更新
        if (!CheckLocalVersion())
        {
            return;
        }

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

    static bool CheckLocalVersion()
    {
        try
        {
            if (ApplicationManager.Instance.m_useAssetsBundle)
            {
                string path = PathTool.GetAbsolutePath(ResLoadLocation.Streaming,c_versionFileName.ToLower());

#if UNITY_EDITOR 
                //判断本地文件是否存在
                if (!File.Exists(path))
                {
                    Debug.LogError("本地 Version 文件不存在，请先创建本地文件！");
                    UpdateDateCallBack(HotUpdateStatusEnum.UpdateFail, 1);
                    return false;
                }
#endif 

                AssetBundle ab = AssetBundle.LoadFromFile(path);

                TextAsset text = ab.LoadAsset<TextAsset>(c_versionFileName);
                string StreamVersionContent = text.text;

                ab.Unload(true);

                //stream版本
                Dictionary<string, object> StreamVersion = (Dictionary<string, object>)FrameWork.Json.Deserialize(StreamVersionContent);

                //Streaming版本如果比Persistent版本还要新，则更新Persistent版本
                if ((GetInt(StreamVersion[c_largeVersionKey]) > GetInt(s_versionConfig[c_largeVersionKey])) ||
                    (GetInt(StreamVersion[c_smallVersonKey]) > GetInt(s_versionConfig[c_smallVersonKey]))
                    )
                {
                    Debug.Log("Streaming版本比Persistent版本还要新");

                    RecordManager.CleanRecord(c_HotUpdateRecordName);
                    Init();
                }
                return true;
            }
            else
            {
                Debug.Log("没有使用Bundle 无需更新");
                UpdateDateCallBack(HotUpdateStatusEnum.NoUpdate, 0);
                return false;
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
            UpdateDateCallBack(HotUpdateStatusEnum.UpdateFail, 0);
        }

        return false;
    }

    public static string GetHotUpdateVersion()
    {
        if(s_versionConfig == null)
        {
            return "0.0";
        }
        else
        {
            return GetInt(s_versionConfig[c_largeVersionKey]) + "." + GetInt(s_versionConfig[c_smallVersonKey]);
        }
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
            Debug.LogError("Version File DownLoad Error URL:" + s_versionFileDownLoadPath + " error:" + www.error);

            UpdateDateCallBack(HotUpdateStatusEnum.VersionFileDownLoadFail, 0);
            yield break;
        }

        s_versionFileCache = www.assetBundle.LoadAsset<TextAsset>(c_versionFileName).text;

        www.assetBundle.Unload(true);

        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingVersionFile, GetHotUpdateProgress(false, false, 1));

        //Debug.Log("Version File :text: " + m_versionFileCatch);

        //Debug.Log("Service Version File :text: " + s_versionFileCache);
        //Debug.Log("local Version  : " + GetInt(s_versionConfig[c_largeVersionKey]) + " " + GetInt(s_versionConfig[c_smallVersonKey]));

        Dictionary<string, object> ServiceVersion = (Dictionary<string, object>)FrameWork.Json.Deserialize(s_versionFileCache);

        //服务器大版本比较大，需要整包更新
        if ( GetInt(s_versionConfig[c_largeVersionKey])
            < GetInt(ServiceVersion[c_largeVersionKey]))
        {
            Debug.Log("需要更新整包");
            UpdateDateCallBack(HotUpdateStatusEnum.NeedUpdateApplication, GetHotUpdateProgress(true, false, 0));
        }
        //服务器大版本比较小，无需更新
        else if (GetInt(s_versionConfig[c_largeVersionKey])
                > GetInt(ServiceVersion[c_largeVersionKey]))
        {
            Debug.Log("服务器大版本比较小，无需更新，直接进入游戏");
            UpdateDateCallBack(HotUpdateStatusEnum.NoUpdate, 1);
            yield break;
        }
        //服务器小版本比较大，更新文件
        else if (GetInt(s_versionConfig[c_smallVersonKey]) 
            < GetInt(ServiceVersion[c_smallVersonKey]) )
        {
            Debug.Log("服务器小版本比较大，更新文件");

            UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, false, 0));

            yield return DownLoadFile();
        }
        //服务器小版本比较小，无需更新
        else
        {
            Debug.Log("服务器小版本比较小或者相同，无需更新，直接进入游戏");
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
        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingManifestFile, GetHotUpdateProgress(true, false, 0));
        //取得服务器版本文件
        WWW www = new WWW(s_ManifestFileDownLoadPath);
        Debug.Log("服务器获取清单文件 ：" + s_ManifestFileDownLoadPath);
        //yield return www;

        while (!www.isDone)
        {
            UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingManifestFile, GetHotUpdateProgress(true, false, www.progress));
            yield return new WaitForEndOfFrame();
        } 

        if (www.error != null && www.error != "")
        {
            //下载失败
            Debug.LogError("MD5 DownLoad Error " + www.error);

            UpdateDateCallBack(HotUpdateStatusEnum.Md5FileDownLoadFail, GetHotUpdateProgress(true,false,0));
            yield break;
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        s_ManifestCache = www.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        s_ManifestByteCache = www.bytes;
        www.assetBundle.Unload(false);

        UpdateDateCallBack(HotUpdateStatusEnum.DownLoadingManifestFile, GetHotUpdateProgress(true, false, 1));

        s_downLoadList = new List<DownLoadData>();

        CheckBundleList(s_ManifestCache, AssetsManifestManager.GetManifest());

        yield return StartDownLoad();
    }

    static void CheckBundleList(AssetBundleManifest service, AssetBundleManifest local)
    {
        string[] allServiceBundle = service.GetAllAssetBundles();

        for (int i = 0; i < allServiceBundle.Length; i++)
        {
            Hash128 sHash = service.GetAssetBundleHash(allServiceBundle[i]);
            Hash128 lHash = local.GetAssetBundleHash(allServiceBundle[i]);

            if (!sHash.Equals(lHash))
            {
                //Debug.Log("sHash" + sHash);
                //Debug.Log("lHash" + lHash);

                DownLoadData data = new DownLoadData();
                data.name = allServiceBundle[i];
                data.md5 = sHash;

                s_downLoadList.Add(data);
            }
        }
    }

    static List<DownLoadData> s_downLoadList = new List<DownLoadData>();
    //static List<ResourcesConfig> s_deleteList = new List<ResourcesConfig>();

    static IEnumerator StartDownLoad()
    {
        Debug.Log("下载服务器文件到本地");

        UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, true,  GetDownLoadFileProgress(0)));

        RecordTable hotupdateData = RecordManager.GetData(c_HotUpdateRecordName);

        for (int i = 0; i < s_downLoadList.Count; i++)
        {
            Hash128 md5Tmp = Hash128.Parse( hotupdateData.GetRecord(s_downLoadList[i].name, "null"));

            if (md5Tmp.Equals(s_downLoadList[i].md5))
            {
                Debug.Log("文件已更新完毕 " + s_downLoadList[i].name);
                //该文件已更新完毕
                UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, true, GetDownLoadFileProgress(i)));
            }
            else
            {
                string downloadPath = s_resourcesFileDownLoadPath + s_downLoadList[i].name;

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

                    ResourceIOTool.CreateFile(PathTool.GetAssetsBundlePersistentPath() + "/" + s_downLoadList[i].name , www.bytes);
                    RecordManager.SaveRecord(c_HotUpdateRecordName, s_downLoadList[i].name, s_downLoadList[i].md5.ToString());

                    UpdateDateCallBack(HotUpdateStatusEnum.Updating, GetHotUpdateProgress(true, true, GetDownLoadFileProgress(i)));
                }
            }
        }

        //保存版本信息
        //保存文件信息
        //ResourceIOTool.CreateFile(PathTool.GetAssetsBundlePersistentPath()+ c_versionFileName , s_versionByteCache);
        ResourceIOTool.CreateFile(PathTool.GetAssetsBundlePersistentPath() + AssetsManifestManager.c_ManifestFileName, s_ManifestByteCache);

        //从stream读取配置
        RecordManager.SaveRecord(c_HotUpdateRecordName, c_useHotUpdateRecordKey, true);

        UpdateDateCallBack(HotUpdateStatusEnum.UpdateSuccess, 1);

        //重新生成资源配置
        ResourcesConfigManager.LoadResourceConfig();
        AssetsManifestManager.LoadAssetsManifest();
    }

    static void Init()
    {
        s_versionConfig   = (Dictionary<string,object>) FrameWork.Json.Deserialize(ReadVersionContent());
        s_hotUpdateConfig = ConfigManager.GetData(c_HotUpdateConfigName);

        string downLoadServicePath = null;
        bool isTest = s_hotUpdateConfig[c_UseTestDownLoadPathKey].GetBool();

        //使用测试下载地址
        if(isTest)
        {
            downLoadServicePath = s_hotUpdateConfig[c_testDownLoadPathKey].GetString();
        }
        else
        {
            downLoadServicePath = s_hotUpdateConfig[c_downLoadPathKey].GetString();
        }

        string downLoadPath = downLoadServicePath + "/" + platform + "/" + Application.version + "/";
        

        s_versionFileDownLoadPath   = downLoadPath + c_versionFileName.ToLower() ;
        s_ManifestFileDownLoadPath  = downLoadPath + AssetsManifestManager.c_ManifestFileName;
        s_resourcesFileDownLoadPath = downLoadPath;

        Debug.Log("=====>" + s_versionFileDownLoadPath);
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
        if (s_downLoadList.Count == 0)
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
                string persistentPath = PathTool.GetAssetsBundlePersistentPath() + c_versionFileName;

                AssetBundle ab = AssetBundle.LoadFromFile(persistentPath);
                TextAsset text = ab.LoadAsset<TextAsset>(c_versionFileName);
                dataJson = text.text;
                ab.Unload(true);
            }
            else
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathTool.GetAbsolutePath(type,c_versionFileName.ToLower()));
                    TextAsset text = ab.LoadAsset<TextAsset>(c_versionFileName);
                    dataJson = text.text;
                    ab.Unload(true);
            }
        }

        return dataJson;
    }

    public struct DownLoadData
    {
        public string name;
        public Hash128 md5;
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
    DownLoadingManifestFile,      //下载清单文件中
    Updating,                //更新中
}

