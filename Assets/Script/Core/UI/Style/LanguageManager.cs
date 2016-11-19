using UnityEngine;
using System.Collections;
using System.Text;

public class LanguageManager  
{
    const string s_DataFilePrefix = "LangData_";
    static public SystemLanguage s_currentLanguage = SystemLanguage.ChineseSimplified;
    static DataTable s_languageData;
	public void Init()
    {
        SetLanguage(Application.systemLanguage);
    }

    public static void SetLanguage(SystemLanguage l_lang)
    {
        s_currentLanguage = l_lang;
        s_languageData = DataManager.GetData(s_DataFilePrefix + s_languageData.ToString());
    }

    public static string GetContent(string l_contentID)
    {
        if (s_languageData.ContainsKey(l_contentID))
        {
            return s_languageData[l_contentID].GetString("value");
        }
        else
        {
            Debug.LogError("Dont find language :" + l_contentID);
            return l_contentID;
        }
    }

    public static string GetContent(string l_contentID, params object[] l_contents)
    {
        string l_content = GetContent(l_contentID);
        if (l_content.Equals(l_contentID))
        {
            Debug.LogError("Dont find language :" + l_contentID);
            return l_content;
        }

        if (l_contents == null || l_contents.Length == 0)
            return l_content;
        else
        { 
            for (int i = 0; i < l_contents.Length; i++)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("{");
                builder.Append(i);
                builder.Append("}");
                l_content = l_content.Replace(builder.ToString(), l_contents[i].ToString());
            }

            return l_content;
        }
    }
}
