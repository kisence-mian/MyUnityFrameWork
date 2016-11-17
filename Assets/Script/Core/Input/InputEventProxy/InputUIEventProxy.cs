using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputUIEventProxy : IInputProxyBase
{
    public static InputEventRegisterInfo<InputUIOnClickEvent> AddOnClickListener(Button button, string UIName, string ComponentName, string parm, InputEventHandle<InputUIOnClickEvent> callback)
    {
        InputEventRegisterInfo<InputUIOnClickEvent> info = new InputEventRegisterInfo<InputUIOnClickEvent>();

        info.eventKey = InputUIOnClickEvent.GetEventKey(UIName, ComponentName, parm);
        info.callBack = callback;

        info.AddListener();

        button.onClick.AddListener(() =>
        {
            DispatchClickEvent(UIName, ComponentName, parm);
        });

        return info;
    }

    public static void DispatchClickEvent(string UIName, string ComponentName, string parm)
    {
        //只有允许输入时才派发事件
        if (IsAvtive)
        {
            InputUIOnClickEvent eventTmp = new InputUIOnClickEvent(UIName, ComponentName, parm);
            InputManager.Dispatch<InputUIOnClickEvent>(eventTmp);
        }
    }
}