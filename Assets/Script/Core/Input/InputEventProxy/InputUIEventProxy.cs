using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class InputUIEventProxy : IInputProxyBase
{
    const int c_clickPoolSize = 2;
    const int c_srollPoolSize = 3;

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

    #region 事件池

    static InputUIOnScrollEvent[] m_scrollPool;
    static InputUIOnClickEvent[] m_clickPool;

    static int m_scrollIndex = 0;
    static int m_clickIndex = 0;

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