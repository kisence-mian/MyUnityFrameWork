using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

/// <summary>
/// bundle管理器，用来管理所有的bundle
/// </summary>
public static class AssetsBundleManager 
{
    public const string c_HotUpdateRecordName = "HotUpdateRecord";
    public const string c_AssetsBundlesExpandName = "assetBundle";

    static Dictionary<string, Bundle> s_bundles        = new Dictionary<string, Bundle>();
    static Dictionary<string, RelyBundle> s_relyBundle = new Dictionary<string, RelyBundle>(); //所有依赖包

    /// <summary>
    /// 同步加载一个bundles
    /// </summary>
    /// <param name="name">bundle名</param>
    public static Bundle LoadBundle(string bundleName)
    {
        ResourcesConfig configTmp = ResourcesConfigManager.GetBundleConfig(bundleName);

        string path = GetBundlePath(configTmp);

        //加载依赖包
        for(int i = 0;i<configTmp.relyPackages.Length;i++ )
        {
            LoadRelyBundle(configTmp.relyPackages[i]);
        }

        return AddBundle(bundleName,AssetBundle.LoadFromFile(path));
    }

    //加载一个依赖包
    public static RelyBundle LoadRelyBundle(string relyBundleName)
    {
        RelyBundle tmp = null;

        if (s_relyBundle.ContainsKey(relyBundleName))
        {
            tmp = s_relyBundle[relyBundleName];
            tmp.relyCount++;
        }
        else
        {
            ResourcesConfig configTmp = ResourcesConfigManager.GetRelyBundleConfig(relyBundleName);
            string path = GetBundlePath(configTmp);

            tmp = AddRelyBundle(relyBundleName, AssetBundle.LoadFromFile(path));
        }

        return tmp;
    }

    /// <summary>
    /// 异步加载一个bundle
    /// </summary>
    /// <param name="bundleName">bundle名</param>
    public static void LoadBundleAsync(string bundleName, BundleLoadCallBack callBack)
    {
        ResourcesConfig configTmp = ResourcesConfigManager.GetBundleConfig(bundleName);

        if (configTmp == null)
        {
            Debug.LogError("LoadBundleAsync: " + bundleName + " dont exist!");
            return;
        }

        string path = GetBundlePath(configTmp);

        LoadState state = new LoadState();
        Dictionary<string, LoadState> loadStateDict = new Dictionary<string, LoadState>();

        //先加载依赖包
        for (int i = 0; i < configTmp.relyPackages.Length; i++)
        {
            LoadRelyBundleAsync(configTmp.relyPackages[i], (LoadState relyLoadState, RelyBundle RelyBundle) => 
            {
                if (RelyBundle != null && relyLoadState.isDone)
                {
                    Debug.Log(RelyBundle.bundle.name);

                    loadStateDict.Add(RelyBundle.bundle.name, relyLoadState);
                    state.progress += 1 / ((float)configTmp.relyPackages.Length + 1);
                }

                //所有依赖包加载完毕加载资源包
                if (loadStateDict.Keys.Count == configTmp.relyPackages.Length)
                {
                    ResourceIOTool.AssetsBundleLoadAsync(path, (LoadState bundleLoadState, AssetBundle bundle) => 
                    {
                        if (bundleLoadState.isDone)
                        {
                            callBack(LoadState.CompleteState, AddBundle(bundleName, bundle));
                        }
                        else 
                        {
                            state.progress += bundleLoadState.progress / ((float)configTmp.relyPackages.Length + 1);
                            callBack(state, null);
                        }
                    });
                }
                else
                {
                    callBack(state,null);
                }
            });
        }
    }

    /// <summary>
    /// 异步加载一个依赖包
    /// </summary>
    /// <param name="relyBundleName"></param>
    /// <param name="callBack"></param>
    public static void LoadRelyBundleAsync(string relyBundleName, RelyBundleLoadCallBack callBack)
    {
        if (s_relyBundle.ContainsKey(relyBundleName))
        {
            RelyBundle tmp = s_relyBundle[relyBundleName];
            tmp.relyCount++;

            callBack(LoadState.CompleteState, tmp);
        }
        else
        {
            //先占位，避免重复加载
            s_relyBundle.Add(relyBundleName, null);

            ResourcesConfig configTmp = ResourcesConfigManager.GetRelyBundleConfig(relyBundleName);
            string path = GetBundlePath(configTmp);

            ResourceIOTool.AssetsBundleLoadAsync(path, (LoadState state,AssetBundle bundle)=>
            {
                if (!state.isDone)
                {
                    callBack(state,null);
                }
                else
                {
                    callBack(state, AddRelyBundle(relyBundleName, bundle));
                }
            });
        }
    }

    /// <summary>
    /// 同步读取一个资源
    /// </summary>
    /// <param name="name">资源Key，必须在资源表中</param>
    /// <returns>目标资源</returns>
    public static object Load(string name)
    {
        if(s_bundles.ContainsKey(name))
        {
            return s_bundles[name].mainAsset;
        }
        else
        {
            return LoadBundle(name).mainAsset;
        }
    }

    public static T Load<T>(string name) where T: UnityEngine.Object
    {
        if (s_bundles.ContainsKey(name))
        {
            return (T)s_bundles[name].mainAsset;
        }
        else
        {
            return (T)LoadBundle(name).mainAsset;
        }
    }

    //所有缓存起来的回调
    static Dictionary<string, LoadCallBack> LoadAsyncDict = new Dictionary<string, LoadCallBack>();
    /// <summary>
    /// 异步读取一个资源
    /// </summary>
    /// <param name="name">>资源Key，必须在资源表中</param>
    /// <param name="callBack">回调，返回加载进度和目标资源</param>
    public static void LoadAsync(string name, LoadCallBack callBack)
    {
        try
        {
            if (s_bundles.ContainsKey(name))
            {
                //如果加载完了就直接回调
                //如果没有加载完,就缓存起来,等到加载完了一起回调
                if (s_bundles[name] != null)
                {
                    callBack(LoadState.CompleteState, s_bundles[name].mainAsset);
                }
                else
                {
                    //等待加载完毕再一起回调,这里先缓存起来
                    if(LoadAsyncDict.ContainsKey(name))
                    {
                        LoadAsyncDict[name] += callBack;
                    }
                    else
                    {
                        LoadAsyncDict.Add(name,callBack);
                    }
                }
            }
            else
            {
                //先占位，避免重复加载
                s_bundles.Add(name, null);

                LoadBundleAsync(name, (LoadState state, Bundle bundlle) =>
                {
                    if (state.isDone)
                    {
                        callBack(state, bundlle.mainAsset);
                    }
                    else
                    {
                        callBack(state, null);
                    }
                });
            }
        }
        catch(Exception e)
        {
            Debug.LogError("LoadAsync: " + e.ToString());
        }
    }

    /// <summary>
    /// 卸载bundle
    /// </summary>
    /// <param name="bundleName"></param>
    public static void UnLoadBundle(string bundleName)
    {
        if (s_bundles.ContainsKey(bundleName))
        {
            ResourcesConfig configTmp = s_bundles[bundleName].bundleConfig;
            //卸载依赖包
            for (int i = 0; i < configTmp.relyPackages.Length; i++)
            {
                UnLoadRelyBundle(configTmp.relyPackages[i]);
            }

            s_bundles[bundleName].bundle.Unload(true);

            s_bundles.Remove(bundleName);
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
        if (s_relyBundle.ContainsKey(relyBundleName))
        {
            s_relyBundle[relyBundleName].relyCount --;

            if (s_relyBundle[relyBundleName].relyCount <=0)
            {
                s_relyBundle[relyBundleName].bundle.Unload(true);
                s_relyBundle.Remove(relyBundleName);
            }
        }
        else
        {
            Debug.LogError("UnLoadRelyBundle: " + relyBundleName + " dont exist !");
        }
    }

    public static Bundle AddBundle(string bundleName, AssetBundle aess)
    {
        Bundle bundleTmp = new Bundle();
        ResourcesConfig configTmp = ResourcesConfigManager.GetBundleConfig(bundleName);

        if (s_bundles.ContainsKey(bundleName))
        {
            s_bundles[bundleName] = bundleTmp;
        }
        else
        {
            s_bundles.Add(bundleName, bundleTmp);
        }

        bundleTmp.bundleConfig = configTmp;

        if (aess != null)
        {
            bundleTmp.bundle = aess;
            bundleTmp.bundle.name = bundleName;
            bundleTmp.mainAsset = bundleTmp.bundle.mainAsset;
            bundleTmp.bundle.Unload(false);

            //如果有缓存起来的回调这里一起回调
            if( LoadAsyncDict.ContainsKey(bundleName))
            {
                try
                {
                    LoadAsyncDict[bundleName](LoadState.CompleteState,bundleTmp.mainAsset);
                }
                catch(Exception e)
                {
                    Debug.Log("LoadAsync AddBundle " + e.ToString());
                }
            }
        }
        else
        {
            Debug.LogError("AddBundle: " + bundleName + " dont exist!");
        }

        return bundleTmp;
    }

    public static RelyBundle AddRelyBundle(string relyBundleName, AssetBundle aess)
    {
        RelyBundle tmp = new RelyBundle();

        tmp.relyCount = 1;
        tmp.bundle = aess;

        if (s_relyBundle.ContainsKey(relyBundleName))
        {
            s_relyBundle[relyBundleName] = tmp;
        }
        else
        {
            s_relyBundle.Add(relyBundleName, tmp);
        }

        if (tmp.bundle == null)
        {
            Debug.LogError("AddRelyBundle: " + relyBundleName + " dont exist!");
        }
        

        return tmp;
    }

    /// <summary>
    /// 根据bundleName获取加载路径
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public static string GetBundlePath(ResourcesConfig config)
    {
        bool isLoadByPersistent = RecordManager.GetData(c_HotUpdateRecordName).GetRecord(config.name, false);

        ResLoadType loadType = ResLoadType.Streaming;

        //加载路径由 加载根目录 和 相对路径 合并而成
        //加载根目录由配置决定
        if (isLoadByPersistent)
        {
            loadType = ResLoadType.Persistent;
        }

        return PathTool.GetAbsolutePath(loadType, config.path + "." + c_AssetsBundlesExpandName);
    }
}

public delegate void BundleLoadCallBack(LoadState state, Bundle bundlle);
public delegate void RelyBundleLoadCallBack(LoadState state, RelyBundle bundlle);
public class Bundle
{
    public object mainAsset;
    public AssetBundle bundle;
    public ResourcesConfig bundleConfig;
}

public class RelyBundle
{
    public int relyCount = 0;  //依赖次数
    public AssetBundle bundle; //包
}


