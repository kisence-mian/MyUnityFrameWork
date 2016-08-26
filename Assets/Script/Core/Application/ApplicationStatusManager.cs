using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ApplicationStatusManager 
{
    /// <summary>
    /// 当前程序在哪个状态
    /// </summary>
    public static IApplicationStatus s_currentAppStatus;
    static List<IApplicationStatus> s_statusList = new List<IApplicationStatus>();

    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += AppUpdate;
    }

    /// <summary>
    /// 切换游戏状态
    /// </summary>
    /// <param name="l_appStatus"></param>
    public static void EnterStatus<T>() where T:IApplicationStatus,new()
    {
        if (s_currentAppStatus!=null)
        {
            s_currentAppStatus.OnExitStatus();
        }

        s_currentAppStatus = GetAppStatus<T>();
        s_currentAppStatus.OnEnterStatus();
    }


    public static IApplicationStatus GetAppStatus<T>() where T:IApplicationStatus,new()
    {
        for (int i = 0; i < s_statusList.Count;i++ )
        {
            if((s_statusList[i] is T) != true)
            {
                return s_statusList[i];
            }
        }

        IApplicationStatus l_statusTmp = new T();
        s_statusList.Add(l_statusTmp);

        return l_statusTmp;
    }

    /// <summary>
    /// 应用程序每帧调用
    /// </summary>
    public static void AppUpdate()
    {
        if(s_currentAppStatus != null)
        {
            s_currentAppStatus.OnUpdate();
        }
    }

    /// <summary>
    /// 测试模式，流程入口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void EnterTestModel<T>() where T : IApplicationStatus, new()
    {
        if (s_currentAppStatus != null)
        {
            s_currentAppStatus.OnExitStatus();
        }

        s_currentAppStatus = GetAppStatus<T>();
        s_currentAppStatus.EnterStatusTestData();
        s_currentAppStatus.OnEnterStatus();
    }
}
