using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class InputManager 
{
    //操作
    static List<IInputOperation> s_OperationList = new List<IInputOperation>();

    static Dictionary<string, InputEventCallBack> s_events = new Dictionary<string, InputEventCallBack>();

    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += Update;

    }

    public static void Update()
    {
        for (int i = 0; i < s_OperationList.Count; i++)
        {
            if (s_OperationList[i].IsCreatOperation())
            {
                Dispatch(s_OperationList[i].GetOperationEvent());
            }
        }
    }

    public static void AddListener<T>(InputEventCallBack callBack) where T:IInputEventBase
    {
        AddListener( typeof(T).Name, callBack);
    }

    public static void AddListener(string eventKey, InputEventCallBack callBack)
    {
        if(!s_events.ContainsKey(eventKey))
        {
            s_events.Add(eventKey, callBack);
        }
        else
        {
            s_events[eventKey] += callBack;
        }
    }

    public static void RemoveListener(string eventKey, InputEventCallBack callBack)
    {
        if (s_events.ContainsKey(eventKey))
        {
            s_events[eventKey] -= callBack;
        }
    }

    public static void Dispatch(IInputEventBase inputEvent)
    {
        string eventName = inputEvent.GetEventKey();

        if (s_events.ContainsKey(eventName))
        {
            InputEventCallBack[] tmp = (InputEventCallBack[])s_events[eventName].GetInvocationList();

            foreach (var callBack in tmp)
            {
                try
                {
                    callBack(inputEvent);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }
}

public delegate void InputEventCallBack(IInputEventBase inputEvent);

[System.Serializable]
public class InputEvent
{
    public InputEventType m_type;
}

public enum InputEventType
{
    NetWork,
    UI,
    Operation,

}
