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
}
