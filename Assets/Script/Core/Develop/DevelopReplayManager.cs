using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using MiniJSON;

public class DevelopReplayManager 
{
    const string c_directoryName     = "DevelopReplay";
    public const string c_expandName = "json";

    const string c_recordName    = "DevelopReplay";
    const string c_qucikLunchKey = "qucikLunch";

    const string c_eventStreamKey = "EventStream";
    const string c_randomListKey = "RandomList";

    const string c_eventNameKey = "e";
    const string c_serializeInfoKey = "s";

    static bool s_isReplay = false;
    static List<IInputEventBase> s_eventStream;

    static List<int> s_randomList;

    private static Action s_onLunchCallBack;

    public static Action OnLunchCallBack
    {
        get { return s_onLunchCallBack; }
        set { s_onLunchCallBack = value; }
    }

    public static void Init(bool isQuickLunch)
    {
        if (isQuickLunch)
        {
            //复盘模式可以手动开启
            if (!RecordManager.GetData(c_recordName).GetRecord(c_qucikLunchKey, true))
            {
                isQuickLunch = false;
            }
        }

        if (isQuickLunch)
        {
            ChoseReplayMode(false);
        }
        else
        {
            Time.timeScale = 0;
            ApplicationManager.s_OnApplicationOnGUI += ReplayMenuGUI;
        }
    }

    static void ChoseReplayMode(bool isReplay,string replayFileName = null)
    {
        s_isReplay = isReplay;

        if (s_isReplay)
        {
            LoadReplayFile(replayFileName);
            ApplicationManager.s_OnApplicationUpdate += OnReplayUpdate;
            GUIConsole.onGUICallback += ReplayModeGUI;

            //传入随机数列
            RandomService.SetRandomList(s_randomList);

            //关闭正常输入，保证回放数据准确
            InputUIEventProxy.IsActive = false;
            InputOperationEventProxy.IsActive = false;
            InputNetworkEventProxy.IsActive = false;
        }
        else
        {
            Log.Init(true); //日志记录启动

            s_eventStream = new List<IInputEventBase>();
            s_randomList = new List<int>();

            ApplicationManager.s_OnApplicationUpdate += OnRecordUpdate;
            InputManager.OnEveryEventDispatch += OnEveryEventCallBack;
            GUIConsole.onGUICallback += RecordModeGUI;
            RandomService.OnRandomCreat += OnGetRandomCallBack;
            SaveFileName = GetLogFileName();
        }

        ApplicationManager.s_OnApplicationOnGUI -= ReplayMenuGUI;

        Time.timeScale = 1;

        try
        {
            s_onLunchCallBack();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public static void OnEveryEventCallBack(string eventName, IInputEventBase inputEvent)
    {
        s_eventStream.Add(inputEvent);
    }

    public static void OnGetRandomCallBack(int random)
    {
        s_randomList.Add(random);
    }



    #region SaveReplayFile


    public static void SaveReplayFile(string fileName)
    {
        List<Dictionary<string,string>> EventStreamContent = SaveEventStream();
        List<int> randomListContent = SaveRandomList();

        Dictionary<string, object> replayInfo = new Dictionary<string, object>();

        replayInfo.Add(c_eventStreamKey, EventStreamContent);
        replayInfo.Add(c_randomListKey, randomListContent);

        string content = Json.Serialize(replayInfo);

        ResourceIOTool.WriteStringByFile(
                PathTool.GetAbsolutePath(ResLoadType.Persistent,
                                         PathTool.GetRelativelyPath(
                                                        c_directoryName,
                                                        fileName,
                                                        c_expandName))
                , content);
    }

    static List<Dictionary<string, string>> SaveEventStream()
    {
        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

        for (int i = 0; i < s_eventStream.Count; i++)
        {
            Dictionary<string, string> tmp = new Dictionary<string, string>();

            tmp.Add(c_eventNameKey, s_eventStream[i].GetType().Name);
            tmp.Add(c_serializeInfoKey,s_eventStream[i].Serialize());

            list.Add(tmp);
        }

        return list;
    }

    static List<int> SaveRandomList()
    {
        List<int> list = new List<int>();

        for (int i = 0; i < s_randomList.Count; i++)
        {
            list.Add(s_randomList[i]);
        }

        return list;
    }

    #endregion 

    #region LoadReplayFile

    public static void LoadReplayFile(string fileName)
    {
        string content = ResourceIOTool.ReadStringByFile(
            PathTool.GetAbsolutePath(ResLoadType.Persistent,
                                     PathTool.GetRelativelyPath(
                                                    c_directoryName,
                                                    fileName,
                                                    c_expandName)));

        Dictionary<string, object> replayInfo = (Dictionary<string, object>)Json.Deserialize(content);

        LoadEventStream((List<object>)replayInfo[c_eventStreamKey]);
        LoadRandomList((List<object>)replayInfo[c_randomListKey]);
    }

    static void LoadEventStream(List<object> content)
    {
        s_eventStream = new List<IInputEventBase>();

        for (int i = 0; i < content.Count; i++)
        {
            Dictionary<string, object> info = (Dictionary<string, object>)(content[i]);

            //Debug.Log(info[c_eventNameKey].ToString());
            //Debug.Log(info[c_eventNameKey].ToString());

            IInputEventBase eTmp = (IInputEventBase)JsonUtility.FromJson(info[c_serializeInfoKey].ToString(), Type.GetType(info[c_eventNameKey].ToString()));
            s_eventStream.Add(eTmp);
        }
    }

    static void LoadRandomList(List<object> content)
    {
        s_randomList = new List<int>();

        for (int i = 0; i < content.Count; i++)
        {
            s_randomList.Add(int.Parse(content[i].ToString()));
        }
    }

    #endregion

    #region GUI

    #region ReplayMenu

    static Rect windowRect = new Rect(Screen.width * 0.2f, Screen.height * 0.05f, Screen.width * 0.6f, Screen.height * 0.9f);

    static void ReplayMenuGUI()
    {
        GUILayout.Window(1, windowRect, MenuWindow, "Develop Menu");
    }

    static DevMenuEnum MenuStatus = DevMenuEnum.MainMenu;
    //static bool isWatchLog = true;
    static string[] FileNameList = new string[0];
    static Vector2 scrollPos = Vector2.one;
    static void MenuWindow(int windowID)
    {
        GUIUtil.SetGUIStyle();

        windowRect = new Rect(Screen.width * 0.2f, Screen.height * 0.05f, Screen.width * 0.6f, Screen.height * 0.9f);

        if (MenuStatus == DevMenuEnum.MainMenu)
        {
            if (GUILayout.Button("正常启动", GUILayout.ExpandHeight(true)))
            {
                ChoseReplayMode(false);
            }

            if (GUILayout.Button("复盘模式", GUILayout.ExpandHeight(true)))
            {
                MenuStatus = DevMenuEnum.Replay;
                FileNameList = GetRelpayFileNames();
            }

            if (GUILayout.Button("查看日志", GUILayout.ExpandHeight(true)))
            {
                MenuStatus = DevMenuEnum.Log;
                FileNameList = LogOutPutThread.GetLogFileNameList();
            }
        }
        else if (MenuStatus  == DevMenuEnum.Replay)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < FileNameList.Length; i++)
            {
                if (GUILayout.Button(FileNameList[i]))
                {
                    ChoseReplayMode(true, FileNameList[i]);
                }
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("返回上层"))
            {
                MenuStatus = DevMenuEnum.MainMenu;
            }
        }
        else if (MenuStatus == DevMenuEnum.Log)
        {
            LogGUI();
        }
    }

    #region LogGUI

    static bool isShowLog = false;

    static void LogGUI()
    {
        if (isShowLog)
        {
            ShowLog();
        }
        else
        {
            ShowLogList();
        }
    }

    static string LogContent = "";

    static void ShowLogList()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < FileNameList.Length; i++)
        {
            if (GUILayout.Button(FileNameList[i]))
            {
                isShowLog = true;
                scrollPos = Vector2.zero;
                LogContent = LogOutPutThread.LoadLogContent(FileNameList[i]);
            }
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("返回上层"))
        {
            MenuStatus = DevMenuEnum.MainMenu;
        }
    }

    static void ShowLog()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        GUILayout.TextArea(LogContent);

        GUILayout.EndScrollView();

        if (GUILayout.Button("返回上层"))
        {
            isShowLog = false;
        }
    }



    #endregion

    #endregion

    static int margin = 3;

    static Rect consoleRect = new Rect(margin, Screen.height * 0.6f, Screen.width * 0.3f, Screen.height * 0.4f - margin);

    static string SaveFileName = "";

    static void RecordModeGUI()
    {
        consoleRect = new Rect(margin, Screen.height * 0.5f, Screen.width * 0.4f, Screen.height * 0.5f - margin);

        GUILayout.Window(2, consoleRect, RecordModeGUIWindow, "Replay Panel");
    }
    static void RecordModeGUIWindow(int id)
    {
        SaveFileName = GUILayout.TextField(SaveFileName,GUILayout.ExpandHeight(true));

        if (GUILayout.Button("保存录像文件", GUILayout.ExpandHeight(true)))
        {
            SaveReplayFile(SaveFileName);
        }

        if (RecordManager.GetData(c_recordName).GetRecord(c_qucikLunchKey, true))
        {
            if (GUILayout.Button("开启后台", GUILayout.ExpandHeight(true)))
            {
                RecordManager.SaveRecord(c_recordName, c_qucikLunchKey, false);
            }
        }
        else
        {
            if (GUILayout.Button("关闭后台", GUILayout.ExpandHeight(true)))
            {
                RecordManager.SaveRecord(c_recordName, c_qucikLunchKey, true);
            }
        }


    }

    /// <summary>
    /// 回放模式的GUI
    /// </summary>
    static void ReplayModeGUI()
    {

    }

    #endregion

    #region Update

    static float s_currentTime = 0;

    public static float CurrentTime
    {
        get { return DevelopReplayManager.s_currentTime; }
        //set { DevelopReplayManager.s_currentTime = value; }
    }

    public static void OnReplayUpdate()
    {
        for (int i = 0; i < s_eventStream.Count; i++)
        {
            if (s_eventStream[i].m_t < Time.time)
            {
                InputManager.Dispatch(s_eventStream[i].GetType().Name, s_eventStream[i]);

                s_eventStream.RemoveAt(i);
                i--;
            }
        }
    }

    static void OnRecordUpdate()
    {
        s_currentTime += Time.deltaTime;
    }

    #endregion

    public static string[] GetRelpayFileNames()
    {
        FileTool.CreatPath(PathTool.GetAbsolutePath(ResLoadType.Persistent, c_directoryName));

        List<string> relpayFileNames = new List<string>();
        string[] allFileName = Directory.GetFiles(PathTool.GetAbsolutePath(ResLoadType.Persistent, c_directoryName));
        foreach (var item in allFileName)
        {
            if (item.EndsWith("." + c_expandName))
            {
                string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                relpayFileNames.Add(configName);
            }
        }

        return relpayFileNames.ToArray() ?? new string[0];
    }

    static string GetLogFileName()
    {
        DateTime now = System.DateTime.Now;
        string logName = string.Format("Replay{0}-{1}-{2}#{3}-{4}-{5}",
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

        return logName;
    }

    enum DevMenuEnum
    {
        MainMenu,
        Replay,
        Log
    }
}
