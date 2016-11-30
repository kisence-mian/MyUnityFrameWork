using UnityEngine;
using System.Collections;

public abstract class IInputProxyBase
{
    /// <summary>
    /// 激活状态
    /// </summary>
    private static bool s_isActive = true;

    public static bool IsActive
    {
        get { return IInputProxyBase.s_isActive; }
        set { IInputProxyBase.s_isActive = value; }
    }
}

public class InputEventRegisterInfo<T> where T: IInputEventBase
{
    public string eventKey;
    public InputEventHandle<T> callBack;

    public void AddListener()
    {
        InputManager.AddListener<T>(eventKey, callBack);
    }

    public virtual void RemoveListener()
    {
        InputManager.RemoveListener<T>(eventKey, callBack);
    }
}
