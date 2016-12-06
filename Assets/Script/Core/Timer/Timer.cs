using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Timer : MonoBehaviour 
{
    static Timer s_instance;

    public static Timer s_Instance
    {
        get {
            if (s_instance == null)
            {
                s_instance = new GameObject("Timer").AddComponent<Timer>();
                DontDestroyOnLoad(s_instance.gameObject);
            }
            return s_instance;
        }
        set { s_instance = value; }
    }

    public List<TimerEvent> m_timers = new List<TimerEvent>();

	void Update () 
    {
        for (int i = 0; i < m_timers.Count;i++ )
        {
            m_timers[i].Update();

            if(m_timers[i].m_isDone)
            {
                TimerEvent e = m_timers[i];

                if (m_timers[i].m_repeatCount == 0)
                {
                    m_timers.Remove(m_timers[i]);
                    i--;
                }

                e.CompleteTimer();
            }
        }
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
    /// <param name="l_spaceTime">间隔时间</param>
    /// <param name="l_isIgnoreTimeScale">是否忽略时间缩放</param>
    /// <param name="l_callBackCount">重复调用的次数</param>
    /// <param name="l_timerName">Timer的名字</param>
    /// <param name="l_callBack">回调函数</param>
    /// <param name="l_objs">回调函数的参数</param>
    /// <returns></returns>
    public static TimerEvent AddTimer(float l_spaceTime, bool l_isIgnoreTimeScale, int l_callBackCount, string l_timerName,TimerCallBack l_callBack, params object[] l_objs)
    {
        TimerEvent l_te = new TimerEvent();

        l_te.m_timerName = l_timerName;

        l_te.m_currentTimer = 0;
        l_te.m_timerSpace = l_spaceTime;

        l_te.m_callBack = l_callBack;
        l_te.m_objs = l_objs;

        l_te.m_isIgnoreTimeScale = l_isIgnoreTimeScale;
        l_te.m_repeatCount = l_callBackCount;

        s_Instance.m_timers.Add(l_te);

        return l_te;
    }

    public static void DestroyTimer(TimerEvent l_timer)
    {
        if(s_instance.m_timers.Contains(l_timer))
        {
            s_instance.m_timers.Remove(l_timer);
        }
        else
        {
            Debug.LogError("Timer DestroyTimer error: dont exist timer " + l_timer);
        }
    }

    public static void DestroyTimer(string l_timerName)
    {
        for (int i = 0; i < s_instance.m_timers.Count;i++ )
        {
            if (s_instance.m_timers[i].m_timerName.Equals(l_timerName))
            {
                DestroyTimer(s_instance.m_timers[i]);
            }
        }
    }

    public static void DestroyAllTimer(string l_timerName)
    {
        for (int i = 0; i < s_instance.m_timers.Count; i++)
        {
            DestroyTimer(s_instance.m_timers[i]);
        }
    }

    public static void ResetTimer(TimerEvent l_timer)
    {
        if(s_instance.m_timers.Contains(l_timer))
        {
            l_timer.ResetTimer();
        }
        else
        {
            Debug.LogError("Timer ResetTimer error: dont exist timer "+ l_timer);
        }
    }

    public static void ResetTimer(string l_timerName)
    {
        for (int i = 0; i < s_instance.m_timers.Count; i++)
        {
            if (s_instance.m_timers[i].m_timerName.Equals(l_timerName))
            {
                ResetTimer(s_instance.m_timers[i]);
            }
        }
    }
}


