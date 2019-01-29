
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// 资源文件变化事件
/// </summary>
public class PeojectAssetChangeEvent  {

    [InitializeOnLoadMethod]
    static void EventFileChange()
    {
        PeojectAssetWillModificationEvent.OnCreateAssetCallBack += OnCreateAssetCallBack;
        PeojectAssetWillModificationEvent.OnDeleteAssetCallBack += OnDeleteAssetCallBack;
        PeojectAssetWillModificationEvent.OnMoveAssetCallBack += OnMoveAssetCallBack;
        PeojectAssetWillModificationEvent.OnSaveAssetsCallBack += OnSaveAssetsCallBack;
        EditorApplication.projectWindowChanged += OnProjectWindowChanged;
    }

    private static void OnProjectWindowChanged()
    {
        UpdateAsset(null);
    }

    private static void OnSaveAssetsCallBack(string[] t)
    {
        List<string> paths = new List<string>();
        paths.AddRange(t);
        UpdateAsset(paths);
        //Debug.Log("OnSaveAssetsCallBack");
        //GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnSaveAssets, t);
    }

    private static void OnMoveAssetCallBack(AssetMoveResult t, string t1, string t2)
    {
        List<string> paths = new List<string>();
        paths.Add(t1);
        paths.Add(t2);
        UpdateAsset(paths);
        //Debug.Log("OnMoveAssetCallBack");
        //GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnMoveAsset, t,t1,t2);
    }

    private static void OnDeleteAssetCallBack(AssetDeleteResult t, string t1, RemoveAssetOptions t2)
    {
        List<string> paths = new List<string>();
        paths.Add(t1);
        UpdateAsset(paths);
        //Debug.Log("OnDeleteAssetCallBack");
        //GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnDeleteAsset, t, t1, t2);


    }

    private static void OnCreateAssetCallBack(string t)
    {
        List<string> paths = new List<string>();
        paths.Add(t);
        UpdateAsset(paths);
        //Debug.Log("OnCreateAssetCallBack");
        //GlobalEvent.DispatchEvent(EditorGlobalEventEnum.OnCreateAsset, t);
    }

    private static void UpdateAsset(List<string> paths)
    {
        bool isUpdate = false;
        if (paths == null)
            isUpdate = true;
        else
        {
            foreach (var item in paths)
            {
                if (item.Contains("Assets/Resources"))
                {
                    isUpdate = true;
                    break;
                }
            }
        }
        if (isUpdate)
        {
            if(ResourcesConfigManager.GetIsExistResources())
            {
                ResourcesConfigManager.CreateResourcesConfig();
                ResourcesConfigManager.ClearConfig();
                AssetDatabase.Refresh();
                Debug.Log("创建资源路径文件");
            }
        }
    }
}
