using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FrameWork;

public class JsonTool
{
    #region Util

    public static T Json2Object<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public static string Object2Json(object obj)
    {
        if(obj is List<object>)
        {
            return List2Json<object>(obj as List<object>);
        }
        else if (obj is Dictionary<string,object>)
        {
            return Dictionary2Json<object>(obj as Dictionary<string, object>);
        }
        else
        {
            return JsonUtility.ToJson(obj);
        }
    }

    #endregion

    //目前unity的json不支持List 和 Dictionary 在此用MINIjson做了封装
    //不支持嵌套
    #region List

    public static List<T> Json2List<T>(string jsonData)
    {
        List<T> datas = new List<T>();
        if (!string.IsNullOrEmpty(jsonData))
        {
            List<object> listData = Json.Deserialize(jsonData) as List<object>;
            if (listData == null)
                return datas;

            for (int i = 0; i < listData.Count; i++)
            {
                datas.Add(Json2Object<T>(listData[i].ToString()));
            }
        }
        return datas;
    }

    public static string List2Json<T>(List<T> datas)
    {
        List<object> temp = new List<object>();

        for (int i = 0; i < datas.Count; i++)
        {
            temp.Add(Object2Json(datas[i]));
        }

        return  Json.Serialize(temp);
    }

    #endregion

    #region Dictionary

    public static Dictionary<string,T> Json2Dictionary<T>(string jsonData)
    {
        Dictionary<string,T> datas = new Dictionary<string,T>();
        if (!string.IsNullOrEmpty(jsonData))
        {
            Dictionary<string, object> listData = Json.Deserialize(jsonData) as Dictionary<string, object>;
            if (listData == null)
            {
                return datas;
            }

            foreach (string key in listData.Keys)
            {
                datas.Add(key, Json2Object<T>(listData[key].ToString()));
            }
        }
        return datas;
    }

    public static string Dictionary2Json<T>(Dictionary<string,T>  datas)
    {
        Dictionary<string,object> temp = new Dictionary<string,object>();

        foreach (string key in datas.Keys)
        {
            temp.Add(key, Object2Json(datas[key]));
        }

        return Json.Serialize(temp);
    }

    #endregion

}
