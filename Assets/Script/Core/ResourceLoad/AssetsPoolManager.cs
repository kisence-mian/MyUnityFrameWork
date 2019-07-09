using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 记录资源的加载与标记回收
/// </summary>
public class AssetsPoolManager
{
    /// <summary>
    /// 是否开启内存回收（默认开启）
    /// </summary>
    public static bool OpenMemoryClean = true;
    /// <summary>
    /// 触发回收机制的数量限制
    /// </summary>
    public static int triggerUnloadNumber = 12;
    /// <summary>
    /// 每过多久释放一个资源
    /// </summary>
    public static float unloadDelayTime = 0.15f;

    private static Dictionary<string, AssetLoadInfo> loadedAssets = new Dictionary<string, AssetLoadInfo>();
 
    private static List<AssetLoadInfo> recycleAssets = new List<AssetLoadInfo>();

    public static Dictionary<string, AssetLoadInfo> GetLoadedAssets()
    {
        return loadedAssets;
    }
    public static List<AssetLoadInfo> GetRecycleAssets()
    {
        return recycleAssets;
    }
    /// <summary>
    /// 根据设备内存大小开启对应的回收设置
    /// </summary>
   [RuntimeInitializeOnLoadMethod]
    private static void SetAssetRecycleLevel()
    {
       
        if (AssetRecycleLevelController.NowRecycleLevel== AssetRecycleLevel.Level1000)
        {
            triggerUnloadNumber = 12;
            unloadDelayTime = 0.12f;
        }
        else if (AssetRecycleLevelController.NowRecycleLevel == AssetRecycleLevel.Level1500)
        {
            triggerUnloadNumber = 25;
            unloadDelayTime = 0.15f;
        }
        else if (AssetRecycleLevelController.NowRecycleLevel == AssetRecycleLevel.Level2000)
        {
            triggerUnloadNumber = 35;
            unloadDelayTime = 0.2f;
        }
        else if (AssetRecycleLevelController.NowRecycleLevel == AssetRecycleLevel.Level3000)
        {
            triggerUnloadNumber = 50;
            unloadDelayTime = 0.5f;
        }
        else
        {
            triggerUnloadNumber = 150;
            unloadDelayTime = 0.5f;
        }
    }
    #region 加载资源
    public static T Load<T>(string name) where T : UnityEngine.Object
    {

        T t = ResourceManager.Load<T>(name);
        if (t != null)
        {
            MarkeFlag(name,t.GetType());
        }
        return t;
    }
    public static object Load(string name)
    {
        object t = ResourceManager.Load(name);
        if (t != null)
        {
            MarkeFlag(name, t.GetType());
        }
        return t;
    }
    public static string ReadTextFile(string textName)
    {
        if (textName == null)
        {
            throw new Exception("ResourceManager ReadTextFile -> textName is null !");
        }

        TextAsset text = Load<TextAsset>(textName);

        if (text == null)
        {
            throw new Exception("ReadTextFile not find " + textName);
        }
        else
        {
            return text.text;
        }
    }
    public static void LoadAsync(string name, Type resType, CallBack<LoadState,object> callback)
    {
        ResourceManager.LoadAsync(name,resType, (status, t) =>
        {
            if (status.isDone)
            {
                MarkeFlag(name, t.GetType());
            }
            try
            {
                if (callback != null)
                    callback(status, t);
            }
            catch (Exception e)
            {
                Debug.LogError("CreateGameObjectByPoolAsync Exception: " + e.ToString());
            }
           
        });
    }
    #endregion
    /// <summary>
    /// 标记加载次数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isGameObject">是否是GameObjext预制</param>
    public static void MarkeFlag(string name,Type assetType)
    {
        foreach (var item in recycleAssets)
        {
            if (item.name == name)
            {
                recycleAssets.Remove(item);
                break;
            }
        }
        if (!loadedAssets.ContainsKey(name))
        {
            loadedAssets.Add(name,new AssetLoadInfo(name,assetType, 1));
        }
        else
        {
            if (assetType!=typeof(GameObject))
                loadedAssets[name]++;
        }
    }

    public static void DestroyByPool(UnityEngine.Object asset)
    {
         string name = asset.name;
         DestroyByPool(name);
        
    }
    /// <summary>
    /// 放入回收池，标记回收
    /// </summary>
    /// <param name="name"></param>
    public static void DestroyByPool(string name, int times = 1)
    {
        if (times <= 0)
            times = 1;
        if (loadedAssets.ContainsKey(name))
        {
            loadedAssets[name] -= times;
            if (loadedAssets[name].number <= 0)
            {

                recycleAssets.Add(loadedAssets[name]);
                loadedAssets.Remove(name);
            }
        }
    }
    #region 释放资源
    public static void DisposeAll()
    {
        foreach (var item in recycleAssets)
        {
            if (ResourcesConfigManager.GetIsExitRes(item.name))
                ResourceManager.UnLoad(item.name);
        }
        recycleAssets.Clear();
    }
    private static List<AssetLoadInfo> removeItems = new List<AssetLoadInfo>();
    public static void DisposeOne()
    {
        removeItems.Clear();

        foreach (var item in recycleAssets)
        {
            if (ResourcesConfigManager.GetIsExitRes(item.name))
            {
                ResourceManager.UnLoad(item.name);
                removeItems.Add(item);
                break;
            }
            else
            {
                removeItems.Add(item);
            }
        }

        foreach (var item in removeItems)
        {
            recycleAssets.Remove(item);
        }
       
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += OnUpdate;
    }

    private static float tempTime = 0;
    private static void OnUpdate()
    {
        if (!OpenMemoryClean)
            return;

        if (recycleAssets.Count > triggerUnloadNumber)
        {
            if (tempTime <= 0)
            {
                tempTime = unloadDelayTime;
                DisposeOne();
            }
            else
            {
                tempTime -= Time.deltaTime;
            }
        }
    }
    #endregion
}

public class AssetLoadInfo
{
    public string name;
    public Type assetType;
    public int number;

    public AssetLoadInfo(string name,Type assetType,int number)
    {
        this.name = name;
        this.assetType = assetType;
        this.number = number;
    }

    public static AssetLoadInfo operator ++(AssetLoadInfo info)
    {
         info.number++;
        return info;
    }
    public static AssetLoadInfo operator -(AssetLoadInfo info,int num)
    {
        info.number-=num;
        return info;
    }
}

