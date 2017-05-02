using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.Text;
using System;
using LuaInterface;
using System.IO;

/*
 * 数据管理器，只读，可热更新，可使用默认值
 * 通过ResourceManager加载
 * */
public class DataManager
{
    public const string c_directoryName = "Data";
    public const string c_expandName = "txt";

    /// <summary>
    /// 数据缓存
    /// </summary>
    static Dictionary<string, DataTable> s_dataCatch = new Dictionary<string, DataTable>();

    public static bool GetIsExistData(string DataName)
    {
        return ResourcesConfigManager.GetIsExitRes(DataName);
    }

    public static DataTable GetData(string DataName)
    {
        try
        {
            //编辑器下不处理缓存
            if (s_dataCatch.ContainsKey(DataName))
            {
                return s_dataCatch[DataName];
            }

            DataTable data = null;
            string dataJson = "";

#if UNITY_EDITOR

            if (Application.isPlaying)
            {
                dataJson = ResourceManager.ReadTextFile(DataName);
            }
            else
            {
                dataJson = ResourceIOTool.ReadStringByResource(
                        PathTool.GetRelativelyPath(c_directoryName,
                                                    DataName,
                                                    c_expandName));
            }
#else
            dataJson = ResourceManager.ReadTextFile(DataName);
#endif

            if (dataJson == "")
            {
                throw new Exception("Dont Find ->" + DataName + "<-");
            }

            data = DataTable.Analysis(dataJson);
            data.m_tableName = DataName;

            s_dataCatch.Add(DataName, data);
            return data;
        }
        catch (Exception e)
        {
            throw new Exception("GetData Exception ->" + DataName + "<- : " + e.ToString());
        }
    }

    /// <summary>
    /// 清除缓存
    /// </summary>
    public static void CleanCatch()
    {
        s_dataCatch.Clear();
    }

    //只在编辑器下能够使用
#if UNITY_EDITOR
    [NoToLuaAttribute]
    public static bool GetIsExistDataEditor(string DataName)
    {
        return "" != ResourceIOTool.ReadStringByResource(
                        PathTool.GetRelativelyPath(c_directoryName,
                                                    DataName,
                                                    c_expandName));
    }

    [NoToLuaAttribute]
    public static void SaveData(string ConfigName, DataTable data)
    {
        ResourceIOTool.WriteStringByFile(
            PathTool.GetAbsolutePath(
                ResLoadLocation.Resource,
                PathTool.GetRelativelyPath(
                    c_directoryName,
                    ConfigName,
                    c_expandName)),
            DataTable.Serialize(data));

        UnityEditor.AssetDatabase.Refresh();
    }

    /// <summary>
    /// 读取编辑器数据
    /// </summary>
    /// <param name="ConfigName">数据名称</param>
    [NoToLuaAttribute]
    public static Dictionary<string, object> GetEditorData(string dataName)
    {
        UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOTool.ReadStringByFile(PathTool.GetEditorPath(c_directoryName, dataName, c_expandName));

        if (dataJson == "")
        {
            Debug.Log(dataName + " dont find!");
            return new Dictionary<string, object>();
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
    [NoToLuaAttribute]
    public static void SaveEditorData(string ConfigName, Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

        ResourceIOTool.WriteStringByFile(PathTool.GetEditorPath(c_directoryName, ConfigName, c_expandName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

    static void ConvertUtf8()
    {
        //ConvertFileEncoding("e:\\555.txt", null, System.Text.Encoding.UTF8);
        string stmp = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        Console.WriteLine(stmp);

        DirectoryInfo TheFolder = new DirectoryInfo(stmp);

        foreach (FileInfo NextFile in TheFolder.GetFiles())
        {
            if (NextFile.Name.EndsWith("txt"))
            {
                FileTool.ConvertFileEncoding(stmp + NextFile.Name, null, System.Text.Encoding.UTF8);
            }
        }

        foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
        {
            string stmp_inside = stmp + NextFolder.Name + @"\";
            DirectoryInfo TheFolder_inside = new DirectoryInfo(stmp_inside);
            foreach (FileInfo NextFile2 in TheFolder_inside.GetFiles())
            {
                if (NextFile2.Name.EndsWith("txt"))
                {
                    FileTool.ConvertFileEncoding(stmp_inside + NextFile2.Name, null, System.Text.Encoding.UTF8);
                }
            }
        }
    }



#endif
}
