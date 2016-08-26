using UnityEngine;
using System.Collections;
using System;

public class ApplicationManager : MonoBehaviour 
{
    public AppMode m_AppMode = AppMode.Developing;

    public void Awake()
    {
        AppLaunch();
    }

    /// <summary>
    /// 程序启动
    /// </summary>
    public void AppLaunch()
    {
        SetResourceLoadType();            //设置资源加载类型
        Log.Init();                       //日志系统启动
        ApplicationStatusManager.Init();  //游戏流程初始化

        if (m_AppMode != AppMode.Release)
        {
            GUIConsole.Init();                                     //运行时Debug开启
            ApplicationStatusManager.EnterTestModel<GameStatus>(); //可以从此处进入测试流程
        }
        else
        {
            ApplicationStatusManager.EnterStatus<GameStatus>();
        }
    }

    #region 程序生命周期事件派发

    public static ApplicationCallback s_OnApplicationQuit = null;
    public static ApplicationCallback s_OnApplicationUpdate = null;
    public static ApplicationCallback s_OnApplicationOnGUI = null;

    void OnApplicationQuit()
    {
        if (s_OnApplicationQuit != null)
        {
            try
            {
                s_OnApplicationQuit();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    void Update()
    {
        if (s_OnApplicationUpdate != null)
            s_OnApplicationUpdate();
    }

    void OnGUI()
    {
        if (s_OnApplicationOnGUI != null)
            s_OnApplicationOnGUI();
    }

    #endregion

    #region 程序启动细节
    /// <summary>
    /// 设置资源加载方式
    /// </summary>
    void SetResourceLoadType()
    {
        if (m_AppMode == AppMode.Developing)
        {
            ResourceManager.gameLoadType = ResLoadType.Resource;
        }
        else
        {
            ResourceManager.gameLoadType = ResLoadType.HotUpdate;
        }
    }
    #endregion
}

public enum AppMode
{
    Developing,
    QA,
    Release
}

public delegate void ApplicationCallback();
