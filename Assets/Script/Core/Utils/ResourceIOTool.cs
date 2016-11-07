using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

/// <summary>
/// 资源读取器，负责从不同路径读取资源
/// </summary>
public class ResourceIOTool :MonoBehaviour
{

    static ResourceIOTool instance;
    public static ResourceIOTool GetInstance()
    {
        if (instance == null)
        {
            GameObject resourceIOTool = new GameObject();
            resourceIOTool.name = "ResourceIO";
            DontDestroyOnLoad(resourceIOTool);

            instance = resourceIOTool.AddComponent<ResourceIOTool>();
        }

        return instance;
    }

    #region 读操作
    public static string ReadStringByFile(string path)
    {
        StringBuilder line = new StringBuilder();
        try
        {
            if (!File.Exists(path))
            {
                Debug.Log("path dont exists ! : " + path);
                return "";
            }

            StreamReader sr = File.OpenText(path);
            line.Append(sr.ReadToEnd());

            sr.Close();
            sr.Dispose();
        }
        catch (Exception e)
        {
            Debug.Log("Load text fail ! message:" + e.Message);
        }

        return line.ToString();
    }

    public static string ReadStringByBundle(string path)
    {
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        TextAsset ta = (TextAsset)ab.mainAsset;
        string content = ta.ToString();
        ab.Unload(true);

        return content;
    }

    public static string ReadStringByResource(string path)
    {
        path = FileTool.RemoveExpandName(path);
        TextAsset text = (TextAsset)Resources.Load(path);

        if(text == null)
        {
            return "";
        }
        else
        {
            return text.text;
        }
    }

    public static void ResourceLoadAsync(string path,LoadCallBack callback)
    {
        GetInstance().MonoLoadMethod(path, callback);
    }

    public void MonoLoadMethod(string path, LoadCallBack callback)
    {
        StartCoroutine(MonoLoadByResourcesAsync(path, callback));
    }

    public IEnumerator MonoLoadByResourcesAsync(string path, LoadCallBack callback)
    {
        ResourceRequest status = Resources.LoadAsync(path);
        LoadState loadState = new LoadState(); 

        while (!status.isDone)
        {
            loadState.UpdateProgress(status);
            callback(loadState,null);

            yield return 0;
        }

        loadState.UpdateProgress(status);
        callback(loadState, status.asset);
    }

    /// <summary>
    /// 异步加载单个assetsbundle
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    public static void AssetsBundleLoadAsync(string path, AssetBundleLoadCallBack callback)
    {
        GetInstance().MonoLoadAssetsBundleMethod(path, callback);
    }

    public void MonoLoadAssetsBundleMethod(string path, AssetBundleLoadCallBack callback)
    {
        StartCoroutine(MonoLoadByAssetsBundleAsync(path, callback));
    }

    public IEnumerator MonoLoadByAssetsBundleAsync(string path, AssetBundleLoadCallBack callback)
    {
        AssetBundleCreateRequest status = AssetBundle.LoadFromFileAsync(path);
        LoadState loadState = new LoadState();

        while (!status.isDone)
        {
            loadState.UpdateProgress(status);
            callback(loadState, null);

            yield return 0;
        }

        status.assetBundle.name = path;
        loadState.UpdateProgress(status);
        callback(loadState, status.assetBundle);
    }

    #endregion

    #region 写操作

    public static void WriteStringByFile(string path, string content)
    {
        byte[] dataByte = Encoding.GetEncoding("UTF-8").GetBytes(content);

        CreateFile(path, dataByte);
    }

    //web Player 不支持该函数
#if !UNITY_WEBPLAYER
    public static void CreateFile(string path, byte[] byt)
    {
        try
        {
            FileTool.CreatFilePath(path);

            File.WriteAllBytes(path, byt);
        }
        catch (Exception e)
        {
            Debug.LogError("File Create Fail! \n" + e.Message);
        }
    }

#endif

    #endregion

}

public delegate void AssetBundleLoadCallBack(LoadState state, AssetBundle bundlle);
public delegate void LoadCallBack(LoadState loadState, object resObject);
public class LoadState
{
    private static LoadState completeState;

    public static LoadState CompleteState
    {
        get {
            if (completeState == null)
            {
                completeState = new LoadState();
                completeState.isDone = true;
                completeState.progress = 1;
            }
            return completeState; 
        }
    }

    //public object asset;
    public bool isDone;
    public float progress;

    public void UpdateProgress(ResourceRequest resourceRequest)
    {
        isDone = resourceRequest.isDone;
        progress = resourceRequest.progress;
    }

    public void UpdateProgress(AssetBundleCreateRequest assetBundleCreateRequest)
    {
        isDone = assetBundleCreateRequest.isDone;
        progress = assetBundleCreateRequest.progress;
    }

}
