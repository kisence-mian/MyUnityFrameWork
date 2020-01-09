using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataGenerateManager<T> where T : DataGenerateBase, new()
{
    //static T s_dataCatch;
    static Dictionary<string, T> s_dict = new Dictionary<string, T>();
    static List<T> s_ListData = new List<T>();

    static bool s_isInit = false;

    static string s_dataName = null;

    public static string DataName
    {
        get
        {
            if(s_dataName ==  null)
            {
                s_dataName = typeof(T).Name.Replace("Generate", "");
            }

            return s_dataName;
        }
    }

    public static T GetData(string key) 
    {
        if (key == null)
        {
            throw new Exception("DataGenerateManager<" + typeof(T).Name + "> GetData key is Null !");
        }

        //清理缓存
        if (!s_isInit)
        {
            s_isInit = true;
            GlobalEvent.AddEvent(MemoryEvent.FreeHeapMemory, CleanCache);
        }

        if (s_dict.ContainsKey(key))
        {
            return s_dict[key];
        }
        else
        {
            T data = new T();
            data.LoadData(key);
            s_dict.Add(key,data);
            s_ListData.Add(data);
            return data;
        }
    }

    public static bool GetExistKey(string key)
    {
        return DataManager.GetData(DataName).ContainsKey(key);
    }

    /// <summary>
    /// 全查表
    /// </summary>
    public static void PreLoad()
    {
        //清理缓存
        if (!s_isInit)
        {
            s_isInit = true;
            GlobalEvent.AddEvent(MemoryEvent.FreeHeapMemory, CleanCache);
        }

        DataTable data = GetDataTable();
        for (int i = 0; i < data.TableIDs.Count; i++)
        {
            GetData(data.TableIDs[i]);
        }
    }

    public static Dictionary<string, T> GetAllData()
    {
        CleanCache();
        PreLoad();
        return s_dict;
    }
    public static List<T> GetAllDataList()
    {
        CleanCache();
        PreLoad();
        return s_ListData;
    }
    public static DataTable GetDataTable()
    {
        return DataManager.GetData(DataName);
    }

    public static void CleanCache(params object[] objs)
    {
        s_dict.Clear();
        s_ListData.Clear();
    }
}


public abstract class DataGenerateBase
{
    public virtual void LoadData(string key)
    {

    }
    public virtual void LoadData(DataTable table, string key)
    {
        Debug.LogError("默认方法不能加载数据！");
    }
}

