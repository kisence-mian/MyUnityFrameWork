using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputUIOnDragEvent : InputUIEventBase
{
    public Vector2 m_dragPosition;
    public Vector2 m_delta;

    public InputUIOnDragEvent()
    {
        m_type = InputUIEventType.Drag;
    }

    public InputUIOnDragEvent(string UIName, string ComponentName, Vector2 dragPosition)
        : base(UIName, ComponentName, InputUIEventType.Drag)
    {
        m_dragPosition = dragPosition;
    }

    public static string GetEventKey(string UIName, string ComponentName, string pram = null)
    {
        return UIName + "." + ComponentName + "." + pram + "." + InputUIEventType.Drag.ToString();
    }

    public override string Serialize()
    {
        return base.Serialize();
    }
}
