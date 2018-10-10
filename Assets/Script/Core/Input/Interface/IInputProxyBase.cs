using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
        T tmp = HeapObjectPool<T>.GetObject();
        tmp.EventKey = eventKey;

        return tmp;
    }

    #endregion
}

public class InputEventRegisterInfo : IHeapObjectInterface
{
    public string eventKey;
    public void OnInit(){}
    public void OnPop(){}
    public void OnPush(){}
    public virtual void AddListener() { }
    public virtual void RemoveListener() { }
}

public class InputEventRegisterInfo<T> : InputEventRegisterInfo where T : IInputEventBase
{
    public InputEventHandle<T> callBack;

    public InputEventRegisterInfo()
    {
    }

    /// <summary>
    /// 添加监听和派发
    /// </summary>
    /// <param name="isRegister">这个eventKey是否已经注册过了，如果是则不在派发对象上重复派发</param>
    public override void AddListener()
    {
        InputManager.AddListener<T>(eventKey, callBack);
    }

    /// <summary>
    /// 移除监听和派发
    /// </summary>
    /// <param name="isRegister">这是不是这个eventKey最后一个监听事件，如果是则移除派发</param>
    public override void RemoveListener()
    {
        InputManager.RemoveListener<T>(eventKey, callBack);
        HeapObjectPool<InputEventRegisterInfo<T>>.PutObject(this);
    }
}