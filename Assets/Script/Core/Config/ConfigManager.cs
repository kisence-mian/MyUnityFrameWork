
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.Text;

/// <summary>
/// 配置管理器，可读可写，可同步，有默认值
/// 不通过ResourceManager加载,也不受热更新影响
/// </summary>
public static class ConfigManager 
{
    public const string directoryName = "Config";

    public static Dictionary<string, object> GetConfigData(string ConfigName)
    {
        string dataJson = "";

        dataJson = ResourceManager.ReadTextFile(GetConfigPath(ConfigName));

        if (dataJson == "")
        {
            return null;
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    public static void SaveConfigData(string ConfigName, Dictionary<string, object> data)
    {
        //ResourceManager.WriteTextFile(GetConfigPath(ConfigName), Json.Serialize(data)); 
    }

    //获取的是相对路径
    static string GetConfigPath(string ConfigName)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Config/");
        builder.Append(ConfigName);
        builder.Append(".json");

        return builder.ToString();
    }

//只在编辑器下能够使用
#if UNITY_EDITOR
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
        builder.Append("/EditorConfig/");
        builder.Append(ConfigName);
        builder.Append(".json");

        return builder.ToString();
    }
#endif
}
