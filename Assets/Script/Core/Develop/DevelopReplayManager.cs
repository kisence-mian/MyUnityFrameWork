using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class DevelopReplayManager 
{
    const string c_directoryName = "DevelopReplay";
    public const string c_expandName = "json";

    const string c_RecordName = "DevelopReplay";
    const string c_qucikLunchKey = "qucikLunch";

    static bool s_isReplay = false;
    static List<IInputEventBase> s_eventStream;

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
            if (!RecordManager.GetData(c_RecordName).GetRecord(c_qucikLunchKey, true))
            {
                isQuickLunch = false;
            }
        }

        if (isQuickLunch)
        {
            choseReplayMode(false);
        }
        else
        {
            Time.timeScale = 0;
            ApplicationManager.s_OnApplicationOnGUI += ReplayMenuGUI;
        }
    }

    static void choseReplayMode(bool isReplay,string replayFileName = null)
    {
        s_isReplay = isReplay;

        if (s_isReplay)
        {
            LoadEventStream(replayFileName);
            ApplicationManager.s_OnApplicationUpdate += OnUpdate;
            GUIConsole.onGUICallback += ReplayModeGUI;

            InputUIEventProxy.IsAvtive = false;
            InputOperationEventProxy.IsAvtive = false;
            
        }
        else
        {
            s_eventStream = new List<IInputEventBase>();
            InputManager.OnEveryEventDispatch += OnEveryEventCallBack;
            GUIConsole.onGUICallback += RecordModeGUI;
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

    public static void OnUpdate()
    {
        for (int i = 0; i < s_eventStream.Count; i++)
        {
            if (s_eventStream[i].m_eventTime < Time.time)
            {
                InputManager.Dispatch(s_eventStream[i].GetType().Name, s_eventStream[i]);

                s_eventStream.RemoveAt(i);
                i--;
            }
        }
    }


    public static void SaveEventStream(string fileName)
    {
        List<EventSerializeInfo> list = new List<EventSerializeInfo>();

        for (int i = 0; i < s_eventStream.Count; i++)
        {
            EventSerializeInfo tmp = new EventSerializeInfo();

            tmp.eventName = s_eventStream[i].GetType().Name;
            tmp.serializeInfo = s_eventStream[i].Serialize();
            list.Add(tmp);
        }

        string content = JsonTool.List2Json<EventSerializeInfo>(list);
        ResourceIOTool.WriteStringByFile(
                PathTool.GetAbsolutePath(ResLoadType.Persistent,
                                         PathTool.GetRelativelyPath(
                                                        c_directoryName,
                                                        fileName,
                                                        c_expandName))
                , content);
    }

    public static void LoadEventStream(string fileName)
    {
        List<EventSerializeInfo> list = new List<EventSerializeInfo>();
        s_eventStream = new List<IInputEventBase>();

        string content = ResourceIOTool.ReadStringByFile(
            PathTool.GetAbsolutePath(ResLoadType.Persistent,
                                     PathTool.GetRelativelyPath(
                                                    c_directoryName,
                                                    fileName,
                                                    c_expandName)));

        list = JsonTool.Json2List<EventSerializeInfo>(content);

        for (int i = 0; i < list.Count; i++)
        {
            IInputEventBase eTmp = (IInputEventBase)JsonUtility.FromJson(list[i].serializeInfo, Type.GetType(list[i].eventName));
            s_eventStream.Add(eTmp);
        }
    }

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

    #region GUI

    #region ReplayMenu

    static Rect windowRect = new Rect(Screen.width * 0.2f, Screen.height * 0.05f, Screen.width * 0.6f, Screen.height * 0.9f);

    static void ReplayMenuGUI()
    {
        GUILayout.Window(1, windowRect, MenuWindow, "Lunch Menu");
    }

    static bool isOpenMenu = true;
    static string[] replayFileNameList = new string[0];
    static Vector2 scrollPos = Vector2.one;
    static void MenuWindow(int windowID)
    {
        GUIUtil.SetGUIStyle();

        windowRect = new Rect(Screen.width * 0.2f, Screen.height * 0.05f, Screen.width * 0.6f, Screen.height * 0.9f);

        if (isOpenMenu)
        {
            if (GUILayout.Button("正常启动", GUILayout.ExpandHeight(true)))
            {
                choseReplayMode(false);
            }

            if (GUILayout.Button("复盘模式", GUILayout.ExpandHeight(true)))
            {
                isOpenMenu = false;
                replayFileNameList = GetRelpayFileNames();
            }
        }
        else
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < replayFileNameList.Length; i++)
            {
                if (GUILayout.Button(replayFileNameList[i]))
                {
                    choseReplayMode(true, replayFileNameList[i]);
                }
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("返回上层"))
            {
                isOpenMenu = true;
            }
        }
    }

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
            SaveEventStream(SaveFileName);
        }

        if (RecordManager.GetData(c_RecordName).GetRecord(c_qucikLunchKey, true))
        {
            if (GUILayout.Button("开启复盘模式", GUILayout.ExpandHeight(true)))
            {
                RecordManager.SaveRecord(c_RecordName, c_qucikLunchKey, false);
            }
        }
        else
        {
            if (GUILayout.Button("关闭复盘模式", GUILayout.ExpandHeight(true)))
            {
                RecordManager.SaveRecord(c_RecordName, c_qucikLunchKey, true);
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

    static string GetLogFileName()
    {
        DateTime now = System.DateTime.Now;
        string logName = string.Format("Replay{0}-{1}-{2}#{3}-{4}-{5}",
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

        return logName;
    }

    [System.Serializable]
    class EventSerializeInfo
    {
        public string eventName;
        public string serializeInfo;
    }
}
