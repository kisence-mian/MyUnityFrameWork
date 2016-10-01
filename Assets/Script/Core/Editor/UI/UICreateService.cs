using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;

public class UICreateService 
{

    public static void CreatUIManager(Vector2 l_referenceResolution, CanvasScaler.ScreenMatchMode l_MatchMode, bool l_isOnlyUICamera, bool l_isVertical)
    {
        //UIManager
        GameObject l_UIManagerGo = new GameObject("UIManager");
        l_UIManagerGo.layer = LayerMask.NameToLayer("UI");
        //UIManager l_UIManager = l_UIManagerGo.AddComponent<UIManager>();
        l_UIManagerGo.AddComponent<UIManager>();

        //UIcamera
        GameObject l_cameraGo = new GameObject("UICamera");
        l_cameraGo.transform.SetParent(l_UIManagerGo.transform);
        l_cameraGo.transform.localPosition = new Vector3(0, 0, -1000);
        Camera l_camera = l_cameraGo.AddComponent<Camera>();
        l_camera.cullingMask = LayerMask.GetMask("UI");
        l_camera.orthographic = true;
        if (!l_isOnlyUICamera)
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
        l_scaler.referenceResolution = l_referenceResolution;
        l_scaler.screenMatchMode = l_MatchMode;

        if (l_isVertical)
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
        l_goTmp.layer = LayerMask.NameToLayer("UI");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_goTmp.transform.localScale = Vector3.one;
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_GameUILayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("Fixed");
        l_goTmp.layer = LayerMask.NameToLayer("UI");
        l_goTmp.transform.localScale = Vector3.one;
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_FixedLayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("Normal");
        l_goTmp.layer = LayerMask.NameToLayer("UI");
        l_goTmp.transform.localScale = Vector3.one;
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_NormalLayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("TopBar");
        l_goTmp.layer = LayerMask.NameToLayer("UI");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_goTmp.transform.localScale = Vector3.one;
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_TopbarLayerParent = l_goTmp.transform;

        l_goTmp = new GameObject("PopUp");
        l_goTmp.layer = LayerMask.NameToLayer("UI");
        l_goTmp.transform.SetParent(l_UIManagerGo.transform);
        l_goTmp.transform.localScale = Vector3.one;
        l_rtTmp = l_goTmp.AddComponent<RectTransform>();
        l_rtTmp.anchorMax = new Vector2(1, 1);
        l_rtTmp.anchorMin = new Vector2(0, 0);
        l_rtTmp.anchoredPosition3D = Vector3.zero;
        l_rtTmp.sizeDelta = Vector2.zero;
        l_layerTmp.m_PopUpLayerParent = l_goTmp.transform;
        //m_UILayerManager = l_layerTmp;

        ProjectWindowUtil.ShowCreatedAsset(l_UIManagerGo);

        string Path = "Resources/UI/UIManager.prefab";
        FileTool.CreatFilePath(Application.dataPath + "/" + Path);
        PrefabUtility.CreatePrefab("Assets/" + Path, l_UIManagerGo, ReplacePrefabOptions.ConnectToPrefab);

    }

    public static void CreatUI(string l_UIWindowName, UIType l_UIType,UILayerManager l_UILayerManager,bool l_isAutoCreatePrefab)
    {
        GameObject l_uiGo = new GameObject(l_UIWindowName);

        Type type = EditorTool.GetType(l_UIWindowName);
        UIWindowBase l_uiBaseTmp = l_uiGo.AddComponent(type) as UIWindowBase;

        l_uiGo.layer = LayerMask.NameToLayer("UI");

        l_uiBaseTmp.m_UIType = l_UIType;

        l_uiGo.AddComponent<Canvas>();
        l_uiGo.AddComponent<GraphicRaycaster>();

        RectTransform l_ui = l_uiGo.GetComponent<RectTransform>();
        l_ui.sizeDelta = Vector2.zero;
        l_ui.anchorMin = Vector2.zero;
        l_ui.anchorMax = Vector2.one;

        GameObject l_BgGo = new GameObject("BG");

        l_BgGo.layer = LayerMask.NameToLayer("UI");
        RectTransform l_Bg = l_BgGo.AddComponent<RectTransform>();
        l_Bg.SetParent(l_ui);
        l_Bg.sizeDelta = Vector2.zero;
        l_Bg.anchorMin = Vector2.zero;
        l_Bg.anchorMax = Vector2.one;

        GameObject l_rootGo = new GameObject("root");
        l_rootGo.layer = LayerMask.NameToLayer("UI");
        RectTransform l_root = l_rootGo.AddComponent<RectTransform>();
        l_root.SetParent(l_ui);
        l_root.sizeDelta = Vector2.zero;
        l_root.anchorMin = Vector2.zero;
        l_root.anchorMax = Vector2.one;

        l_uiBaseTmp.m_bgMask = l_BgGo;
        l_uiBaseTmp.m_uiRoot = l_rootGo;

        if (l_UILayerManager)
        {
            l_UILayerManager.SetLayer(l_uiBaseTmp);
        }

        if (l_isAutoCreatePrefab)
        {
            string Path = "Resources/UI/" + l_UIWindowName + "/" + l_UIWindowName + ".prefab";
            FileTool.CreatFilePath(Application.dataPath + "/" + Path);
            PrefabUtility.CreatePrefab("Assets/" + Path, l_uiGo, ReplacePrefabOptions.ConnectToPrefab);
        }

        ProjectWindowUtil.ShowCreatedAsset(l_uiGo);
    }

    public static void CreatUIScript(string l_UIWindowName)
    {
        string LoadPath = Application.dataPath + "/Script/Core/Editor/res/UIWindowClassTemplate.txt";
        string SavePath = Application.dataPath + "/Script/UI/" + l_UIWindowName + "/" + l_UIWindowName + ".cs";

        string l_UItemplate = ResourceIOTool.ReadStringByFile(LoadPath);
        string l_classContent = l_UItemplate.Replace("{0}", l_UIWindowName);

        ResourceIOTool.WriteStringByFile(SavePath, l_classContent);

        AssetDatabase.Refresh();
    }
}
