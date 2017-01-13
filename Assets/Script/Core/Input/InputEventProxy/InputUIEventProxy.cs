using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputUIEventProxy : IInputProxyBase
{
    const int c_clickPoolSize = 2;
    const int c_srollPoolSize = 3;
    const int c_dragPoolSize = 3;
    const int c_beginDragPoolSize = 3;
    const int c_endDragPoolSize = 3;

    public static InputEventRegisterInfo<InputUIOnClickEvent> AddOnClickListener(Button button, string UIName, string ComponentName, string parm, InputEventHandle<InputUIOnClickEvent> callback)
    {
        InputButtonClickRegisterInfo info = new InputButtonClickRegisterInfo();

        info.eventKey = InputUIOnClickEvent.GetEventKey(UIName, ComponentName, parm);
        info.callBack = callback;
        info.m_button = button;
        info.m_OnClick = () =>
        {
            DispatchClickEvent(UIName, ComponentName, parm);
        };

        info.AddListener();

        button.onClick.AddListener(info.m_OnClick);

        return info;
    }

    public static void DispatchClickEvent(string UIName, string ComponentName, string parm)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InitPool();
            InputUIOnClickEvent eventTmp = GetClickEvent(UIName, ComponentName, parm);
            InputManager.Dispatch<InputUIOnClickEvent>(eventTmp);
        }
    }

    public static InputEventRegisterInfo<InputUIOnScrollEvent> AddOnScrollListener(string UIName, string ComponentName, InputEventHandle<InputUIOnScrollEvent> callback)
    {
        InputEventRegisterInfo<InputUIOnScrollEvent> info = new InputEventRegisterInfo<InputUIOnScrollEvent>();

        info.eventKey = InputUIOnScrollEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;

        InputManager.AddListener<InputUIOnScrollEvent>(
            InputUIOnScrollEvent.GetEventKey(UIName, ComponentName), callback);

        return info;
    }

    public static InputEventRegisterInfo<InputUIOnDragEvent> AddOnDragListener(string UIName, string ComponentName, InputEventHandle<InputUIOnDragEvent> callback)
    {
        InputEventRegisterInfo<InputUIOnDragEvent> info = new InputEventRegisterInfo<InputUIOnDragEvent>();

        info.eventKey = InputUIOnDragEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;

        InputManager.AddListener<InputUIOnDragEvent>(
            InputUIOnDragEvent.GetEventKey(UIName, ComponentName), callback);

        return info;
    }

    public static InputEventRegisterInfo<InputUIOnBeginDragEvent> AddOnBeginDragListener(string UIName, string ComponentName, InputEventHandle<InputUIOnBeginDragEvent> callback)
    {
        InputEventRegisterInfo<InputUIOnBeginDragEvent> info = new InputEventRegisterInfo<InputUIOnBeginDragEvent>();

        info.eventKey = InputUIOnDragEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;

        InputManager.AddListener<InputUIOnBeginDragEvent>(
            InputUIOnBeginDragEvent.GetEventKey(UIName, ComponentName), callback);

        return info;
    }

    public static InputEventRegisterInfo<InputUIOnEndDragEvent> AddOnEndDragListener(string UIName, string ComponentName, InputEventHandle<InputUIOnEndDragEvent> callback)
    {
        InputEventRegisterInfo<InputUIOnEndDragEvent> info = new InputEventRegisterInfo<InputUIOnEndDragEvent>();

        info.eventKey = InputUIOnEndDragEvent.GetEventKey(UIName, ComponentName);
        info.callBack = callback;

        InputManager.AddListener<InputUIOnEndDragEvent>(
            InputUIOnEndDragEvent.GetEventKey(UIName, ComponentName), callback);

        return info;
    }


    public static void DispatchScrollEvent(string UIName, string ComponentName, Vector2 position)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InitPool();
            InputUIOnScrollEvent e = GetConnectMsgEvent(UIName, ComponentName, position);
            InputManager.Dispatch<InputUIOnScrollEvent>(e);
        }
    }

    public static void DispatchDragEvent(string UIName, string ComponentName, Vector2 pos)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InitPool();
            InputUIOnDragEvent e = GetDragEvent(UIName, ComponentName, pos);
            InputManager.Dispatch<InputUIOnDragEvent>(e);
        }
    }

    public static void DispatchBeginDragEvent(string UIName, string ComponentName)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InitPool();
            InputUIOnBeginDragEvent e = GetBeginDragEvent(UIName, ComponentName);
            InputManager.Dispatch<InputUIOnBeginDragEvent>(e);
        }
    }

    public static void DispatchEndDragEvent(string UIName, string ComponentName)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InitPool();
            InputUIOnEndDragEvent e = GetEndDragEvent(UIName, ComponentName);
            InputManager.Dispatch<InputUIOnEndDragEvent>(e);
        }
    }

    #region 事件池

    static InputUIOnScrollEvent[] m_scrollPool;
    static InputUIOnClickEvent[] m_clickPool;
    static InputUIOnDragEvent[] m_dragPool;
    static InputUIOnBeginDragEvent[] m_beginDragPool;
    static InputUIOnEndDragEvent[] m_endDragPool;

    static int m_scrollIndex = 0;
    static int m_clickIndex = 0;
    static int m_dragIndex = 0;
    static int m_beginDragIndex = 0;
    static int m_endDragIndex = 0;

    static bool isInit = false;

    static void InitPool()
    {
        if (!isInit)
        {
            isInit = true;
            m_scrollPool = new InputUIOnScrollEvent[c_clickPoolSize];
            for (int i = 0; i < c_clickPoolSize; i++)
            {
                m_scrollPool[i] = new InputUIOnScrollEvent();
            }

            m_clickPool = new InputUIOnClickEvent[c_srollPoolSize];
            for (int i = 0; i < c_srollPoolSize; i++)
            {
                m_clickPool[i] = new InputUIOnClickEvent();
            }

            m_dragPool = new InputUIOnDragEvent[c_dragPoolSize];
            for (int i = 0; i < c_dragPoolSize; i++)
            {
                m_dragPool[i] = new InputUIOnDragEvent();
            }

            m_beginDragPool = new InputUIOnBeginDragEvent[c_beginDragPoolSize];
            for (int i = 0; i < c_beginDragPoolSize; i++)
            {
                m_beginDragPool[i] = new InputUIOnBeginDragEvent();
            }

            m_endDragPool = new InputUIOnEndDragEvent[c_endDragPoolSize];
            for (int i = 0; i < c_endDragPoolSize; i++)
            {
                m_endDragPool[i] = new InputUIOnEndDragEvent();
            }
        }
    }

    static InputUIOnClickEvent GetClickEvent(string UIName, string ComponentName, string parm)
    {
        InputUIOnClickEvent msg = m_clickPool[m_clickIndex];
        msg.Reset();

        msg.m_name = UIName;
        msg.m_compName = ComponentName;
        msg.m_pram = parm;

        m_clickIndex++;

        if (m_clickIndex >= m_clickPool.Length)
        {
            m_clickIndex = 0;
        }

        return msg;
    }

    static InputUIOnScrollEvent GetConnectMsgEvent(string UIName, string ComponentName, Vector2 position)
    {
        InputUIOnScrollEvent msg = m_scrollPool[m_scrollIndex];
        msg.Reset();

        msg.m_name = UIName;
        msg.m_compName = ComponentName;
        msg.m_pos = position;

        m_scrollIndex++;

        if (m_scrollIndex >= m_scrollPool.Length)
        {
            m_scrollIndex = 0;
        }


        return msg;
    }

    static InputUIOnDragEvent GetDragEvent(string UIName, string ComponentName, Vector2 pos)
    {
        InputUIOnDragEvent msg = m_dragPool[m_dragIndex];
        msg.Reset();

        msg.m_name = UIName;
        msg.m_compName = ComponentName;
        msg.m_dragPosition = pos;

        m_dragIndex++;

        if (m_dragIndex >= m_dragPool.Length)
        {
            m_dragIndex = 0;
        }


        return msg;
    }

    static InputUIOnBeginDragEvent GetBeginDragEvent(string UIName, string ComponentName)
    {
        InputUIOnBeginDragEvent msg = m_beginDragPool[m_beginDragIndex];
        msg.Reset();

        msg.m_name = UIName;
        msg.m_compName = ComponentName;

        m_beginDragIndex++;

        if (m_beginDragIndex >= m_beginDragPool.Length)
        {
            m_beginDragIndex = 0;
        }

        return msg;
    }

    static InputUIOnEndDragEvent GetEndDragEvent(string UIName, string ComponentName)
    {
        InputUIOnEndDragEvent msg = m_endDragPool[m_endDragIndex];
        msg.Reset();

        msg.m_name = UIName;
        msg.m_compName = ComponentName;

        m_endDragIndex++;

        if (m_endDragIndex >= m_endDragPool.Length)
        {
            m_endDragIndex = 0;
        }

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