using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuideWindowBase : UIWindowBase
{
    List<PoolObject> m_effectList = new List<PoolObject>();
    List<RectTransform> m_uiList = new List<RectTransform>();

    #region 特效相关
    public GameObject CreateEffect(string effectName)
    {
        PoolObject po = GameObjectManager.GetPoolObject(effectName, RectTransform.gameObject);
        m_effectList.Add( po);

        return po.gameObject;
    }

    public void CreateEffect(string effectName,string windowName,string itemName,bool isFollow)
    {
        UIWindowBase ui = UIManager.GetUI(windowName);

        UIBase item = ui.GetItem(itemName);
    }

    public void CreateEffect(string effectName, UIBase ui,Vector3 offset,bool isFollow)
    {
        GameObject effect =  CreateEffect(effectName);

        effect.transform.SetParent(ui.transform);
        effect.transform.localPosition = offset;

        if(!isFollow)
        {
            effect.transform.SetParent(m_uiRoot.transform);
        }
    }

    public void ClearEffect()
    {
        for (int i = 0; i < m_effectList.Count; i++)
        {
            GameObjectManager.DestroyPoolObject(m_effectList[i]);
        }

        m_effectList.Clear();
    }

    #endregion

    #region 指示图标相关

    public void ShowGuideUIByItem(string uiName,UIWindowBase ui, string itemName, Vector3 offset, bool isFollow)
    {
        UIBase aimUI =  ui.GetItem(itemName);

        ShowGuideUI(GetRectTransform(uiName), aimUI, offset, isFollow);
    }

    public void ShowGuideUIByObject(string uiName, UIWindowBase ui, string objName, Vector3 offset, bool isFollow)
    {
        UIBase aimUI = ui.GetItem(objName);

        ShowGuideUI(GetRectTransform(uiName), aimUI, offset, isFollow);
    }

    public void ShowGuideUI(RectTransform guideUI, UIBase aimUI, Vector3 offset, bool isFollow)
    {
        m_uiList.Add(guideUI);

        guideUI.SetParent(aimUI.RectTransform);
        guideUI.anchoredPosition3D = offset;

        if (!isFollow)
        {
            guideUI.SetParent(RectTransform);
        }
    }

    public void HideGuideUI(string uiName)
    {
        SetActive(uiName,false);
    }

    public void HideAllGuideUI()
    {
        for (int i = 0; i < m_uiList.Count; i++)
        {
            m_uiList[i].gameObject.SetActive(false);
        }

        m_uiList.Clear();
    }

    #endregion

    #region 提示文本相关

    public virtual void ShowTips(string content,Vector3 pos)
    {

    }

    #endregion

    #region 动画

    #endregion
}
