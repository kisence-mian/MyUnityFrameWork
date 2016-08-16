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
    /// 请把游戏初始化逻辑放在此处
    /// </summary>
    void GameStart()
    {

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

    /// <summary>
    /// 程序启动
    /// </summary>
    public void AppLaunch()
    {
        SetResourceLoadType();
        Log.Init(); //日志系统启动

        if(m_AppMode != AppMode.Release)
        {
            GUIConsole.Init();
        }

        //等待热更新
        HotUpdate();
    }

    /// <summary>
    /// 资源热更新
    /// </summary>
    public void HotUpdate()
    {
        CompleteHotUpdate();
    }

    /// <summary>
    /// 热更新完毕再调用这个方法
    /// </summary>
    public void CompleteHotUpdate()
    {
        BundleConfigManager.Initialize();
        UIManager.Init();

        //游戏开始逻辑放在此处
        GameStart();
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
