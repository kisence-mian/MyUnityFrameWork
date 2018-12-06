using UnityEngine;
using System.Collections;

public class InputUIOnMouseEvent : InputUIEventBase
{
    public bool m_isDown;

    public InputUIOnMouseEvent() : base()
    {
       
    }

    public InputUIOnMouseEvent(bool isDown) : base()
    {
        m_isDown = isDown;
        m_type = isDown?InputUIEventType.PressDown: InputUIEventType.PressUp;
    }


    public static string GetEventKey(string UIName, string ComponentName, bool isDown, string pram = null)
    {
        string type = isDown ? InputUIEventType.PressDown.ToString() : InputUIEventType.PressUp.ToString();

        return UIName + "." + ComponentName + "." + pram + "." + type;
    }
}
