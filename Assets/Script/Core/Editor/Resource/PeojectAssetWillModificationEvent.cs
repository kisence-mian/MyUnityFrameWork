using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Peoject 文件发生变化事件
/// </summary>
public class PeojectAssetWillModificationEvent : UnityEditor.AssetModificationProcessor
{
    [InitializeOnLoadMethod]
    private static void Add()
    {
        EditorApplication.update += ProjectWindowChanged;
    }

    private static void ProjectWindowChanged()
    {
        //Debug.Log("ProjectWindowChanged" + " :" + EditorApplication.timeSinceStartup);
        if (OnCreateAssetP != null)
        {
            if (OnCreateAssetCallBack != null)
                OnCreateAssetCallBack((string)OnCreateAssetP[0]);
            OnCreateAssetP = null;
        }

        if (OnSaveAssetsP != null)
        {
            if (OnSaveAssetsCallBack != null)
                OnSaveAssetsCallBack((string[])OnSaveAssetsP[0]);
            OnSaveAssetsP = null;
        }

        if (OnMoveAssetP != null)
        {
            if (OnMoveAssetCallBack != null)
                OnMoveAssetCallBack((AssetMoveResult)OnMoveAssetP[0], (string)OnMoveAssetP[1], (string)OnMoveAssetP[2]);
            OnMoveAssetP = null;
        }

        if (OnDeleteAssetP != null)
        {
            if (OnDeleteAssetCallBack != null)
                OnDeleteAssetCallBack((AssetDeleteResult)OnDeleteAssetP[0], (string)OnDeleteAssetP[1], (RemoveAssetOptions)OnDeleteAssetP[2]);
            OnDeleteAssetP = null;
        }

    }

    public static CallBack<string> OnWillCreateAssetCallBack;
    public static CallBack<string> OnCreateAssetCallBack;
    private static object[] OnCreateAssetP;
    private static void OnWillCreateAsset(string path)
    {
        //Debug.Log("OnWillCreateAsset " + path +" :"+ EditorApplication.timeSinceStartup);
        if (OnWillCreateAssetCallBack != null)
            OnWillCreateAssetCallBack(path);

        OnCreateAssetP = new object[] {path };
    }
    public static CallBackR<string[], string[]> OnWillSaveAssetsCallBack;
    public static CallBack< string[]> OnSaveAssetsCallBack;
    private static object[] OnSaveAssetsP;
    private static string[] OnWillSaveAssets(string[] paths)
    {
        if (OnWillSaveAssetsCallBack != null)
            OnWillSaveAssetsCallBack(paths);
        List<string> result = new List<string>();
        foreach (var path in paths)
        {
            if (IsUnlocked(path))
            {
                result.Add(path);
            }
            else
                Debug.LogError(path + " is read-only.");
            //Debug.Log("OnWillSaveAssets:" + path);
        }
        OnSaveAssetsP = new object[] { result .ToArray()};
        //Debug.Log("OnWillSaveAssets" + " :" + EditorApplication.timeSinceStartup);
     
        return result.ToArray();
    }

    public static CallBackR<AssetMoveResult, string, string> OnWillMoveAssetCallBack;
    public static CallBack<AssetMoveResult, string, string> OnMoveAssetCallBack;
    private static object[] OnMoveAssetP;
    private static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
    {
        if (OnWillMoveAssetCallBack != null)
            OnWillMoveAssetCallBack(oldPath, newPath);
        AssetMoveResult result = AssetMoveResult.DidNotMove;
        if (IsLocked(oldPath))
        {
            Debug.LogError(string.Format("Could not move {0} to {1} because {0} is locked!", oldPath, newPath));
            return AssetMoveResult.FailedMove;
        }
        else if (IsLocked(newPath))
        {
            Debug.LogError(string.Format("Could not move {0} to {1} because {1} is locked!", oldPath, newPath));
            return AssetMoveResult.FailedMove;
        }

        OnMoveAssetP = new object[] { result, oldPath, newPath };
        return result;
    }
    public static CallBackR<AssetDeleteResult, string, RemoveAssetOptions> OnWillDeleteAssetCallBack;
    public static CallBack<AssetDeleteResult, string, RemoveAssetOptions> OnDeleteAssetCallBack;
    private static object[] OnDeleteAssetP;
    private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions option)
    {
        if (OnWillDeleteAssetCallBack != null)
            OnWillDeleteAssetCallBack(assetPath, option);

        AssetDeleteResult res = AssetDeleteResult.DidDelete;
        if (IsLocked(assetPath))
        {
            Debug.LogError(string.Format("Could not delete {0} because it is locked!", assetPath));
            res= AssetDeleteResult.FailedDelete;
        }
        OnDeleteAssetP = new object[] { res, assetPath, option };
        Debug.Log("OnWillDeleteAsset:" + assetPath + " :" + EditorApplication.timeSinceStartup);

        return AssetDeleteResult.DidNotDelete;
    }

    public static CallBack<string> OnOpenAssetForEditCallBack;
    private static bool IsOpenForEdit(string assetPath, out string message)
    {
      //  Debug.Log("IsOpenForEdit: " + assetPath);
        if (IsLocked(assetPath))
        {
            message = "File is locked for editing!";
            return false;
        }
        else
        {
            message = null;
            if (OnOpenAssetForEditCallBack != null)
                OnOpenAssetForEditCallBack(assetPath);
            return true;
        }
    }

    static bool IsUnlocked(string path)
    {
        return !IsLocked(path);
    }

    static bool IsLocked(string path)
    {
        if (!File.Exists(path))
            return false;
        FileInfo fi = new FileInfo(path);
        return fi.IsReadOnly;
    }
}
