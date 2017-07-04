using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputUIEventProxy : IInputProxyBase
{
    #region 添加监听
    public static InputEventRegisterInfo<InputUIOnClickEvent> AddOnClickListener(Button button, string UIName, string ComponentName, string parm, InputEventHandle<InputUIOnClickEvent> callback)
    {
        InputButtonClickRegisterInfo info = HeapObjectPool<InputButtonClickRegisterInfo>.GetObject();

        info.eventKey = InputUIOnClickEvent.GetEventKey(UIName, ComponentName, parm);
        info.callBack = callback;
        info.m_button = button;
        info.m_OnClick = () =>
        {
            DispatchOnClickEvent(UIName, ComponentName, parm);
        };

        info.AddListener();

        button.onClick.AddListener(info.m_OnClick);

        return info;
    }

    public static InputEventRegisterInfo<InputUILongPressEvent> AddLongPressListener(LongPressAcceptor acceptor, string UIName, string ComponentName, string parm, InputEventHandle<InputUILongPressEvent> callback)
    {
        InputlongPressRegisterInfo info = HeapObjectPool<InputlongPressRegisterInfo>.GetObject();

        info.eventKey = InputUILongPressEvent.GetEventKey(UIName, ComponentName, parm);
        info.callBack = callback;
        info.m_acceptor = acceptor;
        info.m_OnLongPress = (type) =>
        {
            DispatchLongPressEvent(UIName, ComponentName, parm, type);
        };

        info.AddListener();
        acceptor.OnLongPress += info.m_OnLongPress;

        return info;
    }

    public static InputEventRegisterInfo<InputUIOnScrollEvent> AddOnScrollListener(string UIName, string ComponentName, InputEventHandle<InputUIOnScrollEvent> callback)
    {
        InputEventRegisterInfo<InputUIOnScrollEvent> info = HeapObjectPool<InputEventRegisterInfo<InputUIOnScrollEvent>>.GetObject();

        info.eventKey = InputUIOnScrollEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;

        InputManager.AddListener(
            InputUIOnScrollEvent.GetEventKey(UIName, ComponentName), callback);

        return info;
    }

    public static InputEventRegisterInfo<InputUIOnDragEvent> AddOnDragListener(DragAcceptor acceptor, string UIName, string ComponentName, string parm, InputEventHandle<InputUIOnDragEvent> callback)
    {
        InputDragRegisterInfo info = HeapObjectPool<InputDragRegisterInfo>.GetObject();

        info.eventKey = InputUIOnDragEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;
        info.m_acceptor = acceptor;
        info.m_OnDrag = (data) =>
            {
                DispatchDragEvent(UIName, ComponentName, parm, data);
            };

        info.AddListener();

        InputManager.AddListener(
            InputUIOnDragEvent.GetEventKey(UIName, ComponentName, parm), callback);

        return info;
    }

    public static InputBeginDragRegisterInfo AddOnBeginDragListener(DragAcceptor acceptor, string UIName, string ComponentName, string parm, InputEventHandle<InputUIOnBeginDragEvent> callback)
    {
        InputBeginDragRegisterInfo info = HeapObjectPool<InputBeginDragRegisterInfo>.GetObject();

        info.eventKey = InputUIOnDragEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;
        info.m_acceptor = acceptor;
        info.m_OnBeginDrag = (data) =>
        {
            DispatchBeginDragEvent(UIName, ComponentName, parm, data);
        };

        info.AddListener();

        InputManager.AddListener(
            InputUIOnBeginDragEvent.GetEventKey(UIName, ComponentName, parm), callback);

        return info;
    }

    public static InputEndDragRegisterInfo AddOnEndDragListener(DragAcceptor acceptor, string UIName, string ComponentName, string parm, InputEventHandle<InputUIOnEndDragEvent> callback)
    {
        InputEndDragRegisterInfo info = HeapObjectPool<InputEndDragRegisterInfo>.GetObject();

        info.eventKey = InputUIOnDragEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;
        info.m_acceptor = acceptor;
        info.m_OnEndDrag = (data) =>
        {
            DispatchEndDragEvent(UIName, ComponentName, parm, data);
        };

        info.AddListener();

        InputManager.AddListener(
            InputUIOnDragEvent.GetEventKey(UIName, ComponentName, parm), callback);

        return info;
    }

    #endregion

    #region 事件派发

    //public static void DispatchUIEvent<T>(string UIName, string ComponentName, string parm) where T : InputUIEventBase, new()
    //{
    //    //只有允许输入时才派发事件
    //    if (IsActive)
    //    {
    //        T eventTmp = GetUIEvent<T>(UIName, ComponentName, parm);
    //        InputManager.Dispatch<T>(eventTmp);
    //    }
    //}

    public static void DispatchOnClickEvent(string UIName, string ComponentName, string parm)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputUIOnClickEvent e = GetUIEvent<InputUIOnClickEvent>(UIName, ComponentName, parm);
            InputManager.Dispatch("InputUIOnClickEvent", e);
        }
    }

    public static void DispatchLongPressEvent(string UIName, string ComponentName, string parm, InputUIEventType type)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputUILongPressEvent e = GetUIEvent<InputUILongPressEvent>(UIName, ComponentName, parm);
            e.m_LongPressType = type;

            e.EventKey = InputUILongPressEvent.GetEventKey(UIName, ComponentName, parm);
            InputManager.Dispatch("InputUILongPressEvent",e);
        }
    }

    public static void DispatchScrollEvent(string UIName, string ComponentName, string parm, Vector2 position)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputUIOnScrollEvent e = GetOnScrollEvent(UIName, ComponentName, parm, position);
            InputManager.Dispatch("InputUIOnScrollEvent",e);
        }
    }

    public static void DispatchDragEvent(string UIName, string ComponentName,string parm, PointerEventData data)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputUIOnDragEvent e = GetUIEvent<InputUIOnDragEvent>(UIName, ComponentName, parm);
            e.m_dragPosition = data.position;
            e.m_delta = data.delta;

            InputManager.Dispatch("InputUIOnDragEvent",e);
        }
    }

    public static void DispatchBeginDragEvent(string UIName, string ComponentName, string parm, PointerEventData data)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputUIOnBeginDragEvent e = GetUIEvent<InputUIOnBeginDragEvent>(UIName, ComponentName, parm);
            e.m_dragPosition = data.position;
            e.m_delta = data.delta;
            InputManager.Dispatch("InputUIOnBeginDragEvent",e);
        }
    }

    public static void DispatchEndDragEvent(string UIName, string ComponentName, string parm , PointerEventData data)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputUIOnEndDragEvent e = GetUIEvent<InputUIOnEndDragEvent>(UIName, ComponentName, parm);
            e.m_dragPosition = data.position;
            e.m_delta = data.delta;
            InputManager.Dispatch("InputUIOnEndDragEvent",e);
        }
    }

    #endregion

    #region 事件池

    static T GetUIEvent<T>(string UIName, string ComponentName, string parm) where T:InputUIEventBase,new()
    {
        T msg = HeapObjectPool<T>.GetObject();
        msg.Reset();
        msg.m_name = UIName;
        msg.m_compName = ComponentName;
        msg.m_pram = parm;

        return msg;
    }

    static InputUIOnScrollEvent GetOnScrollEvent(string UIName, string ComponentName, string parm, Vector2 position)
    {
        InputUIOnScrollEvent msg = GetUIEvent<InputUIOnScrollEvent>(UIName, ComponentName,parm);

        msg.m_pos = position;

        return msg;
    }


    #endregion
}

public class InputButtonClickRegisterInfo : InputEventRegisterInfo<InputUIOnClickEvent>
{
    public Button m_button;
    public UnityAction m_OnClick;

    public override void RemoveListener()
    {
        base.RemoveListener();
        m_button.onClick.RemoveListener(m_OnClick);
    }
}

public class InputlongPressRegisterInfo : InputEventRegisterInfo<InputUILongPressEvent>
{
    public LongPressAcceptor m_acceptor;
    public InputUIEventLongPressCallBack m_OnLongPress;

    public override void RemoveListener()
    {
        base.RemoveListener();
        m_acceptor.OnLongPress -= m_OnLongPress;
    }
}

public class InputDragRegisterInfo : InputEventRegisterInfo<InputUIOnDragEvent>
{
    public DragAcceptor m_acceptor;
    public InputUIEventDragCallBack m_OnDrag;

    public override void AddListener()
    {
        base.AddListener();
        m_acceptor.m_OnDrag += m_OnDrag;
    }


    public override void RemoveListener()
    {
        base.RemoveListener();
        m_acceptor.m_OnDrag -= m_OnDrag;
    }
}

public class InputBeginDragRegisterInfo : InputEventRegisterInfo<InputUIOnBeginDragEvent>
{
    public DragAcceptor m_acceptor;
    public InputUIEventDragCallBack m_OnBeginDrag;

    public override void AddListener()
    {
        base.AddListener();
        m_acceptor.m_OnBeginDrag += m_OnBeginDrag;
    }

    public override void RemoveListener()
    {
        base.RemoveListener();
        m_acceptor.m_OnBeginDrag -= m_OnBeginDrag;
    }
}

public class InputEndDragRegisterInfo : InputEventRegisterInfo<InputUIOnEndDragEvent>
{
    public DragAcceptor m_acceptor;
    public InputUIEventDragCallBack m_OnEndDrag;

    public override void AddListener()
    {
        base.AddListener();
        m_acceptor.m_OnEndDrag += m_OnEndDrag;
    }
    public override void RemoveListener()
    {
        base.RemoveListener();
        m_acceptor.m_OnEndDrag -= m_OnEndDrag;

    }
}