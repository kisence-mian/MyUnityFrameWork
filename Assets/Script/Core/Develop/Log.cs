using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Text;
using System;

public class Log 
{
    //日志输出线程
    static LogOutPutThread s_LogOutPutThread = new LogOutPutThread();
    public static void Init(bool isOpenLog = true)
    {
        if (Application.platform != RuntimePlatform.WindowsEditor &&
            Application.platform != RuntimePlatform.LinuxEditor)
        {
            int status = PlayerPrefs.GetInt("Log", -1);
            if (status != -1)
            {
                isOpenLog = status == 1 ? true : false;
            }
        }
        PlayerPrefs.SetInt("Log", (isOpenLog ? 1 : 0));

        if (isOpenLog)
        {
            s_LogOutPutThread.Init();
            ApplicationManager.s_OnApplicationQuit += OnApplicationQuit;
            Application.logMessageReceivedThreaded += UnityLogCallBackThread;
            Application.logMessageReceived += UnityLogCallBack;
        }
    }

    private static void OnApplicationQuit()
    {
        Application.logMessageReceivedThreaded -= UnityLogCallBackThread;
        Application.logMessageReceived -= UnityLogCallBack;
        s_LogOutPutThread.Close();
    }

    static void UnityLogCallBackThread(string log, string track, LogType type)
    {
        LogInfo l_logInfo = new LogInfo
        {
            m_logContent = log,
            m_logTrack = track,
            m_logType = type
        };

        s_LogOutPutThread.Log(l_logInfo);
    }

    static void UnityLogCallBack(string log, string track, LogType type)
    {
    }
}

public class LogInfo
{
    public string m_logContent;
    public string m_logTrack;
    public LogType m_logType;
}
