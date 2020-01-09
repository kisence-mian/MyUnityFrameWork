using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManifestManager
{
    public const string c_ManifestFileName = "StreamingAssets";
    static bool s_isInit = false;
    static AssetBundleManifest s_manifest;

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
            path = PathTool.GetAssetsBundlePersistentPath() + c_ManifestFileName;
        }
        else
        {
            Debug.Log("LoadAssetsManifest 读取stream路径");
            path = PathTool.GetAbsolutePath(type, c_ManifestFileName);
        }

        AssetBundle ab = AssetBundle.LoadFromFile(path);
        s_manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        ab.Unload(false);
        LoadDependenciePaths();
    }
    public static Hash128 GetHash(string bundleName)
    {
        Initialize();

        return s_manifest.GetAssetBundleHash(bundleName);
    }

    public static AssetBundleManifest GetManifest()
    {
        Initialize();

        return s_manifest;
    }

    #region New Res Use
    private static Dictionary<string, string[]> dependenciePathsDic = new Dictionary<string, string[]>();

    public static Dictionary<string, string[]> GetDependencieNamesDic()
    {
        Dictionary<string, string[]> dic = new Dictionary<string, string[]>();

        foreach (var item in dependenciePathsDic)
        {
            List<string> names = new List<string>();
            foreach (var pathArr in item.Value)
            {
                string name = PathUtils.GetFileName(pathArr);
                names.Add(name);
            }

            dic.Add(PathUtils.GetFileName(item.Key), names.ToArray());
        }

        return dic;
    }

    //储存含有依赖的资源的路径列表
    private static List<string> hasDependenciesPathList = new List<string>();
    private static void LoadDependenciePaths()
    {
        dependenciePathsDic.Clear();


        string[] sArr = s_manifest.GetAllAssetBundles();
        for (int i = 0; i < sArr.Length; i++)
        {
            string assetPath = sArr[i];
            //string assetName = Path.GetFileNameWithoutExtension(assetPath);
            string[] dependenPaths = s_manifest.GetDirectDependencies(assetPath);
            //Debug.Log("===========>>"+assetPath);
            string[] dependens = new string[dependenPaths.Length];
            for (int j = 0; j < dependenPaths.Length; j++)
            {
                dependens[j] = ResourcesConfigManager.GetLoadPathBase(ResourceManager.LoadType, dependenPaths[j]);
            }

            dependenciePathsDic.Add(ResourcesConfigManager.GetLoadPathBase(ResourceManager.LoadType, assetPath), dependens);

        }

        hasDependenciesPathList.Clear();
        foreach (var assetPath in dependenciePathsDic.Keys)
        {
            bool hasDep = false;
            foreach (var depList in dependenciePathsDic.Values)
            {
                foreach (var item in depList)
                {
                    if (item == assetPath)
                    {
                        hasDep = true;
                        hasDependenciesPathList.Add(assetPath);
                        break;
                    }
                }
                if (hasDep)
                {
                    break;
                }
            }
            //if (!hasDep)
            //{
            //    Debug.Log("没有依赖：" + assetPath);
            //}
        }

    }
    public static string[] GetAllDependenciesPaths(string path)
    {
        if (!s_isInit)
        {
            Initialize();
        }
        if (dependenciePathsDic.Count == 0)
            return new string[0];

        if (dependenciePathsDic.ContainsKey(path))
        {
            return dependenciePathsDic[path];
        }
        else
        {
            Debug.LogError("没找到依赖 GetAllDependenciesName.Name :" + path + " dependencieNamesDic=>" + dependenciePathsDic.Count);
            return new string[0];
        }
    }
    /// <summary>
    /// 是否被依赖或依赖别人
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsHaveDependencies(string path)
    {
        if (!s_isInit)
        {
            Initialize();
        }
        if (hasDependenciesPathList.Contains(path))
        {
            return true;
        }
        return false;
    }
    #endregion
}
