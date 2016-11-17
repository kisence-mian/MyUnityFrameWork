using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class IInputDispatcher 
{
    public InputEventCallBack m_OnAllEventDispatch;

    public abstract void AddListener(string eventKey, InputEventHandle<IInputEventBase> callBack);

    public abstract void Dispatch(IInputEventBase inputEvent);

    protected void AllEventDispatch(string eventName, IInputEventBase inputEvent)
    {
        if (m_OnAllEventDispatch != null)
        {
            Delegate[] eventArray = m_OnAllEventDispatch.GetInvocationList();

            for (int i = 0; i < eventArray.Length; i++)
            {
                try
                {
                    InputEventCallBack callBack = (InputEventCallBack)eventArray[i];
                    callBack(eventName, inputEvent);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }
}
public delegate void InputEventHandle<T>(T inputEvent);
