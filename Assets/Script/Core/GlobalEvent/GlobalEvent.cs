using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void EventHandle(params object[] args);

public class GlobalEvent {

    private static Dictionary<Enum, List<EventHandle>> mEventDic = new Dictionary<Enum, List<EventHandle>>();
    private static Dictionary<Enum, List<EventHandle>> mUseOnceEventDic = new Dictionary<Enum, List<EventHandle>>();

    private static Dictionary<string, List<EventHandle>> m_stringEventDic = new Dictionary<string, List<EventHandle>>();
    private static Dictionary<string, List<EventHandle>> m_stringOnceEventDic = new Dictionary<string, List<EventHandle>>();
    /// <summary>
    /// 添加事件及回调
    /// </summary>
    /// <param name="type">事件枚举</param>
    /// <param name="handle">回调</param>
    /// <param name="isUseOnce"></param>
    public static void AddEvent(Enum type, EventHandle handle, bool isUseOnce = false)
    {
        if (isUseOnce)
        {
            if (mUseOnceEventDic.ContainsKey(type))
            {
                if (!mUseOnceEventDic[type].Contains(handle))
                    mUseOnceEventDic[type].Add(handle);
                else
                    Debug.LogWarning("already existing EventType: " + type + " handle: " + handle);
            }
            else
            {
                List<EventHandle> temp = new List<EventHandle>();
                temp.Add(handle);
                mUseOnceEventDic.Add(type, temp);
            }
        }
        else
        {
            if (mEventDic.ContainsKey(type))
            {
                if (!mEventDic[type].Contains(handle))
                    mEventDic[type].Add(handle);
                else
                    Debug.LogWarning("already existing EventType: "+ type+" handle: "+ handle );
            }
            else
            {
                List<EventHandle> temp = new List<EventHandle>();
                temp.Add(handle);
                mEventDic.Add(type, temp);
            }        
        }
    }

    /// <summary>
    /// 添加事件及回调
    /// </summary>
    /// <param name="type">事件枚举</param>
    /// <param name="handle">回调</param>
    /// <param name="isUseOnce"></param>
    public static void AddEvent(string eventKey, EventHandle handle, bool isUseOnce = false)
    {
        if (isUseOnce)
        {
            if (m_stringOnceEventDic.ContainsKey(eventKey))
            {
                if (!m_stringOnceEventDic[eventKey].Contains(handle))
                    m_stringOnceEventDic[eventKey].Add(handle);
                else
                    Debug.LogWarning("already existing EventType: " + eventKey + " handle: " + handle);
            }
            else
            {
                List<EventHandle> temp = new List<EventHandle>();
                temp.Add(handle);
                m_stringOnceEventDic.Add(eventKey, temp);
            }
        }
        else
        {
            if (m_stringEventDic.ContainsKey(eventKey))
            {
                if (!m_stringEventDic[eventKey].Contains(handle))
                    m_stringEventDic[eventKey].Add(handle);
                else
                    Debug.LogWarning("already existing EventType: " + eventKey + " handle: " + handle);
            }
            else
            {
                List<EventHandle> temp = new List<EventHandle>();
                temp.Add(handle);
                m_stringEventDic.Add(eventKey, temp);
            }
        }
    }

    /// <summary>
    /// 移除某类事件的一个回调
    /// </summary>
    /// <param name="type"></param>
    /// <param name="handle"></param>
    public static void RemoveEvent(Enum type, EventHandle handle)
    {
            if (mUseOnceEventDic.ContainsKey(type))
            {
                if (mUseOnceEventDic[type].Contains(handle))
                {
                    mUseOnceEventDic[type].Remove(handle);
                    if (mUseOnceEventDic[type].Count == 0)
                    {
                        mUseOnceEventDic.Remove(type);
                    }
                }
            }

            if (mEventDic.ContainsKey(type))
            {
                if (mEventDic[type].Contains(handle))
                {
                    mEventDic[type].Remove(handle);
                    if (mEventDic[type].Count == 0)
                    {
                        mEventDic.Remove(type);
                    }
                }
            }
    }

    /// <summary>
    /// 移除某类事件的一个回调
    /// </summary>
    /// <param name="type"></param>
    /// <param name="handle"></param>
    public static void RemoveEvent(string eventKey, EventHandle handle)
    {
        if (m_stringEventDic.ContainsKey(eventKey))
        {
            if (m_stringEventDic[eventKey].Contains(handle))
            {
                m_stringEventDic[eventKey].Remove(handle);
                //if (m_stringEventDic[eventKey].Count == 0)
                //{
                //    m_stringEventDic.Remove(eventKey);
                //}
            }
        }

        if (m_stringOnceEventDic.ContainsKey(eventKey))
        {
            if (m_stringOnceEventDic[eventKey].Contains(handle))
            {
                m_stringOnceEventDic[eventKey].Remove(handle);
                //if (m_stringOnceEventDic[eventKey].Count == 0)
                //{
                //    m_stringOnceEventDic.Remove(eventKey);
                //}
            }
        }
    }

    /// <summary>
    /// 移除某类事件
    /// </summary>
    /// <param name="type"></param>
    public static void RemoveEvent(Enum type)
    {   
            if (mUseOnceEventDic.ContainsKey(type))
            {

                mUseOnceEventDic.Remove(type);
            }
   
            if (mEventDic.ContainsKey(type))
            {

                mEventDic.Remove(type);
            }
    }

    /// <summary>
    /// 移除某类事件
    /// </summary>
    /// <param name="eventKey"></param>
    public static void RemoveEvent(string eventKey)
    {
        if (m_stringEventDic.ContainsKey(eventKey))
        {

            m_stringEventDic.Remove(eventKey);
        }

        if (m_stringOnceEventDic.ContainsKey(eventKey))
        {

            m_stringOnceEventDic.Remove(eventKey);
        }
    }

    /// <summary>
    /// 移除所有事件
    /// </summary>
    public static void RemoveAllEvent()
    {   
        mUseOnceEventDic.Clear();

        mEventDic.Clear();

        m_stringEventDic.Clear();
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    public static void DispatchEvent(Enum type, params object[] args)
    {
        if (mEventDic.ContainsKey(type))
        {
            for (int i = 0; i < mEventDic[type].Count; i++)
            {
                //遍历委托链表
                foreach (EventHandle callBack in mEventDic[type][i].GetInvocationList())
                {
                    try
                    {
                        callBack(args);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            }
        }

        if (mUseOnceEventDic.ContainsKey(type))
        {
            for (int i = 0; i < mUseOnceEventDic[type].Count; i++)
            {
                //遍历委托链表
                foreach (EventHandle callBack in mUseOnceEventDic[type][i].GetInvocationList())
                {
                    try
                    {
                        callBack(args);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            }
            RemoveEvent(type);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="eventKey"></param>
    /// <param name="args"></param>
    public static void DispatchEvent(string eventKey, params object[] args)
    {
        if (m_stringEventDic.ContainsKey(eventKey))
        {
            for (int i = 0; i < m_stringEventDic[eventKey].Count; i++)
            {
                //遍历委托链表
                foreach (EventHandle callBack in m_stringEventDic[eventKey][i].GetInvocationList())
                {
                    try
                    {
                        callBack(args);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            }
        }

        if (m_stringOnceEventDic.ContainsKey(eventKey))
        {
            for (int i = 0; i < m_stringOnceEventDic[eventKey].Count; i++)
            {
                //遍历委托链表
                foreach (EventHandle callBack in m_stringOnceEventDic[eventKey][i].GetInvocationList())
                {
                    try
                    {
                        callBack(args);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            }
            RemoveEvent(eventKey);
        }
    }
	
}

public class EventHandRegisterInfo
{
    public Enum m_EventKey;
    public EventHandle m_hande;

    public void RemoveListener()
    {
        GlobalEvent.RemoveEvent(m_EventKey, m_hande);
    }
}
