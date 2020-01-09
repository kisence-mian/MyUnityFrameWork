using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Profiling;


public class AssetsPoolEditorWindow : EditorWindow
{

    [MenuItem("Window/资源池管理(1101)", priority = 1101)]
    public static void OpenWindow()
    {
        AssetsPoolEditorWindow win = GetWindow<AssetsPoolEditorWindow>();
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
        EditorWindow.FocusWindowIfItsOpen<AssetsPoolEditorWindow>();
        win.Ini();
    }

    private void OnEnable()
    {
        FocusWindowIfItsOpen<AssetsPoolEditorWindow>();
        Ini();
    }

    Dictionary<string, AssetsData> assetsCaches = new Dictionary<string, AssetsData>();

    private void Ini()
    {
        assetsCaches = ResourceManager.GetLoadAssetsController().GetLoadAssets();

    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "资源池", "对象池", "Bundle依赖" };

    private int toolbarOption1 = 0;
    private string[] toolbarTexts1 = { "资源池", "卸载池" };
    private void OnGUI()
    {
        assetsCaches = ResourceManager.GetLoadAssetsController().GetLoadAssets();
        EditorDrawGUIUtil.CanEdit = false;

        GUILayout.Box("内存占用：" + (Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f).ToString("F") + "MB");
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));

        switch (toolbarOption)
        {
            case 0:
                toolbarOption1 = GUILayout.Toolbar(toolbarOption1, toolbarTexts1, GUILayout.Width(Screen.width * 3 / 4));

                GUILayout.Space(3);
                bundleSearchValue = EditorDrawGUIUtil.DrawSearchField(bundleSearchValue);
                GUILayout.Space(4);
                switch (toolbarOption1)
                {
                    case 0:
                        DrawLoadAssets();
                        break;
                    case 1:
                        DrawUnloadAssets();
                        break;

                }
                //AssetsPoolGUI();
                break;
            case 1:
                GameObjectPoolGUI();
                break;
            case 2:
                DrawBundleDependencies();
                break;
        }

        EditorDrawGUIUtil.CanEdit = true;
    }
    private string dependencieSearchValue = "";
    private void DrawBundleDependencies()
    {
        dependencieSearchValue = EditorDrawGUIUtil.DrawSearchField(dependencieSearchValue);

        Dictionary<string, string[]> dependencieNamesDic = AssetsManifestManager.GetDependencieNamesDic();

        EditorDrawGUIUtil.DrawScrollView(this, () =>
        {
            foreach (var par in dependencieNamesDic)
            {

                if (!string.IsNullOrEmpty(dependencieSearchValue))
                {
                    bool isSearch = false;

                    foreach (var item in par.Value)
                    {
                        if (item.Contains(dependencieSearchValue))
                        {
                            isSearch = true;
                            break;
                        }
                    }
                    if (!isSearch)
                    {
                        if (par.Key.Contains(dependencieSearchValue))
                        {
                            isSearch = true;
                        }
                    }
                    if (!isSearch)
                        continue;
                }
                EditorDrawGUIUtil.DrawFoldout(par.Key, par.Key + "(" + par.Value.Length + ")", () =>
                        {
                            EditorDrawGUIUtil.CanEdit = true;
                            foreach (var assName in par.Value)
                            {

                                EditorDrawGUIUtil.DrawBaseValue(assName, assName);
                            }
                            EditorDrawGUIUtil.CanEdit = false;
                        });
            }
        }, "box");
    }

    private void DrawUnloadAssets()
    {


        GUILayout.Space(4);
        EditorDrawGUIUtil.DrawScrollView(this, () =>
        {
            List<UnloadAssetInfo> assetList = AssetsUnloadHandler.noUsedAssetsList;

            for (int i = 0; i < assetList.Count; i++)
            {
                UnloadAssetInfo item = assetList[i];
                if (!string.IsNullOrEmpty(bundleSearchValue))
                {
                    if (!item.assetsName.Contains(bundleSearchValue.ToLower()))
                        continue;
                }

                EditorDrawGUIUtil.DrawFoldout(item, "(" + item.useTimes + ")" + item.assetsName + "(" + item.GetFrequency() + ")", () =>
                             {
                                 DrawAssetsData(i, item.assets);
                             });
            }
        }, "box");
    }

    private void GameObjectPoolGUI()
    {
        EditorDrawGUIUtil.DrawScrollView(this, () =>
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

                //DataDetail();
                Dictionary<string, Dictionary<int, GameObject>> createPools = GameObjectManager.GetCreatePool();
            Dictionary<string, Dictionary<int, GameObject>> recyclePools = GameObjectManager.GetRecyclePool();
            GUILayout.Box("创建记录(" + createPools.Count + ")：");
            foreach (var item in createPools)
            {
                string ss = " Create -->" + item.Key + " : " + item.Value.Count + " Recycle : ";
                if (recyclePools.ContainsKey(item.Key))
                {
                    ss += "" + recyclePools[item.Key].Count;
                }
                EditorDrawGUIUtil.DrawFoldout(item, ss, () =>
                  {
                      foreach (var obj in item.Value.Values)
                      {
                          EditorDrawGUIUtil.DrawBaseValue(obj.name, obj);
                      }
                  });
                    // GUILayout.Label(ss);
                }

            GUILayout.EndVertical();
                // GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();

            Dictionary<string, Dictionary<int, GameObject>> noCreate = new Dictionary<string, Dictionary<int, GameObject>>();
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
    private string bundleSearchValue = "";
    private bool onlyShowNoRef = false;
    private void DrawLoadAssets()
    {
        long allSize = 0;
        foreach (var item in assetsCaches)
        {
            allSize += item.Value.GetTotalMemorySize();

        }
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("资源占用：" + (allSize / 1024f / 1024).ToString("F") + "MB");
        GUILayout.Space(4);
        EditorDrawGUIUtil.CanEdit = true;
        onlyShowNoRef = (bool)EditorDrawGUIUtil.DrawBaseValue("是否只显示无引用资源", onlyShowNoRef);
        EditorDrawGUIUtil.CanEdit = false;
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        if (GUILayout.Button("清空对象池配置缓存"))
        {
            MemoryManager.FreeMemory();
        }
        if (GUILayout.Button("全部卸载(" + AssetsUnloadHandler.noUsedAssetsList.Count + ")"))
        {
            AssetsUnloadHandler.UnloadAll();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(4);
        EditorDrawGUIUtil.DrawScrollView(this, () =>
        {
            List<AssetsData> assetList = new List<AssetsData>(assetsCaches.Values);
            assetList.Sort((t0, t1) =>
            {
                if (t0.GetTotalMemorySize() > t1.GetTotalMemorySize())
                    return -1;
                else if (t0.GetTotalMemorySize() < t1.GetTotalMemorySize())
                    return 1;

                return 0;
            });
            for (int i = 0; i < assetList.Count; i++)
            {
                AssetsData item = assetList[i];
                if (!string.IsNullOrEmpty(bundleSearchValue))
                {
                    if (!item.assetName.Contains(bundleSearchValue.ToLower()))
                        continue;
                }
                if (onlyShowNoRef && item.refCount > 0)
                {
                    continue;
                }
                DrawAssetsData(i, item);
            }
        }, "box");
    }
    private void DrawAssetsData(int index, AssetsData assetsData)
    {

        EditorDrawGUIUtil.DrawFoldout(assetsData, index + ".(" + assetsData.refCount + ")" + assetsData.assetName + "(" + GetSizeString(assetsData.GetObjectsMemorySize()) + "+" + GetSizeString(assetsData.GetBundleMemorySize()) + ")", () =>
                {
                    List<UnityEngine.Object> objList = new List<UnityEngine.Object>(assetsData.Assets);
                    objList.Sort((t0, t1) =>
                    {
                        long s0 = Profiler.GetRuntimeMemorySizeLong(t0);
                        long s1 = Profiler.GetRuntimeMemorySizeLong(t1);

                        if (s0 > s1)
                            return -1;
                        else if (s0 < s1)
                            return 1;
                        return 0;
                    });
                    foreach (var item in objList)
                    {
                        string sizeStr = GetSizeString(Profiler.GetRuntimeMemorySizeLong(item));
                        EditorDrawGUIUtil.DrawBaseValue("(" + sizeStr + ")" + item.name, item);
                    }
                });
    }
    private string GetSizeString(long size)
    {
        string sizeStr = ""; ;
        float mSize = size / 1024f;

        if (mSize > 1024f)
        {
            mSize = mSize / 1024f;
            sizeStr = mSize.ToString("F") + "MB";
        }
        else
        {
            sizeStr = mSize.ToString("F") + "KB";
        }
        return sizeStr;
    }

}