using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIWindowBase : UIBase 
{
    public UIType m_UIType;

    public GameObject m_bgMask;
    public GameObject m_uiRoot;



    #region 重载方法

    public virtual void OnOpen()
    {

    }

    public virtual void OnClose()
    {

    }

    public virtual void OnRefresh()
    {
    }

    public virtual IEnumerator EnterAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack,params object[] objs)
    {
        //默认无动画
        l_animComplete(this, l_callBack, objs);

        yield break;
    }

    public virtual void OnCompleteEnterAnim()
    {
    }

    public virtual IEnumerator ExitAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack, params object[] objs)
    {
        //默认无动画
        l_animComplete(this, l_callBack, objs);

        yield break;
    }

    public virtual void OnCompleteExitAnim()
    {

    }

    #endregion 

    #region 继承方法


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
}


