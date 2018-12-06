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

    /// <summary>
    /// 基础输入类型和泛型输入类型在这里进行一次映射
    /// </summary>
    Dictionary<InputEventHandle<IInputEventBase>, InputEventHandle<Event>> m_ListenerHash = new Dictionary<InputEventHandle<IInputEventBase>, InputEventHandle<Event>>();

    public override void AddListener(string eventKey, InputEventHandle<IInputEventBase> callBack)
    {
        InputEventHandle<Event> temp = (inputEvent) =>
        {
            callBack((IInputEventBase)inputEvent);
        };

        m_ListenerHash.Add(callBack, temp);

        AddListener(eventKey, temp);
    }
    public override void RemoveListener(string eventKey, InputEventHandle<IInputEventBase> callBack)
    {
        if (!m_ListenerHash.ContainsKey(callBack))
        {
            throw new Exception("RemoveListener Exception: dont find Listener Hash ! eventKey: ->" + eventKey +"<-");
        }

        InputEventHandle<Event> temp = m_ListenerHash[callBack];
        m_ListenerHash.Remove(callBack);

        RemoveListener(eventKey, temp);
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
        //else
        //{
        //    Debug.LogError("不存在的UI事件 " + eventKey);
        //}
    }

    InputEventHandle<Event> m_handle;
    string m_eventKey;

    public void Dispatch(Event inputEvent)
    {
        m_eventKey = inputEvent.EventKey;

        if (m_Listeners.TryGetValue(m_eventKey,out m_handle))
        {
            DispatchSingleEvent(inputEvent, m_handle);
        }

        //此类事件派发时调用
        DispatchSingleEvent(inputEvent, OnEventDispatch);

        //所有事件派发时都调用
        AllEventDispatch(m_eventKey, inputEvent);
    }

    void DispatchSingleEvent(Event inputEvent, InputEventHandle<Event> callBack)
    {
        if (callBack != null)
        {
            try
            {
                callBack(inputEvent);
            }
            catch (Exception e)
            {
                Debug.LogError("DispatchSingleEvent Name: " + typeof(Event).ToString() + " key: " + inputEvent.EventKey + " Exception: " + e.ToString());
            }
        }
    }
}