using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataTableExtend 
{

    // Use this for initialization
    public static List<T> GetTableDatas<T>(string tableText) where T : DataGenerateBase, new()
    {
        List<T> listData = new List<T>();
        DataTable data = DataTable.Analysis(tableText);
        for (int i = 0; i < data.TableIDs.Count; i++)
        {
            string key =data.TableIDs[i];
            T item = new T();
            item.LoadData(data, key);
            listData.Add(item);
        }

        return listData;
    }

   /// <summary>
   /// 从网络上下载配置并转换成表格
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="url"></param>
   /// <param name="callBack"></param>
    public static void DownLoadTableConfig<T>(string url, Action<List<T>> callBack) where T : DataGenerateBase, new()
    {
        MonoBehaviourRuntime.Instance.StartCoroutine(DownLoadText<T>(url, callBack));
    }
   static IEnumerator  DownLoadText<T>(string url,Action<List<T>>  callBack) where T : DataGenerateBase, new()
    {
        WWW www = new WWW(url);
        yield return www;
        if(!string.IsNullOrEmpty(www.error))
        {
            if (callBack != null)
            {
                callBack(null);
            }
        }
        else
        {
            List<T> configs = GetTableDatas<T>(www.text);
            if (callBack != null)
            {
                callBack(configs);
            }
        }

    }
}
