using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace FrameWork.GuideSystem
{
    public class GuideWindowBase : UIWindowBase
    {
        protected List<GameObject> m_effectList = new List<GameObject>();
        protected List<RectTransform> m_uiList = new List<RectTransform>();

        protected Image m_mask;
        protected Text m_TipText;
        public RectTransform m_TipTransfrom;

        public override void OnOpen()
        {
            m_mask = GetImage("mask");
            m_TipText = GetText("Text_tip");
            m_TipTransfrom = GetRectTransform("Tips");

            OnOpenUI();
        }

        protected virtual void OnOpenUI()
        {
           
        }

        #region 特效相关
        public GameObject CreateEffect(string effectName)
        {
            GameObject po = GameObjectManager.CreateGameObjectByPool(effectName, RectTransform.gameObject);
            m_effectList.Add(po);

            return po.gameObject;
        }

        public void CreateEffect(string effectName, string windowName, string itemName, bool isFollow)
        {
            //UIWindowBase ui = UIManager.GetUI(windowName);

            //UIBase item = ui.GetItem(itemName);
        }

        public void CreateEffect(string effectName, UIBase ui, Vector3 offset, bool isFollow)
        {
            GameObject effect = CreateEffect(effectName);

            effect.transform.SetParent(ui.transform);
            effect.transform.localPosition = offset;

            if (!isFollow)
            {
                effect.transform.SetParent(m_uiRoot.transform);
            }
        }

        public void ClearEffect()
        {
            for (int i = 0; i < m_effectList.Count; i++)
            {
                GameObjectManager.DestroyGameObjectByPool(m_effectList[i]);
            }

            m_effectList.Clear();
        }

        #endregion

        #region 指示图标相关

        public void ShowGuideUIByItem(string uiName, UIWindowBase ui, string itemName, Vector3 offset, bool isFollow)
        {
            GameObject aimUI = ui.GetGuideDynamicCreateItem(itemName);

            ShowGuideUI(GetRectTransform(uiName), aimUI, offset, isFollow);
        }

        public void ShowGuideUIByObject(string uiName, UIWindowBase ui, string objName, Vector3 offset, bool isFollow)
        {
            GameObject aimUI = ui.GetGameObject(objName);

            ShowGuideUI(GetRectTransform(uiName), aimUI, offset, isFollow);
        }

        public void ShowGuideUI(RectTransform guideUI, GameObject aimUI, Vector3 offset, bool isFollow)
        {
            m_uiList.Add(guideUI);

            guideUI.SetParent(aimUI.transform);
            guideUI.SetSiblingIndex(0);
            guideUI.anchoredPosition3D = offset;

            if (!isFollow)
            {
                guideUI.SetParent(RectTransform);
            }
        }

        public void HideGuideUI(string uiName)
        {
            GetRectTransform(uiName).SetParent(m_uiRoot.transform);

            SetActive(uiName, false);
        }

        public void HideAllGuideUI()
        {
            for (int i = 0; i < m_uiList.Count; i++)
            {
                m_uiList[i].SetParent(m_uiRoot.transform);
                m_uiList[i].gameObject.SetActive(false);
            }

            m_uiList.Clear();
        }

        #endregion

        #region 提示文本相关

        public virtual void ShowTips(string content, Vector3 pos)
        {
            if (content != null)
            {
                m_TipTransfrom.gameObject.SetActive(true);

                m_TipText.text = content;
                GetRectTransform("Tips").anchoredPosition3D = pos;
            }
            else
            {
                m_TipTransfrom.gameObject.SetActive(false);
            }
        }

        public virtual void ClearTips()
        {
            m_TipTransfrom.gameObject.SetActive(false);
        }

        public virtual void SetMaskAlpha(float alpha)
        {
            if(alpha != 0)
            {
                if(m_mask.gameObject.activeSelf == false)
                {
                    m_mask.gameObject.SetActive(true);
                }
                
                Color col = m_mask.color;
                col.a = alpha;
                m_mask.color = col;
            }
            else
            {
                m_mask.gameObject.SetActive(false);
            }
        }

        #endregion

        #region 动画

        #endregion
    }
}
