using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// bundle管理器，用来管理所有的bundle
/// </summary>
public static class AssetsBundleManager 
{
    static Dictionary<string, AssetBundle> bundles = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 同步加载一个bundles
    /// </summary>
    /// <param name="name">bundle名</param>
    public static AssetBundle loadBundle(string bundleName)
    {
        string path = getBundlePath(bundleName);

        AssetBundle bundleTmp = AssetBundle.LoadFromFile(path);
        bundles.Add(bundleName, bundleTmp);
        return bundleTmp;
    }

    /// <summary>
    /// 异步加载一个bundle
    /// </summary>
    /// <param name="bundleName">bundle名</param>
    public static void loadBundleAsync(string bundleName,bundleLoadCallBack callBack)
    {

    }

    /// <summary>
    /// 根据bundleName获取加载路径
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static string getBundlePath(string bundleName)
    {
        PackageConfig configTmp = PackageConfigManager.GetPackageConfig(bundleName);
        //加载路径由 加载根目录 和 相对路径 合并而成
        //加载根目录由配置决定
        return ResourceManager.GetPath(configTmp.path,configTmp.loadType);
    }
}

public delegate void bundleLoadCallBack(AssetBundle bundlle);
