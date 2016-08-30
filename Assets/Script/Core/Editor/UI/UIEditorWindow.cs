using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
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
        GameObject uiManager = GameObject.Find("UIManager");

        if(uiManager)
        {
            m_UILayerManager = uiManager.GetComponent<UILayerManager>();
        }

        AnalysisStyleData();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        UIManagerGUI();

        CreateUIGUI();

        UIStyleGUI();

        EditorGUILayout.EndVertical();
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
                CreatUIManager();
            }
        }
    }

    void CreatUIManager()
    {
        //UIManager
        GameObject l_UIManagerGo = new GameObject("UIManager");
        l_UIManagerGo.layer = LayerMask.NameToLayer("UI");
        //UIManager l_UIManager = l_UIManagerGo.AddComponent<UIManager>();
        l_UIManagerGo.AddComponent<UIManager>();

        //UIcamera
        GameObject l_cameraGo = new GameObject("UICamera");
        l_cameraGo.transform.SetParent(l_UIManagerGo.transform);
        Camera l_camera = l_cameraGo.AddComponent<Camera>();
        l_camera.cullingMask = LayerMask.GetMask("UI");
        l_camera.orthographic = true;
        if (!m_isOnlyUICamera)
        {
            l_camera.clearFlags = CameraClearFlags.Depth;
            l_camera.depth = 1;
        }
        else
        {
            l_camera.clearFlags = CameraClearFlags.SolidColor;
            l_camera.backgroundColor = Color.black;
        }

        //Canvas
        Canvas canvas = l_UIManagerGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = l_camera;

        //UI Raycaster
        //GraphicRaycaster l_Graphic = l_UIManagerGo.AddComponent<GraphicRaycaster>();
        l_UIManagerGo.AddComponent<GraphicRaycaster>();

        //CanvasScaler
        CanvasScaler l_scaler = l_UIManagerGo.AddComponent<CanvasScaler>();
        l_scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        l_scaler.referenceResolution = m_referenceResolution;
        l_scaler.screenMatchMode = m_MatchMode;

        if(m_isVertical)
        {
            l_scaler.matchWidthOrHeight = 1;
        }
        else
        {
            l_scaler.matchWidthOrHeight = 0;
        }

        //挂载点
        GameObject l_goTmp = null;
        RectTransform l_rtTmp = null;
        UILayerManager l_layerTmp = l_UIManagerGo.GetComponent<UILayerManager>();

        l_goTmp = new GameObject("GameUI");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_GameUILayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("Fixed");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_FixedLayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("Normal");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_NormalLayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("TopBar");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_TopbarLayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("PopUp");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_PopUpLayerParent = l_goTmp.transform;
        m_UILayerManager = l_layerTmp;

        ProjectWindowUtil.ShowCreatedAsset(l_UIManagerGo);

        string Path = "Resources/UI/UIManager.prefab";
        FileTool.CreatFilePath(Application.dataPath +"/"+ Path);
        PrefabUtility.CreatePrefab("Assets/" + Path, l_UIManagerGo, ReplacePrefabOptions.ConnectToPrefab);
        
    }

    #endregion

    #region createUI

    bool isFoldCreateUI = false;
    string m_UIname = "";
    UIType m_UIType = UIType.Normal;

    void CreateUIGUI()
    {
        EditorGUI.indentLevel = 0;
        isFoldCreateUI = EditorGUILayout.Foldout(isFoldCreateUI, "CreateUI:");
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
                            CreatUI(l_nameTmp, m_UIType);
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
                        CreatUIScript(l_nameTmp);
                    }
                }
            }
        }
    }

    bool isAutoCreatePrefab = true;
    void CreatUI(string l_UIWindowName, UIType l_UIType)
    {
        GameObject l_uiGo = new GameObject(l_UIWindowName);

        Type type = EditorTool.GetType(l_UIWindowName);
        UIWindowBase l_uiBaseTmp = l_uiGo.AddComponent(type) as UIWindowBase;

        l_uiBaseTmp.m_UIType = l_UIType;

        l_uiGo.AddComponent<Canvas>();
        l_uiGo.AddComponent<GraphicRaycaster>();

        RectTransform l_ui = l_uiGo.GetComponent<RectTransform>();
        l_ui.sizeDelta = Vector2.zero;
        l_ui.anchorMin = Vector2.zero;
        l_ui.anchorMax = Vector2.one;

        GameObject l_BgGo = new GameObject("BG");
        RectTransform l_Bg = l_BgGo.AddComponent<RectTransform>();
        l_Bg.SetParent(l_ui);
        l_Bg.sizeDelta = Vector2.zero;
        l_Bg.anchorMin = Vector2.zero;
        l_Bg.anchorMax = Vector2.one;

        GameObject l_rootGo = new GameObject("root");
        RectTransform l_root = l_rootGo.AddComponent<RectTransform>();
        l_root.SetParent(l_ui);
        l_root.sizeDelta = Vector2.zero;
        l_root.anchorMin = Vector2.zero;
        l_root.anchorMax = Vector2.one;

        l_uiBaseTmp.m_bgMask = l_BgGo;
        l_uiBaseTmp.m_uiRoot = l_rootGo;

        if(m_UILayerManager)
        {
            m_UILayerManager.SetLayer(l_uiBaseTmp);
        }

        if (isAutoCreatePrefab)
        {
            string Path = "Resources/UI/" + l_UIWindowName + "/" + l_UIWindowName + ".prefab";
            FileTool.CreatFilePath(Application.dataPath + "/" + Path);
            PrefabUtility.CreatePrefab("Assets/" + Path, l_uiGo, ReplacePrefabOptions.ConnectToPrefab);
        }

        ProjectWindowUtil.ShowCreatedAsset(l_uiGo);
    }

    void CreatUIScript(string l_UIWindowName)
    {
        string LoadPath = Application.dataPath + "/Script/Core/Editor/res/UIWindowClassTemplate.txt";
        string SavePath = Application.dataPath + "/Script/UI/" + l_UIWindowName + "/" + l_UIWindowName + ".cs";

        string l_UItemplate = ResourceIOTool.ReadStringByFile(LoadPath);
        string l_classContent = l_UItemplate.Replace("{0}", l_UIWindowName);

        ResourceIOTool.WriteStringByFile(SavePath , l_classContent);

        AssetDatabase.Refresh();
    }
    #endregion

    #region UIStyle

    bool isFoldUIStyle = false;
    void UIStyleGUI()
    {
        EditorGUI.indentLevel = 0;
        isFoldUIStyle = EditorGUILayout.Foldout(isFoldUIStyle, "UIStyle:");
        if (isFoldUIStyle)
        {
            EditorGUI.indentLevel = 1;
            if (GUILayout.Button("以当前UI 生成Style"))
            {
                CreateUIStyle();
            }
        }
    }

    void ShowStyleGUI()
    {
        //for()
        //{

        //}
    }


    void CreateUIStyle()
    {

    }

    public void AnalysisStyleData()
    {

    }


    #endregion
}


