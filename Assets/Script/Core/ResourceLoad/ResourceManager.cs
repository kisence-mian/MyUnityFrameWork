using UnityEngine;
using System.Collections;
using System.Text;

public static class ResourceManager 
{
    /// <summary>
    /// 游戏内资源读取类型
    /// </summary>
    public static ResLoadType gameLoadType = ResLoadType.Resource; //默认从resourcePath中读取

    public static string GetPath(string localPath, ResLoadType loadType)
    {
        StringBuilder path = new StringBuilder();
        switch (loadType)
        {
            case ResLoadType.Resource: 
                #if UNITY_EDITOR
                    path.Append( Application.dataPath);
                    path.Append("/Resourse/");
                    break;
                #endif
            case ResLoadType.Streaming:
                #if UNITY_EDITOR
                    path.Append(Application.dataPath);
                    path.Append("/StreamingAssets/");
                    break;
                #else
                    path.Append(Application.streamingAssetsPath);
                    path.Append("/");
                    break;
                #endif

            case ResLoadType.Persistent:
                path.Append(Application.persistentDataPath);
                path.Append("/");
                break;

            case ResLoadType.Catch:
                path.Append(Application.temporaryCachePath);
                path.Append("/");
                break;

            default:
                Debug.LogError("Type Error !" + loadType);
                break;
        }

        path.Append(localPath);

        return path.ToString();
    }

    //读取一个文本
    public static string ReadTextFile(string path,ResLoadType type = ResLoadType.Default)
    {
        return  ResourceIOTool.ReadStringByFile(GetPath(path,type));
    }

    //保存一个文本
    public static void WriteTextFile(string path,string content ,ResLoadType type = ResLoadType.Default)
    {
        ResourceIOTool.WriteStringByFile(GetPath(path, type),content);
    }

    //public static T GetResource<T>(string path)
    //{
    //    T resouce = new T();

    //    return resouce;
    //}
}

public enum ResLoadType
{
    Default,

    Resource,
    Streaming,
    Persistent,
    Catch,

    HotUpdate
}

public delegate void ResourceLoadCallBack<T>(T resObject);
