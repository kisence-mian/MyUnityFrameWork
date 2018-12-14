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
        InputUIEventProxy.GetOnMouseListener(m_UIEventKey, name, name, true, OnMouseDownEvent);
        InputUIEventProxy.GetOnMouseListener(m_UIEventKey, name, name, false, OnMouseUpEvent);
    }

    public virtual void OnMouseDownEvent(InputUIOnMouseEvent inputEvent)
    {

    }
    public virtual void OnMouseUpEvent(InputUIOnMouseEvent inputEvent)
    {

    }

    public void DisposeEvent()
    {
        inputUIOnMouseEventDown.RemoveListener();
        inputUIOnMouseEventUp.RemoveListener();

        inputUIOnMouseEventDown = null;
        inputUIOnMouseEventUp = null;
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
