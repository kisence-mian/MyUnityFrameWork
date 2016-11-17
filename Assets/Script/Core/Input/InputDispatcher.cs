using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputDispatcher<Event> : IInputDispatcher where Event : IInputEventBase
{
    protected Dictionary<string, InputEventHandle<Event>> m_Listeners = new Dictionary<string, InputEventHandle<Event>>();

    /// <summary>
    /// 所有此类输入事件调用时都会调用
    /// </summary>
    public InputEventHandle<Event> OnEventDispatch;

    public override void AddListener(string eventKey, InputEventHandle<IInputEventBase> callBack)
    {
        AddListener(eventKey, (inputEvent) => {
            callBack((IInputEventBase)inputEvent);
        });
    }

    public override void Dispatch( IInputEventBase inputEvent)
    {
        Dispatch((Event)inputEvent);
    }

    public void AddListener(string eventKey, InputEventHandle<Event> callBack)
    {
        if (!m_Listeners.ContainsKey(eventKey))
        {
            m_Listeners.Add(eventKey, callBack);
        }
        else
        {
            m_Listeners[eventKey] += callBack;
        }
    }

    public void RemoveListener(string eventKey, InputEventHandle<Event> callBack)
    {
        if (m_Listeners.ContainsKey(eventKey))
        {
            m_Listeners[eventKey] -= callBack;
        }
    }

    public void Dispatch(Event inputEvent)
    {
        string eventKey = inputEvent.EventKey;

        if (m_Listeners.ContainsKey(eventKey))
        {
            DispatchSingleEvent(inputEvent, m_Listeners[eventKey]);
        }

        //此类事件派发时调用
        DispatchSingleEvent(inputEvent, OnEventDispatch);

        //所有事件派发时都调用
        AllEventDispatch(typeof(Event).Name, inputEvent);
    }

    void DispatchSingleEvent(Event inputEvent, InputEventHandle<Event> callBack)
    {
        if (callBack != null)
        {
            Delegate[] eventArray = callBack.GetInvocationList();
            for (int i = 0; i < eventArray.Length; i++)
            {
                try
                {
                    InputEventHandle<Event> tmp = (InputEventHandle<Event>)eventArray[i];
                    tmp(inputEvent);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }
}