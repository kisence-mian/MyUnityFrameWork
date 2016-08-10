using UnityEngine;
using System.Collections;

public class UIWindowBase : MonoBehaviour 
{
    public UIType m_UIType;

    public GameObject m_bgMask;
    public GameObject m_uiRoot;

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

    //刷新是主动调用
    public virtual void Refresh()
    {
        UISystemEvent.Dispatch(this, UIEvent.OnRefresh);
    }

    public virtual void EnterAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack,params object[] objs)
    {
        //默认无动画
        l_animComplete(this, l_callBack, objs);
    }

    public virtual void OnCompleteEnterAnim()
    {

    }

    public virtual void ExitAnim(UIAnimCallBack l_animComplete, UICallBack l_callBack, params object[] objs)
    {
        //默认无动画
        l_animComplete(this, l_callBack, objs);
    }

    public virtual void OnCompleteExitAnim()
    {

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