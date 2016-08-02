using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// bundle管理器，用来管理所有的bundle
/// </summary>
public static class AssetsBundleManager 
{
    static Dictionary<string, AssetBundle> bundles   = new Dictionary<string, AssetBundle>();
    static Dictionary<string, RelyBundle> relyBundle = new Dictionary<string, RelyBundle>(); //所有依赖包

    /// <summary>
    /// 同步加载一个bundles
    /// </summary>
    /// <param name="name">bundle名</param>
    public static AssetBundle LoadBundle(string bundleName)
    {
        PackageConfig configTmp = BundleConfigManager.GetBundleConfig(bundleName);

        string path = GetBundlePath(configTmp);

        //加载依赖包
        for(int i = 0;i<configTmp.relyPackages.Length;i++ )
        {
            LoadRelyBundle(configTmp.relyPackages[i]);
        }

        AssetBundle bundleTmp = AssetBundle.LoadFromFile(path);
        bundles.Add(bundleName, bundleTmp);
        return bundleTmp;
    }

    //加载一个依赖包
    public static RelyBundle LoadRelyBundle(string relyBundleName)
    {
        RelyBundle tmp = null;

        if (relyBundle.ContainsKey(relyBundleName))
        {
            tmp = relyBundle[relyBundleName];
            tmp.relyCount++;
            relyBundle[relyBundleName] = tmp;
        }
        else
        {
            PackageConfig configTmp = BundleConfigManager.GetRelyBundleConfig(relyBundleName);
            string path = GetBundlePath(configTmp);

            tmp = new RelyBundle();
            tmp.relyCount = 1;
            tmp.bundle = AssetBundle.LoadFromFile(path);

            relyBundle.Add(relyBundleName, tmp);
        }

        return tmp;
    }

    /// <summary>
    /// 异步加载一个bundle
    /// </summary>
    /// <param name="bundleName">bundle名</param>
    public static void LoadBundleAsync(string bundleName,bundleLoadCallBack callBack)
    {

    }

    public static object Load(string name)
    {
        if(bundles.ContainsKey(name))
        {
            return bundles[name].mainAsset;
        }
        else
        {
            return LoadBundle(name).mainAsset;
        }
    }

    /// <summary>
    /// 获取 一个bundle里面的子资源
    /// </summary>
    /// <param name="bundleName">包名</param>
    /// <param name="name">子资源名</param>
    public static object LoadSubAsset(string bundleName,string name)
    {
        if (bundles.ContainsKey(bundleName))
        {
            return bundles[bundleName].LoadAsset(name);
        }
        else
        {
            return LoadBundle(bundleName).LoadAsset(name);
        }
    }

    public static void LoadAsync(string name, LoadCallBack callBack)
    {

    }

    /// <summary>
    /// 卸载bundle
    /// </summary>
    /// <param name="bundleName"></param>
    public static void UnLoadBundle(string bundleName)
    {
        if (bundles.ContainsKey(bundleName))
        {
            PackageConfig configTmp = BundleConfigManager.GetBundleConfig(bundleName);
            //卸载依赖包
            for (int i = 0; i < configTmp.relyPackages.Length; i++)
            {
                UnLoadRelyBundle(configTmp.relyPackages[i]);
            }

            bundles[bundleName].Unload(true);
        }
        else
        {
            Debug.LogError("UnLoadBundle: " + bundleName + " dont exist !");
        }
    }

    /// <summary>
    /// 卸载依赖包
    /// </summary>
    /// <param name="relyBundleName"></param>
    public static void UnLoadRelyBundle(string relyBundleName)
    {
        if (relyBundle.ContainsKey(relyBundleName))
        {
            relyBundle[relyBundleName].relyCount --;

            if (relyBundle[relyBundleName].relyCount <=0)
            {
                relyBundle[relyBundleName].bundle.Unload(true);
                relyBundle.Remove(relyBundleName);
            }
        }
        else
        {
            Debug.LogError("UnLoadRelyBundle: " + relyBundleName + " dont exist !");
        }
    }

    /// <summary>
    /// 根据bundleName获取加载路径
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static string GetBundlePath(PackageConfig config)
    {
        //加载路径由 加载根目录 和 相对路径 合并而成
        //加载根目录由配置决定
        return ResourceManager.GetPath(config.path, config.loadType);
    }
}

public class RelyBundle
{
    public int relyCount = 0;  //依赖次数
    public AssetBundle bundle; //包
}

public delegate void bundleLoadCallBack(AssetBundle bundlle);
