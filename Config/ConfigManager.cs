
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

    public static Dictionary<string, object> GetData(string ConfigName)
    {
        string dataJson = "";

        if (ResourceManager.gameLoadType != ResLoadType.Resource)
        {
            dataJson = ResourceIOTool.ReadStringByFile(GetAbsolutePath(ConfigName));
        }
        else
        {
            dataJson = ResourceIOTool.ReadStringByResource(GetRelativelyPath(ConfigName));
        }

        if (dataJson == "")
        {
            Debug.Log(ConfigName + " dont find!");
            return new Dictionary<string,object>();
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    public static void SaveData(string ConfigName, Dictionary<string, object> data)
    {
        ResourceIOTool.WriteStringByFile(GetAbsolutePath(ConfigName), Json.Serialize(data));
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
        builder.Append(directoryName);
        builder.Append("/");
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
        builder.Append("/Editor");
        builder.Append(directoryName);
        builder.Append("/");
        builder.Append(ConfigName);
        builder.Append(".json");

        return builder.ToString();
    }
#endif
}
