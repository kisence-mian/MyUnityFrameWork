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
        try
        {
            DataTable data = DataTable.Analysis(tableText);
            for (int i = 0; i < data.TableIDs.Count; i++)
            {
                string key = data.TableIDs[i];
                T item = new T();
                item.LoadData(data, key);
                listData.Add(item);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("表格数据解析错误：" + e);
        }
        return listData;
    }

    /// <summary>
    /// 从网络上下载配置并转换成表格
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="callBack">List<T> data,string error</param>
    public static void DownLoadTableConfig<T>(string url, Action<List<T>,string> callBack) where T : DataGenerateBase, new()
    {
        try
        {
            MonoBehaviourRuntime.Instance.StartCoroutine(DownLoadText<T>(url, callBack));
        }
        catch (Exception e)
        {
            if (callBack != null)
            {
                callBack(null, e.ToString());
            }
        }
       
    }
   static IEnumerator  DownLoadText<T>(string url,Action<List<T>,string>  callBack) where T : DataGenerateBase, new()
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("下载数据失败URL is null" );
            if (callBack != null)
            {
                callBack(null,"url is null");
            }
            yield break;
        }

        WWW www = new WWW(url);
        yield return www;
        if(!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("下载数据失败URL:" + url + "\n error:" + www.error);
            if (callBack != null)
            {
                callBack(null,www.error);
            }
        }
        else
        {
            List<T> configs = GetTableDatas<T>(www.text);
            if (callBack != null)
            {
                callBack(configs,null);
            }
        }

    }
}
