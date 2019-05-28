using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BundleEditorWindow : EditorWindow
{

    #region GUI

    //[MenuItem("Window/新打包设置编辑器 &1")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BundleEditorWindow));
    }

    void OnEnable()
    {
        EditorGUIStyleData.Init();
    }

    string[] toolbarTexts = { "打包", "热更新设置" };
    private int toolbarOption;
    bool deleteManifestFile = true;

    void OnGUI()
    {
        titleContent.text = "打包设置编辑器";
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
                PackageGUI();
                break;

            case 1:
                HotUpdateGUI();
                break;
        }
    }

    void PackageGUI()
    {
        GUILayout.Space(10);

        if (GUILayout.Button("生成资源路径文件"))
        {
            ResourcesConfigManager.CreateResourcesConfig();
        }

        if (GUILayout.Button("生成 AssetsBundle 设置"))
        {
            PackageService.SetAssetBundlesName();
        }

        if (GUILayout.Button("清除 AssetsBundle 设置"))
        {
            PackageService.ClearAssetBundlesName();
        }

        GUILayout.Space(10);

        deleteManifestFile = GUILayout.Toggle(deleteManifestFile, "打包后删除清单文件");

        if (GUILayout.Button("5.0 打包"))
        {
            PackageService.Package_5_0(deleteManifestFile);
        }
    }

    void HotUpdateGUI()
    {
        GUILayout.Space(10);

        if (!ConfigManager.GetIsExistConfig(HotUpdateManager.c_HotUpdateConfigName))
        {
            if (GUILayout.Button("热更新设置初始化"))
            {
                InitHotUpdateConfig();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();

            VersionService.LargeVersion = EditorGUILayout.IntField("large", VersionService.LargeVersion);
            VersionService.SmallVersion = EditorGUILayout.IntField("small", VersionService.SmallVersion);

            if (GUILayout.Button("保存版本文件"))
            {
                VersionService.CreateVersionFile();
            }

            GUILayout.EndHorizontal();
        }
    }

    #endregion

    #region 热更新

    void InitHotUpdateConfig()
    {
        Dictionary<string, SingleField> hotUpdateConfig = new Dictionary<string, SingleField>();
        hotUpdateConfig.Add(HotUpdateManager.c_testDownLoadPathKey, new SingleField(""));
        hotUpdateConfig.Add(HotUpdateManager.c_downLoadPathKey, new SingleField(""));
        hotUpdateConfig.Add(HotUpdateManager.c_UseTestDownLoadPathKey, new SingleField(false));

        ConfigEditorWindow.SaveData(HotUpdateManager.c_HotUpdateConfigName, hotUpdateConfig);
    }

    #endregion
}
