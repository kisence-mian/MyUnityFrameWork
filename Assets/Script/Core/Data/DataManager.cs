using UnityEngine;
using System.Collections.Generic;
using System;

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
    static Dictionary<string, DataTable> s_dataCache = new Dictionary<string, DataTable>();

    public static bool GetIsExistData(string DataName)
    {
        return ResourcesConfigManager.GetIsExitRes(DataName);
    }

    public static DataTable GetData(string DataName)
    {
        try
        {
            //编辑器下不处理缓存
            if (s_dataCache.ContainsKey(DataName))
            {
                return s_dataCache[DataName];
            }

            DataTable data = null;
            string dataJson = "";

            if (Application.isPlaying)
            {
                dataJson = ResourceManager.LoadText(DataName);
            }
            else
            {
                dataJson = ResourceIOTool.ReadStringByResource(
                        PathTool.GetRelativelyPath(c_directoryName,
                                                    DataName,
                                                    c_expandName));
            }

            if (dataJson == "")
            {
                throw new Exception("Dont Find ->" + DataName + "<-");
            }
            data = DataTable.Analysis(dataJson);
            data.m_tableName = DataName;

            s_dataCache.Add(DataName, data);
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
    public static void CleanCache()
    {
        foreach (var item in s_dataCache.Keys)
        {
            ResourceManager.DestoryAssetsCounter(item);
        }
        s_dataCache.Clear();
    }
}
