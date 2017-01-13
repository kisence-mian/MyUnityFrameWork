using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputUIOnScrollEvent :InputUIEventBase
{
    public Vector2 m_pos;
    
    public InputUIOnScrollEvent()
    {
        m_type = InputUIEventType.Scroll;
    }

    public InputUIOnScrollEvent(string UIName, string ComponentName, Vector2 position)
        : base(UIName, ComponentName,InputUIEventType.Scroll)
    {
        m_pos = position;
    }



    public static string GetEventKey(string UIName, string ComponentName, string pram = null)
    {
        return UIName + "." + ComponentName + "." + pram + "." + InputUIEventType.Scroll.ToString();
    }
}
