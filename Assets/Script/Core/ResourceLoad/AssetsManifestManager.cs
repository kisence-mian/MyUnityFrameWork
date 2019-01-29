using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManifestManager
{
    public const string c_ManifestFileName = "StreamingAssets";
    static bool s_isInit = false;
    static AssetBundleManifest s_manifest;


    static Dictionary<string, bool> s_relayData = new Dictionary<string, bool>();

    static void Initialize()
    {
        if (!s_isInit)
        {
            s_isInit = true;
            LoadAssetsManifest();
        }
    }

    public static void LoadAssetsManifest()
    {
        ResLoadLocation type = ResLoadLocation.Streaming;
        string path = null;

        if (RecordManager.GetData(HotUpdateManager.c_HotUpdateRecordName).GetRecord(HotUpdateManager.c_useHotUpdateRecordKey, false))
        {
            Debug.Log("LoadAssetsManifest 读取沙盒路径");

            type = ResLoadLocation.Persistent;

            //更新资源存放在Application.persistentDataPath+"/Resources/"目录下
            path = PathTool.GetAssetsBundlePersistentPath() + c_ManifestFileName ;
        }
        else
        {
            Debug.Log("LoadAssetsManifest 读取stream路径");
            path = PathTool.GetAbsolutePath(type, c_ManifestFileName);
        }

        AssetBundle ab = AssetBundle.LoadFromFile(path);
        s_manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        ab.Unload(false);

        InitRelayData();
    }

    static void InitRelayData()
    {
        s_relayData = new Dictionary<string, bool>();

        string[] allBundles = s_manifest.GetAllAssetBundles();
        for (int i = 0; i < allBundles.Length; i++)
        {
            string bundleName = allBundles[i];
            string[] allDependencies = s_manifest.GetAllDependencies(bundleName);
            for (int j = 0; j < allDependencies.Length; j++)
            {
                if(!s_relayData.ContainsKey(allDependencies[j]))
                {
                    s_relayData.Add(allDependencies[j], true);
                }
            }
        }
    }

    public static bool GetIsDependencies(string bundleName)
    {
        return s_relayData.ContainsKey(bundleName);
    }

    public static Hash128 GetHash(string bundleName)
    {
        if(!s_isInit)
        {
            Initialize();
        }

        return s_manifest.GetAssetBundleHash(bundleName);
    }

    public static string[] GetAllDependencies(string bundleName)
    {
        if (!s_isInit)
        {
            Initialize();
        }

        return s_manifest.GetAllDependencies(bundleName);
    }

    public static AssetBundleManifest GetManifest()
    {
        if (!s_isInit)
        {
            Initialize();
        }

        return s_manifest;
    }
}
