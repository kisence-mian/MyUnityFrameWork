using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITemplateConfigManager : MonoBehaviour {

    const string ConfigName = "UITemplateConfigData";
    static Dictionary<string,object> s_templateData = new Dictionary<string,object>();

    //从文件读取数据
    public static void LoadData()
    {
        //if (s_templateData == null)
        //{
            s_templateData = ConfigManager.GetEditorConfigData(ConfigName);

        //}
        
    }

    //保存数据
    public static void SaveData()
    {
        ConfigManager.SaveEditorConfigData(ConfigName, s_templateData);
 
    }


    //添加数据
    public static void AddData(string name, object obj = null)
    {
        
        LoadData();
        if (s_templateData.ContainsKey(name))
        {
            s_templateData[name] = obj;
        }
        else
        {
            s_templateData.Add(name, obj);
        }
        SaveData();
 
    }

    //删除数据
    public static void DestroyData(string name)
    {
        LoadData();
        if (s_templateData.ContainsKey(name))
        {
            s_templateData.Remove(name);
        }
        else
        {
            Debug.Log("没有"+ name +"模板！");
        }
        SaveData();
 
    }

    //某模板是否存在
    public static bool HaveTheTemplate(string name)
    {
        LoadData();
        if (name != null && name != "" && s_templateData.ContainsKey(name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //获取所有模板名
    public static string[] GetUIStyleList()
    {
        LoadData();
        string[] allTemplate = new string[s_templateData.Count];
        s_templateData.Keys.CopyTo(allTemplate, 0);

        return allTemplate;

    }
}
