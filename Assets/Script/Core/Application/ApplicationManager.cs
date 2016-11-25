using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour 
{
    public AppMode m_AppMode = AppMode.Developing;

    private static ApplicationManager instance;

    public static ApplicationManager Instance
    {
        get { return ApplicationManager.instance; }
        set { ApplicationManager.instance = value; }
    }

    //快速启动
    public bool m_quickLunch = true;

    [HideInInspector]
    public string m_Status = "";

    [HideInInspector]
    public List<string> m_globalLogic;

    public void Awake()
    {
        instance = this;
        AppLaunch();
    }

    /// <summary>
    /// 程序启动
    /// </summary>
    public void AppLaunch()
    {
        DontDestroyOnLoad(this);
        SetResourceLoadType();               //设置资源加载类型
        ResourcesConfigManager.Initialize(); //资源路径管理器启动

        InputManager.Init();                 //输入管理器启动
        UIManager.Init();                    //UIManager启动

        ApplicationStatusManager.Init();     //游戏流程状态机初始化

        //初始化全局逻辑
        GlobalLogicManager.Init();

        if (m_AppMode != AppMode.Release)
        {
            GUIConsole.Init(); //运行时Console

            DevelopReplayManager.OnLunchCallBack += () =>
            {
                LuaManager.Init();
                InitGlobalLogic();//全局逻辑
                ApplicationStatusManager.EnterTestModel(m_Status);//可以从此处进入测试流程
            };

            DevelopReplayManager.Init(m_quickLunch);   //开发者复盘管理器                              
        }
        else
        {
            Log.Init(false); //关闭 Debug

            LuaManager.Init();
            //全局逻辑
            InitGlobalLogic();
            //游戏流程状态机，开始第一个状态
            ApplicationStatusManager.EnterStatus(m_Status);
        }
    }

    #region 程序生命周期事件派发

        
    public static ApplicationVoidCallback s_OnApplicationQuit = null;
    public static ApplicationBoolCallback s_OnApplicationPause = null;
    public static ApplicationBoolCallback s_OnApplicationFocus = null;
    public static ApplicationVoidCallback s_OnApplicationUpdate = null;
    public static ApplicationVoidCallback s_OnApplicationOnGUI = null;

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

    /*
     * 强制暂停时，先 OnApplicationPause，后 OnApplicationFocus
     * 重新“启动”游戏时，先OnApplicationFocus，后 OnApplicationPause
     */
    void OnApplicationPause(bool pauseStatus)
    {
        if (s_OnApplicationPause != null)
        {
            try
            {
                s_OnApplicationPause(pauseStatus);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

    void OnApplicationFocus(bool focusStatus)
    {
        if (s_OnApplicationFocus != null)
        {
            try
            {
                s_OnApplicationFocus(focusStatus);
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
            ResourceManager.m_gameLoadType = ResLoadType.Resource;
        }
        else
        {
            ResourceManager.m_gameLoadType = ResLoadType.HotUpdate;
        }
    }

    /// <summary>
    /// 初始化全局逻辑
    /// </summary>
    void InitGlobalLogic()
    {


        for (int i = 0; i < m_globalLogic.Count; i++)
        {
            GlobalLogicManager.InitLogic(m_globalLogic[i]);
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

public delegate void ApplicationBoolCallback(bool status);
public delegate void ApplicationVoidCallback();
