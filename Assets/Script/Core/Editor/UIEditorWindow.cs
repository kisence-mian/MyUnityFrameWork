using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
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
        UIManager l_UIManager = l_UIManagerGo.AddComponent<UIManager>();

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
            m_UIname = EditorGUILayout.TextField("UIname:", m_UIname);
            m_UIType = (UIType)EditorGUILayout.EnumPopup("UIType:", m_UIType);

            if (GUILayout.Button("创建UI"))
            {
                if (m_UIname != "")
                {
                    CreatUI(m_UIname, m_UIType);
                    m_UIname = "";
                }
                else
                {
                    EditorUtility.DisplayDialog("错误","UI名不能为空！","好的");
                }
            }
        }
    }

    void CreatUI(string l_UIWindowName, UIType l_UIType)
    {
        GameObject l_uiGo = new GameObject(l_UIWindowName);
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


        if(m_UILayerManager)
        {
            m_UILayerManager.SetLayer(new UIWindowBase());
        }
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
            if (GUILayout.Button("创建UI Style"))
            {
                CreateUIStyle();
            }
        }
    }


    void CreateUIStyle()
    {

    }
    #endregion
}
