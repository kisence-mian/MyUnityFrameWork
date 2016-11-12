using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class IInputDispatcher 
{
    public InputEventCallBack m_OnAllEventDispatch;

    protected void AllEventDispatch(string eventName, IInputEventBase inputEvent)
    {
        if (m_OnAllEventDispatch != null)
        {
            Delegate[] eventArray = m_OnAllEventDispatch.GetInvocationList();

            for (int i = 0; i < eventArray.Length; i++)
            {
                try
                {

                    
                    //eventArray[i](eventName, inputEvent);
                    InputEventCallBack callBack = (InputEventCallBack)eventArray[i];

                    Debug.Log(callBack);
                    Debug.Log(inputEvent);
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
