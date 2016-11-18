using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollRectInput : ScrollRect
{
    public string m_UIEventKey;

    public virtual void Init(string UIEventKey)
    {
        m_UIEventKey = UIEventKey;
        InputUIEventProxy.AddOnScrollListener(m_UIEventKey, name, OnSetContentAnchoredPosition);
    }

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        InputUIEventProxy.DispatchScrollEvent(m_UIEventKey, name, position);
    }

    protected virtual void OnSetContentAnchoredPosition(InputUIOnScrollEvent e)
    {
        base.SetContentAnchoredPosition(e.m_pos);
    }

}
