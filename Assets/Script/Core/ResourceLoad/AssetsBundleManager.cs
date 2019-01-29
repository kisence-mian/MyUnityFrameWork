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
    //public const string c_AssetsBundlesExpandName = "assetBundle";

    static Dictionary<string, Bundle> s_bundles        = new Dictionary<string, Bundle>();//所有包

#if !UNITY_WEBGL

    /// <summary>
    /// 同步加载一个bundles
    /// </summary>
    /// <param name="name">bundle名</param>
    public static Bundle LoadBundle(string bundleName)
    {
        string path = GetBundlePath(bundleName);

        string[] AllDependencies = AssetsManifestManager.GetAllDependencies(bundleName);

        //加载依赖包
        for (int i = 0;i< AllDependencies.Length;i++ )
        {
            LoadRelyBundle(AllDependencies[i]);
        }
        
        if(!AssetsManifestManager.GetIsDependencies(bundleName))
        {
            return AddBundle(bundleName, AssetBundle.LoadFromFile(path));
        }
        //如果这个包被别人依赖，则当做依赖包处理
        else
        {
            return LoadRelyBundle(bundleName);
        }
    }

    //加载一个依赖包
    public static Bundle LoadRelyBundle(string relyBundleName)
    {
        Bundle tmp = null;

        if (s_bundles.ContainsKey(relyBundleName))
        {
            tmp = s_bundles[relyBundleName];
            tmp.relyCount++;
        }
        else
        {
            string path = GetBundlePath(relyBundleName);
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
        string[] AllDependencies = AssetsManifestManager.GetAllDependencies(bundleName);

        string path = GetBundlePath(bundleName);

        LoadState state = new LoadState();

        int LoadCount = 0;

        if (AllDependencies.Length > 0 )
        {
            //先加载依赖包
            for (int i = 0; i < AllDependencies.Length; i++)
            {
                if (AllDependencies[i] != "")
                {
                    LoadRelyBundleAsync(AllDependencies[i], (LoadState relyLoadState, Bundle RelyBundle) =>
                    {
                        if (RelyBundle != null && relyLoadState.isDone)
                        {
                            LoadCount++;
                            state.progress += 1 / ((float)AllDependencies.Length + 1);
                        }

                        //所有依赖包加载完毕加载资源包
                        if (LoadCount == AllDependencies.Length)
                        {
                            ResourceIOTool.AssetsBundleLoadAsync(path, (LoadState bundleLoadState, AssetBundle bundle) =>
                            {
                                if (bundleLoadState.isDone)
                                {
                                    callBack(LoadState.CompleteState, AddBundle(bundleName, bundle));
                                }
                                else
                                {
                                    state.progress += bundleLoadState.progress / ((float)AllDependencies.Length + 1);
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
                    state.progress += bundleLoadState.progress / ((float)AllDependencies.Length + 1);
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
        if (s_bundles.ContainsKey(relyBundleName))
        {
            Bundle tmp = s_bundles[relyBundleName];
            tmp.relyCount++;

            callBack(LoadState.CompleteState, tmp);
        }
        else
        {
            //先占位，避免重复加载
            s_bundles.Add(relyBundleName, null);

            string path = GetBundlePath(relyBundleName);

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
        try
        {
            if (s_bundles.ContainsKey(name))
            {
                return s_bundles[name].Load(name);
            }
            else
            {
#if !UNITY_WEBGL
                if (MemoryManager.s_allowDynamicLoad)
                {
                    return LoadBundle(name).Load(name);
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
        catch(Exception e)
        {
            throw new Exception("AssetsBundleManager Load Exception ->" + name + "<- " + e.ToString());
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
    public static void LoadAsync(string name,Type type, LoadCallBack callBack)
    {
        try
        {
            if (s_bundles.ContainsKey(name))
            {
                //如果加载完了就直接回调
                //如果没有加载完,就缓存起来,等到加载完了一起回调
                if (s_bundles[name] != null)
                {
                    callBack(LoadState.CompleteState, s_bundles[name].GetAeests(type));
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
                        callBack(state, bundlle.GetAeests(type));
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
            string[] AllDependencies = s_bundles[bundleName].allDependencies;
            //卸载依赖包
            for (int i = 0; i < AllDependencies.Length; i++)
            {
                UnLoadRelyBundle(AllDependencies[i]);
            }

            s_bundles[bundleName].relyCount--;

            //普通包也有可能被依赖
            if(s_bundles[bundleName].relyCount <=0)
            {
                //这里不能执行Unload(true);
                UnloadBundle(s_bundles[bundleName]);
                s_bundles.Remove(bundleName);
            }
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
        if(obj == null)
        {
            return;
        }

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

        if (s_bundles.ContainsKey(relyBundleName))
        {
            s_bundles[relyBundleName].relyCount --;

            if (s_bundles[relyBundleName].relyCount <=0)
            {
                s_bundles[relyBundleName].bundle.Unload(true);
                s_bundles.Remove(relyBundleName);
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
        string[] AllDependencies = AssetsManifestManager.GetAllDependencies(bundleName);

        if (s_bundles.ContainsKey(bundleName))
        {
            s_bundles[bundleName] = bundleTmp;
        }
        else
        {
            s_bundles.Add(bundleName, bundleTmp);
        }

        bundleTmp.allDependencies = AllDependencies;

        if (asset != null)
        {
            bundleTmp.bundle = asset;
            bundleTmp.bundle.name = bundleName;
            bundleTmp.mainAsset = asset.mainAsset;
            bundleTmp.allAsset = bundleTmp.bundle.LoadAllAssets();

            //延迟卸载资源，因为unity的资源卸载有时会异步
            Timer.DelayCallBack(5, (obj) => {
                bundleTmp.bundle.Unload(false);
            });

            //如果有缓存起来的回调这里一起回调
            if( LoadAsyncDict.ContainsKey(bundleName))
            {
                try
                {
                    LoadAsyncDict[bundleName](LoadState.CompleteState,bundleTmp.GetAeests(null));
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

    static Bundle AddRelyBundle(string relyBundleName, AssetBundle asset)
    {
        Bundle tmp = new Bundle();

        tmp.relyCount = 1;
        tmp.bundle = asset;
        tmp.mainAsset = asset.mainAsset;
        tmp.allAsset = asset.LoadAllAssets();

        if (s_bundles.ContainsKey(relyBundleName))
        {
            s_bundles[relyBundleName] = tmp;
        }
        else
        {
            s_bundles.Add(relyBundleName, tmp);
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
    static string GetBundlePath(string bundleName)
    {
#if !UNITY_WEBGL

        bool isLoadByPersistent = RecordManager.GetData(HotUpdateManager.c_HotUpdateRecordName).GetRecord(bundleName, "null") =="null" ? false:true;
        ResLoadLocation loadType = ResLoadLocation.Streaming;

        //加载路径由 加载根目录 和 相对路径 合并而成
        //加载根目录由配置决定
        if (isLoadByPersistent)
        {
            loadType = ResLoadLocation.Persistent;
            return PathTool.GetAssetsBundlePersistentPath() + bundleName;
        }
        else
        {
            loadType = ResLoadLocation.Streaming;
            return PathTool.GetAbsolutePath(loadType, bundleName);
        }

#else
        return PathTool.GetLoadURL(config.path + "." + c_AssetsBundlesExpandName);
#endif
    }
}

public delegate void BundleLoadCallBack(LoadState state, Bundle bundlle);
public delegate void RelyBundleLoadCallBack(LoadState state, Bundle bundlle);
public class Bundle
{
    public UnityEngine.Object mainAsset;
    public UnityEngine.Object[] allAsset;
    public AssetBundle bundle;
    public string[] allDependencies;

    public int relyCount = 0;  //依赖次数

    public T Load<T>() where T : UnityEngine.Object
    {
        if (mainAsset!= null &&  mainAsset.GetType() == typeof(T))
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

        throw new Exception("Bundle Load Exception : not find by " + typeof(T).Name  + " " + mainAsset.name + "->" + mainAsset.GetType().Name);
    }

    public object Load(string name)
    {
        if(allAsset.Length > 0)
        {
            return allAsset[0];
        }

        throw new Exception("Bundle Load Exception : not find by " + name + " ");
    }

    public object GetAeests(Type type)
    {
        if(type == null)
        {
            if (allAsset.Length > 0)
            {
                return allAsset[0];
            }
        }
        else
        {
            for (int i = 0; i < allAsset.Length; i++)
            {
                if(allAsset[i].GetType() == type)
                {
                    return allAsset[i];
                }
            }
        }


        throw new Exception("GetAeests Exception : Asset length is 0 ");
    }
}

//public class RelyBundle
//{
//    public int relyCount = 0;  //依赖次数
//    public AssetBundle bundle; //包
//}


