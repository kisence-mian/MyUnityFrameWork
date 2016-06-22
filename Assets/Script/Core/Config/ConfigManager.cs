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
        string dataJson = "";
#if UNITY_EDITOR
        dataJson = ResourceManager.LoadTextFile(GetConfigPath(ConfigName), ResLoadType.ResourcePath);
#else
        dataJson = ResourceManager.LoadTextFile(GetConfigPath(ConfigName));
#endif

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

    public static void SaveConfigData(string ConfigName,Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

#if UNITY_EDITOR
        ResourceManager.SaveTextFile(GetConfigPath(ConfigName), configDataJson,ResLoadType.ResourcePath); //编辑器下保存到Resources路径下
#else
        ResourceManager.SaveTextFile(GetConfigPath(ConfigName), configDataJson);
#endif
    }

    //获取的是相对路径
    static string GetConfigPath(string ConfigName)
    {
        return "Config/" + ConfigName;
    }

//只在编辑器下能够使用
#if UNITY_EDITOR
    public static Dictionary<string, object> GetEditorConfigData(string ConfigName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOService.LoadStringByFile(GetEditorPath(ConfigName));

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

        ResourceIOService.SaveStringByFile(GetEditorPath(ConfigName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

    public static string GetEditorPath(string ConfigName)
    {
        
        return Application.dataPath + "/EditorConfig/" + ConfigName + ".json";
    }
#endif
}
