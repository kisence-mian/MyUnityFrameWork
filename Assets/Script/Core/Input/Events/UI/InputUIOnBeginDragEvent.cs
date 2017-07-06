using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUIOnBeginDragEvent : InputUIEventBase {

    public Vector2 m_dragPosition;
    public Vector2 m_delta;
    public InputUIOnBeginDragEvent()
    {
        m_type = InputUIEventType.BeginDrag;
    }

    public InputUIOnBeginDragEvent(string UIName, string ComponentName)
        : base(UIName, ComponentName,InputUIEventType.BeginDrag)
    {
    }

    public static string GetEventKey(string UIName, string ComponentName, string pram = null)
    {
        return UIName + "." + ComponentName + "." + pram + "." + InputUIEventType.BeginDrag.ToString();
    }
}
