using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDownUp : MonoBehaviour
{

    private string m_UIEventKey;

    InputEventRegisterInfo<InputUIOnMouseEvent> inputUIOnMouseEventDown;

    InputEventRegisterInfo<InputUIOnMouseEvent> inputUIOnMouseEventUp;
    public virtual void InitEvent(string UIEventKey)
    {
        m_UIEventKey = UIEventKey;
        inputUIOnMouseEventDown = InputUIEventProxy.GetOnMouseListener(m_UIEventKey, name, name, true, OnMouseDownEvent);
        inputUIOnMouseEventUp = InputUIEventProxy.GetOnMouseListener(m_UIEventKey, name, name, false, OnMouseUpEvent);

    }

    public virtual void OnMouseDownEvent(InputUIOnMouseEvent inputEvent)
    {

    }
    public virtual void OnMouseUpEvent(InputUIOnMouseEvent inputEvent)
    {

    }

    public void DisposeEvent()
    {
        InputManager.RemoveListener<InputUIOnMouseEvent>(OnMouseUpEvent);
        InputManager.RemoveListener<InputUIOnMouseEvent>(OnMouseDownEvent);
    }

    private void OnMouseDown()
    {
        InputUIEventProxy.DispatchMouseEvent(name, name, true, null);
    }

    private void OnMouseUp()
    {

        InputUIEventProxy.DispatchMouseEvent(name, name, false, null);
    }

}
