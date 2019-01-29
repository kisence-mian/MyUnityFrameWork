using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUIInput: UIWindowBase, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerDownHandler,IPointerUpHandler
{
    private string m_UIEventKey;
    public InputEventRegisterInfo<InputUIOnBeginDragEvent> m_begionDrag;
    public InputEventRegisterInfo<InputUIOnDragEvent> m_onDrag;
    public InputEventRegisterInfo<InputUIOnEndDragEvent> m_endDrag;
    public InputEventRegisterInfo<InputUIOnMouseEvent> inputUIOnMouseEventDown;
    public InputEventRegisterInfo<InputUIOnMouseEvent> inputUIOnMouseEventUp;

    public virtual void InitEvent(string UIEventKey)
    {
        m_UIEventKey = UIEventKey;
        m_begionDrag = InputUIEventProxy.GetOnBeginDragListener(m_UIEventKey, name, name, OnBeginDragEvent);
        m_onDrag = InputUIEventProxy.GetOnDragListener(m_UIEventKey, name, name, OnDragEvent);
        m_endDrag = InputUIEventProxy.GetOnEndDragListener(m_UIEventKey, name, name, OnEndDragEvent);
        inputUIOnMouseEventDown = InputUIEventProxy.GetOnMouseListener(m_UIEventKey, name, name, true, OnMouseDownEvent);
        inputUIOnMouseEventUp = InputUIEventProxy.GetOnMouseListener(m_UIEventKey, name, name, false, OnMouseUpEvent);
    }

    protected override void OnUIDestroy()
    {
        if (m_begionDrag != null)
        {
            m_begionDrag.RemoveListener();
        }
        if (m_begionDrag != null)
        {
            m_onDrag.RemoveListener();
        }

        if (m_begionDrag != null)
        {
            m_endDrag.RemoveListener();
        }
        base.OnUIDestroy();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InputUIEventProxy.DispatchBeginDragEvent(name, name, null, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InputUIEventProxy.DispatchDragEvent(name, name, null, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InputUIEventProxy.DispatchEndDragEvent(name, name, null, eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InputUIEventProxy.DispatchMouseEvent(name, name, true, null);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputUIEventProxy.DispatchMouseEvent(name, name, false, null);
    }

    public virtual void OnMouseDownEvent(InputUIOnMouseEvent inputEvent)
    {

    }
    public virtual void OnMouseUpEvent(InputUIOnMouseEvent inputEvent)
    {

    }

    public virtual void OnEndDragEvent(InputUIOnEndDragEvent inputEvent)
    {

    }

    public virtual void OnDragEvent(InputUIOnDragEvent inputEvent)
    {

    }

    public virtual void OnBeginDragEvent(InputUIOnBeginDragEvent inputEvent)
    {

    }


}
