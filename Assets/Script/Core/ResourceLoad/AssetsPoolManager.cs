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
    private static Dictionary<string, int> loadedAssets = new Dictionary<string, int>();
    private static List<string> recycleAssets = new List<string>();

    public static Dictionary<string, int> GetLoadedAssets()
    {
        return loadedAssets;
    }
    public static List<String> GetRecycleAssets()
    {
        return recycleAssets;
    }
    public static T Load<T>(string name) where T : UnityEngine.Object
    {

        T t = ResourceManager.Load<T>(name);
        if (t != null)
        {
            bool isObj = t is GameObject;

                MarkeFlag(name,isObj);
            
            
        }
        return t;
    }
    public static object Load(string name)
    {
        object t = ResourceManager.Load(name);
        if (t != null)
        {

            bool isObj = t is GameObject;

            MarkeFlag(name, isObj);
        }
        return t;
    }
    public static void LoadAsync(string name, Type resType, CallBack<LoadState,object> callback)
    {
        ResourceManager.LoadAsync(name,resType, (status, res) =>
        {
            if (status.isDone)
            {
                bool isObj = res is GameObject;

                MarkeFlag(name, isObj);
            }
            try
            {
                if (callback != null)
                    callback(status, res);
            }
            catch (Exception e)
            {
                Debug.LogError("CreateGameObjectByPoolAsync Exception: " + e.ToString());
            }
           
        });
    }
    public static void MarkeFlag(string name,bool isGameObject)
    {
        if (recycleAssets.Contains(name))
        {
            recycleAssets.Remove(name);
        }
        if (!loadedAssets.ContainsKey(name))
        {
            loadedAssets.Add(name, 1);
        }
        else
        {
            if (!isGameObject)
                loadedAssets[name]++;
        }
    }

    public static void DestroyByPool(UnityEngine.Object asset)
    {
         string name = asset.name;
         DestroyByPool(name);
        
    }
    public static void DestroyByPool(string name)
    { 
            if (loadedAssets.ContainsKey(name))
            {
                loadedAssets[name]--;
                if (loadedAssets[name] <= 0)
                {
                    loadedAssets.Remove(name);
                    recycleAssets.Add(name);
                }
            }
    }

    public static void Dispose()
    {
        foreach (var item in recycleAssets)
        {
            ResourceManager.UnLoad(item);
        }
        recycleAssets.Clear();
    }
}

