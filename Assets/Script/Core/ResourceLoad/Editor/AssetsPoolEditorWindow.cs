using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class AssetsPoolEditorWindow : EditorWindow {

    [MenuItem("Window/资源池管理(1003)", priority = 1003)]
    public static void OpenWindow()
    {
        AssetsPoolEditorWindow win = GetWindow<AssetsPoolEditorWindow>();
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
        EditorWindow.FocusWindowIfItsOpen<AssetsPoolEditorWindow>();
        win.Ini();
    }
    Dictionary<Type, List<AssetLoadInfo>> assetsInfos = new Dictionary<Type, List<AssetLoadInfo>>();
    Dictionary<Type, List<AssetLoadInfo>> recycleAssetsInfos = new Dictionary<Type, List<AssetLoadInfo>>();
    private void Ini()
    {
        assetsInfos.Add(typeof(AudioClip), new List<AssetLoadInfo>());
        assetsInfos.Add(typeof(Texture2D), new List<AssetLoadInfo>());
        assetsInfos.Add(typeof(Sprite), new List<AssetLoadInfo>());
        assetsInfos.Add(typeof(GameObject), new List<AssetLoadInfo>());
        assetsInfos.Add(typeof(TextAsset), new List<AssetLoadInfo>());
        assetsInfos.Add(typeof(Shader), new List<AssetLoadInfo>());
        assetsInfos.Add(typeof(Material), new List<AssetLoadInfo>());
    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "资源池", "对象池" };
    private void OnGUI()
    {
        GUILayout.Box("内存占用：" + MemoryManager.totalAllocatedMemory.ToString("F") + "MB");
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));

        switch (toolbarOption)
        {
            case 0:
                AssetsPoolGUI();
                break;
            case 1:
                GameObjectPoolGUI();
                break;
        }
              
       
    }

    private void GameObjectPoolGUI()
    {
        EditorDrawGUIUtil.DrawScrollView(this, () =>
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            DataDetail();
            Dictionary<string, List<GameObject>> createPools = GameObjectManager.GetCreatePool();
            Dictionary<string, List<GameObject>> recyclePools = GameObjectManager.GetRecyclePool();
            GUILayout.Box("创建记录(" + createPools.Count + ")：");
            foreach (var item in createPools)
            {
                string ss = " Create -->" + item.Key + " : " + item.Value.Count+ " Recycle : ";
                if (recyclePools.ContainsKey(item.Key))
                {
                    ss += "" + recyclePools[item.Key].Count;
                }
                GUILayout.Label(ss);
            }
           
            GUILayout.EndVertical();
            // GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            Dictionary<string, List<GameObject>> noCreate = new Dictionary<string, List<GameObject>>();
            foreach (var item in recyclePools)
            {
                if (createPools.ContainsKey(item.Key))
                    continue;
                else
                {
                    noCreate.Add(item.Key, item.Value);
                }
            }
          
            GUILayout.Box("回收池无创建记录(" + noCreate.Count + ")：");
            foreach (var item in noCreate)
            {
                string ss = " Recycle -->" + item.Key + " : " + item.Value.Count;
                GUILayout.Label(ss);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        });
    }

    private void AssetsPoolGUI()
    {
        AssetsPoolManager.triggerUnloadNumber = (int)EditorDrawGUIUtil.DrawBaseValue("回收触发数量", AssetsPoolManager.triggerUnloadNumber);
        AssetsPoolManager.unloadDelayTime = (float)EditorDrawGUIUtil.DrawBaseValue("回收时间间隔", AssetsPoolManager.unloadDelayTime);
        if (AssetsPoolManager.unloadDelayTime <= 0)
            AssetsPoolManager.unloadDelayTime = 0.05f;
        EditorDrawGUIUtil.DrawScrollView(this, () =>
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            DataDetail();
            Dictionary<string, AssetLoadInfo> loadedAssets = AssetsPoolManager.GetLoadedAssets();
            GUILayout.Box("加载记录(" + loadedAssets.Count + ")：");
            foreach (var item in assetsInfos)
            {
                DrawTypeAssets(item.Key, item.Value);
            }
            GUILayout.EndVertical();
            // GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            List<AssetLoadInfo> reCover = AssetsPoolManager.GetRecycleAssets();
            GUILayout.Box("回收池记录(" + reCover.Count + ")：");
            foreach (var item in recycleAssetsInfos)
            {
                DrawTypeAssets(item.Key, item.Value);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        });
    }
    private void DataDetail()
    {
        foreach (var item in assetsInfos)
        {
            item.Value.Clear();
        }
        Dictionary<string, AssetLoadInfo> loadedAssets = AssetsPoolManager.GetLoadedAssets();
        foreach (var item in loadedAssets)
        {
            if (assetsInfos.ContainsKey(item.Value.assetType))
            {
                assetsInfos[item.Value.assetType].Add(item.Value);
            }
            else
            {
                assetsInfos.Add(item.Value.assetType, new List<AssetLoadInfo>() { item.Value });
            }
        }

        foreach (var item in recycleAssetsInfos)
        {
            item.Value.Clear();
        }
        List<AssetLoadInfo> reCover = AssetsPoolManager.GetRecycleAssets();
        foreach (var item in reCover)
        {
            if (recycleAssetsInfos.ContainsKey(item.assetType))
            {
                recycleAssetsInfos[item.assetType].Add(item);
            }
            else
            {
                recycleAssetsInfos.Add(item.assetType, new List<AssetLoadInfo>() { item });
            }
        }
    }
    private void DrawTypeAssets(Type assetType,List<AssetLoadInfo> infos)
    {
        EditorDrawGUIUtil.DrawFoldout(infos, assetType.Name+"("+infos.Count+")", () =>
          {
              foreach (var item in infos)
              {
                  GUILayout.Box("=>" + item.name + " : " + item.number);
              }
          });
    }
}
