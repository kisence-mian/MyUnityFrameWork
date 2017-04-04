using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using MiniJSON;

public class DevelopReplayManager 
{
    const string c_directoryName     = "DevelopReplay";
    public const string c_eventExpandName = "inputEvent";
    public const string c_randomExpandName = "random";

    const string c_recordName    = "DevelopReplay";
    const string c_qucikLunchKey = "qucikLunch";

    const string c_eventStreamKey = "EventStream";
    const string c_randomListKey = "RandomList";

    const string c_eventNameKey = "e";
    const string c_serializeInfoKey = "s";

    static bool s_isReplay = false;

    static List<Dictionary<string, string>> s_eventStreamSerialize;
    static List<IInputEventBase> s_eventStream;

    static List<int> s_randomList;

    private static Action s_onLunchCallBack;

    private static StreamWriter m_EventWriter = null;
    private static StreamWriter m_RandomWriter = null;

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

            ApplicationManager.s_OnApplicationUpdate += OnRecordUpdate;
            InputManager.OnEveryEventDispatch += OnEveryEventCallBack;
            GUIConsole.onGUICallback += RecordModeGUI;
            RandomService.OnRandomCreat += OnGetRandomCallBack;

            OpenWriteFileStream(GetLogFileName());
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
        Dictionary<string, object> tmp = new Dictionary<string, object>();

        tmp.Add(c_eventNameKey, inputEvent.GetType().Name);
        tmp.Add(c_serializeInfoKey, inputEvent.Serialize());
        try
        {
            WriteInputEvent(Json.Serialize(tmp));
        }
        catch(Exception e)
        {
            Debug.LogError("Write Dev Log Error! : " + e.ToString());
        }
        
    }

    public static void OnGetRandomCallBack(int random)
    {
        WriteRandomRecord(random);
    }

    #region SaveReplayFile

    static void OpenWriteFileStream(string fileName)
    {

        try
        {
            string path = PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                                         PathTool.GetRelativelyPath(
                                                        c_directoryName,
                                                        fileName,
                                                        c_randomExpandName));

            string dirPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);


            //Debug.Log("EventStream Name: " + PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
            //                             PathTool.GetRelativelyPath(
            //                                            c_directoryName,
            //                                            fileName,
            //                                            c_randomExpandName)));

            m_RandomWriter = new StreamWriter(PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                                         PathTool.GetRelativelyPath(
                                                        c_directoryName,
                                                        fileName,
                                                        c_randomExpandName)));
            m_RandomWriter.AutoFlush = true;

            m_EventWriter = new StreamWriter(PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                                         PathTool.GetRelativelyPath(
                                                        c_directoryName,
                                                        fileName,
                                                        c_eventExpandName)));
            m_EventWriter.AutoFlush = true;

        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    static void WriteRandomRecord(int random)
    {
        if (m_RandomWriter != null)
        {
            m_RandomWriter.WriteLine(random.ToString());
        }
    }

    static void WriteInputEvent(string EventSerializeContent)
    {
        if (m_EventWriter != null)
        {
            m_EventWriter.WriteLine(EventSerializeContent);
        }
    }

    #endregion 

    #region LoadReplayFile

    public static void LoadReplayFile(string fileName)
    {
        string eventContent = ResourceIOTool.ReadStringByFile(
            PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                                     PathTool.GetRelativelyPath(
                                                    c_directoryName,
                                                    fileName,
                                                    c_eventExpandName)));

        string randomContent = ResourceIOTool.ReadStringByFile(
            PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                                     PathTool.GetRelativelyPath(
                                                    c_directoryName,
                                                    fileName,
                                                    c_randomExpandName)));

        LoadEventStream(eventContent.Split('\n'));
        LoadRandomList(randomContent.Split('\n'));
    }

    static void LoadEventStream(string[] content)
    {
        s_eventStream = new List<IInputEventBase>();
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i] != "")
            {
                Dictionary<string, object> info = (Dictionary<string, object>)(Json.Deserialize(content[i]));
                IInputEventBase eTmp = (IInputEventBase)JsonUtility.FromJson(info[c_serializeInfoKey].ToString(), Type.GetType(info[c_eventNameKey].ToString()));
                s_eventStream.Add(eTmp);
            }
        }
    }

    static void LoadRandomList(string[] content)
    {
        s_randomList = new List<int>();

        for (int i = 0; i < content.Length; i++)
        {
            if (content[i] != "")
            {
                s_randomList.Add(int.Parse(content[i].ToString()));
            }
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

    static void RecordModeGUI()
    {
        consoleRect = new Rect(margin, Screen.height * 0.5f, Screen.width * 0.4f, Screen.height * 0.5f - margin);

        GUILayout.Window(2, consoleRect, RecordModeGUIWindow, "Replay Panel");
    }
    static void RecordModeGUIWindow(int id)
    {
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
        FileTool.CreatPath(PathTool.GetAbsolutePath(ResLoadLocation.Persistent, c_directoryName));

        List<string> relpayFileNames = new List<string>();
        string[] allFileName = Directory.GetFiles(PathTool.GetAbsolutePath(ResLoadLocation.Persistent, c_directoryName));
        foreach (var item in allFileName)
        {
            if (item.EndsWith("." + c_eventExpandName))
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
