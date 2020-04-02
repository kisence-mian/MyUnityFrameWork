using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void EventHandle(params object[] args);
public delegate void EventHandle<T>(T e, params object[] args);

public class GlobalEvent
{
    #region 以Enum为Key的事件派发

    private static Dictionary<Enum, EventHandle> mEventDic = new Dictionary<Enum, EventHandle>();
    private static Dictionary<Enum, List<EventHandle>> mUseOnceEventDic = new Dictionary<Enum, List<EventHandle>>();

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
                mEventDic[type]+= handle;
            }
            else
            {
                mEventDic.Add(type, handle);
            }
        }
    }

    internal static void AddTypeEvent<T>(string v)
    {
        throw new NotImplementedException();
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
            mEventDic[type]-= handle;
        }
    }

    internal static void AddTypeEvent<T>()
    {
        throw new NotImplementedException();
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
    /// 触发事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    public static void DispatchEvent(Enum type, params object[] args)
    {
        if (mEventDic.ContainsKey(type))
        {
            try
            {
                if (mEventDic[type] != null)
                {
                    mEventDic[type](args);
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
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

    #endregion

    #region 以String为Key的事件派发
    private static Dictionary<string, List<EventHandle>> m_stringEventDic = new Dictionary<string, List<EventHandle>>();
    private static Dictionary<string, List<EventHandle>> m_stringOnceEventDic = new Dictionary<string, List<EventHandle>>();

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

    internal static void AddEvent<T>(object onRequestRealNameResult)
    {
        throw new NotImplementedException();
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
    #endregion

    #region 以Type为Key的事件派发

    private static Dictionary<Type, EventDispatcher> mTypeEventDic        = new Dictionary<Type, EventDispatcher>();
    private static Dictionary<Type, EventDispatcher> mTypeUseOnceEventDic = new Dictionary<Type, EventDispatcher>();

    /// <summary>
    /// 添加事件及回调
    /// </summary>
    /// <param name="type">事件枚举</param>
    /// <param name="handle">回调</param>
    /// <param name="isUseOnce"></param>
    public static void AddTypeEvent<T>( EventHandle<T> handle, bool isUseOnce = false)
    {
        GetEventDispatcher<T>(isUseOnce).m_CallBack += handle;
    }

    /// <summary>
    /// 移除某类事件的一个回调
    /// </summary>
    /// <param name="type"></param>
    /// <param name="handle"></param>
    public static void RemoveTypeEvent<T>(EventHandle<T> handle, bool isUseOnce = false)
    {
        GetEventDispatcher<T>(isUseOnce).m_CallBack -= handle;
    }

    /// <summary>
    /// 移除某类事件
    /// </summary>
    /// <param name="type"></param>
    public static void RemoveTypeEvent<T>(bool isUseOnce = false)
    {
        GetEventDispatcher<T>(isUseOnce).m_CallBack = null;
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    public static void DispatchTypeEvent<T>(T e, params object[] args)
    {
        GetEventDispatcher<T>(false).Call(e,args);

        //只调用一次的调用后就清除
        GetEventDispatcher<T>(true).Call(e, args);
        GetEventDispatcher<T>(true).m_CallBack = null;
    }

    static EventDispatcher<T> GetEventDispatcher<T>(bool isOnce)
    {
        Type type = typeof(T);

        if (isOnce)
        {
            if (mTypeUseOnceEventDic.ContainsKey(type))
            {
                return (EventDispatcher<T>)mTypeUseOnceEventDic[type];
            }
            else
            {
                EventDispatcher<T> temp = new EventDispatcher<T>();
                mTypeUseOnceEventDic.Add(type, temp);
                return temp;
            }
        }
        else
        {
            if (mTypeEventDic.ContainsKey(type))
            {
                return (EventDispatcher<T>)mTypeEventDic[type];
            }
            else
            {
                EventDispatcher<T> temp = new EventDispatcher<T>();
                mTypeEventDic.Add(type, temp);
                return temp;
            }
        }
    }

    abstract class EventDispatcher{}

    class EventDispatcher<T> : EventDispatcher
    {
        public EventHandle<T> m_CallBack;

        public void Call(T e,params object[] args)
        {
            if(m_CallBack != null)
            {
                try
                {
                    m_CallBack(e, args);
                }
                catch(Exception exc)
                {
                    Debug.LogError(exc.ToString());
                }
            }
        }
    }

        #endregion
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
