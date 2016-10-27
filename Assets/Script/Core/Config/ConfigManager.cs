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

    public static Dictionary<string, SingleField> GetData(string ConfigName)
    {
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
            #if !UNITY_EDITOR
                throw new Exception("ConfigManager GetData not find " + ConfigName);
            #else
                return new Dictionary<string, SingleField>();
            #endif
        }
        else
        {
            return JsonTool.Json2Dictionary<SingleField>(dataJson);
            //return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }


//只在编辑器下能够使用
#if UNITY_EDITOR

    public static void SaveData(string ConfigName, Dictionary<string, SingleField> data)
    {
        ResourceIOTool.WriteStringByFile(PathTool.GetRelativelyPath(c_directoryName,
                                                ConfigName,
                                                c_expandName),
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
