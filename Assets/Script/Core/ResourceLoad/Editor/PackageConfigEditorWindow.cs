using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class PackageConfigEditorWindow : EditorWindow
{
    [MenuItem("Window/打包设置编辑器")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PackageConfigEditorWindow));
    }

    //所有依赖包
    List<EditPackageConfig> relyPackages = new List<EditPackageConfig>();

    //所有普通包
    List<EditPackageConfig> bundles = new List<EditPackageConfig>();

    #region GUI

    int RelyMaskFilter = -1; //依赖包过滤器

    bool isFoldRelyPackages = false; //是否展开依赖包
    bool isFoldBundles = false;      //是否展开普通包

    Vector2 scrollPos = new Vector2();

    string[] RelyPackageNames = new string[1];

    GUIStyle errorMsg = new GUIStyle();
    GUIStyle warnMsg = new GUIStyle();

    GUIContent title = new GUIContent();

    bool isProgress = false;
    float progress = 0;
    string progressContent = "";

    bool isContent = false;
    string messageContent = "";

    void OnGUI()
    {
        title.text = "打包设置编辑器";
        //title.tooltip = "HaHa";
        //title.image = new Texture();

        base.titleContent = title;

        errorMsg.normal.textColor = Color.red;
        warnMsg.normal.textColor = Color.yellow;
        UpdateRelyPackageNames();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("过滤器：");

        RelyMaskFilter = EditorGUILayout.MaskField(RelyMaskFilter, RelyPackageNames);

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

        if (GUILayout.Button("检查依赖关系"))
        {
            CheckPackage();
        }

        if (GUILayout.Button("保存设置文件"))
        {
            CreatPackageFile();
        }

        if (GUILayout.Button("打包"))
        {
            Package();
        }

        if (isContent)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(messageContent);

            if(GUILayout.Button("清除"))
            {
                isContent = false;
                messageContent = "";
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
                relyPackages[i].path = "RelyAssetsBundlePath/" + relyPackages[i].name;
                EditorGUILayout.LabelField("Path: ", relyPackages[i].path);

                //子资源视图
                relyPackages[i].isFold_objects = EditorGUILayout.Foldout(relyPackages[i].isFold_objects, "Objects");
                EditorGUI.indentLevel = 5;
                if (relyPackages[i].isFold_objects)
                {
                    ObjectListView(relyPackages[i].objects);
                }
            }

            //消息视图
            MessageView(relyPackages[i]);
        }

        EditorGUILayout.Space();
        EditorGUI.indentLevel = 1;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Button:");

        if (GUILayout.Button("增加一个依赖包"))
        {
            EditPackageConfig EditPackageConfigTmp = new EditPackageConfig();
            EditPackageConfigTmp.name = "NewRelyAssetsBundle" + relyPackages.Count;

            relyPackages.Add(EditPackageConfigTmp);
        }
        EditorGUILayout.EndHorizontal();
    }

    void ObjectListView(List<Object> objects)
    {
        EditorGUILayout.LabelField("Size: " + objects.Count);
        for (int j = 0; j < objects.Count; j++)
        {
            EditorGUILayout.BeginHorizontal();
            objects[j] = EditorGUILayout.ObjectField(objects[j], typeof(Object));

            if (GUILayout.Button("删除"))
            {
                objects.RemoveAt(j);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Button:");
        if (GUILayout.Button("增加选中资源"))
        {
            Object[] selects = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            for (int k = 0; k < selects.Length; k++)
            {
                if (!isExists(objects, selects[k]))
                {
                    objects.Add(selects[k]);
                }
                else
                {
                    Debug.Log(selects[k].ToString() + " has Exists");
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
            EditorGUILayout.LabelField("ERROR: " + package.errorMsg[i], errorMsg);
        }

        for (int i = 0; i < package.warnMsg.Count; i++)
        {
            EditorGUILayout.LabelField("WARN: " + package.warnMsg[i], warnMsg);
        }
    }

    void BundlesView()
    {
        for (int i = 0; i < bundles.Count; i++)
        {
            if (!GetIsShowByRelyMask(bundles[i]))
            {
                continue;
            }

            //折叠标签
            EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            bundles[i].isFold = EditorGUILayout.Foldout(bundles[i].isFold, bundles[i].name);

            //删除视图
            if (GUILayout.Button("删除"))
            {
                bundles.RemoveAt(i);
                continue;
            }
            EditorGUILayout.EndHorizontal();

            //内容
            EditorGUI.indentLevel = 3;
            if (bundles[i].isFold)
            {
                //名称
                EditorGUI.indentLevel = 4;
                if (bundles[i].mainObject != null)
                {
                    bundles[i].name = bundles[i].mainObject.name;
                }
                else
                {
                    bundles[i].name = "Null";
                }

                //主资源
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("主资源:");
                EditorGUILayout.ObjectField(bundles[i].mainObject, typeof(Object));
                EditorGUILayout.EndHorizontal();

                //依赖包
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("依赖包:");
                bundles[i].relyPackagesMask = EditorGUILayout.MaskField(bundles[i].relyPackagesMask, RelyPackageNames);
                EditorGUILayout.EndHorizontal();

                //Debug.Log(" bundles[i].relyPackagesMask:  " + bundles[i].relyPackagesMask);

                //加载路径
                EditorGUILayout.LabelField("路径: ", bundles[i].path);
                bundles[i].isFold_objects = EditorGUILayout.Foldout(bundles[i].isFold_objects, "Objects");

                //子资源视图
                EditorGUI.indentLevel = 5;
                if (bundles[i].isFold_objects)
                {
                    ObjectListView(bundles[i].objects);
                }
            }
            //消息视图
            MessageView(bundles[i]);
        }

        EditorGUILayout.Space();

        //按钮
        EditorGUI.indentLevel = 1;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Button:");

        if (GUILayout.Button("自动添加所有Resource目录下的资源"))
        {
            AddAllResourceBundle();
        }

        if (GUILayout.Button("增加选中bundle包"))
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
        }
        EditorGUILayout.EndHorizontal();
    }

    void ShowMessage(string msg)
    {
        isContent = true;
        messageContent = msg;
    }


    #endregion

    #region 支持函数

    bool isExists(List<Object> objects, Object obj)
    {
        foreach (Object tmp in objects)
        {
            if (tmp.Equals(obj))
            {
                return true;
            }
        }

        return false;
    }

    void UpdateRelyPackageNames()
    {
        RelyPackageNames = new string[relyPackages.Count];
        for (int i = 0; i < relyPackages.Count; i++)
        {
            RelyPackageNames[i] = relyPackages[i].name;
        }
    }


    string GetObjectPath()
    {
        return "";
    }

    void AddAssetBundle(Object obj,string path)
    {
        if (isExist_AllBundle(obj))
        {
            Debug.Log(obj.name + " 已经存在！");
        }
        else
        {
            EditPackageConfig EditPackageConfigTmp = new EditPackageConfig();
            EditPackageConfigTmp.name = obj.name;
            EditPackageConfigTmp.mainObject = obj;
            EditPackageConfigTmp.path = path;

            Object[] res = GetCorrelationResource(obj);

            //判断依赖包中含不含有该资源，如果有，则不将此资源放入bundle中
            for (int j = 0; j < res.Length; j++)
            {
                bool isExistRelyPackage = false;

                for (int i = 0; i < relyPackages.Count; i++)
                {
                    if (isExist_Bundle(res[j], relyPackages[i]))
                    {
                        //在依赖包选项中添加此依赖包
                        EditPackageConfigTmp.relyPackagesMask = EditPackageConfigTmp.relyPackagesMask | 1 << i;
                        isExistRelyPackage = true;
                        break;
                    }
                }

                //该资源不在依赖包中，并且也与主资源不同时，放入包中
                if (isExistRelyPackage == false
                    && !EditPackageConfigTmp.mainObject.Equals(res[j]))
                {
                    EditPackageConfigTmp.objects.Add(res[j]);
                }

               
            }

            bundles.Add(EditPackageConfigTmp);
        }
    }

    /// <summary>
    /// 判断一个资源是否已经在bundle列表中
    /// </summary>
    /// <param name="obj">资源对象</param>
    /// <returns>是否存在</returns>
    bool isExist_AllBundle(Object obj)
    {
        for (int i = 0; i < bundles.Count; i++)
        {
            if (bundles[i].mainObject.Equals(obj))
            {
                return true;
            }

            if (isExist_Bundle(obj, bundles[i]))
            {
                return true;
            }
        }

        for (int i = 0; i < relyPackages.Count; i++)
        {
            if (isExist_Bundle(obj, relyPackages[i]))
            {
                return true;
            }
        }
        return false;
    }

    bool isExist_Bundle(Object obj, EditPackageConfig package)
    {
        return isExist_List(package.objects, obj);
    }

    bool isExist_List(List<Object> list, Object obj)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Equals(obj))
            {
                return true;
            }
        }

        return false;
    }

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

                Debug.Log(relyPackages[i].name);
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

    //自定义TosString()方法
    string CustomToString(Object obj)
    {
        if ((obj as MonoScript) != null)
        {
            return obj.name + " (MonoScript)";
        }
        else
        {
            return obj.ToString();
        }

        return "";
    }

    #endregion

    #region 按钮回调函数

    [MenuItem("Tool/显示选中对象所有相关资源")]
    public static void ShowAllCorrelationResource()
    {
        Object[] roots = Selection.GetFiltered(typeof(Object), SelectionMode.Unfiltered);
        Selection.objects = EditorUtility.CollectDependencies(roots);
    }

    int direIndex = 0;

    string resourceParentPath = "/Resources/";
    //自动将Resource下所有资源打包
    void AddAllResourceBundle()
    {
        string resourcePath = Application.dataPath +resourceParentPath;

        Debug.Log("开始添加~");
        Debug.Log(resourcePath);

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
            RecursionDirectory(dires[i]);
        }

        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            string relativePath = files[i].Substring(direIndex);

            if (relativePath.EndsWith(".prefab")
                || relativePath.EndsWith(".mp3")
                || relativePath.EndsWith(".wav")
                || relativePath.EndsWith(".txt")
                || relativePath.EndsWith(".json")
                || relativePath.EndsWith(".xml")
                )
            {
                relativePath = relativePath.Remove(relativePath.LastIndexOf("."));
                Object tmp = Resources.Load(relativePath);
                AddAssetBundle(tmp,relativePath);
            }
        }
    }

    Dictionary<string, List<Object>> checkDict = new Dictionary<string, List<Object>>();
    Dictionary<string, int> bundleName = new Dictionary<string, int>();
    int errorCount = 0;
    int warnCount = 0;
    /// <summary>
    /// 依赖包检查
    /// </summary>
    void CheckPackage()
    {
        ClearCheckLog();
        checkDict = new Dictionary<string, List< Object>>();
        bundleName = new Dictionary<string, int>();

        //包名重复检查
        //资源重复检查
        for (int i = 0; i < relyPackages.Count; i++)
        {
            checkPackage(relyPackages[i]);
        }

        for (int i = 0; i < bundles.Count; i++)
        {
            checkPackage(bundles[i]);
        }

        //资源丢失检查
        for (int i = 0; i < bundles.Count; i++)
        {
            checkMissRes(bundles[i]);
        }

        ShowMessage("检查完毕!  错误：" + errorCount + " 警告： "+ warnCount);
    }

    /// <summary>
    /// 检查某个包内有没有重复资源
    /// </summary>
    /// <param name="pack"></param>

    void checkPackage(EditPackageConfig pack)
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
            string resNameTmp = CustomToString(pack.mainObject);

            if (checkDict.ContainsKey(resNameTmp))
            {
                if (isExist_List(checkDict[resNameTmp], pack.mainObject))
                {
                    pack.warnMsg.Add("MainObject 重复! " + resNameTmp);
                    warnCount++;
                }
                else
                {
                    checkDict[resNameTmp].Add(pack.mainObject);
                }
            }
            else
            {
                checkDict.Add(resNameTmp,new List<Object>());
                checkDict[resNameTmp].Add(pack.mainObject);
            }
        }

        for (int i = 0; i < pack.objects.Count; i++)
        {
            string resNameTmp = CustomToString(pack.objects[i]);

            if (checkDict.ContainsKey(resNameTmp))
            {
                if (isExist_List(checkDict[resNameTmp], pack.objects[i]))
                {
                    pack.warnMsg.Add("Objects存在重复资源 ! " + resNameTmp);
                    warnCount++;
                }
                else
                {
                    checkDict[resNameTmp].Add(pack.objects[i]);
                }
            }
            else
            {
                checkDict.Add(resNameTmp, new List<Object>());
                checkDict[resNameTmp].Add(pack.objects[i]);
            }
        }
    }

    /// <summary>
    /// 检查单个包有没有丢失资源
    /// </summary>
    /// <param name="pack"></param>
    void checkMissRes(EditPackageConfig pack)
    {
        if(pack.mainObject == null)
        {
            pack.errorMsg.Add("没有主资源！");
            errorCount++;
            return;
        }

        Object[] res = GetCorrelationResource(pack.mainObject);

        for (int i = 0; i < res.Length; i++)
        {
            if (!GetResIsUse(pack, res[i]))
            {
                pack.errorMsg.Add( CustomToString(res[i]) + " 资源丢失！");
                errorCount++;
            }
        }
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
    /// 检查单个资源是否全都被引用
    /// </summary>
    bool GetResIsUse(EditPackageConfig pack,Object res)
    {
        if (pack.mainObject.Equals(res))
        {
            return true;
        }

        if (isExist_Bundle(res,pack))
        {
            return true;
        }

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

    void CreatPackageFile()
    {

    }

    void Package()
    {

    }

    #endregion

}

public class EditPackageConfig
{
    public bool isFold = false; //是否折叠
    public string name = "";
    public string path = "";    //存放路径

    public List<Object> objects = new List<Object>();  //所有子资源
    public bool isFold_objects = false; //子资源是否折叠

    //bundle独有
    public Object mainObject;     //主资源
    public int relyPackagesMask;
    public List<string> relyPackages = new List<string>(); //所有依赖包

    public List<string> warnMsg = new List<string>();  //错误日志
    public List<string> errorMsg = new List<string>(); //警告日志

}
