using UnityEngine;
using System.Collections;

public class InputUIOnClickEvent : InputUIEventBase
{
    public InputUIOnClickEvent() : base()
    {
        m_type = InputUIEventType.Click;
    }

    public InputUIOnClickEvent(string UIName, string ComponentName, string pram = null)
        : base(UIName, ComponentName, InputUIEventType.Click, pram)
    {
    }

    public override string GetEventKey()
    {
        return base.GetEventKey();
    }
}
