using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStyleConfigManager 
{
    const string ConfigName = "UIStyleConfigData";
    const string DataName   = "UIStyle";
    static Dictionary<string, UIStyleInfo> s_StyleData;

    public static Dictionary<string, UIStyleInfo> GetData()
    {
        LoadData();

        return s_StyleData;
    }

    public static void SaveData()
    {
        LoadData();

        Dictionary<string, object> dataTmp = new Dictionary<string, object>();

        dataTmp.Add(DataName, JsonTool.Dictionary2Json<UIStyleInfo>(s_StyleData));

        ConfigEditorWindow.SaveEditorConfigData(ConfigName, dataTmp);
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

    public static void DeleteData(string key)
    {
        LoadData();
        if (s_StyleData.ContainsKey(key))
        {
            s_StyleData.Remove(key);
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
        if (s_StyleData == null)
        {
            Dictionary<string, object> dataTmp = ConfigEditorWindow.GetEditorConfigData(ConfigName);

            if (dataTmp != null && dataTmp.ContainsKey(DataName))
            {
                s_StyleData = JsonTool.Json2Dictionary<UIStyleInfo>((string)dataTmp[DataName]);
            }
            else
            {
                s_StyleData = new Dictionary<string, UIStyleInfo>();
            }
            
        }
    }
}
