using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DevelopReplayService 
{
    const string c_directoryName = "DevelopReplay";
    public const string c_expandName = "json";

    static bool s_isReplay = false;
    static List<EventSerializeInfo> s_eventStream;

    public static void Init()
    {
        if (s_isReplay)
        {
            LoadEventStream("");
            ApplicationManager.s_OnApplicationUpdate += OnUpdate;
        }
        else
        {
            s_eventStream = new List<EventSerializeInfo>();
            InputManager.OnEventDispatch += OnEventDispatch;
        }
    }

    public static void OnEventDispatch(string eventName, IInputEventBase e)
    {
        EventSerializeInfo eInfo = new EventSerializeInfo();

        eInfo.eventName = eventName;
        eInfo.serializeInfo = e.Serialize();
    }

    public static void OnUpdate()
    {

    }


    public static void SaveEventStream(string fileName)
    {
        string content = JsonTool.List2Json<EventSerializeInfo>(s_eventStream);
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
        string content = ResourceIOTool.ReadStringByFile(
            PathTool.GetAbsolutePath(ResLoadType.Persistent,
                                     PathTool.GetRelativelyPath(
                                                    c_directoryName,
                                                    fileName,
                                                    c_expandName)));

        s_eventStream = JsonTool.Json2List<EventSerializeInfo>(content);
        
    }

    public static string[] GetRelpayFileNames()
    {
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

        return relpayFileNames.ToArray();
    }

    #region GUI

    void RecordModeGUI()
    {

    }

    /// <summary>
    /// 回放模式的GUI
    /// </summary>
    void ReplayModeGUI()
    {

    }

    #endregion

    [System.Serializable]
    class EventSerializeInfo
    {
        public string eventName;
        public string serializeInfo;
    }
}
