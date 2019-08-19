using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BundleEditorWindow : EditorWindow
{

    #region GUI



    [MenuItem("Window/新打包设置编辑器 &1")]

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
            long start = DateTime.Now.Ticks;
            Debug.Log("生成 AssetsBundle 设置=>");
            PackageService.SetAssetBundlesName();
            Debug.Log("结束，用时：" + ((DateTime.Now.Ticks - start) / 10000 / 1000f) + "s");
        }
        //if (GUILayout.Button("生成 AssetsBundle 设置 New"))
        //{
        //    long start = DateTime.Now.Ticks;
        //    Debug.Log("生成 AssetsBundle 设置New=>");
        //    PackageService.SetAllResourceBundleName("Assets/Resources/",null);
        //    Debug.Log("结束，用时：" + ((DateTime.Now.Ticks - start) / 10000 / 1000f) + "s");
        //}

        if (GUILayout.Button("清除 AssetsBundle 设置"))
        {
            long start = DateTime.Now.Ticks;
            Debug.Log("清除 AssetsBundle 设置=>");
            PackageService.ClearAssetBundlesName();
            Debug.Log("结束，用时：" + ((DateTime.Now.Ticks - start) / 10000 / 1000f) + "s");
        }

        if (GUILayout.Button("清除并重新生成 AssetsBundle 设置"))
        {
            PackageService.ClearAssetBundlesName();
            PackageService.SetAssetBundlesName();
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

    #region 菜单工具

    [MenuItem("Tools/资源/显示选中对象所有依赖资源")]
    public static void ShowAllCorrelationResource()
    {
        UnityEngine.Object[] roots = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
        Selection.objects = EditorUtility.CollectDependencies(roots);
    }

    [MenuItem("Tools/资源/显示所有引用选中资源的对象")]
    public static void ShowAllQuotePefab()
    {
        selects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
        objects.Clear();
        resourcePath = Application.dataPath + "/Resources/";
        direIndex = resourcePath.LastIndexOf("/Resources/");
        direIndex += "/Resources/".Length;
        assetsIndex = resourcePath.LastIndexOf("Assets/");

        RecursionDirectory(Application.dataPath + "/Resources/");
        //RecursionDirectory(Application.dataPath + "/_Resources/");

        Debug.Log("共有 " + objects.Count + " 个预设使用了选中资源");

        Selection.objects = objects.ToArray();
    }

    static UnityEngine.Object[] selects;
    static int direIndex = 0;
    static int assetsIndex = 0;
    static string resourcePath;

    static List<UnityEngine.Object> objects = new List<UnityEngine.Object>();

    static void RecursionDirectory(string path)
    {
        if (!File.Exists(path))
        {
            FileTool.CreatPath(path);
        }

        string[] dires = Directory.GetDirectories(path);

        for (int i = 0; i < dires.Length; i++)
        {
            RecursionDirectory(dires[i]);
        }

        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            string f = files[i];
            if (f.EndsWith(".meta") || f.EndsWith(".exe"))
                continue;
            else
            {
                string relativePath = FileTool.RemoveExpandName(f.Substring(direIndex));
                string assetsPath = f.Substring(assetsIndex);
                UnityEngine.Object obj = Resources.Load(relativePath);
                if (obj == null)
                {
                    Debug.LogError("Resources obj is null ->" + relativePath);
                }

                FindAsset(obj, assetsPath);
            }
        }
    }

    static UnityEngine.Object[] GetCorrelationResource(UnityEngine.Object go)
    {
        UnityEngine.Object[] roots = new UnityEngine.Object[] { go };
        return EditorUtility.CollectDependencies(roots);
    }

    private static void FindAsset(UnityEngine.Object obj, string assetsPath)
    {
        //Debug.Log(assetsPath);
        UnityEngine.Object[] objs = GetCorrelationResource(obj);

        for (int i = 0; i < selects.Length; i++)
        {
            for (int j = 0; j < objs.Length; j++)
            {
                if(selects[i] == objs[j])
                {
                    objects.Add(obj);
                }
            }
        }
    }

    #endregion
}
