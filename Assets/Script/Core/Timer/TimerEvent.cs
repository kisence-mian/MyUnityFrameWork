using UnityEngine;
using System.Collections;
using System;

public class TimerEvent
{
    public string m_timerName = "";

    /// <summary>
    /// 重复调用次数,-1代表一直调用
    /// </summary>
    public int m_repeatCount   = 0;
    public int m_currentRepeat = 0;

    /// <summary>
    /// 是否忽略时间缩放
    /// </summary>
    public bool m_isIgnoreTimeScale = false;
    public TimerCallBack m_callBack;
    public object[] m_objs;

    public float m_timerSpace;
    public float m_currentTimer = 0;

    public bool m_isDone = false;

    public bool m_isStop = false;

    public void Update()
    {
        if (m_isIgnoreTimeScale)
        {
            m_currentTimer += Time.unscaledDeltaTime;
        }
        else
        {
            m_currentTimer += Time.deltaTime;
        }

        if (m_currentTimer >= m_timerSpace)
        {
            m_isDone = true;
        }
    }

    public void CompleteTimer()
    {
        CallBackTimer();
        
        if(m_repeatCount > 0)
        {
            m_currentRepeat++;
        }

        if (m_currentRepeat != m_repeatCount)
        {
            m_isDone = false;
            m_currentTimer = 0;
        }
    }

    public void CallBackTimer()
    {
        if(this == Timer.test)
        {
            Debug.Log("CallBackTimer " + (m_callBack == null));
        }

        if (m_callBack != null)
        {
            try
            {
                m_callBack(m_objs);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    public void ResetTimer()
    {
        m_currentTimer = 0;
        m_currentRepeat = 0;
    }

    public void OnInit() { }

    public void OnPop()
    {
        m_timerName = "";
        m_repeatCount = 0;
        m_currentRepeat = 0;
        m_isIgnoreTimeScale = false;
        m_callBack = null;
        m_objs = null;
        m_timerSpace = 0; ;
        m_currentTimer = 0;
        m_isDone = false;
        m_isStop = false;
    }

    public void OnPush()
    {
        m_isStop = true;
    }
}

public delegate void TimerCallBack(params object[] l_objs);