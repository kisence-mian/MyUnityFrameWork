using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMonitor
{
#if UNITY_EDITOR
    static Dictionary<string, object> s_GameData = new Dictionary<string, object>();

    public static Dictionary<string, object> GameData
    {
        get { return GameDataMonitor.s_GameData; }
        set
        {
            GameDataMonitor.s_GameData = value;
        }
    }
#endif

    public static void PushData(string dataKey,object obj)
    {
#if UNITY_EDITOR
        if (!s_GameData.ContainsKey(dataKey))
        {
            s_GameData.Add(dataKey, obj);
        }
        else
        {
            s_GameData[dataKey] = obj;
        }
#endif
    }

    public static void RemoveData(string dataKey)
    {
#if UNITY_EDITOR
        if (s_GameData.ContainsKey(dataKey))
        {
            s_GameData.Remove(dataKey);
        }
        else
        {
            Debug.LogError("RemoveData dataKey dont exist ! ->" + dataKey + "<-");
        }
#endif
    }
}
