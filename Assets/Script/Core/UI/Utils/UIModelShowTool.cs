#if UNITY_5_5_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class UIModelShowTool
{
    /// <summary>
    /// 每个UImodelCamera的间隔
    /// </summary>
    static Vector3 s_ShowSpace = new Vector3(0, 20, 0);

    static string s_defaultLayer = "UI";
    const bool c_defaultOrthographic = true;
    const float c_defaultOrthographicSize = 0.72f;
    const float c_defaultFOV = 60;
    static Color s_defaultBackgroundColor = new Color(0, 0, 0, 0 / 255f);
    static Vector3 s_StartPosition = new Vector3(5000, 5000, 5000);
    static Vector3 s_defaultLocationPosition = new Vector3(0, 0, 10);
    static Vector3 s_defaultEulerAngles = new Vector3(0, 180, 0);
    static Vector3 s_defaultLocalScale = Vector3.one;
    static Vector3 s_defaultTexSize = new Vector3(512, 512, 100);
    static Vector2 s_clippingPlanes = new Vector2(0.2f, 20);

    static List<UIModelShowData> modelShowList = new List<UIModelShowData>();

    static void ResetModelShowPosition()
    {
        for (int i = 0; i < modelShowList.Count; i++)
        {
            modelShowList[i].top.transform.position = s_StartPosition + i * s_ShowSpace;
        }
    }
     

    public static void DisposeModelShow(UIModelShowData data)
    {
        data.Dispose();
        modelShowList.Remove(data);
        ResetModelShowPosition();
    }

    public static UIModelShowData CreateModelData(string prefabName,
        string layerName = null,
        bool? orthographic = null,
        float? orthographicSize = null,
        Color? backgroundColor = null,
        Vector3? localPosition = null,
        Vector3? eulerAngles = null,
        Vector3? localScale = null,
        Vector3? texSize = null,
        float? nearClippingPlane = null,
        float? farClippingPlane = null)
    {
        //默认值设置
        layerName = layerName ?? s_defaultLayer;
        Vector3 localPositionTmp = localPosition ?? s_defaultLocationPosition;
        Vector3 eulerAnglesTmp = eulerAngles ?? s_defaultEulerAngles;
        Vector3 texSizeTmp = texSize ?? s_defaultTexSize;
        Vector3 localScaleTmp = localScale ?? s_defaultLocalScale;
        Color backgroundColorTmp = backgroundColor ?? s_defaultBackgroundColor;
        float orthographicSizeTmp = orthographicSize ?? c_defaultOrthographicSize;
        bool orthographicTmp = orthographic??c_defaultOrthographic;
        float fieldOfView = orthographicSize ?? c_defaultFOV;
        float nearClippingPlaneTmp = nearClippingPlane ?? s_clippingPlanes.x;
        float farClippingPlaneTmp = farClippingPlane ?? s_clippingPlanes.y;
        //构造Camera
        UIModelShowData data = new UIModelShowData();

        GameObject uiModelShow = new GameObject("UIShowModelCamera");
        data.top = uiModelShow;

        GameObject camera = new GameObject("Camera");

        camera.transform.SetParent(uiModelShow.transform);
        camera.transform.localPosition = Vector3.zero;
        Camera ca = camera.AddComponent<Camera>();
        data.camera = ca;

        ca.clearFlags = CameraClearFlags.SolidColor;
        ca.backgroundColor = backgroundColorTmp;
        ca.orthographic = orthographicTmp;
        ca.orthographicSize = orthographicSizeTmp;
        ca.fieldOfView = fieldOfView;
        ca.depth = 100;
        ca.nearClipPlane = nearClippingPlaneTmp;
        ca.farClipPlane = farClippingPlaneTmp;
        ca.cullingMask = 1 << LayerMask.NameToLayer(layerName);

        GameObject root = new GameObject("Root");
        data.root = root;

        root.transform.SetParent(camera.transform);
        root.transform.localPosition = localPositionTmp;
        root.transform.eulerAngles = eulerAnglesTmp;
        root.transform.localScale = localScaleTmp;

        //创建模型
        GameObject obj = GameObjectManager.CreateGameObject(prefabName);
        data.model = obj;

        obj.transform.SetParent(root.transform);
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        obj.SetLayer(LayerMask.NameToLayer(layerName));

        SkinnedMeshRenderer[] mes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < mes.Length; i++)
        {
            mes[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mes[i].receiveShadows = false;
        }
        
        //设置randerTexture
        RenderTexture tex = new RenderTexture((int)texSizeTmp.x, (int)texSizeTmp.y, (int)texSizeTmp.z);
        data.renderTexture = tex;

        tex.autoGenerateMips = false;
        tex.anisoLevel = 1;

        ca.targetTexture = tex;

        modelShowList.Add(data);
        ResetModelShowPosition();

        return data;
    }


    public static GameObject Create(string prefabName, out RenderTexture tex)
    {
        GameObject temp0 = new GameObject("UIModelShow");
        GameObject temp1 = new GameObject("Camera");
        temp1.transform.SetParent(temp0.transform);
        temp1.transform.localPosition = new Vector3(0, 5000, 0);
        Camera ca = temp1.AddComponent<Camera>();
        ca.clearFlags = CameraClearFlags.SolidColor;
        ca.backgroundColor = new Color(0, 0, 0, 5 / 255f);
        ca.orthographic = true;
        ca.orthographicSize = 0.72f;
        ca.depth = 100;
        ca.cullingMask = 1 << LayerMask.NameToLayer("UI");

        GameObject root = new GameObject("Root");
        root.transform.SetParent(temp1.transform);
        root.transform.localPosition = new Vector3(0, 0, 100);
        root.transform.eulerAngles = new Vector3(0, 180, 0);

        GameObject obj = GameObjectManager.CreateGameObject(prefabName);
        obj.transform.SetParent(root.transform);
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localEulerAngles = Vector3.zero;

        Transform[] trans = obj.GetComponentsInChildren<Transform>();
        for (int i = 0; i < trans.Length; i++)
        {
            trans[i].gameObject.layer = LayerMask.NameToLayer("UI");
        }

        SkinnedMeshRenderer[] mes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < mes.Length; i++)
        {
            mes[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mes[i].receiveShadows = false;
        }

        tex = new RenderTexture(512, 512, 100);
        tex.autoGenerateMips = false;
        tex.anisoLevel = 1;

        //  tex.antiAliasing = 2

        ca.targetTexture = tex;
        return obj;
    }

    public static void AddDrag(GameObject UIObj, GameObject modelObj)
    {
        ModelRotate mro = modelObj.AddComponent<ModelRotate>();

        EventTrigger trigger;
        trigger = UIObj.GetComponent<EventTrigger>();
        if (trigger)
        {
            trigger.triggers.Clear();
        }
        else
        {
            trigger = UIObj.AddComponent<EventTrigger>();
        }

        trigger.triggers.Add(GetEvent(EventTriggerType.Drag, mro.OnDrag));

    }

    private static EventTrigger.Entry GetEvent(EventTriggerType type, UnityAction<BaseEventData> eventFun)
    {
        UnityAction<BaseEventData> eventDrag = new UnityAction<BaseEventData>(eventFun);
        EventTrigger.Entry myclick = new EventTrigger.Entry();
        myclick.eventID = EventTriggerType.Drag;
        myclick.callback.AddListener(eventDrag);
        return myclick;
    }

    public class UIModelShowData
    {
        public GameObject top;
        public GameObject root;
        public GameObject model;
        public Camera camera;
        public RenderTexture renderTexture;

        public void Dispose()
        {
            GameObjectManager.DestroyGameObject(model);
            GameObject.Destroy(top);
        }

        public void ChangeModel(string modelName)
        {
            int layer = model.layer;
            GameObjectManager.DestroyGameObject(model);

            model = GameObjectManager.CreateGameObject(modelName);
            model.transform.SetParent(root.transform);
            model.transform.localPosition = new Vector3(0, 0, 0);
            model.transform.localEulerAngles = Vector3.zero;
            model.transform.localScale = Vector3.one;

            model.SetLayer(layer);
        }
    }
}
#endif