using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
public class BundleConfigEditorWindow : EditorWindow
{
    const string configFileName = "BundleConfigEditor";

    const string relyAssetsBundlePath = "RelyBundle"; //所有依赖包放在此目录下

    const string key_relyPackages = "relyBundles";
    const string key_bundles      = "AssetsBundles";

    int largeVersion = 1;  //大版本号 
    int smallVersion = 1; //小版本号

    //所有依赖包
    List<EditPackageConfig> relyPackages = new List<EditPackageConfig>();
    //所有普通包
    List<EditPackageConfig> bundles      = new List<EditPackageConfig>();

    //所有普通包的层级信息
    PathPoint allBundlesLayerInfo;

    #region 初始化

    void OnEnable()
    {
        //Debug.Log("初始化");
        EditorGUIStyleData.Init();

        LoadAndAnalysisJson();
        AnalysisVersionFile();
        UpdateRelyPackageNames();

        ArrangeBundlesByLayer();
    }

    #endregion

    #region GUI

    int RelyMaskFilter = -1; //依赖包过滤器
    string bundleQuery = ""; //查询内容

    bool isFoldRelyPackages = true; //是否展开依赖包
    bool isFoldBundles      = true; //是否展开普通包

    Vector2 scrollPos = new Vector2();

    string[] RelyPackageNames = new string[1];

    bool isProgress        = false;
    float progress         = 0;
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
        bundleQuery    = EditorGUILayout.TextField("", bundleQuery);
        EditorGUILayout.EndHorizontal();

        scrollPos = GUILayout.BeginScrollView(scrollPos);

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

        EditorGUI.indentLevel = 0;
        GUILayout.BeginHorizontal();

        checkMaterial = EditorGUILayout.Toggle("检查材质球和贴图", checkMaterial);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("检查依赖关系"))
        {
            CheckPackage();
        }

        if (GUILayout.Button("保存编辑器设置文件"))
        {
            CreatPackageFile();
        }

        if (GUILayout.Button("生成游戏资源路径文件"))
        {
            CheckAndCreatBundelPackageConfig();
        }

        if (GUILayout.Button("打包 并生成MD5文件"))
        {
            CheckAndPackage();
        }

        if (GUILayout.Button("生成MD5"))
        {
            CheckAndCreatBundelPackageConfig();
        }

        GUILayout.BeginHorizontal();

        largeVersion = EditorGUILayout.IntField("large", largeVersion);
        smallVersion = EditorGUILayout.IntField("small", smallVersion);

        if (GUILayout.Button("保存版本文件"))
        {
            CreatVersionFile();
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

            if (errorCount != 0|| warnCount != 0)
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

        if(isProgress)
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
                relyPackages[i].path = relyAssetsBundlePath + "/" + relyPackages[i].name;
                EditorGUILayout.LabelField("Path: ", relyPackages[i].path);

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
            EditorGUILayout.ObjectField(pack.objects[j].obj,typeof(Object),false);

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
                    Debug.Log(CustomToString(selects[k])+ " has Exists");
                }
            }
        }
        //if (GUILayout.Button("资源去重"))
        //{
        //}
        EditorGUILayout.EndHorizontal();
    }

    //消息视图
    void MessageView(EditPackageConfig package)
    {
        for (int i = 0; i < package.errorMsg.Count; i++)
        {
            EditorGUILayout.LabelField("ERROR: " + package.errorMsg[i], EditorGUIStyleData.s_ErrorMessageLabel);
        }

        for (int i = 0; i < package.warnMsg.Count; i++)
        {
            EditorGUILayout.LabelField("WARN: " + package.warnMsg[i], EditorGUIStyleData.s_WarnMessageLabel);
        }
    }

    //Bundle包视图
    void BundlesView()
    {
        if(RelyMaskFilter == -1 && bundleQuery == "")
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

    void ShowMessage(string msg)
    {
        isContent = true;
        messageContent = msg;
    }

    void ShowProgress(float p,string content)
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
            Debug.LogError("路径数据为空！");
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
                ShowSingleBundleGUI(bundles[i],4);
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

        //}

    }
    #endregion

    #endregion

    #region 工具函数

    static BuildTarget getTargetPlatform
    {
        get
        {
            BuildTarget target = BuildTarget.StandaloneWindows;

#if UNITY_ANDROID //安卓
                target = BuildTarget.Android;
#elif UNITY_IOS //iPhone
                target = BuildTarget.iOS;
#endif

            return target;
        }
    }

    void UpdateRelyPackageNames()
    {
        RelyPackageNames = new string[relyPackages.Count];
        for (int i = 0; i < relyPackages.Count; i++)
        {
            RelyPackageNames[i] = relyPackages[i].name;
        }
    }

    string GetObjectPath(Object obj)
    {
         string path =  AssetDatabase.GetAssetPath(obj);

         return path;
    }

    string GetRelativePath(string path)
    {
        //Debug.Log(path);
        int direIndexTmp = path.LastIndexOf(resourceParentPath);
        //Debug.Log(direIndexTmp);
        if (direIndexTmp != -1)
        {
            direIndexTmp += resourceParentPath.Length;
            return path.Substring(direIndexTmp);
        }
        else
        {
            return path;
        }
    }

    #region 各种判断存在

    /// <summary>
    /// 判断一个资源是否已经在bundle列表中
    /// </summary>
    /// <param name="obj">资源对象</param>
    /// <returns>是否存在</returns>
    bool isExist_AllBundle(EditorObject obj)
    {
        for (int i = 0; i < bundles.Count; i++)
        {
            if (EqualsEditorObject(bundles[i].mainObject,obj))
            {
                return true;
            }

            if (isExist_Bundle(obj, bundles[i]))
            {
                return true;
            }
        }

        //for (int i = 0; i < relyPackages.Count; i++)
        //{
        //    if (isExist_Bundle(obj, relyPackages[i]))
        //    {
        //        return true;
        //    }
        //}
        return false;
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

            if (EqualsEditorObject(list[i],obj))
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

        for (int i = 0; i < relyPackages.Count;i++ )
        {
            if ((mask & 1<<i) != 0)
            {
                result.Add(relyPackages[i]);
            }
        }


        return result;
    }

    string[] GetRelyPackNames(int mask)
    {
        List<string> names = new List<string>();

        List<EditPackageConfig> tmp = GetRelyPackListByMask(mask);

        for (int i = 0; i < tmp.Count; i++)
        {
            names.Add(tmp[i].name);
        }

        return names.ToArray();
    }

    bool GetIsShowByRelyMask(EditPackageConfig package)
    {
        if (RelyMaskFilter == -1)
        {
            return true;
        }

        if (RelyMaskFilter == 0)
        {
            if(package.relyPackagesMask == 0)
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
        if(bundleQuery == "")
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

        if ((obj is MonoScript) )
        {
            return obj.name + " (MonoScript)";
        }
        if ((obj is TextAsset) )
        {
            return obj.name + " (TextAsset)";
        }
        else
        {
            return obj.ToString();
        }
    }

    //重新加载Object
    void ReLoadGameObject(EditPackageConfig pack)
    {
        if (pack.mainObject != null)
        {
            ReLoadEditObject(pack.mainObject);
        }

        for (int i = 0; i < pack.objects.Count; i++)
        {
            ReLoadEditObject(pack.objects[i]);
        }
    }

    void ReLoadEditObject(EditorObject editObj)
    {
        editObj.obj = AssetDatabase.LoadAssetAtPath<Object>(editObj.path);
    }

    string GetExportPath(string path, string name)
    {
        return Application.dataPath + "/StreamingAssets/" + GetRelativePath(FileTool.RemoveExpandName(path)) + "." + AssetsBundleManager.c_AssetsBundlesExpandName;
    }

    #endregion

    #region 添加菜单按钮

    [MenuItem("Tool/显示选中对象所有相关资源")]
    public static void ShowAllCorrelationResource()
    {
        Object[] roots = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);
        Selection.objects = EditorUtility.CollectDependencies(roots);
    }

    [MenuItem("Window/打包设置编辑器")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BundleConfigEditorWindow));
    }
    #endregion

    #region 自动添加Resource目录下的所有资源

    int direIndex = 0;
    string resourceParentPath = "/Resources/";
    string resourcePath;
    //自动添加Resource目录下的所有资源
    void AddAllResourceBundle()
    {
        resourcePath = Application.dataPath +resourceParentPath;

        direIndex = resourcePath.LastIndexOf(resourceParentPath);
        direIndex += resourceParentPath.Length;

        RecursionDirectory(resourcePath);

        ShowMessage("添加完成! 详情请看输出日志。");
    }

    //递归所有目录
    void RecursionDirectory(string path)
    {
        string[] dires =  Directory.GetDirectories(path);

        for (int i = 0; i < dires.Length;i++ )
        {
            ////配置不打包
            //if (!dires[i].Equals(resourcePath + ConfigManager.c_directoryName))
            //{
                RecursionDirectory(dires[i]);
            //}
        }

        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            string relativePath = files[i].Substring(direIndex);

            if (relativePath.EndsWith(".prefab")
                || relativePath.EndsWith(".png")
                || relativePath.EndsWith(".jpg")
                || relativePath.EndsWith(".mp3")
                || relativePath.EndsWith(".wav")
                || relativePath.EndsWith(".txt")
                || relativePath.EndsWith(".json")
                || relativePath.EndsWith(".xml")
                || relativePath.EndsWith(".csv")
                || relativePath.EndsWith(".tga")
                )
            {
                relativePath = FileTool.RemoveExpandName(relativePath);
                Object tmp = Resources.Load(relativePath);
                AddAssetBundle(tmp,relativePath);
            }
        }
    }

    void AddAssetBundle(Object obj, string path)
    {
        EditorObject objTmp = new EditorObject();
        objTmp.obj = obj;
        objTmp.path = GetObjectPath(obj);

        if (isExist_AllBundle(objTmp))
        {
            //Debug.Log(obj.name + " 已经存在！");
        }
        else
        {
            EditPackageConfig EditPackageConfigTmp = new EditPackageConfig();
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
                    Debug.LogWarning(obj + "　有资源丢失！");
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
                        //在依赖包选项中添加此依赖包
                        EditPackageConfigTmp.relyPackagesMask = EditPackageConfigTmp.relyPackagesMask | 1 << i;
                        //isExistRelyPackage = true;
                        break;
                    }
                }

                ////该资源不在依赖包中，并且也与主资源不同时，放入包中
                //if (isExistRelyPackage == false
                //    &&!EqualsEditorObject(EditPackageConfigTmp.mainObject,tmp)
                //    )
                //{
                    
                //    EditPackageConfigTmp.objects.Add(tmp);
                //}
            }

            bundles.Add(EditPackageConfigTmp);
        }
    }

    Dictionary<string, Shader> shaderFilter = new Dictionary<string, Shader>();
    //组件过滤器
    bool ComponentFilter(Object comp)
    {
        //过滤掉unity自带对象
        string path = GetObjectPath(comp);
        if (path.IndexOf("Assets") != 0)
        {
            return true;
        }

        //过滤掉所有shander
        if (comp as Shader != null)
        {
            if (!shaderFilter.ContainsKey(comp.ToString()))
            {
                shaderFilter.Add(comp.ToString(), (Shader)comp);
                Debug.LogWarning("包含 Shader! :" + comp.ToString());
            }

            return true;
        }

        return false;
    }

    #endregion

    #region  依赖包检查相关

    Dictionary<string, List<EditorObject>> checkDict = new Dictionary<string, List<EditorObject>>();
    Dictionary<string, int> bundleName = new Dictionary<string, int>();
    int errorCount = 0;
    int warnCount  = 0;

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

        ShowMessage("检查完毕!  错误：" + errorCount + " 警告： "+ warnCount);
    }

    /// <summary>
    /// 检查某个资源包内有没有重复资源
    /// </summary>
    /// <param name="pack"></param>

    void CheckBundle(EditPackageConfig pack)
    {
        if (bundleName.ContainsKey(pack.name))
        {
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
                checkDict.Add(resNameTmp,new List<EditorObject>());
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

            if (checkDict.ContainsKey(resNameTmp))
            {
                //判断该资源是否在它的依赖包里，如果不在，加入判重
                if (!EqualsEditorObject(pack.mainObject, tmp) 
                    && !GetResIsUseByRelyBundle(pack, tmp) 
                    && !ComponentFilter(res[i])) 
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
        for (int i = 0; i < pack.objects.Count;i++ )
        {
            if (pack.objects[i].obj == null)
            {
                pack.errorMsg.Add(i + "号资源丢失！");
                errorCount++;
            }
        }

        //将来加入资源缺失检测
        if (pack.mainObject == null)
        {
            pack.errorMsg.Add("没有主资源！");
            errorCount++;
            return; 
        }

        Object[] res = GetCorrelationResource(pack.mainObject.obj);

        for (int i = 0; i < res.Length; i++)
        {
            if (res[i] == null)
            {
                pack.warnMsg.Add("有丢失的脚本！");
                warnCount++;
                continue;
            }

            //查找其他资源是否有掉材质球问题
            List<string> resLostList = FindLostRes(res[i]);
            for (int j = 0; j < resLostList.Count;j++ )
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
        if(pack.objects.Count == 0)
        {
            pack.errorMsg.Add(pack.name + " 依赖包无资源 ！");
            errorCount++;
        }
    }

    //检查丢失的其他资源
    List<string> FindLostRes( Object obj)
    {
        List<string> result = new List<string>();

        GameObject go = obj as GameObject;

        if (go)
        {
            HandleObject(go, result);
            RecursionChilds(go.transform,result);
        }

        return result;
    }

    //递归所有子节点
    void RecursionChilds(Transform treeSource,List<string> list)
    {
        if (treeSource.childCount > 0)
        {
            int i;
            for (i = 0; i < treeSource.childCount; i++)
            {
                HandleObject(treeSource.GetChild(i).gameObject,list);
                RecursionChilds(treeSource.GetChild(i),list);
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
        for (int i = 0; i < bundles.Count ; i++)
        {
            bundles[i].errorMsg.Clear();
            bundles[i].warnMsg.Clear();
        }

        for (int i = 0; i < relyPackages.Count ; i++)
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
    bool GetResIsUseByRelyBundle(EditPackageConfig pack,EditorObject res)
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
    void CreatPackageFile()
    {
        //生成编辑器配置文件
        Dictionary<string, object> editorConfig = new Dictionary<string, object>();

        editorConfig.Add(key_relyPackages, JsonTool.List2Json<EditPackageConfig>(relyPackages)); //依赖包
        editorConfig.Add(key_bundles     , JsonTool.List2Json<EditPackageConfig>(bundles));    //Bundle包

        //保存编辑器配置文件
        ConfigManager.SaveEditorConfigData(configFileName,editorConfig);
    }

    /// <summary>
    /// 读取并且解析Json
    /// </summary>
    void LoadAndAnalysisJson()
    {
        Dictionary<string, object> final = ConfigManager.GetEditorConfigData(configFileName);

        if (final == null)
        {
            Debug.Log(configFileName + " ConfigData dont Exits");
            return;
        }

        //依赖包
        relyPackages = JsonTool.Json2List<EditPackageConfig>((string)final["relyBundles"]);

        for (int i = 0; i < relyPackages.Count; i++)
        {
            //重新加载Object
            ReLoadGameObject(relyPackages[i]);
        }

        //Bundle包
        bundles = JsonTool.Json2List<EditPackageConfig>((string)final["AssetsBundles"]);

        for (int i = 0; i < bundles.Count; i++)
        {
            //重新加载Object
            ReLoadGameObject(bundles[i]);
        }

    }
    #endregion

    #region 打包

    BuildAssetBundleOptions relyBuildOption; //依赖包打包设置

    void CheckAndPackage()
    {
        CheckPackage();

        if (errorCount == 0)
        {
            EditorCoroutineRunner.StartEditorCoroutine( Package());
           
        }
        else
        {
            if(EditorUtility.DisplayDialog("失败","打包设置有错误，请先修复错误！","好的","仍要打包")== false)
            {
                EditorCoroutineRunner.StartEditorCoroutine( Package());
            }
        }
    }

#pragma warning disable
    IEnumerator Package()
    {
        //自动保存设置文件
        CreatPackageFile();

        relyBuildOption = BuildAssetBundleOptions.DeterministicAssetBundle //每次二进制一致
               | BuildAssetBundleOptions.CollectDependencies;   //收集依赖
               //| BuildAssetBundleOptions.CompleteAssets;      //完整资源
                //| BuildAssetBundleOptions.UncompressedAssetBundle //不压缩

        BuildPipeline.PushAssetDependencies();

        float sumCount = relyPackages.Count + bundles.Count;
        float currentCount = 0;

        ShowProgress(0, "删除旧资源");
        yield return 0;

        //删除streaming下所有旧资源
        FileTool.DeleteDirectory(Application.dataPath + "/StreamingAssets");

        ShowProgress(0, "开始打包");
        yield return 0;

        //先打依赖包
        for (int i = 0; i < relyPackages.Count; i++)
        {
            PackageRelyPackage(relyPackages[i]);

            currentCount++;
            ShowProgress(currentCount / sumCount, "打包依赖包 第" + i + "个 共" + relyPackages.Count+"个");

            yield return 0;
        }

        //再打普通包
        for (int i = 0; i < bundles.Count; i++)
        {
            PackageBundle(bundles[i]);

            currentCount++;
            ShowProgress(currentCount / sumCount, "打包普通包 第" + i + "个 共" + bundles.Count + "个");
            yield return 0;
        }

        EndProgress();

        BuildPipeline.PopAssetDependencies();

        AssetDatabase.Refresh();

        CreatBundelPackageConfig();
        Repaint();
    }



    void PackageRelyPackage(EditPackageConfig package)
    {
        //BuildPipeline.PushAssetDependencies();

        if (package.objects.Count == 0)
        {
            Debug.LogError(package.name +  " 没有资源！");
        }

        Object[] res = new Object[package.objects.Count];

        for (int i = 0; i < package.objects.Count;i++ )
        {
            res[i] = package.objects[i].obj;
        }

        string path = GetExportPath(package.path,package.name);

        FileTool.CreatFilePath(path);

        BuildPipeline.BuildAssetBundle(null, res, path, relyBuildOption, getTargetPlatform);

        //BuildPipeline.PopAssetDependencies();
    }

    void PackageBundle(EditPackageConfig package)
    {
        //Debug.Log("PackageBundle " + package.name);
        //导入资源包
        BuildPipeline.PushAssetDependencies();

        //打包
        Object[] res = new Object[package.objects.Count];

        for (int i = 0; i < package.objects.Count; i++)
        {
            res[i] = package.objects[i].obj;
        }

        string path = GetExportPath(package.path, package.name);

        FileTool.CreatFilePath(path);

        BuildPipeline.BuildAssetBundle(package.mainObject.obj, res, path, relyBuildOption, getTargetPlatform);

        BuildPipeline.PopAssetDependencies();
    }
    #endregion

    #region 生成游戏中使用的配置文件

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
        Dictionary<string, SingleField> gameConfig = new Dictionary<string, SingleField>();

        Dictionary<string,ResourcesConfig> gameRelyBundles = new Dictionary<string,ResourcesConfig>();
        for (int i = 0; i < relyPackages.Count; i++)
        {
            //生成游戏中使用的依赖包数据
            ResourcesConfig pack = new ResourcesConfig();
            pack.name          = relyPackages[i].name;
            pack.path          = relyPackages[i].path;
            pack.relyPackages  = new string[0];
            pack.md5           = MD5Tool.GetFileMD5(GetExportPath(pack.path, pack.name)); //获取bundle包的md5
            //pack.loadType      = ResLoadType.Streaming;  //默认放在沙盒路径下

            gameRelyBundles.Add(pack.name,pack);
        }

        Dictionary<string,ResourcesConfig> gameAssetsBundles = new Dictionary<string,ResourcesConfig>();
        for (int i = 0; i < bundles.Count; i++)
        {
            //生成游戏中使用的bundle包数据
            ResourcesConfig pack = new ResourcesConfig();
            pack.name          = bundles[i].name;
            pack.path          = bundles[i].path;
            pack.relyPackages  = GetRelyPackNames(bundles[i].relyPackagesMask); //获取依赖包的名字
            pack.md5           = MD5Tool.GetFileMD5(GetExportPath(pack.path, pack.name)); //获取bundle包的md5
            //pack.loadType      = ResLoadType.Streaming;  //默认放在沙盒路径下

            gameAssetsBundles.Add(pack.name,pack);
        }

        gameConfig.Add(ResourcesConfigManager.c_relyBundleKey  , new SingleField( JsonTool.Dictionary2Json<ResourcesConfig>(gameRelyBundles)));
        gameConfig.Add(ResourcesConfigManager.c_bundlesKey     , new SingleField( JsonTool.Dictionary2Json<ResourcesConfig>(gameAssetsBundles)));

        //保存游戏中读取的配置文件
        ConfigManager.SaveData(ResourcesConfigManager.c_configFileName, gameConfig);
        AssetDatabase.Refresh();
    }

    //生成版本文件
    public void CreatVersionFile()
    {
        Dictionary<string, SingleField> VersionData = ConfigManager.GetData(UpdateManager.versionFileName);

        if (VersionData == null)
        {
            VersionData = new Dictionary<string, SingleField>();
        }

        if (VersionData.ContainsKey(UpdateManager.key_largeVersion))
        {
            VersionData[UpdateManager.key_largeVersion] = new SingleField(largeVersion);
        }
        else
        {
            VersionData.Add(UpdateManager.key_largeVersion, new SingleField(largeVersion));
        }

        if (VersionData.ContainsKey(UpdateManager.key_smallVerson))
        {
            VersionData[UpdateManager.key_smallVerson] = new SingleField(smallVersion);
        }
        else
        {
            VersionData.Add(UpdateManager.key_smallVerson, new SingleField(smallVersion));
        }

        ConfigManager.SaveData(UpdateManager.versionFileName, VersionData);
        AssetDatabase.Refresh();
    }

    //解析版本号文件
    public void AnalysisVersionFile()
    {
        Dictionary<string, SingleField> VersionData = ConfigManager.GetData(UpdateManager.versionFileName);

        if (VersionData == null)
        {
            largeVersion = -1;
            smallVersion = -1;

            return;
        }

        if (VersionData.ContainsKey(UpdateManager.key_largeVersion))
        {
            largeVersion = VersionData[UpdateManager.key_largeVersion].GetInt();
        }
        else
        {
            largeVersion = -1;
        }

        if (VersionData.ContainsKey(UpdateManager.key_smallVerson))
        {
            smallVersion = VersionData[UpdateManager.key_smallVerson].GetInt();
        }
        else
        {
            smallVersion = -1;
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
    public bool isFold = false;
    public string s_nowPathPoint;
    public PathPoint lastPathPoint;
    public Dictionary<string, PathPoint> nextPathPoint;
    public List<EditPackageConfig> bundles;

}
