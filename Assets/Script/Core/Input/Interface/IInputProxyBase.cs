using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    #region 事件池

    //public static T GetEvent<T>() where T : IInputEventBase, new()
    //{
    //    return InputOperationEventProxy.GetEvent<T>();
    //}

    public static T GetEvent<T>(string eventKey) where T : IInputEventBase, new()
    {
        T tmp = HeapObjectPoolTool<T>.GetHeapObject();
        tmp.EventKey = eventKey;

        return tmp;
    }

    #endregion
}

public class InputEventRegisterInfo<T> : HeapObjectBase where T : IInputEventBase
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
        Release();
    }

    
}
