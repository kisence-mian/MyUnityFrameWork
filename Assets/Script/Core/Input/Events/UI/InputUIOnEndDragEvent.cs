using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUIOnEndDragEvent : InputUIEventBase
{
     public InputUIOnEndDragEvent()
    {
        m_type = InputUIEventType.Scroll;
    }

     public InputUIOnEndDragEvent(string UIName, string ComponentName)
        : base(UIName, ComponentName,InputUIEventType.Scroll)
    {
    }



    public static string GetEventKey(string UIName, string ComponentName, string pram = null)
    {
        return UIName + "." + ComponentName + "." + pram + "." + InputUIEventType.Scroll.ToString();
    }
	
}
