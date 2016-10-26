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

    public static Dictionary<string, SingleConfig> GetData(string ConfigName)
    {
        string dataJson = "";

        #if UNITY_EDITOR
                dataJson = ResourceIOTool.ReadStringByResource(GetRelativelyPath(ConfigName));
        #else
                dataJson = ResourceManager.ReadTextFile(ConfigName);
        #endif

        if (dataJson == "")
        {
            #if !UNITY_EDITOR
                throw new Exception("ConfigManager GetData not find " + ConfigName);
            #else
                return new Dictionary<string, SingleConfig>();
            #endif
        }
        else
        {
            return JsonTool.Json2Dictionary<SingleConfig>(dataJson);
            //return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }



    //获取的是绝对路径
    static string GetAbsolutePath(string ConfigName)
    {
        StringBuilder builder = new StringBuilder();

#if UNITY_EDITOR

        builder.Append(Application.dataPath);
        builder.Append("/Resources");
#else
        builder.Append(Application.persistentDataPath);
#endif
        builder.Append("/");
        builder.Append(GetRelativelyPath(ConfigName));

        return builder.ToString();
    }

    //获取相对路径
    static string GetRelativelyPath(string ConfigName)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(c_directoryName);
        builder.Append("/");
        builder.Append(ConfigName);
        builder.Append(".json");

        return builder.ToString();
    }


//只在编辑器下能够使用
#if UNITY_EDITOR

    public static void SaveData(string ConfigName, Dictionary<string, SingleConfig> data)
    {
        ResourceIOTool.WriteStringByFile(GetAbsolutePath(ConfigName), JsonTool.Dictionary2Json<SingleConfig>(data));

        //ResourceIOTool.WriteStringByFile(GetAbsolutePath(ConfigName), Json.Serialize(data));

        UnityEditor.AssetDatabase.Refresh();
    }

    public static Dictionary<string, object> GetEditorConfigData(string ConfigName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOTool.ReadStringByFile(GetEditorPath(ConfigName));

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

        ResourceIOTool.WriteStringByFile(GetEditorPath(ConfigName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

    public static string GetEditorPath(string ConfigName)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Application.dataPath);
        builder.Append("/Editor");
        builder.Append(c_directoryName);
        builder.Append("/");
        builder.Append(ConfigName);
        builder.Append(".json");

        return builder.ToString();
    }
#endif
}
