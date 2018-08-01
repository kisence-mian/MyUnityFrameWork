using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class LanguageDataEditorUtils
{
   public static Dictionary<string, List<string>> LoadEditorConfig(string c_EditorConfigName, ref List<string> s_languageKeyList)
    {

        Dictionary<string, List<string>> s_languageKeyDict =new Dictionary<string, List<string>>();

        Dictionary<string, object> config = ConfigEditorWindow.GetEditorConfigData(c_EditorConfigName);

        if (config == null)
        {
            config = new Dictionary<string, object>();
        }

        foreach (var item in config)
        {
            List<string> list = new List<string>();
            List<object> ObjList = (List<object>)item.Value;
            for (int i = 0; i < ObjList.Count; i++)
            {
                list.Add(ObjList[i].ToString());

                s_languageKeyList.Add(item.Key + "/" + ObjList[i].ToString());
            }

            s_languageKeyDict.Add(item.Key, list);
        }

        return s_languageKeyDict;
    }

    public static List<string> GetLanguageLayersKeyList()
    {
        List<string> list = new List<string>();
        LoadEditorConfig(LanguageDataEditorWindow.c_EditorConfigName, ref list);
        for (int i = 0; i < list.Count; i++)
        {
            string[] ss = list[i].Split('/');

            list[i] = ss[0].Replace('_', '/') + "/" + ss[1];
        }
        return list;
    }
}

