using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.Text;
using System;

/// <summary>
/// 配置管理器，可读可写，可同步，有默认值
/// 不通过ResourceManager加载,也不受热更新影响
/// </summary>
public static class ConfigManager 
{
    public const string c_directoryName = "Config";
    public const string c_expandName    = "json";

    /// <summary>
    /// 配置缓存
    /// </summary>
    static Dictionary<string, Dictionary<string, SingleField>> s_configCatch = new Dictionary<string,Dictionary<string, SingleField>>();

    public static bool GetIsExistConfig(string ConfigName)
    {
        string dataJson = ResourceManager.ReadTextFile(ConfigName);

        if (dataJson == "")
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static Dictionary<string, SingleField> GetData(string ConfigName)
    {
        if (s_configCatch.ContainsKey(ConfigName))
        {
            return s_configCatch[ConfigName];
        }

        string dataJson = "";

        #if UNITY_EDITOR
                dataJson = ResourceIOTool.ReadStringByResource( 
                    PathTool.GetRelativelyPath(c_directoryName,
                                                ConfigName,
                                                c_expandName));
        #else
                dataJson = ResourceManager.ReadTextFile(ConfigName);
        #endif

        if (dataJson == "")
        {
            throw new Exception("ConfigManager GetData not find " + ConfigName);
        }
        else
        {
            Dictionary<string, SingleField> config = JsonTool.Json2Dictionary<SingleField>(dataJson);

            s_configCatch.Add(ConfigName, config);
            return config;
        }
    }

    public static void CleanCatch()
    {
        s_configCatch.Clear();
    }


//只在编辑器下能够使用
#if UNITY_EDITOR

    public static void SaveData(string ConfigName, Dictionary<string, SingleField> data)
    {
        ResourceIOTool.WriteStringByFile(PathTool.GetAbsolutePath(ResLoadType.Resource,
            PathTool.GetRelativelyPath(c_directoryName,
                                                ConfigName,
                                                c_expandName)),
                                        JsonTool.Dictionary2Json<SingleField>(data));

        UnityEditor.AssetDatabase.Refresh();
    }

    public static Dictionary<string, object> GetEditorConfigData(string ConfigName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOTool.ReadStringByFile(PathTool.GetEditorPath(c_directoryName, ConfigName, c_expandName));

        if (dataJson == "")
        {
            return null;
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    public static void SaveEditorConfigData(string ConfigName, Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

        ResourceIOTool.WriteStringByFile(PathTool.GetEditorPath(c_directoryName, ConfigName, c_expandName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
