using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.Text;

/*
 * 数据管理器，只读，可热更新，可使用默认值
 * 通过ResourceManager加载
 * */
public class DataManager 
{
    public const string directoryName = "Data";
    public static Dictionary<string, object> GetData(string ConfigName)
    {
        string dataJson = "";

        #if UNITY_EDITOR
                dataJson = ResourceIOTool.ReadStringByFile(GetEditorPath(ConfigName,false));
        #else
                dataJson = ResourceManager.ReadTextFile(GetDataPath(ConfigName));
        #endif

        if (dataJson == "")
        {
            return null;
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    //获取的是相对路径
    static string GetDataPath(string ConfigName)
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

    public static void SaveData(string ConfigName, Dictionary<string, object> data)
    {
        ResourceIOTool.WriteStringByFile(GetEditorPath(ConfigName, false), Json.Serialize(data));
    }

    /// <summary>
    /// 读取编辑器数据
    /// </summary>
    /// <param name="ConfigName">数据名称</param>
    public static Dictionary<string, object> GetEditorData(string ConfigName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOTool.ReadStringByFile(GetEditorPath(ConfigName,true));

        if (dataJson == "")
        {
            return null;
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    /// <summary>
    /// 保存编辑器数据
    /// </summary>
    /// <param name="ConfigName">数据名称</param>
    /// <param name="data">数据表</param>
    public static void SaveEditorData(string ConfigName, Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

        ResourceIOTool.WriteStringByFile(GetEditorPath(ConfigName,true), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

    public static string GetEditorPath(string ConfigName,bool isEditor)
    {
        StringBuilder builder = new StringBuilder();

        if (isEditor)
        {
            builder.Append(Application.dataPath);
            builder.Append("/Editor");
            builder.Append(directoryName);
            builder.Append("/");
        }
        else
        {
            builder.Append(Application.dataPath);
            builder.Append("/Resources/");
            builder.Append(directoryName);
            builder.Append("/");
        }
        builder.Append(ConfigName);
        builder.Append(".json");

        return builder.ToString();
    }
#endif
}
