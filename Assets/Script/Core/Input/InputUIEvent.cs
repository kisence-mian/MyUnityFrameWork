using UnityEngine;
using System.Collections;

[SerializeField]
public class InputUIEvent : IInputEventBase 
{
    public string m_UIName;

    public string m_ComponentName;

    public InputUIEventType m_EventType;

    public Vector3 m_mousePosition;

    public override string GetEventKey()
    {
        return m_UIName + "." + m_ComponentName+ "." + m_EventType.ToString();
    }

}

public enum InputUIEventType
{
    OnClick,

    OnBeginDrag,
    OnDrag,
    OnEndDrag,
}
