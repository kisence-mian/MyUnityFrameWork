using UnityEngine;
using System.Collections;

public class UILayerManager : MonoBehaviour 
{
    public Transform m_GameUILayerParent;
    public Transform m_FixedLayerParent;
    public Transform m_NormalLayerParent;
    public Transform m_TopbarLayerParent;
    public Transform m_PopUpLayerParent;

    public void Awake()
    {
        if (m_GameUILayerParent == null)
        {
            Debug.LogError("UILayerManager :GameUILayerParent is null!");
        }

        if (m_FixedLayerParent == null)
        {
            Debug.LogError("UILayerManager :FixedLayerParent is null!");
        }

        if (m_NormalLayerParent == null)
        {
            Debug.LogError("UILayerManager :NormalLayerParent is null!");
        }

        if (m_TopbarLayerParent == null)
        {
            Debug.LogError("UILayerManager :TopbarLayerParent is null!");
        }

        if (m_PopUpLayerParent == null)
        {
            Debug.LogError("UILayerManager :popUpLayerParent is null!");
        }
    }

	public void SetLayer(UIWindowBase l_ui)
    {
        RectTransform l_rt = l_ui.GetComponent<RectTransform>();
        switch (l_ui.m_UIType)
        {
            case UIType.GameUI: l_ui.transform.SetParent(m_GameUILayerParent); break;
            case UIType.Fixed: l_ui.transform.SetParent(m_FixedLayerParent); break;
            case UIType.Normal: l_ui.transform.SetParent(m_NormalLayerParent); break;
            case UIType.TopBar: l_ui.transform.SetParent(m_TopbarLayerParent); break;
            case UIType.PopUp: l_ui.transform.SetParent(m_PopUpLayerParent); break;
        }

        l_rt.localScale = Vector3.one;

        if (l_ui.m_UIType != UIType.GameUI)
        {
            l_rt.anchorMin = Vector2.zero;
            l_rt.anchorMax = Vector3.one;

            l_rt.sizeDelta = Vector2.zero;
            l_rt.anchoredPosition = Vector3.zero;
        }
    }
}
