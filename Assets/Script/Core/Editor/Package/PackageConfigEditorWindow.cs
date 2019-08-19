using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
 
public class BundleConfigEditorWindow : EditorWindow
{
    public const string c_configFileName = "BundleConfigEditor";
    public const string c_relyAssetsBundlePath = "RelyBundle"; //所有依赖包放在此目录下
    public const string c_keyRelyPackages = "relyBundles";
    public const string c_keyBundles = "AssetsBundles";

    public const string c_ResourceParentPath = "/Resources/";

    //所有依赖包
    List<EditPackageConfig> relyPackages = new List<EditPackageConfig>();
    //所有普通包
    List<EditPackageConfig> bundles = new List<EditPackageConfig>();

    /// <summary>
    /// 不打Bundle的文件放在此处
    /// </summary>
    List<string> m_NoPackagekFile = new List<string>();

    //所有普通包的层级信息
    PathPoint allBundlesLayerInfo;

    #region 初始化

    
    void OnEnable()
    {
        EditorGUIStyleData.Init();

        m_NoPackagekFile.Clear();
#if UNITY_WEBGL
        m_NoPackagekFile.Add(HotUpdateManager.c_versionFileName);
        m_NoPackagekFile.Add(ResourcesConfigManager.c_ManifestFileName);
#endif

        LoadAndAnalysisJson();
        UpdateRelyPackageNames();
        ArrangeBundlesByLayer();
    }

    #endregion

    #region GUI

    int RelyMaskFilter = -1; //依赖包过滤器
    string bundleQuery = ""; //查询内容

    bool isFoldRelyPackages = true; //是否展开依赖包
    bool isFoldBundles = true; //是否展开普通包

    bool isFoldLog = false; //是否展开日志

    Vector2 scrollPos = new Vector2();
    Vector2 LogsScrollPos = new Vector2();

    string[] RelyPackageNames = new string[1];

    bool isProgress = false;
    float progress = 0;
    string progressContent = "";

    bool isContent = false;
    string messageContent = "";

    int ButtonWidth = EditorGUIStyleData.s_ButtonWidth_large;

    void OnGUI()
    {
        titleContent.text = "打包设置编辑器";

        UpdateRelyPackageNames();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("过滤器：");

        RelyMaskFilter = EditorGUILayout.MaskField(RelyMaskFilter, RelyPackageNames);
        bundleQuery = EditorGUILayout.TextField("", bundleQuery);
        EditorGUILayout.EndHorizontal();

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(false));

        isFoldRelyPackages = EditorGUILayout.Foldout(isFoldRelyPackages, "依赖包:");
        if (isFoldRelyPackages)
        {
            //依赖包视图
            RelyPackagesView();

            EditorGUILayout.Space();
        }

        EditorGUI.indentLevel = 0;
        isFoldBundles = EditorGUILayout.Foldout(isFoldBundles, "AssetsBundle:");
        if (isFoldBundles)
        {
            //bundle包视图
            BundlesView();
        }
        GUILayout.EndScrollView();

        LogsScrollPos = GUILayout.BeginScrollView(LogsScrollPos,GUILayout.ExpandHeight(true));

        EditorGUI.indentLevel = 0;
        isFoldLog = EditorGUILayout.Foldout(isFoldLog, "提示信息:");
        if (isFoldLog)
        {
            //提示信息视图
            LogView();
        }

        GUILayout.EndScrollView();

        EditorGUI.indentLevel = 0;
        GUILayout.BeginHorizontal();

        checkMaterial = EditorGUILayout.Toggle("检查材质球和贴图", checkMaterial);

        GUILayout.EndHorizontal();

        HotUpdateConfigGUI();

        if (GUILayout.Button("检查依赖关系"))
        {
            CheckPackage();
        }

        if (GUILayout.Button("生成并保存编辑器文件"))
        {
            CreatePackageFile(); //保存编辑器文件
            CheckAndCreatBundelPackageConfig(); //生成资源路径文件
        }

        if (GUILayout.Button("重新生成资源路径文件"))
        {
            ResourcesConfigManager.CreateResourcesConfig();
        }
        if(GUILayout.Button("Create Bundle Names"))
        {
            //自动设置打包信息
            SetAssetsInfo();
        }
        if (GUILayout.Button("5.0 打包"))
        {
            NewPackage();
        }

        GUILayout.BeginHorizontal();

        VersionService.LargeVersion = EditorGUILayout.IntField("large", VersionService.LargeVersion);
        VersionService.SmallVersion = EditorGUILayout.IntField("small", VersionService.SmallVersion);

        if (GUILayout.Button("保存版本文件"))
        {
            VersionService.CreateVersionFile();
        }

        GUILayout.EndHorizontal();

        if (isContent)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(messageContent);

            if (GUILayout.Button("关闭"))
            {
                isContent = false;
                messageContent = "";
            }

            if (errorCount != 0 || warnCount != 0)
            {
                if (GUILayout.Button("清除"))
                {
                    isContent = false;
                    messageContent = "";

                    ClearCheckLog();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (isProgress)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.ProgressBar(new Rect(3, position.height - 22, position.width - 6, 18), progress, progressContent);
        }
    }

    /// <summary>
    /// 依赖包视图
    /// </summary>
    void RelyPackagesView()
    {
        for (int i = 0; i < relyPackages.Count; i++)
        {
            relyPackages[i].relyPackagesMask = 1 << i;
            if (!GetIsShowByRelyMask(relyPackages[i]))
            {
                continue;
            }

            //标签头
            EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            relyPackages[i].isFold = EditorGUILayout.Foldout(relyPackages[i].isFold, relyPackages[i].name);

            //删除按钮
            if (GUILayout.Button("删除"))
            {
                relyPackages.RemoveAt(i);
                continue;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = 3;
            if (relyPackages[i].isFold)
            {
                //名称
                EditorGUI.indentLevel = 4;
                relyPackages[i].name = EditorGUILayout.TextField("name:", relyPackages[i].name);

                //加载路径
                relyPackages[i].path = c_relyAssetsBundlePath + "/" + relyPackages[i].name;
                EditorGUILayout.LabelField("Path: ", relyPackages[i].path);

                relyPackages[i].isCollectDependencies = EditorGUILayout.Toggle("收集依赖", relyPackages[i].isCollectDependencies);

                //子资源视图
                relyPackages[i].isFold_objects = EditorGUILayout.Foldout(relyPackages[i].isFold_objects, "Objects");
                EditorGUI.indentLevel = 5;
                if (relyPackages[i].isFold_objects)
                {
                    ObjectListView(relyPackages[i]);
                }
            }
            EditorGUI.indentLevel = 2;
            //消息视图
            MessageView(relyPackages[i]);
        }

        EditorGUILayout.Space();
        EditorGUI.indentLevel = 1;
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Button:");

        if (GUILayout.Button("增加一个依赖包"))
        {
            EditPackageConfig EditPackageConfigTmp = new EditPackageConfig();
            EditPackageConfigTmp.name = "NewRelyAssetsBundle" + relyPackages.Count;

            relyPackages.Add(EditPackageConfigTmp);
        }
        //EditorGUILayout.EndHorizontal();
    }

    void ObjectListView(EditPackageConfig pack)
    {
        EditorGUILayout.LabelField("Size: " + pack.objects.Count);
        for (int j = 0; j < pack.objects.Count; j++)
        {

            EditorGUILayout.LabelField("Path:", pack.objects[j].path);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(pack.objects[j].obj, typeof(Object), false);

            if (GUILayout.Button("删除", GUILayout.Width(ButtonWidth)))
            {
                pack.objects.RemoveAt(j);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Button:");
        if (GUILayout.Button("增加选中资源"))
        {
            Object[] selects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            for (int k = 0; k < selects.Length; k++)
            {
                EditorObject tmp = new EditorObject();

                tmp.obj = selects[k];
                tmp.path = GetObjectPath(selects[k]);

                if (!isExist_EditorList(pack.objects, tmp))
                {
                    pack.objects.Add(tmp);
                }
                else
                {
                    Debug.Log(CustomToString(selects[k]) + " has Exists");
                }
            }
        }

        if (GUILayout.Button("打印所有依赖资源"))
        {
            for (int i = 0; i < pack.objects.Count; i++)
            {
                Object[] objs = GetCorrelationResource(pack.objects[i].obj);
                for (int j = 0; j < objs.Length; j++)
                {
                    Debug.Log(pack.objects[i].obj + " -> " + objs[j]);
                }
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    //消息视图
    void MessageView(EditPackageConfig package)
    {
        for (int i = 0; i < package.errorMsg.Count; i++)
        {
            EditorGUILayout.LabelField("ERROR: " + package.errorMsg[i], EditorGUIStyleData.ErrorMessageLabel);
        }

        for (int i = 0; i < package.warnMsg.Count; i++)
        {
            EditorGUILayout.LabelField("WARN: " + package.warnMsg[i], EditorGUIStyleData.WarnMessageLabel);
        }
    }

    //Bundle包视图
    void BundlesView()
    {
        if (RelyMaskFilter == -1 && bundleQuery == "")
        {
            showBundlesByLayer(bundles);
        }
        else
        {
            ShowBundleBuMaskFilter();
        }

        EditorGUILayout.Space();

        //按钮
        EditorGUI.indentLevel = 1;
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField("Button:");

        if (GUILayout.Button("自动添加 Resource 目录下的资源"))
        {
            AddAllResourceBundle();
            ArrangeBundlesByLayer();
        }

        if (GUILayout.Button("自动添加 Resource 目录下的资源并保存"))
        {
            AddResEndSave();
        }

        if (GUILayout.Button("增加选中资源"))
        {
            //获取选中的资源
            Object[] selects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            if (selects.Length > 0)
            {
                for (int i = 0; i < selects.Length; i++)
                {
                    AddAssetBundle(selects[i], "" + selects[i].name);
                }
            }
            else
            {
                Debug.Log("未选中任何资源！");
            }
            ArrangeBundlesByLayer();
        }

        if (GUILayout.Button("删除全部资源路径数据"))
        {
            if (EditorUtility.DisplayDialog("警告", "确定删除所有的资源路径数据？", "是", "否"))
            {
                bundles.Clear();
                ArrangeBundlesByLayer();
            }
        }
        //EditorGUILayout.EndHorizontal();
    }

    public void AddResEndSave()
    {
        bundles.Clear();
        ArrangeBundlesByLayer();

        AddAllResourceBundle();  //添加资源文件
        ArrangeBundlesByLayer(); //整理资源路径

        CreatePackageFile();                 //保存编辑器文件
        CheckAndCreatBundelPackageConfig(); //生成资源路径文件

        ResourcesConfigManager.CreateResourcesConfig(); //生成游戏用路径文件

        ResourcesConfigManager.ClearConfig();
    }
    void LogView()
    {
        EditorGUI.indentLevel = 1;
        for (int i = 0; i < relyPackages.Count; i++)
        {
            for (int j = 0; j < relyPackages[i].warnMsg.Count; j++)
            {
                EditorGUILayout.LabelField("WARN: " + relyPackages[i].warnMsg[j], EditorGUIStyleData.WarnMessageLabel);
            }

            for (int j = 0; j < relyPackages[i].errorMsg.Count; j++)
            {
                EditorGUILayout.LabelField("ERROR: " + relyPackages[i].errorMsg[j], EditorGUIStyleData.ErrorMessageLabel);
            }
        }

        for (int i = 0; i < bundles.Count; i++)
        {
            for (int j = 0; j < bundles[i].warnMsg.Count; j++)
            {
                EditorGUILayout.LabelField("WARN: " + bundles[i].warnMsg[j], EditorGUIStyleData.WarnMessageLabel);
            }

            for (int j = 0; j < bundles[i].errorMsg.Count; j++)
            {
                EditorGUILayout.LabelField("ERROR: " + bundles[i].errorMsg[j], EditorGUIStyleData.ErrorMessageLabel);
            }
        }
    }

    void ShowMessage(string msg)
    {
        isContent = true;
        messageContent = msg;
    }

    void ShowProgress(float p, string content)
    {
        isProgress = true;
        progress = p;
        progressContent = content;
    }

    void EndProgress()
    {
        isProgress = false;
        progress = 0;
        progressContent = "";
    }

    #region 显示Bundle包

    bool b_ok = true;
    //功能入口
    private void showBundlesByLayer(List<EditPackageConfig> bundles)
    {
        if (b_ok)
        {
            b_ok = false;
            ArrangeBundlesByLayer();
        }
        ShowBundlesByFolder(allBundlesLayerInfo, 1);
    }

    //整理资源路径
    private void ArrangeBundlesByLayer()
    {
        allBundlesLayerInfo = new PathPoint();
        allBundlesLayerInfo.s_nowPathPoint = "Resourse";
        allBundlesLayerInfo.lastPathPoint = null;
        allBundlesLayerInfo.nextPathPoint = new Dictionary<string, PathPoint>();
        allBundlesLayerInfo.bundles = null;

        for (int i = 0; i < bundles.Count; i++)
        {
            EditPackageConfig nowBundle = bundles[i];
            string s_bundlePath = nowBundle.path;
            string[] t_pathPoints = s_bundlePath.Split('/');
            int n_nowPoints = 0;

            PathPoint endPathPoint = allBundlesLayerInfo;
            while (n_nowPoints < t_pathPoints.Length - 1)
            {
                //如果下一个节点中没有需要的节点
                if (endPathPoint.nextPathPoint.ContainsKey(t_pathPoints[n_nowPoints]) == false)
                {
                    PathPoint nextPoint = new PathPoint();
                    nextPoint.s_nowPathPoint = t_pathPoints[n_nowPoints];
                    nextPoint.lastPathPoint = endPathPoint;
                    nextPoint.nextPathPoint = new Dictionary<string, PathPoint>();

                    endPathPoint.nextPathPoint.Add(t_pathPoints[n_nowPoints], nextPoint);

                    endPathPoint = nextPoint;
                }
                else
                {
                    endPathPoint = endPathPoint.nextPathPoint[t_pathPoints[n_nowPoints]];
                }
                n_nowPoints++;
            };

            if (endPathPoint.bundles == null)
            {
                endPathPoint.bundles = new List<EditPackageConfig>();
            }
            endPathPoint.bundles.Add(nowBundle);
        }
    }

    //显示到界面上
    private void ShowBundlesByFolder(PathPoint pathPoint, int n_level)
    {
        if (pathPoint == null)
        {
            //Debug.LogError("路径数据为空！");
            return;
        }

        if (pathPoint.s_nowPathPoint != null)
        {
            if (pathPoint.nextPathPoint != null)
            {
                foreach (var nextPathPoint in pathPoint.nextPathPoint)
                {
                    ShowSubFolderBundle(nextPathPoint.Value, (n_level));
                }
            }

            if (pathPoint.bundles != null)
            {
                for (int i = 0; i < pathPoint.bundles.Count; i++)
                {
                    EditPackageConfig bundle = pathPoint.bundles[i];

                    //bundle节点
                    EditorGUI.indentLevel = n_level;
                    EditorGUILayout.BeginHorizontal();
                    bundle.isFold = EditorGUILayout.Foldout(bundle.isFold, bundle.name);

                    //删除视图
                    if (GUILayout.Button("删除", GUILayout.Width(ButtonWidth)))
                    {
                        bundles.Remove(bundle);
                        pathPoint.bundles.Remove(bundle);
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (bundle.isFold)
                    {
                        ShowSingleBundleGUI(bundle, n_level);
                    }

                    EditorGUI.indentLevel = n_level + 1;
                    //消息视图
                    MessageView(bundle);
                }

            }
        }
    }

    /// <summary>
    /// 显示Resource子Resources的子目录和子文件
    /// </summary>
    /// <param name="pathPoint"></param>
    /// <param name="n_level"></param>
    void ShowSubFolderBundle(PathPoint pathPoint, int n_level)
    {
        if (pathPoint.s_nowPathPoint != null)
        {
            //文件夹节点
            EditorGUI.indentLevel = n_level;
            pathPoint.isFold = EditorGUILayout.Foldout(pathPoint.isFold, "<folder> " + pathPoint.s_nowPathPoint);

            if (pathPoint.isFold)
            {
                if (pathPoint.nextPathPoint != null)
                {
                    foreach (var nextPathPoint in pathPoint.nextPathPoint)
                    {
                        ShowSubFolderBundle(nextPathPoint.Value, (n_level + 1));
                    }
                }

                if (pathPoint.bundles != null)
                {
                    for (int i = 0; i < pathPoint.bundles.Count; i++)
                    {
                        EditPackageConfig bundle = pathPoint.bundles[i];

                        //bundle节点
                        EditorGUI.indentLevel = n_level + 1;
                        EditorGUILayout.BeginHorizontal();
                        bundle.isFold = EditorGUILayout.Foldout(bundle.isFold, bundle.name);
                        //删除视图
                        if (GUILayout.Button("删除", GUILayout.Width(ButtonWidth)))
                        {
                            bundles.Remove(bundle);
                            pathPoint.bundles.Remove(bundle);
                            continue;
                        }
                        EditorGUILayout.EndHorizontal();

                        if (bundle.isFold)
                        {
                            ShowSingleBundleGUI(bundle, n_level + 1);
                        }

                        EditorGUI.indentLevel = n_level + 2;
                        //消息视图
                        MessageView(bundle);

                    }

                }
            }
        }
    }

    void ShowBundleBuMaskFilter()
    {
        for (int i = 0; i < bundles.Count; i++)
        {
            if (
                !
                (GetIsShowByRelyMask(bundles[i])
                && GetIsFitsBundleQuery(bundles[i]))
                )
            {
                continue;
            }

            //折叠标签
            EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            bundles[i].isFold = EditorGUILayout.Foldout(bundles[i].isFold, bundles[i].name);

            if (GUILayout.Button("重新打包", GUILayout.Width(ButtonWidth)))
            {
                PackageService.PackageBundle(bundles[i]);
            }

            //删除视图
            if (GUILayout.Button("删除", GUILayout.Width(ButtonWidth)))
            {
                bundles.RemoveAt(i);
                ArrangeBundlesByLayer();
                continue;
            }
            EditorGUILayout.EndHorizontal();

            //内容
            EditorGUI.indentLevel = 3;
            if (bundles[i].isFold)
            {
                ShowSingleBundleGUI(bundles[i], 4);
            }
            EditorGUI.indentLevel = 2;
            //消息视图
            MessageView(bundles[i]);
        }
    }

    void ShowSingleBundleGUI(EditPackageConfig bundle, int l_foldIndex)
    {
        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel = l_foldIndex + 1;

        if (bundle.mainObject != null
            && bundle.mainObject.obj != null)
        {
            bundle.name = bundle.mainObject.obj.name;
        }
        else
        {
            bundle.name = "Null";
        }

        //主资源
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("主资源:");
        EditorGUILayout.ObjectField(bundle.mainObject.obj, typeof(Object), false);

        if (GUILayout.Button("重新打包", GUILayout.Width(ButtonWidth)))
        {
            PackageService.PackageBundle(bundle);
        }
        EditorGUILayout.EndHorizontal();

        //依赖包
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("依赖包:");
        bundle.relyPackagesMask = EditorGUILayout.MaskField(bundle.relyPackagesMask, RelyPackageNames);
        EditorGUILayout.EndHorizontal();

        //Debug.Log(" bundles[i].relyPackagesMask:  " + bundles[i].relyPackagesMask);

        //加载路径
        EditorGUILayout.LabelField("路径: ", bundle.path);
        bundle.isFold_objects = EditorGUILayout.Foldout(bundle.isFold_objects, "Objects");

        //子资源视图
        EditorGUI.indentLevel = l_foldIndex + 2;
        if (bundle.isFold_objects)
        {
            ObjectListView(bundle);
        }

        EditorGUILayout.EndVertical();
    }

    #endregion

    #endregion

    #region 工具函数

    void UpdateRelyPackageNames()
    {
        RelyPackageNames = new string[1];

        if (relyPackages.Count > 0)
            RelyPackageNames = new string[relyPackages.Count];

        for (int i = 0; i < relyPackages.Count; i++)
        {

            RelyPackageNames[i] = relyPackages[i].name;
        }
    }

    string GetObjectPath(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);
        return path;
    }

    public static string GetRelativePath(string path)
    {
        //Debug.Log(path);
        int direIndexTmp = path.LastIndexOf(c_ResourceParentPath);
        //Debug.Log(direIndexTmp);
        if (direIndexTmp != -1)
        {
            direIndexTmp += c_ResourceParentPath.Length;
            return path.Substring(direIndexTmp);
        }
        else
        {
            return path;
        }
    }

    #region 各种判断存在

    Dictionary<string, EditPackageConfig> m_BundleDictCache = new Dictionary<string, EditPackageConfig>();

    /// <summary>
    /// 判断一个资源是否已经在bundle列表中
    /// </summary>
    /// <param name="obj">资源对象</param>
    /// <returns>是否存在</returns>
    bool isExist_AllBundle(EditorObject obj)
    {
        if (obj != null && obj.obj != null && m_BundleDictCache != null && m_BundleDictCache.ContainsKey(obj.obj.name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsPackage(EditorObject obj)
    {
        bool result = true;

        for (int i = 0; i < m_NoPackagekFile.Count; i++)
        {
            
            if (obj.obj.name == m_NoPackagekFile[i])
            {
                return false;
            }
        }

        return result;
    }

    bool isExist_Bundle(EditorObject obj, EditPackageConfig package)
    {
        return isExist_EditorList(package.objects, obj);
    }

    bool isExist_EditorList(List<EditorObject> list, EditorObject obj)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null
                || list[i].obj == null)
            {
                continue;
            }

            if (EqualsEditorObject(list[i], obj))
            {
                return true;
            }
        }

        return false;
    }

    //比较两个EditorObject是否相等
    bool EqualsEditorObject(EditorObject obj_a, EditorObject obj_b)
    {
        //只比较加载路径，如果加载路径相等则认为是一个
        if (obj_a.path.Equals(obj_b.path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    /// <summary>
    /// 获取所有相关资源
    /// </summary>
    /// <param name="go">目标对象</param>
    /// <returns>所有相关资源</returns>
    Object[] GetCorrelationResource(Object go)
    {
        Object[] roots = new Object[] { go };
        return EditorUtility.CollectDependencies(roots);
    }

    List<EditPackageConfig> GetRelyPackListByMask(int mask)
    {
        List<EditPackageConfig> result = new List<EditPackageConfig>();

        for (int i = 0; i < relyPackages.Count; i++)
        {
            if ((mask & 1 << i) != 0)
            {
                result.Add(relyPackages[i]);
            }
        }

        return result;
    }

    string GetRelyPackNames(int mask)
    {
        string result = "";

        //List<string> names = new List<string>();

        List<EditPackageConfig> tmp = GetRelyPackListByMask(mask);

        for (int i = 0; i < tmp.Count; i++)
        {
            result += tmp[i].name;

            if (i != tmp.Count - 1)
            {
                result += "|";
            }
        }

        return result;
    }

    bool GetIsShowByRelyMask(EditPackageConfig package)
    {
        if (RelyMaskFilter == -1)
        {
            return true;
        }

        if (RelyMaskFilter == 0)
        {
            if (package.relyPackagesMask == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        if ((package.relyPackagesMask & RelyMaskFilter) != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool GetIsFitsBundleQuery(EditPackageConfig package)
    {
        if (bundleQuery == "")
        {
            return true;
        }

        //大小写不敏感
        string nameLower = package.name.ToLower();
        return nameLower.Contains(bundleQuery.ToLower());
    }

    //自定义TosString()方法
    string CustomToString(Object obj)
    {
        if (obj == null)
        {
            return "Null";
        }

        if ((obj is MonoScript))
        {
            return obj.name + " (MonoScript)";
        }
        if ((obj is TextAsset))
        {
            return obj.name + " (TextAsset)";
        }
        else
        {
            return obj.ToString();
        }
    }

    void ReLoadEditObject(EditorObject editObj)
    {
        if (editObj.obj == null)
        {
            editObj.obj = AssetDatabase.LoadAssetAtPath<Object>(editObj.path);
        }
    }

    public string GetExportPath(string path, string name)
    {
        return Application.dataPath + "/StreamingAssets/" + GetRelativePath(path).ToLower();
    }

    #endregion

    #region 添加菜单按钮

    [MenuItem("Window/打包设置编辑器 &9")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BundleConfigEditorWindow));
    }
    #endregion

    #region 自动添加Resource目录下的所有资源

    int direIndex = 0;

    string resourcePath;
    //自动添加Resource目录下的所有资源
    void AddAllResourceBundle()
    {
        resourcePath = Application.dataPath + c_ResourceParentPath;

        direIndex = resourcePath.LastIndexOf(c_ResourceParentPath);
        direIndex += c_ResourceParentPath.Length;

        float time = Time.realtimeSinceStartup;

        bundles.Clear();
        m_BundleDictCache.Clear();

        RecursionDirectory(resourcePath);

        ShowMessage("添加完成! 用时" + (Time.realtimeSinceStartup - time) + "s 详情请看输出日志。");
    }

    //递归所有目录
    void RecursionDirectory(string path)
    {
        if(!File.Exists(path))
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
            string relativePath = files[i].Substring(direIndex);
            if (relativePath.EndsWith(".meta"))
                continue;
            //if (relativePath.EndsWith(".prefab")
            //    || relativePath.EndsWith(".png")
            //    || relativePath.EndsWith(".jpg")
            //    || relativePath.EndsWith(".mp3")
            //    || relativePath.EndsWith(".wav")
            //    || relativePath.EndsWith(".txt")
            //    || relativePath.EndsWith(".proto")
            //    || relativePath.EndsWith(".json")
            //    || relativePath.EndsWith(".xml")
            //    || relativePath.EndsWith(".csv")
            //    || relativePath.EndsWith(".tga")
            //    || relativePath.EndsWith(".shader")
            //    || relativePath.EndsWith(".mat")
            //    )
            else
            {
                relativePath = FileTool.RemoveExpandName(relativePath);
                Object tmp = Resources.Load(relativePath);
                AddAssetBundle(tmp, relativePath);
            }
        }
    }

    void AddAssetBundle(Object obj, string path)
    {
        if (obj == null) return;//都是空了也就是没有意义的
        EditorObject objTmp = new EditorObject();
        objTmp.obj = obj;
        objTmp.path = GetObjectPath(obj);

        if (isExist_AllBundle(objTmp))
        {
            Debug.LogWarning(obj.name + " 已经存在！");
        }
        else if(!IsPackage(objTmp))
        {
            //Debug.LogWarning(obj.name + " 资源不打包！");
        }
        else
        {
            EditPackageConfig EditPackageConfigTmp = new EditPackageConfig();

            if (obj == null)
            {
                Debug.LogError("AddAssetBundle ERROR : path: " + path);
                return;
            }
            EditPackageConfigTmp.name = obj.name;

            EditorObject mainObjTmp = new EditorObject();
            mainObjTmp.obj = obj;
            mainObjTmp.path = GetObjectPath(obj);

            EditPackageConfigTmp.mainObject = mainObjTmp;
            EditPackageConfigTmp.path = GetRelativePath(FileTool.RemoveExpandName(GetObjectPath(obj)));

            Object[] res = GetCorrelationResource(obj);

            //判断依赖包中含不含有该资源，如果有，则不将此资源放入bundle中
            //依赖包判断
            for (int j = 0; j < res.Length; j++)
            {
                if (res[j] == null)
                {
                    //Debug.LogWarning(obj + "　有资源丢失！");
                    continue;
                }

                ////过滤掉一些不必要加载进去的组件
                //if (ComponentFilter(res[j]))
                //{
                //    continue;
                //}

                EditorObject tmp = new EditorObject();
                tmp.obj = res[j];
                tmp.path = GetObjectPath(res[j]);

                //bool isExistRelyPackage = false;

                for (int i = 0; i < relyPackages.Count; i++)
                {
                    if (isExist_Bundle(tmp, relyPackages[i]))
                    {
                        //Debug.Log("添加依赖包 : " + i);
                        //在依赖包选项中添加此依赖包
                        EditPackageConfigTmp.relyPackagesMask = EditPackageConfigTmp.relyPackagesMask | 1 << i;
                        //isExistRelyPackage = true;
                        break;
                    }
                }
            }

            bundles.Add(EditPackageConfigTmp);

            m_BundleDictCache.Add(EditPackageConfigTmp.name, EditPackageConfigTmp);
        }
    }

    //Dictionary<string, Shader> shaderFilter = new Dictionary<string, Shader>();
    //组件过滤器
    bool ComponentFilter(Object comp)
    {
        //过滤掉unity自带对象
        string path = GetObjectPath(comp);
        if (path.IndexOf("Assets") != 0)
        {
            return true;
        }

        ////过滤掉所有shander
        //if (comp as Shader != null)
        //{
        //    if (!shaderFilter.ContainsKey(comp.ToString()))
        //    {
        //        shaderFilter.Add(comp.ToString(), (Shader)comp);
        //        Debug.LogWarning("包含 Shader! :" + comp.ToString());
        //    }

        //    return true;
        //}

        if (comp is MonoScript)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region  依赖包检查相关

    Dictionary<string, List<EditorObject>> checkDict = new Dictionary<string, List<EditorObject>>();
    Dictionary<string, int> bundleName = new Dictionary<string, int>();
    int errorCount = 0;
    int warnCount = 0;

    bool checkMaterial = false;  //是否检查材质球

    /// <summary>
    /// 依赖包检查
    /// </summary>
    void CheckPackage()
    {
        ClearCheckLog();
        checkDict = new Dictionary<string, List<EditorObject>>();
        bundleName = new Dictionary<string, int>();

        //包名重复检查
        //资源重复检查
        for (int i = 0; i < relyPackages.Count; i++)
        {
            CheckRelyRepeatRes(relyPackages[i]);
            CheckRelyPackagesEmptyRes(relyPackages[i]);
        }

        for (int i = 0; i < bundles.Count; i++)
        {
            CheckBundle(bundles[i]);
        }

        //资源丢失检查
        for (int i = 0; i < bundles.Count; i++)
        {
            CheckMissRes(bundles[i]);
        }

        ShowMessage("检查完毕!  错误：" + errorCount + " 警告： " + warnCount);
    }

    /// <summary>
    /// 检查某个资源包内有没有重复资源
    /// </summary>
    /// <param name="pack"></param>

    void CheckBundle(EditPackageConfig pack)
    {
        if (bundleName.ContainsKey(pack.name))
        {
            Debug.LogError(pack.name + "包名重复! ");
            pack.errorMsg.Add("包名重复! ");
            errorCount++;
        }
        else
        {
            bundleName.Add(pack.name, 0);
        }

        if (pack.mainObject != null)
        {
            string resNameTmp = CustomToString(pack.mainObject.obj);

            if (checkDict.ContainsKey(resNameTmp))
            {
                //判断该资源是否在它的依赖包里，如果不在，加入判重
                if (!GetResIsUseByRelyBundle(pack, pack.mainObject)
                    && !ComponentFilter(pack.mainObject.obj))
                {
                    if (isExist_EditorList(checkDict[resNameTmp], pack.mainObject))
                    {
                        pack.warnMsg.Add("MainObject 重复! " + resNameTmp);
                        warnCount++;
                    }
                    else
                    {
                        checkDict[resNameTmp].Add(pack.mainObject);
                    }
                }
            }
            else
            {
                checkDict.Add(resNameTmp, new List<EditorObject>());
                checkDict[resNameTmp].Add(pack.mainObject);
            }
        }

        Object[] res = GetCorrelationResource(pack.mainObject.obj);

        for (int i = 0; i < res.Length; i++)
        {
            string resNameTmp = CustomToString(res[i]);
            EditorObject tmp = new EditorObject();
            tmp.obj = res[i];
            tmp.path = GetObjectPath(res[i]);

            if(ComponentFilter(tmp.obj))
            {
                continue;
            }

            if (checkDict.ContainsKey(resNameTmp))
            {
                //判断该资源是否在它的依赖包里，如果不在，加入判重
                if (!EqualsEditorObject(pack.mainObject, tmp)
                    && !GetResIsUseByRelyBundle(pack, tmp)
                    && !ComponentFilter(res[i]))
                {
                    if (isExist_EditorList(checkDict[resNameTmp], tmp))
                    {
                        pack.warnMsg.Add(pack.path +  " 存在重复资源 ! " + resNameTmp);
                        warnCount++;
                    }
                    else
                    {
                        checkDict[resNameTmp].Add(tmp);
                    }
                }
            }
            else
            {
                checkDict.Add(resNameTmp, new List<EditorObject>());
                checkDict[resNameTmp].Add(tmp);
            }
        }
    }

    /// <summary>
    /// 检查依赖包中有没有重复资源
    /// </summary>
    /// <param name="pack"></param>
    void CheckRelyRepeatRes(EditPackageConfig pack)
    {
        for (int i = 0; i < pack.objects.Count; i++)
        {
            string resNameTmp = CustomToString(pack.objects[i].obj);
            EditorObject tmp = pack.objects[i];

            if (checkDict.ContainsKey(resNameTmp))
            {
                if (isExist_EditorList(checkDict[resNameTmp], tmp))
                {
                    pack.warnMsg.Add("Objects存在重复资源 ! " + resNameTmp);
                    warnCount++;
                }
                else
                {
                    checkDict[resNameTmp].Add(tmp);
                }
            }
            else
            {
                checkDict.Add(resNameTmp, new List<EditorObject>());
                checkDict[resNameTmp].Add(tmp);
            }
        }
    }

    /// <summary>
    /// 检查单个包有没有丢失资源
    /// </summary>
    /// <param name="pack"></param>
    void CheckMissRes(EditPackageConfig pack)
    {
        for (int i = 0; i < pack.objects.Count; i++)
        {
            if (pack.objects[i].obj == null)
            {
                Debug.LogError(pack.name + " " + i + "号资源丢失！");
                pack.errorMsg.Add(i + "号资源丢失！");
                errorCount++;
            }
        }

        //将来加入资源缺失检测
        if (pack.mainObject == null)
        {
            Debug.LogError(pack.name + "没有主资源！");
            pack.errorMsg.Add("没有主资源！");
            errorCount++;
            return;
        }

        Object[] res = GetCorrelationResource(pack.mainObject.obj);

        for (int i = 0; i < res.Length; i++)
        {
            if (res[i] == null)
            {
                pack.warnMsg.Add(pack.mainObject.path + " 有丢失的脚本！");
                warnCount++;
                continue;
            }

            //查找其他资源是否有掉材质球问题
            List<string> resLostList = FindLostRes(res[i]);
            for (int j = 0; j < resLostList.Count; j++)
            {
                pack.warnMsg.Add(resLostList[j]);
                warnCount++;
            }

            //EditorObject tmp = new EditorObject();
            //tmp.obj = res[i];
            //tmp.path = GetObjectPath(res[i]);

            //if (!GetResIsUse(pack, tmp) && !ComponentFilter(res[i]))
            //{
            //    pack.errorMsg.Add( CustomToString(res[i]) + " 资源丢失依赖！");
            //    errorCount++;
            //}
        }
    }

    /// <summary>
    /// 检查依赖包是否是无资源
    /// </summary>
    void CheckRelyPackagesEmptyRes(EditPackageConfig pack)
    {
        if (pack.objects.Count == 0)
        {
            Debug.LogError(pack.name + " 依赖包无资源 ！");
            pack.errorMsg.Add(pack.name + " 依赖包无资源 ！");
            errorCount++;
        }
    }

    //检查丢失的其他资源
    List<string> FindLostRes(Object obj)
    {
        List<string> result = new List<string>();

        GameObject go = obj as GameObject;

        if (go)
        {
            HandleObject(go, result);
            RecursionChilds(go.transform, result);
        }

        return result;
    }

    //递归所有子节点
    void RecursionChilds(Transform treeSource, List<string> list)
    {
        if (treeSource.childCount > 0)
        {
            int i;
            for (i = 0; i < treeSource.childCount; i++)
            {
                HandleObject(treeSource.GetChild(i).gameObject, list);
                RecursionChilds(treeSource.GetChild(i), list);
            }
        }
    }

    void HandleObject(GameObject go, List<string> list)
    {
        if (checkMaterial)
        {
            //检查是否掉材质球
            Renderer render = go.GetComponent<Renderer>();

            if (render != null)
            {
                if (render.sharedMaterials != null)
                {
                    for (int i = 0; i < render.sharedMaterials.Length; i++)
                    {
                        if (render.sharedMaterials[i] != null)
                        {
                            try
                            {
                                if (render.sharedMaterials[i].GetTexture(0) == null)
                                {
                                    list.Add("贴图丢失: name: " + go.name + " (Materials Index: " + i + ")");
                                }
                            }
                            catch (System.Exception e)
                            {
                                e.ToString();
                                list.Add("贴图丢失: name: " + go.name + " (Materials Index: " + i + ")");
                            }
                        }
                        else
                        {
                            list.Add("材质球丢失 : name: " + go.name + " (Materials Index: " + i + ")");
                        }
                    }
                }
                else
                {
                    list.Add("材质球丢失 : name:" + go.name);
                }
            }
        }

        //这里可以添加其他检查
    }

    /// <summary>
    /// 清除所有差错日志
    /// </summary>
    void ClearCheckLog()
    {
        for (int i = 0; i < bundles.Count; i++)
        {
            bundles[i].errorMsg.Clear();
            bundles[i].warnMsg.Clear();
        }

        for (int i = 0; i < relyPackages.Count; i++)
        {
            relyPackages[i].errorMsg.Clear();
            relyPackages[i].warnMsg.Clear();
        }

        errorCount = 0;
        warnCount = 0;
    }

    /// <summary>
    /// 检查单个资源是否全都被其依赖包引用
    /// </summary>
    bool GetResIsUseByRelyBundle(EditPackageConfig pack, EditorObject res)
    {
        //根据mask获取所有依赖包
        List<EditPackageConfig> relysBundles = GetRelyPackListByMask(pack.relyPackagesMask);

        for (int i = 0; i < relysBundles.Count; i++)
        {
            if (isExist_Bundle(res, relysBundles[i]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 判断某个资源是否
    /// </summary>
    /// <param name="pack"></param>
    /// <param name="res"></param>
    /// <returns></returns>
    bool GetResIsUseByMainObject(EditPackageConfig pack, EditorObject res)
    {
        if (EqualsEditorObject(pack.mainObject, res))
        {
            return true;
        }

        return false;
    }

    #endregion

    #region 生成配置文件与解析配置文件
    void CreatePackageFile()
    {
        PackageEditorConfigService.RelyPackages = relyPackages;
        PackageEditorConfigService.Bundles = bundles;

        PackageEditorConfigService.SavePackageEditorConfig();
    }

    /// <summary>
    /// 读取并且解析Json
    /// </summary>
    void LoadAndAnalysisJson()
    {
        relyPackages = PackageEditorConfigService.RelyPackages;
        bundles = PackageEditorConfigService.Bundles;
    }
    #endregion

    #region 打包

    void ProessCallback(float progress,string content)
    {
        ShowProgress(progress, content);

        if(progress == 1)
        {
            AssetDatabase.Refresh();

            CreatBundelPackageConfig();
            Repaint();

            EndProgress();
        }
    }

    void NewPackage()
    {
        //自动保存设置文件
        CreatePackageFile();

        //生成资源路径文件
        ResourcesConfigManager.CreateResourcesConfig();

        //自动增加小版本号
        VersionService.SmallVersion++;
        VersionService.CreateVersionFile();

        //清除旧打包信息
        ClearAssetBundlesName();
        //自动设置打包信息
        SetAssetsInfo();

        //删除streaming下所有旧资源
        if (Directory.Exists(Application.dataPath + "/StreamingAssets"))
        {
            FileTool.DeleteDirectory(Application.dataPath + "/StreamingAssets");
        }
        else
        {
            FileTool.CreatPath(Application.dataPath + "/StreamingAssets");
        }

        BuildPipeline.BuildAssetBundles(Application.dataPath + "/StreamingAssets/", BuildAssetBundleOptions.None, PackageService.GetTargetPlatform);

        //删除所有manifest文件
        DeleteManifestFile(Application.dataPath + "/StreamingAssets/");

        //清除旧打包信息
        ClearAssetBundlesName();
    }

    void DeleteManifestFile(string path)
    {
        string[] dires = Directory.GetDirectories(path);

        for (int i = 0; i < dires.Length; i++)
        {
            DeleteManifestFile(dires[i]);
        }

        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".manifest") 
                || files[i].EndsWith(".meta"))
            {
                File.Delete(files[i]);
            }
        }
    }

    void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("清除前 bundle数目 " + length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log("清除后 bundle数目 " + length);
    }

    void SetAssetsInfo()
    {
        for (int i = 0; i < relyPackages.Count; i++)
        {
            for (int j = 0; j < relyPackages[i].objects.Count; j++)
            {
                AssetImporter assetImporter = AssetImporter.GetAtPath(relyPackages[i].objects[j].path);
                if(assetImporter != null)
                {
                    assetImporter.assetBundleName = c_relyAssetsBundlePath + "/" + relyPackages[i].name;
                }
                else
                {
                    Debug.LogError("SetAssetsInfo relyPackages error :->" + relyPackages[i].objects[j].path);
                }
            }
        }

        for (int i = 0; i < bundles.Count; i++)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(bundles[i].mainObject.path);
            if(assetImporter != null)
            {
                assetImporter.assetBundleName = bundles[i].path;
            }
            else
            {
                Debug.LogError("SetAssetsInfo bundles error :->" + bundles[i].mainObject.path);
            }
        }
    }

    void CheckAndCreatBundelPackageConfig()
    {
        CheckPackage();

        if (errorCount == 0)
        {
            CreatBundelPackageConfig();
        }
        else
        {
            if (EditorUtility.DisplayDialog("失败", "打包设置有错误，请先修复错误！", "好的", "仍要继续") == false)
            {
                CreatBundelPackageConfig();
            }
        }
    }

    //生成游戏中使用的配置文件
    public void CreatBundelPackageConfig()
    {
        ResourcesConfigManager.CreateResourcesConfig();
    }

    #endregion

    #region 热更新设置初始化

    void HotUpdateConfigGUI()
    {
        if (!ConfigManager.GetIsExistConfig(HotUpdateManager.c_HotUpdateConfigName))
        {
            if (GUILayout.Button("热更新设置初始化"))
            {
                Dictionary<string, SingleField> hotUpdateConfig = new Dictionary<string, SingleField>();
                hotUpdateConfig.Add(HotUpdateManager.c_testDownLoadPathKey, new SingleField(""));
                hotUpdateConfig.Add(HotUpdateManager.c_downLoadPathKey, new SingleField(""));
                hotUpdateConfig.Add(HotUpdateManager.c_UseTestDownLoadPathKey, new SingleField(false));

                ConfigEditorWindow.SaveData(HotUpdateManager.c_HotUpdateConfigName, hotUpdateConfig);
            }
        }
    }

    #endregion
}

public class EditPackageConfig
{
    public bool isFold = false; //是否折叠
    public string name = "";
    public string path = "";    //存放路径

    public List<EditorObject> objects = new List<EditorObject>();  //所有子资源
    public bool isFold_objects = false; //子资源是否折叠

    //依赖包独有
    public bool isCollectDependencies = true; //收集依赖

    //bundle独有
    public EditorObject mainObject;     //主资源
    public int relyPackagesMask;        //依赖包mask

    public List<string> warnMsg = new List<string>();  //错误日志
    public List<string> errorMsg = new List<string>(); //警告日志
}

[System.Serializable]
public class EditorObject
{
    [System.NonSerialized]
    public Object obj;
    public string path;
}

public class PathPoint
{
    //是否折叠
    public bool isFold = false;
    //当前节点的名字
    public string s_nowPathPoint;
    //上一个节点（父节点）
    public PathPoint lastPathPoint;
    //子节点列表
    public Dictionary<string, PathPoint> nextPathPoint;
    //改路径点上面的bundle
    public List<EditPackageConfig> bundles;
}