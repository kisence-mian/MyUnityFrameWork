using UnityEngine;
using System.Collections;
using System.Text;
using System;

/*
 * gameLoadType 为 Resource 时 ，所有资源从Resource读取
 * gameLoadType 不为 Resource时，资源读取方式从配置中读取
 * */
public static class ResourceManager 
{
    /// <summary>
    /// 游戏内资源读取类型
    /// </summary>
    public static ResLoadLocation m_gameLoadType = ResLoadLocation.Resource; //默认从resourcePath中读取

    public static ResLoadLocation GetLoadType(ResLoadLocation loadType)
    {
        //如果设置从Resource中加载则忽略打包设置
        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            return ResLoadLocation.Resource;
        }

        return loadType;
    }

    public static bool GetResourceIsExist(string name)
    {
        if (name == null)
        {
            throw new Exception("ResourceManager GetResourceIsExist -> name is null !");
        }

        return  ResourcesConfigManager.GetIsExitRes(name);
    }

    //读取一个文本
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

    public static object Load(string name)
    {
        if(name == null)
        {
            throw new Exception("ResourceManager Load -> name is null !");
        }

        string path = ResourcesConfigManager.GetResourcePath(name);

        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            return Resources.Load(path);
        }
        else
        {
            return AssetsBundleManager.Load(path);
        }
    }

    public static T Load<T>(string name) where T: UnityEngine.Object
    {
        if (name == null)
        {
            throw new Exception("ResourceManager Load<T> -> name is null !");
        }

        string path = ResourcesConfigManager.GetResourcePath(name);

        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            return Resources.Load<T>(path.Replace("/","\\"));
        }
        else
        {
            return AssetsBundleManager.Load<T>(path);
        }
    }

    public static void LoadAsync(string name,LoadCallBack callBack)
    {
        if (name == null)
        {
            throw new Exception("ResourceManager LoadAsync -> name is null !");
        }
       string path = ResourcesConfigManager.GetResourcePath(name);

        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            ResourceIOTool.ResourceLoadAsync(path,null, callBack);
        }
        else
        {
            AssetsBundleManager.LoadAsync(path,null, callBack);
        }
    }
    public static void LoadAsync(string name,Type resType, LoadCallBack callBack)
    {
        if (name == null)
        {
            throw new Exception("ResourceManager LoadAsync -> name is null !");
        }
        string path = ResourcesConfigManager.GetResourcePath(name);

        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            ResourceIOTool.ResourceLoadAsync(path,resType, callBack);
        }
        else
        {
            AssetsBundleManager.LoadAsync(path,resType, callBack);
        }
    }

    public static void UnLoad(string name)
    {
        if (name == null)
        {
            throw new Exception("ResourceManager UnLoad -> name is null !");
        }

        string path = ResourcesConfigManager.GetResourcePath(name);

        if (m_gameLoadType == ResLoadLocation.Resource)
        {

        }
        else
        {
            AssetsBundleManager.UnLoadBundle(path);
        }
    }
}

public enum ResLoadLocation
{
    Resource,
    Streaming,
    Persistent,
    Catch,
}



