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

    public override string GetEventKey()
    {
        return m_name + "." + m_compName + "." + m_pram + "." + m_type.ToString();
    }

}
public enum InputUIEventType
{
    Click,

    PressDown,
    PressUp,

    BeginDrag,
    Drag,
    EndDrag,
}

