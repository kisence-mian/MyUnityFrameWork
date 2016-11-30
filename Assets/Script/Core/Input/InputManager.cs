using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputManager 
{
    static Dictionary<string, IInputDispatcher> s_dispatcher = new Dictionary<string, IInputDispatcher>();

    static InputEventCallBack s_OnEventDispatch;

    /// <summary>
    /// 所有输入事件派发时都会调用
    /// </summary>
    public static InputEventCallBack OnEveryEventDispatch
    {
        get { return InputManager.s_OnEventDispatch; }
        set { InputManager.s_OnEventDispatch = value; }
    }

    public static void Init()
    {
        InputOperationEventProxy.Init();
    }

    #region Get and Load Dispatcher

    public static InputDispatcher<T> LoadDispatcher<T>() where T : IInputEventBase
    {
        string DispatcherName = typeof(T).ToString();

        if (s_dispatcher.ContainsKey(DispatcherName))
        {
            throw new Exception(DispatcherName + " Dispatcher has exist!");
        }

        InputDispatcher<T> Dispatcher = new InputDispatcher<T>();

        Dispatcher.m_OnAllEventDispatch = s_OnEventDispatch;

        s_dispatcher.Add(DispatcherName, (IInputDispatcher)Dispatcher);

        return Dispatcher;
    }

    public static IInputDispatcher LoadDispatcher(string DispatcherName)
    {
        if (s_dispatcher.ContainsKey(DispatcherName))
        {
            throw new Exception(DispatcherName + " Dispatcher has exist!");
        }

        Type typeArgument = Type.GetType(DispatcherName);

        if (typeArgument.IsSubclassOf(typeof(IInputDispatcher)))
        {
            throw new Exception(DispatcherName + " is not IInputDispatcher subclass!");
        }

        Type dispatcherClass = typeof(InputDispatcher<>);
        Type eventEventClass = dispatcherClass.MakeGenericType(typeArgument);

        IInputDispatcher Dispatcher = (IInputDispatcher)Activator.CreateInstance(eventEventClass);

        s_dispatcher.Add(DispatcherName, (IInputDispatcher)Dispatcher);

        Dispatcher.m_OnAllEventDispatch = s_OnEventDispatch;

        return Dispatcher;
    }

    public static void UnLoadDispatcher<T>() where T : IInputEventBase
    {
        string DispatcherName = typeof(T).ToString();

        if (s_dispatcher.ContainsKey(DispatcherName))
        {
            s_dispatcher.Remove(DispatcherName);
        }
    }

    static IInputDispatcher GetDispatcher(string DispatcherName)
    {
        if (s_dispatcher.ContainsKey(DispatcherName))
        {
            return s_dispatcher[DispatcherName];
        }
        else
        {
            return LoadDispatcher(DispatcherName);
        }
    }

    static InputDispatcher<T> GetDispatcher<T>() where T : IInputEventBase
    {
        string DispatcherName = typeof(T).Name;

        if (s_dispatcher.ContainsKey(DispatcherName))
        {
            return (InputDispatcher<T>)s_dispatcher[DispatcherName];
        }
        else
        {
            return LoadDispatcher<T>();
        }
    }

    public static void RemoveDispatcher(string DispatcherName)
    {
        if (s_dispatcher.ContainsKey(DispatcherName))
        {
            s_dispatcher.Remove(DispatcherName);
        }
    }

    public static void RemoveDispatcher<T>() where T : IInputEventBase
    {
        string DispatcherName = typeof(T).ToString();

        if (s_dispatcher.ContainsKey(DispatcherName))
        {
            s_dispatcher.Remove(DispatcherName);
        }
    }

    #endregion

    #region Listen And Dispatch Event

    public static void Dispatch<T>(T inputEvent) where T : IInputEventBase
    {
        InputDispatcher<T> dispatcher = GetDispatcher<T>();
        dispatcher.Dispatch(inputEvent);
    }

    public static void Dispatch(string eventName ,IInputEventBase inputEvent) 
    {
        IInputDispatcher dispatcher = GetDispatcher(eventName);
        dispatcher.Dispatch(inputEvent);
    }

    #region AddListener

    public static void AddListener(string eventName,string eventKey, InputEventHandle<IInputEventBase> callback)
    {
        IInputDispatcher dispatcher = GetDispatcher(eventName);
        dispatcher.AddListener(eventKey, callback);
    }


    public static void AddListener<T>(string eventKey,InputEventHandle<T> callback) where T: IInputEventBase
    {
        InputDispatcher<T> dispatcher = GetDispatcher<T>();
        dispatcher.AddListener(eventKey, callback);
    }

    public static void AddListener<T>(InputEventHandle<T> callback) where T : IInputEventBase
    {
        InputDispatcher<T> dispatcher = GetDispatcher<T>();
        dispatcher.AddListener(typeof(T).Name, callback);
    }

    ///// <summary>
    ///// 添加一个输入事件监听，只有当事件是自定义的操作事件才可以调用这个方法
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="callback"></param>
    //public static void AddListener<T>(InputEventHandle<T> callback) where T : IInputOperationEventBase
    //{
    //    InputDispatcher<T> dispatcher = GetDispatcher<T>();
    //    dispatcher.AddListener(typeof(T).Name, callback);
    //}

    #endregion

    #region RemoveListener

    public static void RemoveListener<T>(string eventKey, InputEventHandle<T> callback) where T : IInputEventBase
    {
        InputDispatcher<T> dispatcher = GetDispatcher<T>();
        dispatcher.RemoveListener(eventKey, callback);
    }

    public static void RemoveListener<T>(InputEventHandle<T> callback) where T : IInputEventBase
    {
        InputDispatcher<T> dispatcher = GetDispatcher<T>();
        dispatcher.RemoveListener(typeof(T).Name, callback);
    }

    #endregion

    #endregion
}
public delegate void InputEventCallBack(string eventType,IInputEventBase inputEvent);