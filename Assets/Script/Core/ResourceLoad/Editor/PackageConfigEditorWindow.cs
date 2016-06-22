using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class PackageConfigEditorWindow : EditorWindow
{
    string ConfigName = "PackageConfigEditor";
    string GameConfigName = "PackageConfig";

    //所有依赖包
    List<EditPackageConfig> relyPackages = new List<EditPackageConfig>();
    //所有普通包
    List<EditPackageConfig> bundles = new List<EditPackageConfig>();

    #region 初始化

    void OnEnable()
    {
        //Debug.Log("初始化");

        LoadAndAnalysisJson();
        UpdateRelyPackageNames();
    }

    #endregion

    #region GUI

    int RelyMaskFilter = -1; //依赖包过滤器

    bool isFoldRelyPackages = true; //是否展开依赖包
    bool isFoldBundles = true;      //是否展开普通包

    Vector2 scrollPos = new Vector2();

    string[] RelyPackageNames = new string[1];

    GUIStyle errorMsg = new GUIStyle();
    GUIStyle warnMsg = new GUIStyle();

    bool isProgress = false;
    float progress = 0;
    string progressContent = "";

    bool isContent = false;
    string messageContent = "";

    void OnGUI()
    {
        titleContent.text = "打包设置编辑器";

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
                    ObjectListView(relyPackages[i]);
                }
            }
            EditorGUI.indentLevel = 2;
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

    void ObjectListView(EditPackageConfig pack)
    {
        EditorGUILayout.LabelField("Size: " + pack.objects.Count);
        for (int j = 0; j < pack.objects.Count; j++)
        {

            EditorGUILayout.LabelField("Path:", pack.objects[j].path);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(pack.objects[j].obj,typeof(Object),false);

            if (GUILayout.Button("删除"))
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

    //Bundle包视图
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
                if (bundles[i].mainObject != null 
                    &&bundles[i].mainObject.obj!= null)
                {
                    bundles[i].name = bundles[i].mainObject.obj.name;
                }
                else
                {
                    bundles[i].name = "Null";
                }

                //主资源
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("主资源:");
                EditorGUILayout.ObjectField(bundles[i].mainObject.obj, typeof(Object),false);
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
                    ObjectListView(bundles[i]);
                }
            }
            EditorGUI.indentLevel = 2;
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

    #region 工具函数

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

        for (int i = 0; i < relyPackages.Count; i++)
        {
            if (isExist_Bundle(obj, relyPackages[i]))
            {
                return true;
            }
        }
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

    List<string> GetRelyPackNames(int mask)
    {
        List<string> names = new List<string>();

        List<EditPackageConfig> tmp = GetRelyPackListByMask(mask);

        for (int i = 0; i < tmp.Count; i++)
        {
            names.Add(tmp[i].name);
        }

        return names;
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
        if (obj == null)
        {
            return "Null";
        }

        if ((obj as MonoScript) != null)
        {
            return obj.name + " (MonoScript)";
        }
        else
        {
            return obj.ToString();
        }
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
        EditorWindow.GetWindow(typeof(PackageConfigEditorWindow));
    }
    #endregion

    #region 自动添加Resource目录下的所有资源

    int direIndex = 0;
    string resourceParentPath = "/Resources/";
    //自动添加Resource目录下的所有资源
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
                //Debug.Log(relativePath);
                relativePath = relativePath.Remove(relativePath.LastIndexOf("."));
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
            Debug.Log(obj.name + " 已经存在！");
        }
        else
        {
            EditPackageConfig EditPackageConfigTmp = new EditPackageConfig();
            EditPackageConfigTmp.name = obj.name;

            EditorObject mainObjTmp = new EditorObject();
            mainObjTmp.obj = obj;
            mainObjTmp.path = GetObjectPath(obj);

            EditPackageConfigTmp.mainObject = mainObjTmp;
            EditPackageConfigTmp.path = GetObjectPath(obj);

            Object[] res = GetCorrelationResource(obj);

            //判断依赖包中含不含有该资源，如果有，则不将此资源放入bundle中
            for (int j = 0; j < res.Length; j++)
            {
                if (res[j] == null)
                {
                    Debug.LogWarning(obj + "　有资源丢失！");
                    continue;
                }

                //过滤掉一些不必要加载进去的组件
                if (ComponentFilter(res[j]))
                {
                    continue;
                }

                EditorObject tmp = new EditorObject();
                tmp.obj = res[j];
                tmp.path = GetObjectPath(res[j]);

                bool isExistRelyPackage = false;

                for (int i = 0; i < relyPackages.Count; i++)
                {
                    if (isExist_Bundle(tmp, relyPackages[i]))
                    {
                        //在依赖包选项中添加此依赖包
                        EditPackageConfigTmp.relyPackagesMask = EditPackageConfigTmp.relyPackagesMask | 1 << i;
                        isExistRelyPackage = true;
                        break;
                    }
                }

                //该资源不在依赖包中，并且也与主资源不同时，放入包中
                if (isExistRelyPackage == false
                    &&!EqualsEditorObject(EditPackageConfigTmp.mainObject,tmp)
                    )
                {
                    
                    EditPackageConfigTmp.objects.Add(tmp);
                }
            }

            bundles.Add(EditPackageConfigTmp);
        }
    }

    Dictionary<string, Shader> shaderFilter = new Dictionary<string, Shader>();
    //组件过滤器
    bool ComponentFilter(Object comp)
    {
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
    int warnCount = 0;
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
            string resNameTmp = CustomToString(pack.mainObject.obj);

            if (checkDict.ContainsKey(resNameTmp))
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
            else
            {
                checkDict.Add(resNameTmp,new List<EditorObject>());
                checkDict[resNameTmp].Add(pack.mainObject);
            }
        }

        for (int i = 0; i < pack.objects.Count; i++)
        {
            string resNameTmp = CustomToString(pack.objects[i].obj);

            if (checkDict.ContainsKey(resNameTmp))
            {
                if (isExist_EditorList(checkDict[resNameTmp], pack.objects[i]))
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
                checkDict.Add(resNameTmp, new List<EditorObject>());
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

            List<string> resLostList = FindLostRes(res[i]);
            for (int j = 0; j < resLostList.Count;j++ )
            {
                pack.warnMsg.Add(resLostList[j]);
            }

            EditorObject tmp = new EditorObject();
            tmp.obj = res[i];
            tmp.path = GetObjectPath(res[i]);

            if (!GetResIsUse(pack, tmp) && !ComponentFilter(res[i]))
            {
                pack.errorMsg.Add( CustomToString(res[i]) + " 资源丢失依赖！");
                errorCount++;
            }
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
        //检查是否掉材质球
        Renderer render = go.GetComponent<Renderer>();

        if (render != null)
        {
            if (render.sharedMaterials != null)
            {
                for (int i = 0; i < render.sharedMaterials.Length;i++ )
                {
                    if (render.sharedMaterials[i]!=null)
                    {
                        try
                        { 
                            if (render.sharedMaterials[i].GetTexture(0) == null)
                            {
                                list.Add("贴图丢失: name: " + go.name + " (Materials Index: " + i + ")");
                            }
                         }
                        catch(System.Exception e)
                        {
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
    /// 检查单个资源是否全都被引用
    /// </summary>
    bool GetResIsUse(EditPackageConfig pack,EditorObject res)
    {
        if (EqualsEditorObject(pack.mainObject,res))
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

    #endregion

    #region 生成配置文件与解析配置文件
    void CreatPackageFile()
    {
        //生成编辑器配置文件
        Dictionary<string, object> editorConfig = new Dictionary<string, object>();
        Dictionary<string, object> gameConfig = new Dictionary<string, object>();

        //依赖包
        List<object> relyBundles = new List<object>();
        List<object> gameRelyBundles = new List<object>();
        for (int i = 0; i < relyPackages.Count;i++ )
        {
            relyBundles.Add(JsonUtility.ToJson(relyPackages[i]));

            //生成游戏中使用的依赖包数据
            PackageConfig pack = new PackageConfig();
            pack.path = relyPackages[i].path;
            pack.relyPackages = new List<string>();
            gameRelyBundles.Add(JsonUtility.ToJson(pack));
        }

        //Bundle包
        List<object> AssetsBundles = new List<object>();
        List<object> gameAssetsBundles = new List<object>();
        for (int i = 0; i < bundles.Count; i++)
        {
            AssetsBundles.Add(JsonUtility.ToJson(bundles[i]));

            //生成游戏中使用的bundle包数据
            PackageConfig pack = new PackageConfig();
            pack.path = bundles[i].path;
            pack.relyPackages = GetRelyPackNames(bundles[i].relyPackagesMask); //获取依赖包的名字
            gameAssetsBundles.Add(JsonUtility.ToJson(pack));
        }

        editorConfig.Add("relyBundles", relyBundles);
        editorConfig.Add("AssetsBundles", AssetsBundles);

        gameConfig.Add("relyBundles", gameRelyBundles);
        gameConfig.Add("AssetsBundles", gameRelyBundles);

        //保存编辑器配置文件
        ConfigManager.SaveEditorConfigData(ConfigName,editorConfig);
        //保存游戏中读取的配置文件
        ConfigManager.SaveConfigData(GameConfigName,gameConfig);

    }

    string stringTmp = "";

    /// <summary>
    /// 读取并且解析Json
    /// </summary>
    void LoadAndAnalysisJson()
    {
        Dictionary<string, object> final = ConfigManager.GetEditorConfigData(ConfigName);

        if (final == null)
        {
            Debug.Log(ConfigName + " ConfigData dont Exits");
            return;
        }

        //依赖包
        List<object> relyBundles = (List<object>)final["relyBundles"];

        relyPackages = new List<EditPackageConfig>();
        for (int i = 0; i < relyBundles.Count; i++)
        {
            EditPackageConfig tmp = JsonUtility.FromJson<EditPackageConfig>((string)relyBundles[i]);
            
            //重新加载Object
            ReLoadGameObject(tmp);

            relyPackages.Add(tmp);
        }

        //Bundle包
        List<object> AssetsBundles = (List<object>)final["AssetsBundles"];

        bundles = new List<EditPackageConfig>();
        for (int i = 0; i < AssetsBundles.Count; i++)
        {
            EditPackageConfig tmp = JsonUtility.FromJson<EditPackageConfig>((string)AssetsBundles[i]);

            //重新加载Object
            ReLoadGameObject(tmp);

            bundles.Add(tmp);
        }

    }

    //重新加载Object

    void ReLoadGameObject(EditPackageConfig pack)
    {
        if (pack.mainObject != null)
        {
            ReLoadEditObject(pack.mainObject);
        }

        for (int i = 0; i < pack.objects.Count;i++ )
        {
            ReLoadEditObject(pack.objects[i]);
        }
    }

    void ReLoadEditObject(EditorObject editObj)
    {
        editObj.obj = AssetDatabase.LoadAssetAtPath<Object>(editObj.path);
    }

    #endregion

    #region 打包

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

    public List<EditorObject> objects = new List<EditorObject>();  //所有子资源
    public bool isFold_objects = false; //子资源是否折叠

    //bundle独有
    public EditorObject mainObject;     //主资源
    public int relyPackagesMask;        //依赖包mask
    //public List<string> relyPackages = new List<string>(); //所有依赖包

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
