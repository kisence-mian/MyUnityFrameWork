#if UNITY_5_5_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class UIModelShowTool  {


    public static GameObject Create(string prefabName,out RenderTexture tex)
    {
        GameObject temp0 = new GameObject("UIModelShow");
        GameObject temp1 = new GameObject("Camera");
        temp1.transform.SetParent(temp0.transform);
        temp1.transform.localPosition = new Vector3(0, 5000, 0);
       Camera ca= temp1.AddComponent<Camera>();
        ca.clearFlags = CameraClearFlags.SolidColor;
        ca.backgroundColor = new Color(0, 0, 0, 5 / 255f);
        ca.orthographic = true;
        ca.orthographicSize = 0.72f;
        ca.depth = 100;
        ca.cullingMask =1<< LayerMask.NameToLayer("UI");

        GameObject root = new GameObject("Root");
        root.transform.SetParent(temp1.transform);
        root.transform.localPosition = new Vector3(0, 0, 100);
        root.transform.eulerAngles = new Vector3(0, 180, 0);

        GameObject obj = GameObjectManager.CreateGameObjectByPool(prefabName);
        obj.transform.SetParent(root.transform);
        obj.transform.localPosition = new Vector3(0, 0, 0);
        obj.transform.localEulerAngles = Vector3.zero;
  
        Transform[] trans = obj.GetComponentsInChildren<Transform>();
        for (int i = 0; i < trans.Length; i++)
        {
            trans[i].gameObject.layer = LayerMask.NameToLayer("UI");
        }

        SkinnedMeshRenderer[] mes= obj.GetComponentsInChildren<SkinnedMeshRenderer>();
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

    public static void AddDrag(GameObject UIObj,GameObject modelObj)
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
}
#endif