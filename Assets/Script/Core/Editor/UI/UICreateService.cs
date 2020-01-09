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
        //新增五个层级
        EditorExpand.AddSortLayerIfNotExist("GameUI");
        EditorExpand.AddSortLayerIfNotExist("Fixed");
        EditorExpand.AddSortLayerIfNotExist("Normal");
        EditorExpand.AddSortLayerIfNotExist("TopBar");
        EditorExpand.AddSortLayerIfNotExist("Upper");
        EditorExpand.AddSortLayerIfNotExist("PopUp");

        //UIManager
        GameObject UIManagerGo = new GameObject("UIManager");
        UIManagerGo.layer = LayerMask.NameToLayer("UI");
        UIManager UIManager = UIManagerGo.AddComponent<UIManager>();

        CreateUICamera(UIManager, "DefaultUI",1, referenceResolution, MatchMode, isOnlyUICamera, isVertical);

        ProjectWindowUtil.ShowCreatedAsset(UIManagerGo);

        //保存UIManager
        ReSaveUIManager(UIManagerGo);
    }

    public static void CreateUICamera(UIManager UIManager,string key, float cameraDepth, Vector2 referenceResolution, CanvasScaler.ScreenMatchMode MatchMode, bool isOnlyUICamera, bool isVertical)
    {
        UILayerManager.UICameraData uICameraData = new UILayerManager.UICameraData();

        uICameraData.m_key = key;

        GameObject UIManagerGo = UIManager.gameObject;
        GameObject canvas = new GameObject(key);
        RectTransform canvasRt = canvas.AddComponent<RectTransform>();

        canvasRt.SetParent(UIManagerGo.transform);
        uICameraData.m_root = canvas;

        //UIcamera
        GameObject cameraGo = new GameObject("UICamera");
        cameraGo.transform.SetParent(canvas.transform);
        cameraGo.transform.localPosition = new Vector3(0, 0, -5000);
        Camera camera = cameraGo.AddComponent<Camera>();
        camera.cullingMask = LayerMask.GetMask("UI");
        camera.orthographic = true;
        camera.depth = cameraDepth;
        uICameraData.m_camera = camera;

        //Canvas
        Canvas canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceCamera;
        canvasComp.worldCamera = camera;

        //UI Raycaster
        canvas.AddComponent<GraphicRaycaster>();

        //CanvasScaler
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = referenceResolution;
        scaler.screenMatchMode = MatchMode;

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
        UILayerManager UILayerManager = UIManagerGo.GetComponent<UILayerManager>();

        goTmp = new GameObject("GameUI");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(canvas.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        uICameraData.m_GameUILayerParent = goTmp.transform;

        goTmp = new GameObject("Fixed");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(canvas.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        uICameraData.m_FixedLayerParent = goTmp.transform;

        goTmp = new GameObject("Normal");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(canvas.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        uICameraData.m_NormalLayerParent = goTmp.transform;

        goTmp = new GameObject("TopBar");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(canvas.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        uICameraData.m_TopbarLayerParent = goTmp.transform;

        goTmp = new GameObject("Upper");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(canvas.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        uICameraData.m_UpperParent = goTmp.transform;

        goTmp = new GameObject("PopUp");
        goTmp.layer = LayerMask.NameToLayer("UI");
        goTmp.transform.SetParent(canvas.transform);
        goTmp.transform.localScale = Vector3.one;
        rtTmp = goTmp.AddComponent<RectTransform>();
        rtTmp.anchorMax = new Vector2(1, 1);
        rtTmp.anchorMin = new Vector2(0, 0);
        rtTmp.anchoredPosition3D = Vector3.zero;
        rtTmp.sizeDelta = Vector2.zero;
        uICameraData.m_PopUpLayerParent = goTmp.transform;

        UILayerManager.UICameraList.Add(uICameraData);

        //重新保存
        ReSaveUIManager(UIManagerGo);
    }

    static void ReSaveUIManager(GameObject UIManagerGo)
    {
        string Path = "Resources/UI/UIManager.prefab";
        FileTool.CreatFilePath(Application.dataPath + "/" + Path);
        PrefabUtility.CreatePrefab("Assets/" + Path, UIManagerGo, ReplacePrefabOptions.ConnectToPrefab);
    }

    public static void CreatUI(string UIWindowName, string UIcameraKey,UIType UIType,UILayerManager UILayerManager,bool isAutoCreatePrefab)
    {
        GameObject uiGo = new GameObject(UIWindowName);

        Type type = EditorTool.GetType(UIWindowName);
        UIWindowBase uiBaseTmp = uiGo.AddComponent(type) as UIWindowBase;

        uiGo.layer = LayerMask.NameToLayer("UI");

        uiBaseTmp.m_UIType = UIType;

        Canvas canvas =  uiGo.AddComponent<Canvas>();

        if(EditorExpand.isExistShortLayer(UIType.ToString()))
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = UIType.ToString();
        }

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

        GameObject tips = new GameObject("Tips");
        tips.layer = LayerMask.NameToLayer("UI");
        RectTransform tipsrt = tips.AddComponent<RectTransform>();
        tipsrt.SetParent(root);

        guideBaseTmp.m_objectList.Add(tips);

        GameObject Text_tips = new GameObject("Text_tip");
        Text_tips.layer = LayerMask.NameToLayer("UI");
        RectTransform txt_tipsrt = Text_tips.AddComponent<RectTransform>();
        //Text text = Text_tips.AddComponent<Text>();
        txt_tipsrt.SetParent(tipsrt);

        guideBaseTmp.m_objectList.Add(Text_tips);

        //guideBaseTmp.m_mask = img;
        //guideBaseTmp.m_TipText = text;
        //guideBaseTmp.m_TipTransfrom = txt_tipsrt;

        guideBaseTmp.m_bgMask = BgGo;
        guideBaseTmp.m_uiRoot = rootGo;

        string Path = "Resources/UI/" + UIWindowName + "/" + UIWindowName + ".prefab";
        FileTool.CreatFilePath(Application.dataPath + "/" + Path);
        PrefabUtility.CreatePrefab("Assets/" + Path, uiGo, ReplacePrefabOptions.ConnectToPrefab);

        ProjectWindowUtil.ShowCreatedAsset(uiGo);
    }
}
