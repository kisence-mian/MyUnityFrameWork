using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RecordManager 
{
    public const string c_directoryName = "Record";
    public const string c_expandName    = "json";

    /// <summary>
    /// 记录缓存
    /// </summary>
    static Dictionary<string, RecordTable> s_RecordCatch = new Dictionary<string, RecordTable>();

    public static RecordTable GetData(string RecordName)
    {
        if (s_RecordCatch.ContainsKey(RecordName))
        {
            return s_RecordCatch[RecordName];
        }

        RecordTable record = null;

        string dataJson = "";

        //记录永远从沙盒路径读取
        dataJson = ResourceIOTool.ReadStringByFile(
            PathTool.GetAbsolutePath(ResLoadType.Persistent,
                PathTool.GetRelativelyPath(c_directoryName,
                                            RecordName,
                                            c_expandName)));

        if (dataJson == "")
        {
            record = new RecordTable();
        }
        else
        {
            record = RecordTable.Analysis(dataJson);
        }

        s_RecordCatch.Add(RecordName, record);

        return new RecordTable();
    }

    public static void SaveData(string RecordName, RecordTable data)
    {
        ResourceIOTool.WriteStringByFile(
            PathTool.GetAbsolutePath(ResLoadType.Persistent,
                PathTool.GetRelativelyPath(c_directoryName,
                                                    RecordName,
                                                    c_expandName)),
                RecordTable.Serialize(data));

        #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    #region 保存封装

    public static void SaveRecord(string RecordName, string key, string value)
    {
        RecordTable table = GetData(RecordName);
        table.SetRecord(key,value);
        SaveData(RecordName, table);
    }

    public static void SaveRecord(string RecordName, string key, int value)
    {
        RecordTable table = GetData(RecordName);
        table.SetRecord(key, value);
        SaveData(RecordName, table);
    }

    public static void SaveRecord(string RecordName, string key, bool value)
    {
        RecordTable table = GetData(RecordName);
        table.SetRecord(key, value);
        SaveData(RecordName, table);
    }

    public static void SaveRecord(string RecordName, string key, float value)
    {
        RecordTable table = GetData(RecordName);
        table.SetRecord(key, value);
        SaveData(RecordName, table);
    }

    public static void SaveRecord(string RecordName, string key, Vector2 value)
    {
        RecordTable table = GetData(RecordName);
        table.SetRecord(key, value);
        SaveData(RecordName, table);
    }

    public static void SaveRecord(string RecordName, string key, Vector3 value)
    {
        RecordTable table = GetData(RecordName);
        table.SetRecord(key, value);
        SaveData(RecordName, table);
    }

    public static void SaveRecord(string RecordName, string key, Color value)
    {
        RecordTable table = GetData(RecordName);
        table.SetRecord(key, value);
        SaveData(RecordName, table);
    }


    #endregion

}
