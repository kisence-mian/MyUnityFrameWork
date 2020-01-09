
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class AssetBundleLoader : LoaderBase
{
    public AssetBundleLoader(AssetsLoadController loadAssetsController) : base(loadAssetsController)
    {
    }

    public override IEnumerator LoadAssetsIEnumerator(string path, Type resType, CallBack<AssetsData> callBack)
    {
        // string temp = UnityExtendUtils.GetWWWLoadPath(path);

        //加载bundle文件
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
        yield return req;
        AssetBundle ab = req.assetBundle;
        AssetBundleRequest abReq = null;
        if (resType != null)
        {
            abReq = ab.LoadAllAssetsAsync(resType);
        }
        else
        {
            abReq = ab.LoadAllAssetsAsync();
        }
        yield return abReq;
        AssetsData ad = new AssetsData(path);
        ad.Assets = abReq.allAssets;

        if (callBack != null)
            callBack(ad);

        yield return 0;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isLoadDependencieRes">是否加载依赖</param>
    /// <returns></returns>
    public override AssetsData LoadAssets(string path)
    {
        //Debug.Log("Load Path :" + path);
        AssetBundle ab = LoadAssetBundle(path);
        if (ab == null)
            return null;
        UnityEngine.Object[] ass = ab.LoadAllAssets();
        AssetsData ad = new AssetsData(path);
        ad.Assets = ass;
        //ab.Unload(false);
        ad.AssetBundle = ab;
        return ad;
    }
    private AssetBundle LoadAssetBundle(string path)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            //Application.streamingAssetsPath ＝ jar:file:///data/app/com.xxx.xxx-1.apk!/assets

            //Application.dataPath+”!assets” ＝ /data/app/com.xxx.xxx-1.apk!assets
            path = path.Replace(@"jar:file://", "");
            path = path.Replace("apk!/assets", "apk!assets");
        }
        AssetBundle ab = AssetBundle.LoadFromFile(path);

        if (ab == null)
        {
            Debug.LogError("Load Sources failed! path: " + path);
            return null;
        }
        return ab;
    }


  
    public override string[] GetAllDependenciesName(string path)
    {
        return AssetsManifestManager.GetAllDependenciesPaths(path);
    }

    public override bool IsHaveDependencies(string path)
    {
        return AssetsManifestManager.IsHaveDependencies(path);
    }
    public override AssetsData LoadAssets<T>(string path)
    {
        return LoadAssets(path);
    }
}


