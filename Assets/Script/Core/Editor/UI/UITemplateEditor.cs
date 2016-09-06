using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

#pragma warning disable
public class UITemplateEditor : EditorWindow
{
    //模板prafeb所在路径
    const string c_TemplateResPath = "Assets/EditorTemplateRes/";
    //空白模板名称
    const string c_BlankTemplateName = "BlankTemplate";
    //空白接口名称
    const string c_BlankPortNmae = "BlankPort";
    //接口名标志
    const string c_Interface = "Interface";

    //所有模板名称
    string[] allTemplateName;

    //当前显示的模板名
    string s_nowTemplateName = null;

    //所有使用了本模板的UI
    List<GameObject> allUsedTheTemplateUI;

    //模板实例
    GameObject go_UITemplate;

    //模板预设
    GameObject pre_UITemplate;

    //当前选中的gameObject
    GameObject go_SelectedNode;

    //选中的GameObject的名称分割
    string[] selectedObjName;

    //选中的是什么类型
    SelectObjType selectObjType;

    //选中的类型枚举
    enum SelectObjType
    {
        Instance,       //模板实例
        PreFab,         //模板
        UsedTemplate,   //用了模板的UI
        NoUsedTemplate  //没用模板的UI
    }

    //debug信息
    List<string> myDebug = new List<string>();

    [MenuItem("Window/UI/UI模板工具")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UITemplateEditor));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        CreatBlankTemple_GUI();
        TempleInfo_GUI();
        ShowAllTemple_GUI();
        EditorGUILayout.EndVertical();

        if (GUILayout.Button("应用所有模板"))
        {
            ReadyApplyTemplate();

            allTemplateName = UITemplateConfigManager.GetUIStyleList();
            foreach (var oneTemplateName in allTemplateName)
            {
                ApplyOneTemplate(oneTemplateName);
            }
        }
    }

    //是否保存模板根节点的Canvas组件

    #region 创建部分
    //隐藏创建部分
    bool b_isFoldCreatTemplate = true;

    /// <summary>
    /// 创建部分UI显示
    /// </summary>
    void CreatBlankTemple_GUI()
    {
        EditorGUI.indentLevel = 0;
        
        b_isFoldCreatTemplate = EditorGUILayout.Foldout((b_isFoldCreatTemplate), "创建模板:");

        if (b_isFoldCreatTemplate)
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.LabelField("提示：模板名中不能含有下划线");
            s_nowTemplateName = EditorGUILayout.TextField("模板名称：", s_nowTemplateName);

            if (IsReasonable(s_nowTemplateName))
            {
                if (HaveTheTemlate(s_nowTemplateName))
                {
                    if (GUILayout.Button("覆盖模板"))
                    {
                        if (EditorUtility.DisplayDialog("警告", "该模板已存在，是否覆盖？", "是", "否"))
                        {
                            CreatUIBlankTemplate();
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("创建一个空白模板"))
                    {
                        CreatUIBlankTemplate();
                    }
                }
            }
        }
    }

    /// <summary>
    /// 判断模板名是否符合规定
    /// </summary>
    bool IsReasonable(string TemplateName)
    {
        //模板名中不能含有下划线
        if (TemplateName != null 
            && TemplateName != ""
            && !TemplateName.Contains("_"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 创建一个空白模板
    /// </summary>
    void CreatUIBlankTemplate()
    {
        pre_UITemplate          = FindPrefabInTemplateRes(c_BlankTemplateName);

        go_UITemplate           = GameObject.Instantiate(pre_UITemplate);
        go_UITemplate.name      = s_nowTemplateName;
        newTemplateName         = s_nowTemplateName;
        pre_UITemplate          = null;

        b_isFoldCreatTemplate   = false;
    }
#endregion

    #region 模板当前信息
    //是否折叠
    bool b_isFoldTemplateInfo = false;
    bool b_isFoldCreateInterface = false;

    //是否折叠所有UI
    bool b_isFoldUsedUI = false;

    //模板的新名称
    string newTemplateName = "";

    void TempleInfo_GUI()
    {
        //SelectCurrentTemplate();
        EditorGUI.indentLevel = 0;
        EditorGUILayout.BeginHorizontal();
        b_isFoldTemplateInfo = EditorGUILayout.Foldout((b_isFoldTemplateInfo), "当前模板信息:");
        //if (GUILayout.Button("查看/编辑选中的模板",GUILayout.Width(200)))
        //{
        //    SelectCurrentTemplate();
        //}

        EditorGUILayout.EndHorizontal();

        if (b_isFoldTemplateInfo && (go_UITemplate != null || pre_UITemplate != null))
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.LabelField("模板名称："+ newTemplateName);

            EditorGUILayout.ObjectField("模板预设：", pre_UITemplate, typeof(GameObject));
            EditorGUILayout.ObjectField("模板实例：", go_UITemplate, typeof(GameObject));
            
            b_isFoldCreateInterface = EditorGUILayout.Foldout(b_isFoldCreateInterface, "创建接口:");
            if (b_isFoldCreateInterface)
            {
                EditorGUI.indentLevel = 2;

                newPortName = EditorGUILayout.TextField("接口名：", newPortName);
                EditorGUILayout.ObjectField("当前节点:", go_SelectedNode, typeof(GameObject));

                if (IsReasonable(newPortName))
                {
                    if (GUILayout.Button("在当前节点添加一个接口"))
                    {
                        CreatOnePort();
                    }
                }

                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel = 1;
            if (GUILayout.Button("保存模板"))
            {
                SaveTemplate();
            }

            if (GUILayout.Button("应用模板"))
            {
                ReadyApplyTemplate();
                ApplyOneTemplate(s_nowTemplateName);
            }

            if (GUILayout.Button("删除模板"))
            {
                if (EditorUtility.DisplayDialog("警告", "该操作不可逆，确定删除该模板？", "是", "否"))
                {
                    DeleteNowTemlate();
                    DestroyImmediate(go_UITemplate);
                }
            }

            EditorGUILayout.Space();
        }
    }

    void SelectCurrentTemplate()
    {
        if (Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets).Length == 0)
        {
            return;
        }

        go_SelectedObj = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets)[0] as GameObject;
        selectedObjName = go_SelectedObj.name.Split('_');
        s_nowTemplateName = JudgeNowSelectObjType();
        newTemplateName = s_nowTemplateName;

        ReadyApplyTemplate();
        allUsedTheTemplateUI = FindHadOneTemplateUI(s_nowTemplateName);

        DisposeSelectObj();
    }

    //模板应用新名字(相当于创建一个新模板，所以要删除旧数据，记录新数据)
    void UseNewTemlateName()
    {
        UITemplateConfigManager.DestroyData(s_nowTemplateName);
        s_nowTemplateName     = newTemplateName;
        AssetDatabase.RenameAsset(c_TemplateResPath + pre_UITemplate.name+".prefab",newTemplateName);
        if (go_UITemplate != null)
        {
            go_UITemplate.name = s_nowTemplateName;
        }
        UITemplateConfigManager.AddData(s_nowTemplateName);

        ReadyApplyTemplate();
        ApplyOneTemplate(s_nowTemplateName);
    }

    //判断当前选中的是那种类型
    string JudgeNowSelectObjType()
    {
        selectObjType = SelectObjType.NoUsedTemplate;
        if (selectedObjName.Length == 1)
        {
            if (HaveTheTemlate(selectedObjName[0]))
            {
                if (GameObject.Find(selectedObjName[0]))
                {
                    selectObjType = SelectObjType.Instance;
                }
                else
                {
                    selectObjType = SelectObjType.PreFab;
                }

                return selectedObjName[0];
            }

            return "";
      
        }
        else
        {
            
            foreach (string name in selectedObjName)
            {
                if (HaveTheTemlate(name))
                {
                    selectObjType = SelectObjType.UsedTemplate;
                    return name;
                }

            }
            return "";
 
        }
 
    }

    //处理当前选中
    void DisposeSelectObj()
    {
        switch(selectObjType)
        {
            case SelectObjType.Instance         : SelectIsInstance()        ; break;
            case SelectObjType.PreFab           : SelectIsPrefab()          ; break;
            case SelectObjType.UsedTemplate     : SelectIsUsedTemplate()    ; break;
            case SelectObjType.NoUsedTemplate   : SelectIsNoUsedTemplate()  ; break;
            default: break;
        }
    }

    //选中的是模板实例
    void SelectIsInstance()
    {
        go_UITemplate           = go_SelectedObj;
        pre_UITemplate          = FindPrefabInTemplateRes(s_nowTemplateName);
        b_isFoldTemplateInfo    = true;
        Debug.Log("选中的是模板实例");
    }

    //选中的是模板
    void SelectIsPrefab()
    {
        go_UITemplate           = null;
        pre_UITemplate          = FindPrefabInTemplateRes(s_nowTemplateName); 
        b_isFoldTemplateInfo    = true;
        Debug.Log("选中的是模板");
    }

    //选中的是使用了模板的UI
    void SelectIsUsedTemplate()
    {
        go_UITemplate           = null;
        pre_UITemplate          = FindPrefabInTemplateRes(s_nowTemplateName);
        b_isFoldTemplateInfo    = true;
        Debug.Log("选中的是使用了模板的UI");
    }

    //选中的是没有使用模板的UI
    void SelectIsNoUsedTemplate()
    {
        go_UITemplate   = null;
        pre_UITemplate  = null;
        Debug.Log("选中的是没有使用模板的UI");
    }

    //创建一个接口
    void CreatOnePort()
    {
        if (go_SelectedNode == null)
        {
            Debug.LogError("请先选中要添加接口的GameObject！");
            return;
        }
        GameObject go_newPort           = Instantiate(FindPrefabInTemplateRes(c_BlankPortNmae));
        go_newPort.transform.parent     = go_SelectedNode.transform;

        go_newPort.name                 = c_Interface+"_"+newPortName ;

    }


#endregion

    #region 保存模板
    //是否隐藏
    bool b_isFoldSaveTemplate = false;
    //是否保留根节点的Canvas等组件
    bool b_saveTemplateCanvas = false;

    void SaveTemple_GUI()
    {
        EditorGUI.indentLevel = 0;
        b_isFoldSaveTemplate = EditorGUILayout.Foldout((b_isFoldSaveTemplate), "保存模板:");
        if (b_isFoldSaveTemplate && go_UITemplate!= null)
        {
            EditorGUI.indentLevel = 1;
            
            b_saveTemplateCanvas = EditorGUILayout.Toggle("保存模板根节点的Canvas", b_saveTemplateCanvas);
            if (GUILayout.Button("保存模板"))
            {
                SaveTemplate();
            }
        }
    }

    //保存模板
    void SaveTemplate()
    {
        pre_UITemplate = PrefabUtility.CreatePrefab(c_TemplateResPath + s_nowTemplateName + ".prefab", go_UITemplate, ReplacePrefabOptions.ConnectToPrefab);
        UITemplateConfigManager.AddData(s_nowTemplateName);
    }
#endregion

    #region 删除模板
    //是否折叠
    bool b_isFoldDeleteTemplate = false;

    void DeleteTemple_GUI()
    {
        EditorGUI.indentLevel   = 0;
        b_isFoldDeleteTemplate  = EditorGUILayout.Foldout((b_isFoldDeleteTemplate), "删除模板:");
        EditorGUI.indentLevel   = 1;
       
        if (b_isFoldDeleteTemplate)
        {
            EditorGUILayout.LabelField("为避免误操作，请折叠本标签！", EditorGUIStyleData.s_ErrorMessageLabel);
            Button_DeleteTemplateGo();
            Button_DeleteTemplatePrefab();
            Button_DeleteTemplatePrefabAndGo();
        }
    }

    //删除当前的模板实例
    void Button_DeleteTemplateGo()
    {
        if (go_UITemplate != null)
        {
            if (GUILayout.Button("删除当前的模板实例"))
            {
                DestroyImmediate(go_UITemplate);

            }

        }
    }

    //删除当前的模板，但不删实例
    void Button_DeleteTemplatePrefab()
    {
        if (pre_UITemplate != null)
        {
            if (GUILayout.Button("删除当前的模板，但不删实例"))
            {
                DeleteNowTemlate();

            }

        }
    }

    //删除当前的模板与实例
    void Button_DeleteTemplatePrefabAndGo()
    {
        if (go_UITemplate != null && pre_UITemplate != null)
        {
            if (GUILayout.Button("删除当前的模板与实例"))
            {
                DeleteNowTemlate();
                DestroyImmediate(go_UITemplate);
                b_isFoldDeleteTemplate = false;

            }

        }
 
    }


    //删除当前模板（包括数据）
    void DeleteNowTemlate()
    {

        UITemplateConfigManager.DestroyData(s_nowTemplateName);
        DestroyImmediate(pre_UITemplate, true);
        
        
    }

#endregion

    #region 使用模板
    //是否折叠
    bool b_isFoldUseTemple      = false;
    bool b_isfoldDebugInfo      = false;

    string newUIName            = "";
    string newPortName          = "";

    //显示所有模板
    void ShowAllTemple_GUI()
    {
        if (Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets).Length > 0)
        {
            go_SelectedNode = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets)[0] as GameObject;
        }
        else
        {
            go_SelectedNode = null;
        }

        EditorGUI.indentLevel = 0;
        b_isFoldUseTemple = EditorGUILayout.Foldout((b_isFoldUseTemple), "模板列表:");
        if (b_isFoldUseTemple)
        {
            EditorGUI.indentLevel   = 1;
            allTemplateName    = UITemplateConfigManager.GetUIStyleList();

            for (int i = 0; i < allTemplateName.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(allTemplateName[i] + ":", GUILayout.Width(70));
                EditorGUILayout.ObjectField(FindPrefabInTemplateRes(allTemplateName[i]), typeof(GameObject), GUILayout.Width(100));


                if(GUILayout.Button("选中此模板"))
                {

                }

                EditorGUILayout.EndHorizontal();
            }
        }


        EditorGUI.indentLevel = 0;
        EditorGUILayout.BeginHorizontal();
        b_isfoldDebugInfo = EditorGUILayout.Foldout((b_isfoldDebugInfo), "反馈信息:");
        if (GUILayout.Button("清理", GUILayout.Width(200)))
        {
            myDebug.Clear();
        }
        EditorGUILayout.EndHorizontal();
        if (b_isfoldDebugInfo)
        {
            EditorGUI.indentLevel = 1;
            foreach (var item in myDebug)
            {
                EditorGUILayout.LabelField(item, EditorGUIStyleData.s_WarnMessageLabel);
            }

        }
 
    }

    //当前选中的节点,将成为新UI的父节点
    GameObject go_SelectedObj;

    //用模板创建UI
    void CreatUIByOneTemplate(string templateName)
    {
        GameObject go_newUI = Instantiate(FindPrefabInTemplateRes(templateName));
        if (go_SelectedNode != null)
        {
            go_newUI.transform.parent = go_SelectedNode.transform;
        }
        
        if (newUIName == "")
        {
            go_newUI.name   = templateName;
        }
        else
        {
            go_newUI.name   = templateName+ "_" + newUIName   ;
        }
        s_nowTemplateName   = templateName;
        newTemplateName     = s_nowTemplateName;
        go_UITemplate       = go_newUI;
        pre_UITemplate      = FindPrefabInTemplateRes(templateName);
 
    }


  


#endregion

    #region 模板替换部分

    //所有UI预设
    Dictionary<string, GameObject> allUIPrefab;
    //所有UI预设名称
    string[] allUIPrefabName;
    //一个UI预设的名称
    string oneUIPrefabName;
    //一个预设的路径
    string oneUIPrefabPsth;

    //应用模板之前的准备
    void ReadyApplyTemplate()
    {
        //myDebug.Clear();
        allUIPrefab = new Dictionary<string, GameObject>();
        ReadAllUIResources("Resources/UI");
        //Debug.Log("“Resources/UI”目录下UI总数：" + allUIPrefab.Count);
 
    }

    //读取“Resources/UI”目录下所有的UI预设
    void ReadAllUIResources(string path)
    {
        allUIPrefabName = Directory.GetFiles(Application.dataPath + "/" + path);
        foreach (var item in allUIPrefabName)
        {
            oneUIPrefabName = item.Split('\\')[1].Split('.')[0];
            if (item.EndsWith(".prefab"))
            {

                oneUIPrefabPsth = path + "/" + oneUIPrefabName + ".prefab";
                allUIPrefab.Add(oneUIPrefabName, AssetDatabase.LoadAssetAtPath("Assets/"+oneUIPrefabPsth, typeof(GameObject)) as GameObject);
            }
            else if (item.Split('.')[(item.Split('.').Length - 2)] != "prefab")
            {
                ReadAllUIResources(path + "/" + oneUIPrefabName);
            }
        }
    }

    //应用一个模板到全局
    void ApplyOneTemplate(string templateName)
    {
        List<GameObject> allUsedUI = FindHadOneTemplateUI(templateName);
        foreach (var oneUI in allUsedUI)
        {
            //Debug.Log(oneUI);
            GameObject goUI = Instantiate(oneUI);
            GameObject goTemplate = Instantiate(FindPrefabInTemplateRes(templateName));

            foreach (Transform child in goUI.transform)
            {
                if (child.name.Contains(c_Interface))
                {
                    foreach (Transform templateChild in goTemplate.transform)
                    {
                        if (child.name == templateChild.name)
                        {
                            child.parent = templateChild.parent;
                            DestroyImmediate(templateChild.gameObject, true);
                        }
                    }
                }
                
            }
            myDebug.Add("应用成功：  模板：" + templateName + "  UI：" + oneUI.name);

            PrefabUtility.ReplacePrefab(goTemplate, oneUI);
            DestroyImmediate(goUI, true);
            DestroyImmediate(goTemplate, true);
           
        }
 
    }



    //获取应用某个模板的所有UI预设
    List<GameObject> FindHadOneTemplateUI(string templateName)
    {
        List<GameObject> findedUI = new List<GameObject>();
        string[] prefabNameSplit;
        foreach (var item in allUIPrefab.Keys)
        {
            prefabNameSplit = item.Split('_');
            foreach (var uiFrag in prefabNameSplit)
            {
                if (uiFrag == templateName)
                {

                    findedUI.Add(allUIPrefab[item]);
                    
                }
            }
        }

        return findedUI;

    }
    #endregion

    #region 封装过的方法
    //获取Prefab
    GameObject FindPrefabInTemplateRes(string name)
    {
        name = c_TemplateResPath + name;
        name += ".prefab";
        return AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;
    }

    //该模板是否已经存在
    bool HaveTheTemlate(string name)
    {
        return UITemplateConfigManager.HaveTheTemplate(name);
    }


    #endregion

}
