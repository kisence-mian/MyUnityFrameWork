using UnityEngine;
using System.Collections;

public abstract class IInputProxyBase
{
    /// <summary>
    /// 激活状态
    /// </summary>
    private static bool s_isAvtive = true;

    public static bool IsAvtive
    {
        get { return IInputProxyBase.s_isAvtive; }
        set { IInputProxyBase.s_isAvtive = value; }
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

    public void RemoveListener()
    {
        InputManager.RemoveListener<T>(eventKey, callBack);
    }
}
