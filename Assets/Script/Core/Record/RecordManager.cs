using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

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

        string fullPath = PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                PathTool.GetRelativelyPath(c_directoryName,
                                            RecordName,
                                            c_expandName));
        if (File.Exists(fullPath))
        {
            //记录永远从沙盒路径读取
            dataJson = ResourceIOTool.ReadStringByFile(fullPath);
        }

        //Debug.Log(RecordName + " dataJson: " + dataJson);

        if (dataJson == "")
        {
            record = new RecordTable();
        }
        else
        {
            record = RecordTable.Analysis(dataJson);
        }

        s_RecordCatch.Add(RecordName, record);

        return record;
    }

    public static void SaveData(string RecordName, RecordTable data)
    {
#if !UNITY_WEBGL

        ResourceIOTool.WriteStringByFile(
            PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                PathTool.GetRelativelyPath(c_directoryName,
                                                    RecordName,
                                                    c_expandName)),
                RecordTable.Serialize(data));

        #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
        #endif
#endif
    }

    public static void CleanRecord(string recordName)
    {
        RecordTable table = GetData(recordName);
        table.Clear();
        SaveData(recordName, table);
    }

    public static void CleanAllRecord()
    {
        FileTool.DeleteDirectory(Application.persistentDataPath + "/" + RecordManager.c_directoryName);
        CleanCatch();
    }

    public static void CleanCatch()
    {
        s_RecordCatch.Clear();
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

    #region 取值封装

    public static int GetIntRecord(string RecordName, string key)
    {
        RecordTable table = GetData(RecordName);

        if(table.ContainsKey(key))
        {
            return table[key].GetInt();
        }
        else
        {
            throw new Exception("GetIntRecord Exception not find RecordName:->"+ RecordName + " key: ->" + key+"<-");
        }
    }

    public static string GetStringRecord(string RecordName, string key, int value)
    {
        RecordTable table = GetData(RecordName);

        if (table.ContainsKey(key))
        {
            return table[key].GetString();
        }
        else
        {
            throw new Exception("GetStringRecord Exception not find RecordName:->" + RecordName + " key: ->" + key + "<-");
        }
    }

    public static bool GetBoolRecord(string RecordName, string key, bool value)
    {
        RecordTable table = GetData(RecordName);

        if (table.ContainsKey(key))
        {
            return table[key].GetBool();
        }
        else
        {
            throw new Exception("GetBoolRecord Exception not find RecordName:->" + RecordName + " key: ->" + key + "<-");
        }
    }

    public static float GetFloatRecord(string RecordName, string key, float value)
    {
        RecordTable table = GetData(RecordName);

        if (table.ContainsKey(key))
        {
            return table[key].GetFloat();
        }
        else
        {
            throw new Exception("GetFloatRecord Exception not find RecordName:->" + RecordName + " key: ->" + key + "<-");
        }
    }

    public static Vector2 GetVector2Record(string RecordName, string key, Vector2 value)
    {
        RecordTable table = GetData(RecordName);

        if (table.ContainsKey(key))
        {
            return table[key].GetVector2();
        }
        else
        {
            throw new Exception("GetVector2Record Exception not find RecordName:->" + RecordName + " key: ->" + key + "<-");
        }
    }

    public static Vector3 GetVector3Record(string RecordName, string key, Vector3 value)
    {
        RecordTable table = GetData(RecordName);

        if (table.ContainsKey(key))
        {
            return table[key].GetVector3();
        }
        else
        {
            throw new Exception("GetVector3Record Exception not find RecordName:->" + RecordName + " key: ->" + key + "<-");
        }
    }

    public static Color GetColorRecord(string RecordName, string key, Vector3 value)
    {
        RecordTable table = GetData(RecordName);

        if (table.ContainsKey(key))
        {
            return table[key].GetColor();
        }
        else
        {
            throw new Exception("GetColorRecord Exception not find RecordName:->" + RecordName + " key: ->" + key + "<-");
        }
    }

    #endregion

}
