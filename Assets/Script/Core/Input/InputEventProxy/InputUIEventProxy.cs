using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class InputUIEventProxy : IInputProxyBase
{
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
            InputUIOnClickEvent eventTmp = new InputUIOnClickEvent(UIName, ComponentName, parm);
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
            InputUIOnScrollEvent e = new InputUIOnScrollEvent(UIName, ComponentName, position);
            InputManager.Dispatch<InputUIOnScrollEvent>(e);
        }
    }
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