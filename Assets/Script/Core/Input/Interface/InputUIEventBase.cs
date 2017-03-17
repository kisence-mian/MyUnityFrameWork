using UnityEngine;
using System.Collections;

public abstract class InputUIEventBase : IInputEventBase 
{
    public string m_name;

    public string m_compName;

    public InputUIEventType m_type;

    public string m_pram;

    public InputUIEventBase():base()
    {
        m_name = "";
        m_compName = "";
    }

    public InputUIEventBase(string UIName, string ComponentName, InputUIEventType type, string pram = null)
        : base()
    {
        m_name = UIName;
        m_compName = ComponentName;
        m_type = type;
        m_pram = pram;
    }

    protected override string GetEventKey()
    {
        return GetEventKey(m_name, m_compName, m_type, m_pram);
    }

    public static string GetEventKey(string UIName, string ComponentName, InputUIEventType type, string pram = null)
    {
        return UIName + "." + ComponentName + "." + pram + "." + type.ToString();
    }

}
public enum InputUIEventType
{
    Click,

    Scroll,

    PressDown,
    PressUp,

    LongPress,

    BeginDrag,
    Drag,
    EndDrag,
}

