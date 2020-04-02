using UnityEngine;
using System.Collections;
using System.Text;
using System;
using Object = UnityEngine.Object;
/*
 * gameLoadType 为 Resource 时 ，所有资源从Resource读取
 * gameLoadType 不为 Resource时，资源读取方式从配置中读取
 * */
public static class ResourceManager 
{
    private static AssetsLoadType loadType = AssetsLoadType.Resources;
    public static AssetsLoadType LoadType
    {
        get
        {
            return loadType;
        }

        //set
        //{
        //    ReleaseAll();

        //    loadType = value;
        //    isInit = false;
        //    Initialize();
        //}
    }
    public static bool UseCache
    {
        get;private set;
    }
    private static AssetsLoadController loadAssetsController;

    //    private static bool isInit = false;
#if UNITY_EDITOR
        //UnityEditor模式下编译完成后自动初始化
    [UnityEditor.InitializeOnLoadMethod]
#endif
    private static void Initialize()
    {
        Initialize(AssetsLoadType.Resources, false);
    }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="loadType"></param>
    /// <param name="useCache"></param>
    public static void Initialize(AssetsLoadType loadType,bool useCache)
    {
        //if (isInit)
        //    return;
       
        if(loadType== AssetsLoadType.AssetBundle)
        {
            useCache = true;
        }
        if (!Application.isPlaying)
        {
            useCache = false;
        }
        UseCache = useCache;
        ResourceManager.loadType = loadType;
        ReleaseAll();
        GameInfoCollecter.AddAppInfoValue("AssetsLoadType", loadType);

        loadAssetsController = new AssetsLoadController(loadType,useCache);
        //Debug.Log("ResourceManager初始化 AssetsLoadType:" + loadType + " useCache:" + useCache);
    }

    public static AssetsLoadController GetLoadAssetsController()
    {
        return loadAssetsController;
    }

    /// <summary>
    /// 同步加载一个资源
    /// </summary>
    /// <param name="name"></param>
    public static Object Load(string name)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        AssetsData assets = loadAssetsController.LoadAssets(path);
        if (assets != null)
        {
            return assets.Assets[0];
        }
        return null;
    }

    public static void LoadAsync(string name, CallBack<Object> callBack)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        loadAssetsController.LoadAsync(path, null, callBack);
    }
    public static void LoadAsync(string name, Type resType, CallBack<Object> callBack)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        loadAssetsController.LoadAsync(path, resType, callBack);
    }

    /// <summary>
    /// 加载资源
    /// 注意释放资源，方法： DestoryAssetsCounter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T Load<T>(string name) where T : Object
    {
        T res =null;
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        //Debug.Log("ResourcesConfigManager.GetLoadPath :"+ path);
        AssetsData assets = loadAssetsController.LoadAssets<T>(path);
        if (assets != null)
        {
            res = assets.GetAssets<T>();

        }
       if(res ==null)
        {
            Debug.LogError("Error=> Load Name :" + name + "  Type:" + typeof(T).FullName + "\n" + " Load Object:" + res );
        }
        return res;
    }
    //public static T EditorLoad<T>(string name) where T : Object
    //{
    //    T res = null;
    //    string path = ResourcesConfigManager.GetLoadPath( AssetsLoadType.Resources, name);
    //    res = Resources.Load<T>(path);
    //    return res;
    //}
    public static string LoadText(string name)
    {
        TextAsset tex = Load<TextAsset>(name);
        if (tex == null)
            return null;
        return tex.text;
    }

    /// <summary>
    /// 释放资源 （通过 ResourceManager.Load<>() 加载出来的）
    /// </summary>
    /// <param name="unityObject"></param>
    /// <param name="times"></param>
    public static void DestoryAssetsCounter(Object unityObject, int times = 1)
    {
        DestoryAssetsCounter(unityObject.name, times);
    }

    public static void DestoryAssetsCounter(string name, int times = 1)
    {
        if (!ResourcesConfigManager.GetIsExitRes(name))
            return;
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        if (times <= 0)
            times = 1;
        for (int i = 0; i < times; i++)
        {
            loadAssetsController.DestoryAssetsCounter(path);
        }
    }

    /// <summary>
    /// 卸载所有资源
    /// </summary>
    /// <param name="isForceAB">是否强制卸载bundle（true:bundle包和资源一起卸载；false：只卸载bundle包）</param>
    public static void ReleaseAll(bool isForceAB=true)
    {
        if (loadAssetsController != null)
            loadAssetsController.ReleaseAll(isForceAB);
        //ResourcesConfigManager.ClearConfig();
    }

    public static void Release(string name)
    {
        string path = ResourcesConfigManager.GetLoadPath(loadType, name);
        loadAssetsController.Release(path);
    }

    public static void ReleaseByPath(string path)
    {
        loadAssetsController.Release(path);
    }

    public static bool GetResourceIsExist(string name)
    {
        return ResourcesConfigManager.GetIsExitRes(name);
    }
}





