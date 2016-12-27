using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    static Dictionary<string, EventPool> m_eventPool = new Dictionary<string, EventPool>();

    public static T GetEvent<T>() where T : IInputEventBase, new()
    {
        string cmdKey = typeof(T).Name;

        if (!m_eventPool.ContainsKey(cmdKey))
        {
            EventPool pool = new EventPool();
            pool.Init<T>(3);

            m_eventPool.Add(cmdKey, pool);
        }


        return (T)m_eventPool[cmdKey].GetEvent();
    }

    class EventPool
    {
        IInputEventBase[] m_pool;
        int m_poolIndex = 0;

        public void Init<T>(int size) where T : IInputEventBase, new()
        {
            m_pool = new T[size];
            for (int i = 0; i < size; i++)
            {
                m_pool[i] = new T();
            }
        }

        public IInputEventBase GetEvent()
        {
            IInputEventBase cmd = m_pool[m_poolIndex];
            cmd.Reset();

            m_poolIndex++;

            if (m_poolIndex >= m_pool.Length)
            {
                m_poolIndex = 0;
            }

            return cmd;
        }
    }

    #endregion
}

public class InputEventRegisterInfo<T> where T: IInputEventBase
{
    public string eventKey;
    public InputEventHandle<T> callBack;

    public void AddListener()
    {
        InputManager.AddListener<T>(eventKey, callBack);
    }

    public virtual void RemoveListener()
    {
        InputManager.RemoveListener<T>(eventKey, callBack);
    }

    
}
