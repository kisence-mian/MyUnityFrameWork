using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIWindowBase : MonoBehaviour 
{
    public UIType m_UIType;

    public GameObject m_bgMask;
    public GameObject m_uiRoot;

    List<Enum> m_EventNames = new List<Enum>();

    //当UI第一次打开时调用OnInit方法，调用时机在OnOpen之前
    public virtual void OnInit()
    {
    }

    public virtual void OnDestroy()
    {
    }

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

    public virtual IEnumerator EnterAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack,params object[] objs)
    {
        //默认无动画
        l_animComplete(this, l_callBack, objs);

        yield return null;
    }

    public virtual void OnCompleteEnterAnim()
    {

    }

    public virtual IEnumerator ExitAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack, params object[] objs)
    {
        //默认无动画
        l_animComplete(this, l_callBack, objs);

        yield return null;
    }

    public virtual void OnCompleteExitAnim()
    {

    }

    public void AddEvent(Enum l_Event)
    {
        if (!m_EventNames.Contains(l_Event))
        {
            m_EventNames.Add(l_Event);
            GlobalEvent.AddEvent(l_Event, Refresh);
        }
    }

    public void RemoveAllEvent()
    {
        for (int i = 0; i < m_EventNames.Count;i++ )
        {
            GlobalEvent.RemoveEvent(m_EventNames[i], Refresh);
        }
    }

    //刷新是主动调用
    public  void Refresh(params object[] args)
    {
        UISystemEvent.Dispatch(this, UIEvent.OnRefresh);
        OnRefresh();
    }
}

/// <summary>
/// UI回调
/// </summary>
/// <param name="objs"></param>
public delegate void UICallBack(UIWindowBase UI,params object[] objs);
public delegate void UIAnimCallBack(UIWindowBase l_UIbase, UICallBack callBack, params object[] objs);

public enum UIType
{
    GameUI,

    Fixed,
    Normal,
    TopBar,
    PopUp
}

public enum UIEvent
{
    OnOpen,
    OnClose,
    OnHide,
    OnShow,

    OnInit,
    OnDestroy,

    OnRefresh,

    OnStartEnterAnim,
    OnCompleteEnterAnim,

    OnStartExitAnim,
    OnCompleteExitAnim,
}