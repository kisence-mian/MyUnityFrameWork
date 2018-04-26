using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

#pragma warning disable
public class UITemplate
{
    //模板prafeb所在路径
    const string c_TemplateResPath = "EditorTemplateRes/";

    //接口名标志
    const string c_InterfaceName = "Interface";
    const string c_TemplateName = "Template";

    //所有模板名称
    List<string> m_allTemplateName = new List<string>();

    //当前显示的模板名
    string m_nowTemplateName = null;

    string m_newTemplateName = null;

    //模板实例
    GameObject m_UITemplate;

    //模板预设
    GameObject m_preUITemplate;

    //当前选中的gameObject
    GameObject m_SelectedNode;

    public void OnEnable()
    {
        EditorGUIStyleData.Init();

        FindAllUITemplate();
    }

    //当工程改变时
    public void OnProjectChange()
    {
        FindAllUITemplate();
    }

    #region GUI

    public void GUI()
    {
        CurrentChildGUI();
        CreatBlankTemple_GUI();
       
        TempleInfo_GUI();

        ShowAllTemple_GUI();
        

        if (GUILayout.Button("应用所有模板"))
        {
            ApplyAllUITemplate();
        }
    }

    #endregion

    #region 创建模板

    //隐藏创建部分
    bool b_isFoldCreatTemplate = false;

    public void CurrentChildGUI()
    {
        EditorGUI.indentLevel = 1;
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField("当前节点:", m_SelectedNode, typeof(GameObject));
        EditorGUILayout.Space();
    }

    /// <summary>
    /// 创建部分UI显示
    /// </summary>
    public void CreatBlankTemple_GUI()
    {
        EditorGUI.indentLevel = 1;
        
        b_isFoldCreatTemplate = EditorGUILayout.Foldout((b_isFoldCreatTemplate), "创建模板:");

        if (b_isFoldCreatTemplate)
        {
            EditorGUI.indentLevel = 2;

            EditorGUILayout.LabelField("提示：模板名中不能含有下划线");
            m_newTemplateName = EditorGUILayout.TextField("模板名称：", m_newTemplateName);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (IsReasonable(m_newTemplateName))
            {
                if (HaveTheTemlate(m_newTemplateName))
                {
                    if (GUILayout.Button("覆盖模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                    {
                        if (EditorUtility.DisplayDialog("警告", "该模板已存在，是否覆盖？", "是", "否"))
                        {
                            CreatUIBlankTemplate();
                            m_newTemplateName = "";
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("创建一个空白模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                    {
                        CreatUIBlankTemplate();
                        m_newTemplateName = "";
                    }
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 判断是否符合规定
    /// </summary>
    bool IsReasonable(string TemplateName)
    {
        //不能含有下划线
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
    public void CreatUIBlankTemplate()
    {
        m_UITemplate = new GameObject(c_TemplateName + "_" + m_newTemplateName);
        m_preUITemplate          = null;

        m_UITemplate.layer = LayerMask.NameToLayer("UI");

        RectTransform rt =  m_UITemplate.AddComponent<RectTransform>();

        if(m_SelectedNode != null)
        {
            m_UITemplate.transform.SetParent(m_SelectedNode.transform);
        }

        m_UITemplate.transform.localScale = Vector3.one;

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;

        rt.anchoredPosition3D = Vector3.zero;
        rt.sizeDelta = Vector2.zero;
        rt.localScale = Vector3.one;

        SaveTemplate(m_UITemplate.name);

        b_isFoldCreatTemplate   = false;
        b_isFoldTemplateInfo    = true;
    }
    #endregion

    #region 模板当前信息

    //是否折叠
    bool b_isFoldTemplateInfo = true;

    List<GameObject> m_interfaceList = new List<GameObject>();

    public void TempleInfo_GUI()
    {
        SelectCurrentTemplate();
        if (m_UITemplate != null)
        {
            EditorGUI.indentLevel = 1;
            b_isFoldTemplateInfo = EditorGUILayout.Foldout((b_isFoldTemplateInfo), "当前模板信息:");

            if (b_isFoldTemplateInfo)
            {
                EditorGUI.indentLevel = 2;

                EditorGUILayout.LabelField("模板名称：" + m_nowTemplateName);

                EditorGUILayout.ObjectField("模板预设：", m_preUITemplate, typeof(GameObject));
                EditorGUILayout.ObjectField("模板实例：", m_UITemplate, typeof(GameObject));

                InterfaceGUI();

                EditorGUI.indentLevel = 2;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("保存模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                {
                    SaveTemplate(m_nowTemplateName);
                }
                //EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("应用模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                {
                    ApplyOneTemplate(m_nowTemplateName);
                }
                //EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("删除模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                {
                    if (EditorUtility.DisplayDialog("警告", "该操作不可逆，确定删除该模板？", "是", "否"))
                    {
                        DeleteTemlate(m_nowTemplateName);
                        GameObject.DestroyImmediate(m_UITemplate);
                    }
                }
                //EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
        }
    }

    public void SelectCurrentTemplate()
    {
        if (Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets).Length == 0)
        {
            return;
        }

        m_UITemplate = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets)[0] as GameObject;
        m_UITemplate = GetTemlateParant(m_UITemplate);

        if (m_UITemplate != null)
        {
            m_nowTemplateName = GetTemplateName(m_UITemplate.name);
            m_preUITemplate = FindPrefabInTemplateRes(m_nowTemplateName);
        }
    }


    //保存模板
    void SaveTemplate(string l_templateName)
    {
        GameObject saveprefab = GetSavePerfab(m_UITemplate);

        m_preUITemplate = PrefabUtility.CreatePrefab("Assets/" + c_TemplateResPath + l_templateName + ".prefab", saveprefab, ReplacePrefabOptions.ConnectToPrefab);

        GameObject.DestroyImmediate(saveprefab);
    }

    GameObject GetSavePerfab(GameObject targetPerfab)
    {
        GameObject goTmp = GameObject.Instantiate(targetPerfab);
        goTmp.name = goTmp.name.Replace("(Clone)", "");

        //除去接口的子节点，以免重复保存
        RemoveInterface(goTmp.transform);

        return goTmp;
    }

    void RemoveInterface(Transform node)
    {
        foreach (Transform nodeTmp in node)
        {
            if (nodeTmp.name.Contains(c_InterfaceName))
            {
                foreach (Transform no in nodeTmp)
                {
                    GameObject.DestroyImmediate(no.gameObject);
                }
            }
            else
            {
                RemoveInterface(nodeTmp);
            }
        }
    }

    //删除模板（包括数据）
    void DeleteTemlate(string l_TemplateName)
    {
        GameObject.DestroyImmediate(FindPrefabInTemplateRes(l_TemplateName), true);

        Debug.Log(Application.dataPath + "/" + c_TemplateResPath + l_TemplateName + ".prefab");

        File.Delete(Application.dataPath + "/" + c_TemplateResPath + l_TemplateName + ".prefab");
        File.Delete(Application.dataPath + "/" + c_TemplateResPath + l_TemplateName + ".prefab.meta");

        AssetDatabase.Refresh();
    }

    #region 接口

    bool b_isFoldInterface = false;
    bool b_isFoldCreateInterface = false;

    bool b_isFoldInterfaceList = false;
    public void InterfaceGUI()
    {
        EditorGUI.indentLevel = 2;
        b_isFoldInterface = EditorGUILayout.Foldout(b_isFoldInterface, "接口:");
        if (b_isFoldInterface)
        {
            m_interfaceList = GetInterfaceList(m_UITemplate);

            EditorGUI.indentLevel = 3;
            b_isFoldInterfaceList = EditorGUILayout.Foldout(b_isFoldInterfaceList, "接口列表:");
            if (b_isFoldInterfaceList)
            {
                EditorGUI.indentLevel = 4;
                InterfaceListGUI();
            }

            EditorGUI.indentLevel = 3;
            b_isFoldCreateInterface = EditorGUILayout.Foldout(b_isFoldCreateInterface, "创建接口:");
            if (b_isFoldCreateInterface)
            {
                EditorGUI.indentLevel = 4;
                newInterfaceName = EditorGUILayout.TextField("接口名：", newInterfaceName);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (IsReasonable(newInterfaceName)
                    && !IsContainsInterfaceName(newInterfaceName))
                {
                    if (GUILayout.Button("在当前节点添加一个接口", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
                    {
                        CreatOneInterface();
                        newInterfaceName = "";
                    }
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndHorizontal();
            }



            EditorGUILayout.Space();
        }
    }

    //创建一个接口
    void CreatOneInterface()
    {
        if (m_SelectedNode == null)
        {
            Debug.LogError("请先选中要添加接口的GameObject！");
            return;
        }
        GameObject go_newPort = new GameObject(c_InterfaceName + "_" + newInterfaceName);
        go_newPort.transform.parent = m_SelectedNode.transform;
        RectTransform rt = go_newPort.AddComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;

        rt.anchoredPosition3D = Vector3.zero;
        rt.sizeDelta = Vector2.zero;
        rt.localScale = Vector3.one;

    }

    void InterfaceListGUI()
    {
        for (int i = 0; i < m_interfaceList.Count;i++ )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(m_interfaceList[i].name.Split('_')[1] +":");
            EditorGUILayout.ObjectField(m_interfaceList[i],typeof(GameObject));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
    }

    #endregion
    #endregion

    #region 模版列表

    //是否折叠
    bool b_isFoldUseTemple      = false;
    bool b_isfoldDebugInfo      = false;

    string newUIName            = "";
    string newInterfaceName     = "";
    Vector2 scrollPos = new Vector2();
    //显示所有模板
    public void ShowAllTemple_GUI()
    {
        if (Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets).Length > 0)
        {
            m_SelectedNode = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets)[0] as GameObject;
        }
        else
        {
            m_SelectedNode = null;
        }

        EditorGUI.indentLevel = 1;
        b_isFoldUseTemple = EditorGUILayout.Foldout((b_isFoldUseTemple), "模板列表:");
        if (b_isFoldUseTemple)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos,GUILayout.ExpandHeight(false));

            for (int i = 0; i < m_allTemplateName.Count; i++)
            {
                EditorGUI.indentLevel = 2;
                bool b_isFoldTmp = EditorGUILayout.Foldout(GetListIsFold(i), m_allTemplateName[i] + ":");
                SetListIsFold(i,b_isFoldTmp);
                if (GetListIsFold(i))
                {
                    SingleTemplateInfo(m_allTemplateName[i]);
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
        }

    }

    int currentIndex = -1;
    bool GetListIsFold(int index)
    {
        return (currentIndex == index);
    }

    void SetListIsFold(int index,bool isFold)
    {
        if (isFold == true)
        {
            currentIndex = index;
        }

        if (isFold == false
            && index == currentIndex)
        {
            currentIndex = -1;
        }
    }

    void SingleTemplateInfo(string templateName)
    {
        EditorGUI.indentLevel = 3;

        
        EditorGUILayout.ObjectField("模板预设：",FindPrefabInTemplateRes(templateName), typeof(GameObject));
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("创建此模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
        {
            CreatUIByOneTemplate(templateName);
        }
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("应用此模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
        {
            ApplyOneTemplate(templateName);
        }
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("删除此模板", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_large)))
        {
            if (EditorUtility.DisplayDialog("警告", "该操作不可逆，确定删除该模板？", "是", "否"))
            {
                DeleteTemlate(templateName);
            }
        }
        EditorGUILayout.Space();
        GUILayout.EndHorizontal();
    }

    //当前选中的节点,将成为新UI的父节点
    GameObject go_SelectedObj;

    //用模板创建UI
    void CreatUIByOneTemplate(string templateName)
    {
        GameObject go_newUI = GameObject.Instantiate(FindPrefabInTemplateRes(templateName));
        if (m_SelectedNode != null)
        {
            go_newUI.transform.SetParent( m_SelectedNode.transform);
        }

        go_newUI.name = templateName;

        RectTransform rt = go_newUI.GetComponent<RectTransform>();

        rt.anchoredPosition3D = Vector3.zero;
        rt.sizeDelta          = Vector2.zero;
        rt.localScale         = Vector3.one;

        m_UITemplate       = go_newUI;
        m_preUITemplate      = FindPrefabInTemplateRes(templateName);
 
    }


#endregion

    #region 模板替换部分

    public void ApplyAllUITemplate()
    {
        foreach (var oneTemplateName in m_allTemplateName)
        {
            ApplyOneTemplate(oneTemplateName);
        }
    }

    //应用一个模板到全局
    void ApplyOneTemplate(string templateName)
    {
        List<GameObject> allUsedUI = FindHadOneTemplateUI(templateName);

        foreach (var oneUI in allUsedUI)
        {
            Debug.Log(oneUI.name);


            ReplaceOneUI(oneUI, FindPrefabInTemplateRes(templateName));
        }
    }

    //替换单个UI
    void ReplaceOneUI(GameObject l_oldUIprefab, GameObject l_newTemplate)
    {
        GameObject UITmp = GameObject.Instantiate(l_oldUIprefab);

        RecursionNodeToReplaceTemplate(UITmp, l_newTemplate);

        PrefabUtility.ReplacePrefab(UITmp, l_oldUIprefab);
        GameObject.DestroyImmediate(UITmp, true);

    }

    //遍历所有子节点去替换模板
    void RecursionNodeToReplaceTemplate(GameObject l_oldNode, GameObject l_newTemplate)
    {
        foreach (Transform node in l_oldNode.transform)
        {
            RecursionNodeToReplaceTemplate(node.gameObject, l_newTemplate);

            if (node.name == l_newTemplate.name)
            {
                ReplaceTemplate(l_oldNode.transform, node.gameObject, l_newTemplate);
            }
            //else
            //{
                
            //}
        }
    }

    struct InterfaceReplaceStruct
    {
        public GameObject go;
        public int index;
    }

    Dictionary<string, InterfaceReplaceStruct> m_interfaceTmp;
    void ReplaceTemplate(Transform l_parent,GameObject l_oldTemplate, GameObject l_newTemplate)
    {
        m_interfaceTmp = new Dictionary<string, InterfaceReplaceStruct>();

        //先把旧UI下的接口移出来，存入表中
        RecursionNodeToReplaceInterfaceStepOne(l_oldTemplate.transform);

        //再把放入新模板的对应接口下

            //删除旧模板
            GameObject.DestroyImmediate(l_oldTemplate);

            //创建新模板
            GameObject l_templateTmp = GameObject.Instantiate(l_newTemplate);

            l_templateTmp.name = l_templateTmp.name.Replace("(Clone)","");
            l_templateTmp.transform.SetParent(l_parent);


        //最后换上接口
        RecursionNodeToReplaceInterfaceStepTwo(l_templateTmp.transform);
    }

    void RecursionNodeToReplaceInterfaceStepOne(Transform l_parent)
    {
        foreach (Transform node in l_parent)
        {
            if(node.name.Contains(c_InterfaceName))
            {
                if (!m_interfaceTmp.ContainsKey(node.name))
                {
                    InterfaceReplaceStruct tmp = new InterfaceReplaceStruct();
                    tmp.go = node.gameObject;
                    tmp.index = node.transform.GetSiblingIndex();

                    m_interfaceTmp.Add(node.name, tmp);
                }
                else
                {
                    Debug.LogError(node.root.name + " : " + node.name + " 接口有重名！", node.gameObject);
                }
                node.SetParent(null);
            }
            else
            {
                RecursionNodeToReplaceInterfaceStepOne(node);
            }
            
        }
    }

    void RecursionNodeToReplaceInterfaceStepTwo(Transform l_parent)
    {
        foreach (Transform node in l_parent)
        {
            if (m_interfaceTmp.ContainsKey(node.name))
            {
                InterfaceReplaceStruct tmp = m_interfaceTmp[node.name];

                m_interfaceTmp.Remove(node.name);
                GameObject.DestroyImmediate(node.gameObject);

                tmp.go.transform.SetParent(l_parent);
                tmp.go.transform.SetSiblingIndex(tmp.index);
            }
            else
            {
                RecursionNodeToReplaceInterfaceStepTwo(node);
            }
        }
    }


    //获取应用某个模板的所有UI预设
    List<GameObject> FindHadOneTemplateUI(string templateName)
    {
        List<GameObject> findedUI = new List<GameObject>();
        string[] prefabNameSplit;
        foreach (var item in UIEditorWindow.allUIPrefab.Keys)
        {
            Debug.Log(item);
            if (UIEditorWindow.allUIPrefab[item] != null &&
                UIEditorWindow.allUIPrefab[item].name != "UIManager")
            {
                findedUI.Add(UIEditorWindow.allUIPrefab[item]);
            }
        }

        return findedUI;

    }
    #endregion

    #region 封装过的方法
    //获取Prefab
    GameObject FindPrefabInTemplateRes(string name)
    {
        name = "Assets/" + c_TemplateResPath + name;
        name += ".prefab";

        //Debug.Log(name);
        return AssetDatabase.LoadAssetAtPath(name, typeof(GameObject)) as GameObject;
    }

    //该模板是否已经存在
    bool HaveTheTemlate(string name)
    {
        return m_allTemplateName.Contains(name);
    }

    //获取一个选中的接口
    GameObject GetTemlateParant(GameObject l_node)
    {
        if (l_node.name.Contains(c_TemplateName))
        {
            return l_node;
        }
        else
        {
            if(l_node.transform.parent == null)
            {
                return null;
            }
            else
            {
                return GetTemlateParant(l_node.transform.parent.gameObject);
            }
        }
    }

    string GetTemplateName(string nodeName)
    {
        return nodeName;
        //return nodeName.Split('_')[1];
    }

    /// <summary>
    /// 获取接口列表
    /// </summary>
    /// <param name="l_node"></param>
    /// <returns></returns>
    List<GameObject> GetInterfaceList(GameObject l_node)
    {
        List<GameObject> result = new List<GameObject>();

        RecursionFindInterface(result, l_node.transform);

        return result;
    }

    void RecursionFindInterface(List<GameObject> interfaceList,Transform node)
    {
        foreach (Transform tr in node)
        {
            if (tr.name.Contains(c_InterfaceName))
            {
                interfaceList.Add(tr.gameObject);
            }

            RecursionFindInterface(interfaceList, tr);
        }
    }

    bool IsContainsInterfaceName(string newName)
    {
        foreach (GameObject tr in m_interfaceList)
        {
            if (tr.name == c_InterfaceName + "_" + newName)
            {
                return true;
            }
        }

        return false;
    }

    void FindAllUITemplate()
    {
        m_allTemplateName = new List<string>();
        FindTemplateName(Application.dataPath +"/"+ c_TemplateResPath);
    }

    public void FindTemplateName(string path)
    {
        if(Directory.Exists(path))
        {
            string[] allUIPrefabName = Directory.GetFiles(path);
            foreach (var item in allUIPrefabName)
            {
                if (item.EndsWith(".prefab"))
                {
                    string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                    m_allTemplateName.Add(configName);
                }
            }
        }
    }


    #endregion

}
