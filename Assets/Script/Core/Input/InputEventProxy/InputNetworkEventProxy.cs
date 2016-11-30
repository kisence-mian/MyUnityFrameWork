using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class InputNetworkEventProxy : IInputProxyBase
{
    public static void DispatchStatusEvent(NetworkState status)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputNetworkConnectStatusEvent e = new InputNetworkConnectStatusEvent(status);

            InputManager.Dispatch<InputNetworkConnectStatusEvent>(e);
        }
    }

    public static void DispatchMessageEvent(string massageType, string message, Dictionary<string, object> data)
    {
        //只有允许输入时才派发事件
        if (IsActive)
        {
            InputNetworkMessageEvent e = new InputNetworkMessageEvent();

            e.m_MessgaeType = massageType;
            e.m_content     = message;
            e.Data = data;

            InputManager.Dispatch<InputNetworkMessageEvent>(e);
        }
    }
}