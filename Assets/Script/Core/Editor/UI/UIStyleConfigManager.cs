using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStyleConfigManager 
{
    const string ConfigName = "UIStyleConfigData";
    static Dictionary<string, UIStyleInfo> s_StyleData;

    public static Dictionary<string,UIStyleInfo> GetData()
    {
        LoadData();

        return s_StyleData;
    }

    public static void SaveData()
    {
        LoadData();

        //Dictionary<string, object> dataTmp = new Dictionary<string, object>();
        //foreach (var obj in s_StyleData)
        //{
        //    dataTmp.Add(obj.Key, UIStyleInfo.StlyleData2String(obj.Value));
        //}

        //ConfigManager.SaveEditorConfigData(ConfigName, dataTmp);
    }

    public static void AddData(string key,UIStyleInfo styleData)
    {
        LoadData();

        if (s_StyleData.ContainsKey(key))
        {
            s_StyleData[key] = styleData;
        }
        else
        {
            s_StyleData.Add(key, styleData);
        }
        SaveData();
    }

    public static UIStyleInfo GetData(string key)
    {
        LoadData();
        if (s_StyleData.ContainsKey(key))
        {
            return s_StyleData[key];
        }
        else
        {
            return null;
        }
    }

    public static string[] GetUIStyleList()
    {
        LoadData();
        string[] result = new string[s_StyleData.Count+1];
        result[0] = "None";
        s_StyleData.Keys.CopyTo(result, 1);

        return result;

    }

    static void LoadData()
    {
        //if (s_StyleData == null)
        //{
        //    Dictionary<string, object> dataTmp = new Dictionary<string, object>();
        //    s_StyleData = new Dictionary<string, UIStyleInfo>();

        //    foreach (var obj in dataTmp)
        //    {
        //        s_StyleData.Add(obj.Key, UIStyleInfo.String2StlyleData((string)obj.Value));
        //    }
        //}
    }
}
