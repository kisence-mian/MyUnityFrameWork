using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class InputManager 
{
    static bool s_isReplay = false;

    public static bool IsReplay
    {
        get { return s_isReplay; }
        set { s_isReplay = value; }
    }

    //操作
    static List<IInputOperation> s_OperationList = new List<IInputOperation>();

    static Dictionary<string, InputNetworkEventCallBack> s_netWorkListener     = new Dictionary<string, InputNetworkEventCallBack>();
    static Dictionary<string, InputUIEventCallBack> s_UIListener               = new Dictionary<string, InputUIEventCallBack>();
    static Dictionary<string, InputOperationEventCallBack> s_opreationListener = new Dictionary<string, InputOperationEventCallBack>();

    /// <summary>
    /// 所有输入事件触发时都调用
    /// </summary>
    private static InputEventHandle s_onInputEvent;

    public static InputEventHandle OnInputEvent
    {
        get { return InputManager.s_onInputEvent; }
        set { InputManager.s_onInputEvent = value; }
    }

    /// <summary>
    /// 所有UI事件触发时都调用
    /// </summary>
    private static InputNetworkEventCallBack s_onInputUIEvent;

    public static InputNetworkEventCallBack OnInputUIEvent
    {
        get { return InputManager.s_onInputUIEvent; }
        set { InputManager.s_onInputUIEvent = value; }
    }

    /// <summary>
    /// 所有网络事件触发时都调用
    /// </summary>
    private static InputNetworkEventCallBack s_onInputNetworkEvent;

    public static InputNetworkEventCallBack OnInputNetworkEvent
    {
        get { return InputManager.s_onInputNetworkEvent; }
        set { InputManager.s_onInputNetworkEvent = value; }
    }

    /// <summary>
    /// 所有操作事件触发时都调用用
    /// </summary>
    private static InputOperationEventCallBack s_onInputOpreationEvent;

    public static InputOperationEventCallBack OnInputOpreationEvent
    {
        get { return InputManager.s_onInputOpreationEvent; }
        set { InputManager.s_onInputOpreationEvent = value; }
    }

    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += Update;
    }

    public static void Update()
    {
        //如果自定义操作满足条件则触发操作事件
        for (int i = 0; i < s_OperationList.Count; i++)
        {
            if (s_OperationList[i].IsCreatOperation())
            {
                DispatchOperation(s_OperationList[i].GetOperationEvent());
            }
        }
    }

    #region NetWork

    public static void AddNetworkEventListener(string messageType, InputNetworkEventCallBack callBack)
    {
        if (!s_netWorkListener.ContainsKey(messageType))
        {
            s_netWorkListener.Add(messageType, callBack);
        }
        else
        {
            s_netWorkListener[messageType] += callBack;
        }
    }

    public static void RemoveNetworkEventListener(string messageType, InputNetworkEventCallBack callBack)
    {
        if (s_netWorkListener.ContainsKey(messageType))
        {
            s_netWorkListener[messageType] -= callBack;
        }
    }

    public static void DispatchNetwork(InputNetworkEvent Event)
    {
        string eventKey = Event.GetEventKey();

        if (s_netWorkListener.ContainsKey(eventKey))
        {
            InputEventCallBack[] tmp = (InputEventCallBack[])s_netWorkListener[eventKey].GetInvocationList();

            foreach (var callBack in tmp)
            {
                try
                {
                    callBack(Event);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }

    #endregion 

    #region UIEvent

    public static void AddUIEventListener(string UIName, string ComponentName, InputEventType type, InputUIEventCallBack callBack)
    {
        AddUIListener(UIName + "." + ComponentName + "." + type.ToString(), callBack);
    }

    public static void AddUIListener(string eventKey, InputUIEventCallBack callBack)
    {
        if (!s_UIListener.ContainsKey(eventKey))
        {
            s_UIListener.Add(eventKey, callBack);
        }
        else
        {
            s_UIListener[eventKey] += callBack;
        }
    }

    public static void RemoveUIListener(string UIName, string ComponentName, InputEventType type, InputUIEventCallBack callBack)
    {
        RemoveUIListener(UIName + "." + ComponentName + "." + type.ToString(), callBack);
    }

    public static void RemoveUIListener(string eventKey, InputUIEventCallBack callBack)
    {
        if (s_UIListener.ContainsKey(eventKey))
        {
            s_UIListener[eventKey] -= callBack;
        }
    }

    public static void DispatchUIEvent(InputUIEvent Event)
    {
        string eventKey = Event.GetEventKey();

        if (s_netWorkListener.ContainsKey(eventKey))
        {
            InputEventCallBack[] tmp = (InputEventCallBack[])s_UIListener[eventKey].GetInvocationList();

            foreach (var callBack in tmp)
            {
                try
                {
                    callBack(Event);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }

    #endregion

    #region OperationEvent

    public static void AddOperationListener<T>(InputOperationEventCallBack callBack) where T : InputOperationEvent
    {
        AddOperationListener(typeof(T).Name, callBack);
    }

    static void AddOperationListener(string eventKey, InputOperationEventCallBack callBack)
    {
        if (!s_opreationListener.ContainsKey(eventKey))
        {
            s_opreationListener.Add(eventKey, callBack);
        }
        else
        {
            s_opreationListener[eventKey] += callBack;
        }
    }

    public static void RemoveOperationListener<T>(InputOperationEventCallBack callBack) where T : InputOperationEvent
    {
        RemoveListener(typeof(T).Name, callBack);
    }

    public static void RemoveListener(string eventKey, InputOperationEventCallBack callBack)
    {
        if (s_opreationListener.ContainsKey(eventKey))
        {
            s_opreationListener[eventKey] -= callBack;
        }
    }

    public static void DispatchOperation(InputOperationEvent Event)
    {
        string eventKey = Event.GetEventKey();

        if (s_netWorkListener.ContainsKey(eventKey))
        {
            InputEventCallBack[] tmp = (InputEventCallBack[])s_opreationListener[eventKey].GetInvocationList();

            foreach (var callBack in tmp)
            {
                try
                {
                    callBack(Event);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }

    #endregion

    #region 派发

    public static void Dispatch(InputEventType eventType ,IInputEventBase inputEvent)
    {
        s_onInputEvent(eventType,inputEvent);

        if(!s_isReplay)
        {

        }

    }

    #endregion
}

public delegate void InputEventCallBack(IInputEventBase inputEvent);

public delegate void InputUIEventCallBack(InputUIEvent uiEvent);
public delegate void InputNetworkEventCallBack(InputNetworkEvent networkEvent);
public delegate void InputOperationEventCallBack(IInputOperation opreationEvent);

public delegate void InputEventHandle(InputEventType eventType, IInputEventBase opreationEvent);

public enum InputEventType
{
    NetWork,
    UI,
    Operation,
}
