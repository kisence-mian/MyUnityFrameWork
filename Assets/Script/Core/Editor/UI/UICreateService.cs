using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using FrameWork.GuideSystem;

public class UICreateService 
{

    public static void CreatUIManager(Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
    {
        //UIManager
        GameObject UIManagerGo = new GameObject("UIManager");
        UIManagerGo.layer = LayerMask.NameToLayer("UI");
        //UIManager UIManager = UIManagerGo.AddComponent<UIManager>();
        UIManagerGo.AddComponent<UIManager>();

        //UIcamera
        GameObject cameraGo = new GameObject("UICamera");
        cameraGo.transform.SetParent(UIManagerGo.transform);
        cameraGo.transform.localPosition = new Vector3(0, 0, -1000);
        Camera camera = cameraGo.AddComponent<Camera>();
        camera.cullingMask = LayerMask.GetMask("UI");
        camera.orthographic = true;
        if (!isOnlyUICamera)
        {
            camera.clearFlags = CameraClearFlags.Depth;
            camera.depth = 1;
        }
        else
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
        }

        //Canvas
        Canvas canvas = UIManagerGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;

        //UI Raycaster
        //GraphicRaycaster Graphic = UIManagerGo.AddComponent<GraphicRaycaster>();
        UIManagerGo.AddComponent<GraphicRaycaster>();

        //CanvasScaler
        CanvasScaler scaler = UIManagerGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = MatchMode;

        if (isVertical)
        {
            scaler.matchWidthOrHeight = 1;
        }
        else
        {
            scaler.matchWidthOrHeight = 0;
        }

        //挂载点
        GameObject goTmp = null;
        RectTransform rtTmp = null;
        UILayerManager layerTmp = UIManagerGo.GetComponent<UILayerManager>();

        goTmp = new GameObject("GameUI");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(UIManagerGo.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        layerTmp.m_GameUILayerParent = goTmp.transform;

        goTmp = new GameObject("Fixed");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(UIManagerGo.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        layerTmp.m_FixedLayerParent = goTmp.transform;

        goTmp = new GameObject("Normal");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(UIManagerGo.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        layerTmp.m_NormalLayerParent = goTmp.transform;

        goTmp = new GameObject("TopBar");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(UIManagerGo.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        layerTmp.m_TopbarLayerParent = goTmp.transform;

        goTmp = new GameObject("PopUp");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(UIManagerGo.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        layerTmp.m_PopUpLayerParent = goTmp.transform;
        //m_UILayerManager = layerTmp;

        ProjectWindowUtil.ShowCreatedAsset(UIManagerGo);

        string Path = "Resources/UI/UIManager.prefab";
        FileTool.CreatFilePath(Application.dataPath + "/" + Path);
        PrefabUtility.CreatePrefab("Assets/" + Path, UIManagerGo, ReplacePrefabOptions.ConnectToPrefab);

    }

    public static void CreatUI(string UIWindowName, UIType UIType,UILayerManager UILayerManager,bool isAutoCreatePrefab)
    {
        GameObject uiGo = new GameObject(UIWindowName);

        Type type = EditorTool.GetType(UIWindowName);
        UIWindowBase uiBaseTmp = uiGo.AddComponent(type) as UIWindowBase;

        uiGo.layer = LayerMask.NameToLayer("UI");

        uiBaseTmp.m_UIType = UIType;

        uiGo.AddComponent<Canvas>();
        uiGo.AddComponent<GraphicRaycaster>();

        RectTransform ui = uiGo.GetComponent<RectTransform>();
        ui.sizeDelta = Vector2.zero;
        ui.anchorMin = Vector2.zero;
        ui.anchorMax = Vector2.one;

        GameObject BgGo = new GameObject("BG");

        BgGo.layer = LayerMask.NameToLayer("UI");
        RectTransform Bg = BgGo.AddComponent<RectTransform>();
        Bg.SetParent(ui);
        Bg.sizeDelta = Vector2.zero;
        Bg.anchorMin = Vector2.zero;
        Bg.anchorMax = Vector2.one;

        GameObject rootGo = new GameObject("root");
        rootGo.layer = LayerMask.NameToLayer("UI");
        RectTransform root = rootGo.AddComponent<RectTransform>();
        root.SetParent(ui);
        root.sizeDelta = Vector2.zero;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;

        uiBaseTmp.m_bgMask = BgGo;
        uiBaseTmp.m_uiRoot = rootGo;

        if (UILayerManager)
        {
            UILayerManager.SetLayer(uiBaseTmp);
        }

        if (isAutoCreatePrefab)
        {
            string Path = "Resources/UI/" + UIWindowName + "/" + UIWindowName + ".prefab";
            FileTool.CreatFilePath(Application.dataPath + "/" + Path);
            PrefabUtility.CreatePrefab("Assets/" + Path, uiGo, ReplacePrefabOptions.ConnectToPrefab);
        }

        ProjectWindowUtil.ShowCreatedAsset(uiGo);
    }

    public static void CreatUIbyLua(string UIWindowName, UIType UIType, UILayerManager UILayerManager, bool isAutoCreatePrefab)
    {
        GameObject uiGo = new GameObject(UIWindowName);

        UIWindowLuaHelper uiBaseTmp = uiGo.AddComponent<UIWindowLuaHelper>();

        uiGo.layer = LayerMask.NameToLayer("UI");

        uiBaseTmp.m_UIType = UIType;

        uiGo.AddComponent<Canvas>();
        uiGo.AddComponent<GraphicRaycaster>();

        RectTransform ui = uiGo.GetComponent<RectTransform>();
        ui.sizeDelta = Vector2.zero;
        ui.anchorMin = Vector2.zero;
        ui.anchorMax = Vector2.one;

        GameObject BgGo = new GameObject("BG");

        BgGo.layer = LayerMask.NameToLayer("UI");
        RectTransform Bg = BgGo.AddComponent<RectTransform>();
        Bg.SetParent(ui);
        Bg.sizeDelta = Vector2.zero;
        Bg.anchorMin = Vector2.zero;
        Bg.anchorMax = Vector2.one;

        GameObject rootGo = new GameObject("root");
        rootGo.layer = LayerMask.NameToLayer("UI");
        RectTransform root = rootGo.AddComponent<RectTransform>();
        root.SetParent(ui);
        root.sizeDelta = Vector2.zero;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;

        uiBaseTmp.m_bgMask = BgGo;
        uiBaseTmp.m_uiRoot = rootGo;

        if (UILayerManager)
        {
            UILayerManager.SetLayer(uiBaseTmp);
        }

        if (isAutoCreatePrefab)
        {
            string Path = "Resources/UI/" + UIWindowName + "/" + UIWindowName + ".prefab";
            FileTool.CreatFilePath(Application.dataPath + "/" + Path);
            PrefabUtility.CreatePrefab("Assets/" + Path, uiGo, ReplacePrefabOptions.ConnectToPrefab);
        }

        ProjectWindowUtil.ShowCreatedAsset(uiGo);
    }

    public static void CreatUIScript(string UIWindowName)
    {
        string LoadPath = Application.dataPath + "/Script/Core/Editor/res/UIWindowClassTemplate.txt";
        string SavePath = Application.dataPath + "/Script/UI/" + UIWindowName + "/" + UIWindowName + ".cs";

        string UItemplate = ResourceIOTool.ReadStringByFile(LoadPath);
        string classContent = UItemplate.Replace("{0}", UIWindowName);

        EditorUtil.WriteStringByFile(SavePath, classContent);

        AssetDatabase.Refresh();
    }

    public static void CreatUILuaScript(string UIWindowName)
    {
        string LoadPath = Application.dataPath + "/Script/Core/Editor/res/UILuaScriptTemplate.txt";
        string SavePath = Application.dataPath + "/Resources/Lua/UI/Lua" + UIWindowName + ".txt";

        string UItemplate = ResourceIOTool.ReadStringByFile(LoadPath);
        string classContent = UItemplate.Replace("{0}", UIWindowName);

        EditorUtil.WriteStringByFile(SavePath, classContent);

        AssetDatabase.Refresh();
    }

    public static void CreateGuideWindow()
    {
        string UIWindowName = "GuideWindow";
        UIType UIType = UIType.TopBar;

        GameObject uiGo = new GameObject(UIWindowName);

        Type type = EditorTool.GetType(UIWindowName);
        GuideWindowBase guideBaseTmp = uiGo.AddComponent(type) as GuideWindowBase;

        uiGo.layer = LayerMask.NameToLayer("UI");

        guideBaseTmp.m_UIType = UIType;

        Canvas can = uiGo.AddComponent<Canvas>();
        uiGo.AddComponent<GraphicRaycaster>();

        can.overrideSorting = true;
        can.sortingLayerName = "Guide";

        RectTransform ui = uiGo.GetComponent<RectTransform>();
        ui.sizeDelta = Vector2.zero;
        ui.anchorMin = Vector2.zero;
        ui.anchorMax = Vector2.one;

        GameObject BgGo = new GameObject("BG");

        BgGo.layer = LayerMask.NameToLayer("UI");
        RectTransform Bg = BgGo.AddComponent<RectTransform>();
        Bg.SetParent(ui);
        Bg.sizeDelta = Vector2.zero;
        Bg.anchorMin = Vector2.zero;
        Bg.anchorMax = Vector2.one;

        GameObject rootGo = new GameObject("root");
        rootGo.layer = LayerMask.NameToLayer("UI");
        RectTransform root = rootGo.AddComponent<RectTransform>();
        root.SetParent(ui);
        root.sizeDelta = Vector2.zero;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;

        GameObject mask = new GameObject("mask");
        mask.layer = LayerMask.NameToLayer("UI");
        RectTransform maskrt = mask.AddComponent<RectTransform>();
        Image img = mask.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.75f);
        maskrt.SetParent(root);
        maskrt.sizeDelta = Vector2.zero;
        maskrt.anchorMin = Vector2.zero;
        maskrt.anchorMax = Vector2.one;

        guideBaseTmp.m_mask = img;

        guideBaseTmp.m_bgMask = BgGo;
        guideBaseTmp.m_uiRoot = rootGo;

        string Path = "Resources/UI/" + UIWindowName + "/" + UIWindowName + ".prefab";
        FileTool.CreatFilePath(Application.dataPath + "/" + Path);
        PrefabUtility.CreatePrefab("Assets/" + Path, uiGo, ReplacePrefabOptions.ConnectToPrefab);

        ProjectWindowUtil.ShowCreatedAsset(uiGo);
    }
}
