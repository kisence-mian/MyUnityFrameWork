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
    public const string c_AssetsBundlesExpandName = "assetBundle";

    static Dictionary<string, Bundle> s_bundles        = new Dictionary<string, Bundle>();
    static Dictionary<string, RelyBundle> s_relyBundle = new Dictionary<string, RelyBundle>(); //所有依赖包

#if !UNITY_WEBGL
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
            if (configTmp.relyPackages[i] != "")
            {
                //Debug.Log("LoadBundle:" + configTmp.relyPackages[i]);

                LoadRelyBundle(configTmp.relyPackages[i]);
            }
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
#endif

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

        int LoadCount = 0;

        if (configTmp.relyPackages.Length > 0 && configTmp.relyPackages[0] != "")
        {
            //先加载依赖包
            for (int i = 0; i < configTmp.relyPackages.Length; i++)
            {
                if (configTmp.relyPackages[i] != "")
                {
                    LoadRelyBundleAsync(configTmp.relyPackages[i], (LoadState relyLoadState, RelyBundle RelyBundle) =>
                    {
                        if (RelyBundle != null && relyLoadState.isDone)
                        {
                            LoadCount++;
                            state.progress += 1 / ((float)configTmp.relyPackages.Length + 1);
                        }

                        //所有依赖包加载完毕加载资源包
                        if (LoadCount == configTmp.relyPackages.Length)
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
                            callBack(state, null);
                        }
                    });
                }
            }
        }
        else
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
#if !UNITY_WEBGL
            if (MemoryManager.s_allowDynamicLoad)
            {
                return LoadBundle(name).mainAsset;
            }
            else
            {
                throw new Exception("已禁止资源动态加载，请检查静态资源加载列表 ->" + name + "<-");
            }
#else
            throw new Exception("WEBGL 不能同步加载Bundle，请先异步加载对应资源！ ->" + name + "<-");
#endif
        }
    }

    public static T Load<T>(string name) where T: UnityEngine.Object
    {
        if (s_bundles.ContainsKey(name))
        {
            return s_bundles[name].Load<T>();
        }
        else
        {
#if !UNITY_WEBGL
            if (MemoryManager.s_allowDynamicLoad)
            {
                return (T)LoadBundle(name).Load<T>();
            }
            else
            {
                throw new Exception("已禁止资源动态加载，请检查静态资源加载列表 ->" + name + "<-");
            }
#else
        throw new Exception("WEBGL 不能同步加载Bundle，请先异步加载对应资源！ ->" + name + "<-");
#endif
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
            Debug.LogError("LoadAsync: ResName:" +name+" Error:"+ e.ToString());
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

            //这里不能执行Unload(true);
            UnloadBundle(s_bundles[bundleName]);

            s_bundles.Remove(bundleName);
        }
        else
        {
            Debug.LogError("UnLoadBundle: " + bundleName + " dont exist !");
        }
    }

    static void UnloadBundle(Bundle bundle)
    {
        UnloadObject(bundle.mainAsset);

        for (int i = 0; i < bundle.allAsset.Length; i++)
        {
            UnloadObject(bundle.allAsset[i]);
        }
    }

    static void UnloadObject(UnityEngine.Object obj)
    {
        if (!(obj is GameObject)
            && !(obj is Component)
            && !(obj is AssetBundle)
            )
        {
            Resources.UnloadAsset(obj);
        }
        else if ((obj is GameObject)
            ||  (obj is Component))
        {
            UnityEngine.Object.DestroyImmediate(obj, true);
        }
        else
        {
            AssetBundle ab = (AssetBundle)obj;
            ab.Unload(true);
        }
    }

    /// <summary>
    /// 卸载依赖包
    /// </summary>
    /// <param name="relyBundleName"></param>
    public static void UnLoadRelyBundle(string relyBundleName)
    {
        if(relyBundleName == "")
        {
            return;
        }

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

    static Bundle AddBundle(string bundleName, AssetBundle asset)
    {
        //Debug.Log("AddBundle " + bundleName);

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

        if (asset != null)
        {
            bundleTmp.bundle = asset;
            bundleTmp.bundle.name = bundleName;
            bundleTmp.mainAsset = bundleTmp.bundle.mainAsset;
            bundleTmp.allAsset = bundleTmp.bundle.LoadAllAssets();
            bundleTmp.bundle.Unload(false);

            Debug.Log("Unload false");

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

    static RelyBundle AddRelyBundle(string relyBundleName, AssetBundle asset)
    {
        RelyBundle tmp = new RelyBundle();

        tmp.relyCount = 1;
        tmp.bundle = asset;

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
    static string GetBundlePath(ResourcesConfig config)
    {
#if !UNITY_WEBGL

        bool isLoadByPersistent = RecordManager.GetData(HotUpdateManager.c_HotUpdateRecordName).GetRecord(config.name, "null") =="null" ? false:true;
        ResLoadLocation loadType = ResLoadLocation.Streaming;

        //加载路径由 加载根目录 和 相对路径 合并而成
        //加载根目录由配置决定
        if (isLoadByPersistent)
        {
            loadType = ResLoadLocation.Persistent;
        }

        return PathTool.GetAbsolutePath(loadType, config.path + "." + c_AssetsBundlesExpandName);
#else

        return PathTool.GetLoadURL(config.path + "." + c_AssetsBundlesExpandName);
#endif
    }
}

public delegate void BundleLoadCallBack(LoadState state, Bundle bundlle);
public delegate void RelyBundleLoadCallBack(LoadState state, RelyBundle bundlle);
public class Bundle
{
    public UnityEngine.Object mainAsset;
    public UnityEngine.Object[] allAsset;
    public AssetBundle bundle;
    public ResourcesConfig bundleConfig;

    public T Load<T>() where T : UnityEngine.Object
    {
        if (mainAsset.GetType() == typeof(T))
        {
            return (T)mainAsset;
        }

        for (int i = 0; i < allAsset.Length; i++)
        {
            if(allAsset[i].GetType() == typeof(T))
            {
                return (T)allAsset[i];
            }
        }

        throw new Exception("Bundle Load Exception : not find by " + typeof(T).Name  + " ");
    }
}

public class RelyBundle
{
    public int relyCount = 0;  //依赖次数
    public AssetBundle bundle; //包
}


