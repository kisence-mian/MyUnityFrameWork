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
    public const string c_directoryName = "Data";
    public const string c_expandName = "txt";
    public static DataTable GetData(string ConfigName)
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
            return null;
        }
        else
        {
            return DataTable.Analysis(dataJson);
        }
    }

    //只在编辑器下能够使用
    #if UNITY_EDITOR

    public static void SaveData(string ConfigName, DataTable data)
    {
        ResourceIOTool.WriteStringByFile(
            PathTool.GetAbsolutePath(
                ResLoadType.Resource,
                PathTool.GetRelativelyPath(
                    c_directoryName,
                    ConfigName,
                    c_expandName)), 
            DataTable.Serialize(data));
    }

    /// <summary>
    /// 读取编辑器数据
    /// </summary>
    /// <param name="ConfigName">数据名称</param>
    public static Dictionary<string, object> GetEditorData(string dataName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOTool.ReadStringByFile(PathTool.GetEditorPath(c_directoryName, dataName, c_expandName));

        if (dataJson == "")
        {
            Debug.Log(dataName + " dont find!");
            return new Dictionary<string,object>();
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

        ResourceIOTool.WriteStringByFile(PathTool.GetEditorPath(c_directoryName, ConfigName, c_expandName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }
    #endif
}
