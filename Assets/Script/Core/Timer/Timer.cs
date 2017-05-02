using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer 
{
    public static List<TimerEvent> m_timers = new List<TimerEvent>();

    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += Update;
    }

	static void Update () 
    {
        for (int i = 0; i < m_timers.Count;i++ )
        {
            m_timers[i].Update();

            if(m_timers[i].m_isDone)
            {
                TimerEvent e = m_timers[i];

                e.CompleteTimer();

                if (e.m_repeatCount == 0)
                {
                    m_timers.Remove(e);
                    HeapObjectPool<TimerEvent>.PutObject(e);
                    i--;
                }
            }
        }
	}

    public static TimerEvent GetTimer(string timerName)
    {
        for (int i = 0; i < m_timers.Count; i++)
        {
            if (m_timers[i].m_timerName == timerName)
            {
                return m_timers[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 延迟调用
    /// </summary>
    /// <param name="l_delayTime">间隔时间</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent DelayCallBack(float l_delayTime,TimerCallBack l_callBack,params object[] l_objs)
    {
        return AddTimer(l_delayTime, false, 0, "", l_callBack, l_objs); 
    }

    /// <summary>
    /// 延迟调用
    /// </summary>
    /// <param name="l_delayTime">间隔时间</param>
    /// <param name="l_isIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent DelayCallBack(float l_delayTime, bool l_isIgnoreTimeScale, TimerCallBack l_callBack, params object[] l_objs)
    {
        return AddTimer(l_delayTime, l_isIgnoreTimeScale, 0, "", l_callBack, l_objs);
    }

    /// <summary>
    /// 间隔一定时间重复调用
    /// </summary>
    /// <param name="l_spaceTime">间隔时间</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent CallBackOfIntervalTimer(float l_spaceTime,TimerCallBack l_callBack, params object[] l_objs)
    {
        return AddTimer(l_spaceTime, false, -1, "", l_callBack, l_objs); 
    }

    /// <summary>
    /// 间隔一定时间重复调用
    /// </summary>
    /// <param name="l_spaceTime">间隔时间</param>
    /// <param name="l_isIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent CallBackOfIntervalTimer(float l_spaceTime, bool l_isIgnoreTimeScale, TimerCallBack l_callBack, params object[] l_objs)
    {
        return AddTimer(l_spaceTime, l_isIgnoreTimeScale, -1, "", l_callBack, l_objs);
    }

    /// <summary>
    /// 间隔一定时间重复调用
    /// </summary>
    /// <param name="l_spaceTime">间隔时间</param>
    /// <param name="l_isIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="l_timerName">Timer的名字</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent CallBackOfIntervalTimer(float l_spaceTime, bool l_isIgnoreTimeScale, string l_timerName,TimerCallBack l_callBack, params object[] l_objs)
    {
        return AddTimer(l_spaceTime, l_isIgnoreTimeScale, -1, l_timerName, l_callBack, l_objs);
    }

    /// <summary>
    /// 有限次数的重复调用
    /// </summary>
    /// <param name="l_spaceTime">间隔时间</param>
    /// <param name="l_callBackCount">重复调用的次数</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent CallBackOfIntervalTimer(float l_spaceTime, int l_callBackCount, TimerCallBack l_callBack, params object[] l_objs)
    {
        return AddTimer(l_spaceTime, false, -1, "",l_callBack, l_objs);
    }

    /// <summary>
    /// 有限次数的重复调用
    /// </summary>
    /// <param name="l_spaceTime">间隔时间</param>
    /// <param name="l_isIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="l_callBackCount">重复调用的次数</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent CallBackOfIntervalTimer(float l_spaceTime, bool l_isIgnoreTimeScale, int l_callBackCount, TimerCallBack l_callBack, params object[] l_objs)
    {
        return AddTimer(l_spaceTime, l_isIgnoreTimeScale, -1, "",l_callBack, l_objs); ;
    }

    /// <summary>
    /// 有限次数的重复调用
    /// </summary>
    /// <param name="l_spaceTime">间隔时间</param>
    /// <param name="l_isIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="l_callBackCount">重复调用的次数</param>
    /// <param name="l_timerName">Timer的名字</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent CallBackOfIntervalTimer(float l_spaceTime, bool l_isIgnoreTimeScale, int l_callBackCount, string l_timerName, TimerCallBack l_callBack, params object[] l_objs)
    {
        return AddTimer(l_spaceTime, l_isIgnoreTimeScale, -1, l_timerName, l_callBack, l_objs);
    }

    /// <summary>
    /// 添加一个Timer
    /// </summary>
    /// <param name="spaceTime">间隔时间</param>
    /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="callBackCount">重复调用的次数</param>
    /// <param name="timerName">Timer的名字</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent AddTimer(float spaceTime, bool isIgnoreTimeScale, int callBackCount, string timerName,TimerCallBack callBack, params object[] objs)
    {
        TimerEvent te = HeapObjectPool<TimerEvent>.GetObject();

        te.m_timerName = timerName;

        te.m_currentTimer = 0;
        te.m_timerSpace = spaceTime;

        te.m_callBack = callBack;
        te.m_objs = objs;

        te.m_isIgnoreTimeScale = isIgnoreTimeScale;
        te.m_repeatCount = callBackCount;

        m_timers.Add(te);

        return te;
    }

    public static void DestroyTimer(TimerEvent timer,bool isCallBack = false)
    {
        if(m_timers.Contains(timer))
        {
            if (isCallBack)
            {
                timer.CallBackTimer();
            }

            m_timers.Remove(timer);
            HeapObjectPool<TimerEvent>.PutObject(timer);
        }
        else
        {
            Debug.LogError("Timer DestroyTimer error: dont exist timer " + timer);
        }
    }

    public static void DestroyTimer(string timerName, bool isCallBack = false)
    {
        for (int i = 0; i < m_timers.Count;i++ )
        {
            if (m_timers[i].m_timerName.Equals(timerName))
            {
                DestroyTimer(m_timers[i], isCallBack);
            }
        }
    }

    public static void DestroyAllTimer(bool isCallBack = false)
    {
        for (int i = 0; i < m_timers.Count; i++)
        {
            if (isCallBack)
            {
                m_timers[i].CallBackTimer();
            }
            HeapObjectPool<TimerEvent>.PutObject(m_timers[i]);
        }

        m_timers.Clear();
    }

    public static void ResetTimer(TimerEvent timer)
    {
        if(m_timers.Contains(timer))
        {
            timer.ResetTimer();
        }
        else
        {
            Debug.LogError("Timer ResetTimer error: dont exist timer "+ timer);
        }
    }

    public static void ResetTimer(string timerName)
    {
        for (int i = 0; i < m_timers.Count; i++)
        {
            if (m_timers[i].m_timerName.Equals(timerName))
            {
                ResetTimer(m_timers[i]);
            }
        }
    }
}


