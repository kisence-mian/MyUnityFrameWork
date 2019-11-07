using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIWindowBase : UIBase 
{
    [HideInInspector]
    public string cameraKey;
    public UIType m_UIType;

    public WindowStatus windowStatus;

    public GameObject m_bgMask;
    public GameObject m_uiRoot;

    public float m_PosZ; //Z轴偏移

    #region 重载方法

    public virtual void OnOpen()
    {
    }

    public virtual void OnClose()
    {

    }

    public virtual void OnHide()
    {

    }

    public virtual void OnShow()
    {

    }

    public virtual void OnRefresh()
    {
    }

    public virtual IEnumerator EnterAnim(UIAnimCallBack animComplete, UICallBack callBack,params object[] objs)
    {
        //默认无动画
        animComplete(this, callBack, objs);

        yield break;
    }

    public virtual void OnCompleteEnterAnim()
    {
    }

    public virtual IEnumerator ExitAnim(UIAnimCallBack animComplete, UICallBack callBack, params object[] objs)
    {
        //默认无动画
        animComplete(this, callBack, objs);

        yield break;
    }

    public virtual void OnCompleteExitAnim()
    {
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion 

    #region 继承方法

    public void InitWindow(int id)
    {
        List<UILifeCycleInterface> list = new List<UILifeCycleInterface>();
        Init(null, id);
        RecursionInitUI(null,this,id, list);
    }

    /// <summary>
    /// 递归初始化UI
    /// </summary>
    /// <param name="uiBase"></param>
    public void RecursionInitUI(UIBase parentUI,UIBase uiBase,int id,List<UILifeCycleInterface> UIList)
    {
        int childIndex = 0;
        for (int i = 0; i < uiBase.m_objectList.Count; i++)
        {
            GameObject go = uiBase.m_objectList[i];

            if(go != null)
            {
                UILifeCycleInterface tmp = go.GetComponent<UILifeCycleInterface>();

                if (tmp != null)
                {
                    if (!UIList.Contains(tmp))
                    {
                        uiBase.AddLifeCycleComponent(tmp);

                        UIList.Add(tmp);

                        UIBase subUI = uiBase.m_objectList[i].GetComponent<UIBase>();
                        if(subUI != null)
                        {
                            RecursionInitUI(uiBase, subUI, childIndex++, UIList);
                        }
                    }
                    else
                    {
                        Debug.LogError("InitWindow 重复的引用 " + uiBase.UIEventKey + " " + uiBase.m_objectList[i].name);
                    }

                }
            }
            else
            {
                Debug.LogWarning("InitWindow objectList[" + i + "] is null !: " + uiBase.UIEventKey );
            }
        }
    }


    //刷新是主动调用
    public void Refresh(params object[] args)
    {
        UISystemEvent.Dispatch(this, UIEvent.OnRefresh);
        OnRefresh();
    }

    public void AddEventListener(Enum l_Event)
    {
        if (!m_EventNames.Contains(l_Event))
        {
            m_EventNames.Add(l_Event);
            GlobalEvent.AddEvent(l_Event, Refresh);
        }
    }

    public override void RemoveAllListener()
    {
        base.RemoveAllListener();

        for (int i = 0; i < m_EventNames.Count; i++)
        {
            GlobalEvent.RemoveEvent(m_EventNames[i], Refresh);
        }

        m_EventNames.Clear();
    }


    #endregion

    public enum WindowStatus
    {
        Create,
        Open,
        Close,
        OpenAnim,
        CloseAnim,
        Hide,
    }
}


