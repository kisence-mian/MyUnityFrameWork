using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUIOnEndDragEvent : InputUIEventBase
{
    public Vector2 m_dragPosition;
    public Vector2 m_delta;
    public InputUIOnEndDragEvent()
    {
        m_type = InputUIEventType.EndDrag;
    }

     public InputUIOnEndDragEvent(string UIName, string ComponentName)
        : base(UIName, ComponentName,InputUIEventType.EndDrag)
    {
    }

    public static string GetEventKey(string UIName, string ComponentName, string pram = null)
    {
        return UIName + "." + ComponentName + "." + pram + "." + InputUIEventType.EndDrag.ToString();
    }
	
}
