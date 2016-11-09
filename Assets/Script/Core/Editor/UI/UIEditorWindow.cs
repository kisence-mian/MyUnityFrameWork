using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
public class UIEditorWindow : EditorWindow
{
    UILayerManager m_UILayerManager;

    [MenuItem("Window/UI编辑器工具")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UIEditorWindow));
    }


    void OnEnable()
    {
        EditorGUIStyleData.Init();
        GameObject uiManager = GameObject.Find("UIManager");

        if(uiManager)
        {
            m_UILayerManager = uiManager.GetComponent<UILayerManager>();
        }

        m_styleManager.OnEnable();
        m_UItemplate.OnEnable();

        FindAllUI();
    }

    void OnGUI()
    {
        titleContent.text = "UI编辑器";

        EditorGUILayout.BeginVertical();

        UIManagerGUI();

        CreateUIGUI();

        UITemplate();

        UIStyleGUI();

        EditorGUILayout.EndVertical();
    }

    void OnSelectionChange()
    {
        if (m_UItemplate != null)
            m_UItemplate.SelectCurrentTemplate();

        base.Repaint();
    }

    //当工程改变时
    void OnProjectChange()
    {
        FindAllUI();
        m_UItemplate.OnProjectChange();
    }

    #region UIManager

    bool isFoldUImanager = false;
    public Vector2 m_referenceResolution = new Vector2(960, 640);
    public CanvasScaler.ScreenMatchMode m_MatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

    public bool m_isOnlyUICamera = false;
    public bool m_isVertical = false;

    void UIManagerGUI()
    {
        EditorGUI.indentLevel = 0;
        isFoldUImanager = EditorGUILayout.Foldout( isFoldUImanager,"UIManager:");
        if (isFoldUImanager)
        {
            EditorGUI.indentLevel = 1;
            m_referenceResolution = EditorGUILayout.Vector2Field("参考分辨率", m_referenceResolution);
            m_isOnlyUICamera = EditorGUILayout.Toggle("只有一个UI摄像机", m_isOnlyUICamera);
            m_isVertical     = EditorGUILayout.Toggle("是否竖屏", m_isVertical);

            if (GUILayout.Button("创建UIManager"))
            {
                UICreateService.CreatUIManager(m_referenceResolution, m_MatchMode, m_isOnlyUICamera, m_isVertical);
            }
        }
    }

    #endregion

    #region createUI

    bool isAutoCreatePrefab = true;
    bool isFoldCreateUI = false;
    string m_UIname = "";
    UIType m_UIType = UIType.Normal;

    void CreateUIGUI()
    {
        EditorGUI.indentLevel = 0;
        isFoldCreateUI = EditorGUILayout.Foldout(isFoldCreateUI, "创建UI:");
        if (isFoldCreateUI)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("提示： 脚本和 UI 名称会自动添加Window后缀");
            m_UIname = EditorGUILayout.TextField("UI Name:", m_UIname);
            m_UIType = (UIType)EditorGUILayout.EnumPopup("UI Type:", m_UIType);
            isAutoCreatePrefab = EditorGUILayout.Toggle("自动生成 Prefab",isAutoCreatePrefab);
            if (m_UIname != "")
            {
                string l_nameTmp = m_UIname + "Window";
                Type l_typeTmp = EditorTool.GetType(l_nameTmp);
                if (l_typeTmp != null)
                {
                    if(l_typeTmp.BaseType.Equals(typeof(UIWindowBase)))
                    {
                        if (GUILayout.Button("创建UI"))
                        {
                            UICreateService.CreatUI(l_nameTmp, m_UIType,m_UILayerManager,isAutoCreatePrefab);
                            m_UIname = "";
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("该类没有继承UIWindowBase");
                    }
                }
                else
                {
                    if (GUILayout.Button("创建UI脚本"))
                    {
                        UICreateService.CreatUIScript(l_nameTmp);
                    }
                }
            }
        }
    }
    #endregion

    #region UITemplate
    UITemplate m_UItemplate = new UITemplate();
    bool isFoldUITemplate = false;
    void UITemplate()
    {
        EditorGUI.indentLevel = 0;
        isFoldUITemplate = EditorGUILayout.Foldout(isFoldUITemplate, "UI模板:");
        if (isFoldUITemplate)
        {
            m_UItemplate.GUI();
        }


    }

    #endregion

    #region UIStyle

    UIStyleManager m_styleManager = new UIStyleManager();

    bool isFoldUIStyle = false;
    void UIStyleGUI()
    {
        EditorGUI.indentLevel = 0;
        isFoldUIStyle = EditorGUILayout.Foldout(isFoldUIStyle, "UIStyle:");
        if (isFoldUIStyle)
        {
            m_styleManager.GUI();
        }
    }


    #endregion

    #region UI

    //所有UI预设
    public static Dictionary<string, GameObject> allUIPrefab;


    /// <summary>
    /// 获取到所有的UIprefab
    /// </summary>
    public void FindAllUI()
    {
        allUIPrefab = new Dictionary<string, GameObject>();
        FindAllUIResources(Application.dataPath + "/" + "Resources/UI");
    }

    //读取“Resources/UI”目录下所有的UI预设
    public void FindAllUIResources(string path)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            string oneUIPrefabName = FileTool.GetFileNameByPath(item);
            if (item.EndsWith(".prefab"))
            {
                string oneUIPrefabPsth = path + "/" + oneUIPrefabName;
                allUIPrefab.Add(oneUIPrefabName, AssetDatabase.LoadAssetAtPath("Assets/" + oneUIPrefabPsth, typeof(GameObject)) as GameObject);
            }
        }

        string[] dires = Directory.GetDirectories(path);

        for (int i = 0; i < dires.Length; i++)
        {
            FindAllUIResources(dires[i]);
        }
    }

    #endregion
}


