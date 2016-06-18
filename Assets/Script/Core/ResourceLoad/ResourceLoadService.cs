using UnityEngine;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// 资源读取器，负责从不同路径读取资源
/// </summary>
public static class ResourceLoadService 
{
    //private static ResLoadType loadType = ResLoadType.ResourcePath; //默认从resourcePath中读取

    ///// <summary>
    ///// 游戏内资源读取类型
    ///// </summary>
    //public static ResLoadType LoadType
    //{
    //    get { return ResourceLoadService.loadType; }
    //    set {
    //        //资源读取器不能使用默认设置
    //        ResourceLoadService.loadType = value;
    //        if (loadType == ResLoadType.useGameSetting)
    //        {
    //            loadType = ResLoadType.ResourcePath;

    //            Debug.LogWarning("ResourceManager.LoadType don't use ResLoadType.useGameSetting !");
    //        }
    //    }
    //}

    ///// <summary>
    ///// 同步读取一个资源
    ///// </summary>
    ///// <param name="path">资源路径</param>
    ///// <param name="mloadType">读取类型，默认使用游戏内设置</param>
    ///// <returns></returns>
    //public static AssetBundle LoadObject(string path,ResLoadType mloadType = ResLoadType.useGameSetting)
    //{
    //    switch (mloadType)
    //    {
    //        case ResLoadType.useGameSetting:
    //            return LoadObject(path,loadType);

    //        case ResLoadType.ResourcePath:
    //            return LoadObjectByResource(path);
    //        case ResLoadType.streamingAssetsPath:
    //            return LoadObjectByStreaming(path);
    //        case ResLoadType.persistentDataPath:
    //            return LoadObjectByPersistent(path);

    //    }
    //    return null;
    //}

    ///// <summary>
    ///// 异步读取一个资源
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="path"></param>
    ///// <param name="callBack"></param>
    ///// <returns></returns>
    //public static object LoadObjectAsync<T>(string path, ResourceLoadCallBack<T> callBack)
    //{
    //    return null;
    //}

    //static AssetBundle LoadObjectByResource(string path)
    //{
    //    //AssetBundle bundle = new AssetBundle();
    //    //bundle.mainAsset = Resources.Load(path);

    //    return null;
    //}

    //static AssetBundle LoadObjectByStreaming(string path)
    //{
    //    Debug.LogError("yet cant sync load streamData resource ! path:" + path);
    //    return null;
    //}

    //static AssetBundle LoadObjectByPersistent(string path)
    //{
    //    //使用流的形式读取
    //    StreamReader sr = null;
    //    try
    //    {
    //        sr = File.OpenText(path );
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError(e.ToString());
    //        //路径与名称未找到文件则直接返回空
    //        return null;
    //    }
    //    string line;
    //    ArrayList arrlist = new ArrayList();
    //    while ((line = sr.ReadLine()) != null)
    //    {
    //        //一行一行的读取
    //        //将每一行的内容存入数组链表容器中
    //        arrlist.Add(line);
    //    }
    //    //关闭流
    //    sr.Close();
    //    //销毁流
    //    sr.Dispose();
    //    return null;
    //}
}

//public enum ResLoadType
//{
//    useGameSetting,
//    ResourcePath,
//    streamingAssetsPath,
//    persistentDataPath
//}

//public delegate void ResourceLoadCallBack<T>(T resObject);
