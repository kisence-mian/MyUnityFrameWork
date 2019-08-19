using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ApplicationStatusManager 
{
    /// <summary>
    /// 当前程序在哪个状态
    /// </summary>
    public static IApplicationStatus s_currentAppStatus;
    public static string s_currentAppStatusName;

    public static CallBack<IApplicationStatus> OnStatusChangeCallBack;

    //可切换状态
    static Dictionary<string,IApplicationStatus> s_status = new Dictionary<string,IApplicationStatus>();

    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += AppUpdate;
        ApplicationManager.s_OnApplicationOnGUI += AppOnGUI;
    }

    private static void AppOnGUI()
    {
        if (s_currentAppStatus != null)
            s_currentAppStatus.OnGUI();
    }
    private static float s_fadeInTime = 0.5f;
    private static float s_afterInDelayTime = 0.1f;
    private static float s_fadeOutTime = 0.4f;
    public static void SetFadeTime(float _fadeInTime, float afterInDelayTime, float _fadeOutTime)
    {
        s_fadeInTime = _fadeInTime;
        s_afterInDelayTime = afterInDelayTime;
        s_fadeOutTime = _fadeOutTime;
    }
    /// <summary>
    /// 切换游戏状态(当当前游戏状态和要进入的游戏状态相同则不执行)
    /// </summary>
    /// <param name="l_appStatus"></param>
    public static void EnterStatus<T>(bool isFade = true) where T:IApplicationStatus
    {
        string statusName = typeof(T).Name;

        EnterStatus(statusName, isFade);
    }
    /// <summary>
    /// 切换游戏状态(当当前游戏状态和要进入的游戏状态相同则也执行)
    /// </summary>
    /// <param name="l_appStatus"></param>
    public static void ForceEnterStatus<T>(bool isFade = true) where T : IApplicationStatus
    {
        string statusName = typeof(T).Name;
        EnterStatusLogic(statusName, isFade);
    }

    public static void EnterStatus(string statusName, bool isFade = true)
    {
        if (s_currentAppStatusName == statusName)
            return;
        EnterStatusLogic(statusName, isFade);
    }
    private  static void EnterStatusLogic(string statusName,bool isFade = true)
    {
       

        if (!isFade)
        {
            if (s_currentAppStatus != null)
            {
                s_currentAppStatus.CloseAllUI();
                try
                {
                    s_currentAppStatus.OnExitStatus();
                    if (MemoryManager.NeedReleaseMemory())
                        MemoryManager.FreeMemory();
                }
                catch (Exception e)
                {
                    Debug.LogError("EnterStatus Exception " + statusName + " " + e.ToString());
                }
            }

            s_currentAppStatusName = statusName;
            ApplicationManager.Instance.currentStatus = statusName;

            s_currentAppStatus = GetStatus(statusName);

            try
            {
                s_currentAppStatus.OnEnterStatus();
            }
            catch (Exception e)
            {
                Debug.LogError("EnterStatus Exception " + statusName + " " + e.ToString());
            }

            try
            {
                if (OnStatusChangeCallBack != null)
                OnStatusChangeCallBack(s_currentAppStatus);
            }
            catch (Exception e)
            {
                Debug.LogError("EnterStatus Exception " + statusName + " " + e.ToString());
            }
        }
        else
        {
            if (s_currentAppStatus != null)
            {
                UIManager.SetEventSystemEnable(false);
                CameraFade.FadeInToOut(s_fadeInTime, s_afterInDelayTime, s_fadeOutTime, () =>
                {
                    UIManager.SetEventSystemEnable(true);
                    s_currentAppStatus.CloseAllUI(false);

                    try
                    {
                        s_currentAppStatus.OnExitStatus();
                    }catch(Exception e)
                    {
                        Debug.LogError("OnExitStatus Exception " + statusName + " " + e.ToString());
                    }
                    
                    if (MemoryManager.NeedReleaseMemory())
                        MemoryManager.FreeMemory();

                    s_currentAppStatusName = statusName;
                    ApplicationManager.Instance.currentStatus = statusName;

                    s_currentAppStatus = GetStatus(statusName);

                    try
                    {
                        s_currentAppStatus.OnEnterStatus();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("EnterStatus Exception " + statusName + " " + e.ToString());
                    }

                    try
                    {
                        if (OnStatusChangeCallBack != null)
                            OnStatusChangeCallBack(s_currentAppStatus);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("EnterStatus Exception " + statusName + " " + e.ToString());
                    }
                });
            }
            else
            {
                s_currentAppStatusName = statusName;
                ApplicationManager.Instance.currentStatus = statusName;

                s_currentAppStatus = GetStatus(statusName);

                try
                {
                    s_currentAppStatus.OnEnterStatus();
                }
                catch (Exception e)
                {
                    Debug.LogError("EnterStatus Exception " + statusName + " " + e.ToString());
                }

                try
                {
                    if (OnStatusChangeCallBack != null)
                    OnStatusChangeCallBack(s_currentAppStatus);
                }
                catch (Exception e)
                {
                    Debug.LogError("EnterStatus Exception " + statusName + " " + e.ToString());
                }
            }
        }
    }

    public static T GetStatus<T>() where T : IApplicationStatus
    {
        return (T)GetStatus(typeof(T).Name);
    }

    public static IApplicationStatus GetStatus(string statusName)
    {
        if (s_status.ContainsKey(statusName))
        {
            return s_status[statusName];
        }
        else
        {
            return CreateStatus(statusName);
        }
    }

    public static T CreateStatus<T>() where T : IApplicationStatus
    {
        return (T)CreateStatus(typeof(T).Name);
    }

    public static IApplicationStatus CreateStatus(string statusName)
    {
        IApplicationStatus statusTmp=null;
        if (!s_status.ContainsKey(statusName))
        {
             statusTmp = (IApplicationStatus)Activator.CreateInstance(Type.GetType(statusName));
            statusTmp.OnCreate();
            s_status.Add(statusName, statusTmp);
        }
        else
        {
            statusTmp= s_status[statusName];
        }
        return statusTmp;
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
    public static void EnterTestModel<T>() where T : IApplicationStatus
    {
        EnterTestModel(typeof(T).Name);
    }

    public static void EnterTestModel(string statusName)
    {
        if (s_currentAppStatus != null)
        {
            s_currentAppStatus.CloseAllUI();
            s_currentAppStatus.OnExitStatus();
        }

        s_currentAppStatus = GetStatus(statusName);

        if(ApplicationManager.AppMode != AppMode.Release)
        {
            s_currentAppStatus.EnterStatusTestData();
        }

        s_currentAppStatus.OnEnterStatus();


        //ApplicationManager.Instance.StartCoroutine(s_currentAppStatus.InChangeScene(()=>{
        //    s_currentAppStatus.EnterStatusTestData();
        //    s_currentAppStatus.OnEnterStatus();
        //}));
    }
}
