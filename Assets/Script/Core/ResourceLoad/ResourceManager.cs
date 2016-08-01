using UnityEngine;
using System.Collections;
using System.Text;
/*
 * gameLoadType 为 Resource 时 ，所有资源从Resource读取
 * gameLoadType 不为 Resource时，资源读取方式从配置中读取
 * */
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
                    path.Append("/Resources/");
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

    public static ResLoadType GetLoadType(ResLoadType loadType)
    {
        if (gameLoadType == ResLoadType.Resource)
        {
            return ResLoadType.Resource;
        }

        if (loadType == ResLoadType.Default)
        {
            return gameLoadType;
        }

        return loadType;
    }

    //读取一个文本
    public static string ReadTextFile(string path)
    {
        TextAsset text = (TextAsset)Load(path);
        return text.text;
    }

    //保存一个文本
    public static void WriteTextFile(string path,string content ,ResLoadType type = ResLoadType.Default)
    {
        #if UNITY_EDITOR
            ResourceIOTool.WriteStringByFile(GetPath(path, ResLoadType.Resource), content);
        #else
            
        #endif

    }

    public static object Load(string name)
    {
        PackageConfig packData  = PackageConfigManager.GetPackageConfig(name);
        ResLoadType loadTypeTmp = GetLoadType(packData.loadType);

        if (loadTypeTmp == ResLoadType.Resource)
        {
            return Resources.Load(packData.path);
        }
        else
        {
            return AssetsBundleManager.Load(name);
        }
    }
    public static void LoadAsync(string name,LoadCallBack callBack)
    {
        PackageConfig packData  = PackageConfigManager.GetPackageConfig(name);
        ResLoadType loadTypeTmp = GetLoadType(packData.loadType);

        if (loadTypeTmp == ResLoadType.Resource)
        {
            ResourceIOTool.ResourceLoadAsync(packData.path, callBack);
        }
        else
        {
            AssetsBundleManager.LoadAsync(name,callBack);
        }
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

public delegate void LoadCallBack(object resObject);
