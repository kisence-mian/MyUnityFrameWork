using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 资源卸载控制器
/// </summary>
public class AssetsUnloadHandler
{

   
    public static Dictionary<string, UnloadAssetInfo> usedAssetsDic = new Dictionary<string, UnloadAssetInfo>();

    public static Dictionary<string, UnloadAssetInfo> noUsedAssetsDic = new Dictionary<string, UnloadAssetInfo>();
    public static List<UnloadAssetInfo> noUsedAssetsList = new List<UnloadAssetInfo>();

    private static Dictionary<string, UnloadAssetInfo> unloadBundleQue = new Dictionary<string, UnloadAssetInfo>();
    /// <summary>
    /// 记录资源的加载
    /// </summary>
    /// <param name="assets"></param>
    public static void MarkUseAssets(AssetsData assets, bool isHaveDependencies)
    {
        if (assets == null)
            return;
        UnloadAssetInfo info= MakeUseLogic(assets.assetName, assets, isHaveDependencies);

        //无依赖的资源进入卸载Bundle压缩包的队列
        if (assets.AssetBundle != null && !isHaveDependencies)
        {
            info.unloadBundleTime = 5;
            if (!unloadBundleQue.ContainsKey(info.assetsName))
                unloadBundleQue.Add(info.assetsName, info);
        }
    }
    /// <summary>
    /// 记录资源的使用（比如对象池重复使用一次记录一次）
    /// </summary>
    /// <param name="assets"></param>
    public static void MarkUseAssets(string assetsName)
    {
        if (!ResourceManager.UseCache)
            return ;
        if (usedAssetsDic.ContainsKey(assetsName))
            MakeUseLogic(assetsName);
    }

    private static UnloadAssetInfo MakeUseLogic(string assetName, AssetsData assets=null, bool isHaveDependencies=true)
    {

        UnloadAssetInfo info = null;
        if (noUsedAssetsDic.ContainsKey(assetName))
        {
            info = noUsedAssetsDic[assetName];
            noUsedAssetsDic.Remove(assetName);

            if (noUsedAssetsList.Contains(info))
                noUsedAssetsList.Remove(info);

            usedAssetsDic.Add(assetName, info);
        }
        else if (usedAssetsDic.ContainsKey(assetName))
        {
            info = usedAssetsDic[assetName];
        }
        else
        {
            info = new UnloadAssetInfo(isHaveDependencies, assets);
            usedAssetsDic.Add(assetName, info);
            //noUsedAssetsList.Add(info);
        }
        if (assets != null)
        {
            info.assets = assets;
        }
        info.useTimes++;
        return info;
    }
    /// <summary>
    /// 当资源在游戏中无引用时，传过来记录
    /// </summary>
    /// <param name="assets"></param>
    public static void DiscardAssets(AssetsData assets)
    {
        if (!usedAssetsDic.ContainsKey(assets.assetName))
        {
            //Debug.LogError("资源未加载=>：" + assets.assetPath);
            return;
        }
        UnloadAssetInfo info = null;
        info = usedAssetsDic[assets.assetName];
        usedAssetsDic.Remove(assets.assetName);

        info.discardTime = Time.realtimeSinceStartup;

        noUsedAssetsDic.Add(info.assetsName, info);
        noUsedAssetsList.Add(info);
        noUsedAssetsList.Sort(SortNoUsedAssetsInfo);
    }

    private static int SortNoUsedAssetsInfo(UnloadAssetInfo x, UnloadAssetInfo y)
    {
        //自资源废弃到现在的时间 / 资源使用次数
        float freqX = x.GetFrequency();
        float freqY = y.GetFrequency();

        if (freqX < freqY)
        {
            return 1;
        }
        else if (freqX > freqY)
            return -1;

        return 0;
    }

    public static void UnloadAll()
    {
        foreach (var info in noUsedAssetsList)
        {
            if (unloadBundleQue.ContainsKey(info.assetsName))
                unloadBundleQue.Remove(info.assetsName);
            ResourceManager.ReleaseByPath(info.assets.assetPath);
        }
        noUsedAssetsList.Clear();
    }
    public static void UnloadOne()
    {
        if (noUsedAssetsList.Count > 0)
        {
            UnloadAssetInfo info = noUsedAssetsList[0];
            noUsedAssetsList.RemoveAt(0);
            if (unloadBundleQue.ContainsKey(info.assetsName))
                unloadBundleQue.Remove(info.assetsName);
            ResourceManager.ReleaseByPath(info.assets.assetPath);
        }
    }
    public static void LateUpdate()
    {
        if (unloadBundleQue.Count > 0)
        {
            foreach (var keyValue in unloadBundleQue)
            {
                var item = keyValue.Value;
                if (item != null)
                {
                    item.unloadBundleTime -= Time.deltaTime;
                    if (item.unloadBundleTime <= 0)
                    {
                        if (item.assets != null && item.assets.AssetBundle != null)
                        {
                            item.assets.AssetBundle.Unload(false);
                            item.assets.AssetBundle = null;
                        }
                        unloadBundleQue.Remove(keyValue.Key);
                        break;
                    }
                }
                else
                {
                    unloadBundleQue.Remove(keyValue.Key);
                    Debug.LogError("Unload res is null .name:" + keyValue.Key);
                    break;
                }
            }
        }
    }
}

public class UnloadAssetInfo
{
    public string assetsName;
    public int useTimes = 0;
    /// <summary>
    /// 资源不再使用（引用为0）时的时间
    /// </summary>
    public float discardTime;
    public bool isHaveDependencies;
    public AssetsData assets;

    public float unloadBundleTime;
    public UnloadAssetInfo(bool isHaveDependencies, AssetsData assets)
    {
        this.assetsName = assets.assetName;
        this.isHaveDependencies = isHaveDependencies;
        this.assets = assets;
    }

    public float GetFrequency()
    {
        return (Time.realtimeSinceStartup - discardTime) / useTimes;
    }
}
