using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUILongPressEvent : InputUIEventBase
{
    public static string GetEventKey(string UIName, string ComponentName, string pram = null)
    {
        return UIName + "." + ComponentName + "." + pram + "." + InputUIEventType.LongPress.ToString();
    }
}
