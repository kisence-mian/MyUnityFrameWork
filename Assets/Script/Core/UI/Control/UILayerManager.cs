using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILayerManager : MonoBehaviour 
{
    public List<UICameraData> UICameraList = new List<UICameraData>();

    public void Awake()
    {
        for (int i = 0; i < UICameraList.Count; i++)
        {
            UICameraData data = UICameraList[i];

            //data.m_root.transform.localPosition = new Vector3(0, 0, i * -2000);

            if (data.m_root == null)
            {
                Debug.LogError("UILayerManager :Root is null! " + " key : " + data.m_key + " index : " + i);
            }

            if (data.m_camera == null)
            {
                Debug.LogError("UILayerManager :Camera is null! " + " key : " + data.m_key + " index : " + i);
            }

            if (data.m_GameUILayerParent == null)
            {
                Debug.LogError("UILayerManager :GameUILayerParent is null!" + " key : " + data.m_key + " index : " + i);
            }

            if (data.m_FixedLayerParent == null)
            {
                Debug.LogError("UILayerManager :FixedLayerParent is null!" + " key : " + data.m_key + " index : " + i);
            }

            if (data.m_NormalLayerParent == null)
            {
                Debug.LogError("UILayerManager :NormalLayerParent is null!" + " key : " + data.m_key + " index : " + i);
            }

            if (data.m_TopbarLayerParent == null)
            {
                Debug.LogError("UILayerManager :TopbarLayerParent is null!" + " key : " + data.m_key + " index : " + i);
            }

            if (data.m_UpperParent == null)
            {
                Debug.LogError("UILayerManager :m_UpperParent is null!" + " key : " + data.m_key + " index : " + i);
            }

            if (data.m_PopUpLayerParent == null)
            {
                Debug.LogError("UILayerManager :popUpLayerParent is null!" + " key : " + data.m_key + " index : " + i);
            }
        }
    }

	public void SetLayer(UIWindowBase ui,string cameraKey = null)
    {
        UICameraData data = GetUICameraDataByKey(cameraKey);

        if(cameraKey == null)
        {
            data = GetUICameraDataByKey(ui.cameraKey);
        }
        else
        {
            data = GetUICameraDataByKey(cameraKey);
        }

        RectTransform rt = ui.GetComponent<RectTransform>();
        switch (ui.m_UIType)
        {
            case UIType.GameUI: ui.transform.SetParent(data.m_GameUILayerParent); break;
            case UIType.Fixed: ui.transform.SetParent(data.m_FixedLayerParent); break;
            case UIType.Normal:
                ui.transform.SetParent(data.m_NormalLayerParent);
                break;
            case UIType.TopBar: ui.transform.SetParent(data.m_TopbarLayerParent); break;
            case UIType.Upper: ui.transform.SetParent(data.m_UpperParent); break;
            case UIType.PopUp: ui.transform.SetParent(data.m_PopUpLayerParent); break;
        }

        rt.localScale = Vector3.one;
        rt.sizeDelta = Vector2.zero;

        if (ui.m_UIType != UIType.GameUI)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector3.one;

            rt.sizeDelta = Vector2.zero;
            rt.transform.localPosition = new Vector3(0, 0, ui.m_PosZ);
            rt.anchoredPosition3D = new Vector3(0, 0, ui.m_PosZ);
            rt.SetAsLastSibling();
        }
        else
        {
            Vector3 lp = rt.transform.localPosition;
            lp.z = 0;
            rt.transform.localPosition = lp;
        }
    }

    public UICameraData GetUICameraDataByKey(string key)
    {
        if(key == null || key == "")
        {
            if(UICameraList.Count > 0)
            {
                return UICameraList[0];
            }
            else
            {
                throw new System.Exception("UICameraList is null ! " + key);
            }
        }

        for (int i = 0; i < UICameraList.Count; i++)
        {
            if(UICameraList[i].m_key == key)
            {
                return UICameraList[i];
            }
        }

        throw new System.Exception("Dont Find UILayerData by " + key);
    }

    //public void RemoveUI(UIWindowBase ui)
    //{
    //    switch (ui.m_UIType)
    //    {
    //        case UIType.GameUI: break;
    //        case UIType.Fixed: break;
    //        case UIType.Normal:
    //            normalUIList.Remove(ui);
    //            break;
    //        case UIType.TopBar: break;
    //        case UIType.PopUp:  break;
    //    }
    //}

    [System.Serializable]
    public struct UICameraData
    {
        public string m_key;
        public GameObject m_root;
        public Camera m_camera;
        public Transform m_GameUILayerParent;
        public Transform m_FixedLayerParent;
        public Transform m_NormalLayerParent;
        public Transform m_TopbarLayerParent;
        public Transform m_UpperParent;
        public Transform m_PopUpLayerParent;
    }
}
