
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourcesLoader : LoaderBase
{
    public ResourcesLoader(AssetsLoadController loadAssetsController) : base(loadAssetsController)
    {
    }

    public override IEnumerator LoadAssetsIEnumerator(string path, Type resType, CallBack<AssetsData> callBack)
    {
        AssetsData rds = null;
        string s = PathUtils.RemoveExtension(path);
        ResourceRequest ass = null;
        if (resType != null)
        {
            ass = Resources.LoadAsync(s, resType);
        }
        else
        {
            ass = Resources.LoadAsync(s);
        }
        yield return ass;

        if (ass.asset != null)
        {
            rds = new AssetsData(path);
            rds.Assets = new Object[] { ass.asset };
        }
        else
        {
            Debug.LogError("加载失败,Path:" + path);
        }

        if (callBack != null)
            callBack(rds);
        yield return new WaitForEndOfFrame();

    }
    public override AssetsData LoadAssets(string path)
    {
        string s = PathUtils.RemoveExtension(path);
        AssetsData rds = null;
        Object ass = Resources.Load(s);
        if (ass != null)
        {
            rds = new AssetsData(path);
            rds.Assets = new Object[] { ass };
        }
        else
        {
            Debug.LogError("加载失败,Path:" + path);
        }
        return rds;
    }


    public override AssetsData LoadAssets<T>(string path)
    {

        string s = PathUtils.RemoveExtension(path);
        AssetsData rds = null;
        T ass = Resources.Load<T>(s);
        if (ass != null)
        {
            rds = new AssetsData(path);
            rds.Assets = new Object[] { ass };
        }
        else
        {
            Debug.LogError("加载失败,Path:" + path);
        }
        return rds;
    }
}

