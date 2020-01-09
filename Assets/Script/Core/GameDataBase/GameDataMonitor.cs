using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMonitor
{
    public class GameMonitorData
    {
        public string key;
        public string description;
        public object showValue;

        public GameMonitorData(string key, string description, object showValue)
        {
            this.key = key;
            this.description = description;
            this.showValue = showValue;
        }
    }

    static Dictionary<string, GameMonitorData> s_GameData = new Dictionary<string, GameMonitorData>();

    public static Dictionary<string, GameMonitorData> GameData
    {
        get { return GameDataMonitor.s_GameData; }
        set
        {
            GameDataMonitor.s_GameData = value;
        }
    }

    public static void PushData(string key,object obj,string description="")
    {
        if (!s_GameData.ContainsKey(key))
        {
            s_GameData.Add(key, new GameMonitorData(key,description,obj));
        }
        else
        {
            s_GameData[key] = new GameMonitorData(key, description, obj);
        }
    }

    public static void RemoveData(string dataKey)
    {

        if (s_GameData.ContainsKey(dataKey))
        {
            s_GameData.Remove(dataKey);
        }
        else
        {
            Debug.LogError("RemoveData dataKey dont exist ! ->" + dataKey + "<-");
        }

    }
}


