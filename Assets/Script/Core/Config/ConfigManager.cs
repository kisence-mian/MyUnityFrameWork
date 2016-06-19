using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

/// <summary>
/// 设置管理器
/// </summary>
public static class ConfigManager 
{
    public static Dictionary<string, object> GetConfigData(string ConfigName)
    {
        return null;
    }

    public static void SaveConfigData(string ConfigName,Dictionary<string, object> data)
    {
        //string configDataJson = Json.Serialize(data);
    }

//只在编辑器下能够使用
#if UNITY_EDITOR
    public static Dictionary<string, object> GetEditorConfigData(string ConfigName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceLoadService.LoadStringByFile(GetEditorPath(ConfigName));

        if (dataJson == "")
        {
            return null;
        }
        else
        {
            Dictionary<string, object> configData = (Dictionary<string, object>)MiniJSON.Json.Deserialize(dataJson);

            return configData;
        }
    }

    public static void SaveEditorConfigData(string ConfigName ,Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

        ResourceLoadService.SaveStringByFile(GetEditorPath(ConfigName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

    public static string GetEditorPath(string ConfigName)
    {
        
        return Application.dataPath + "/EditorConfig/" + ConfigName + ".json";
    }
#endif
}
