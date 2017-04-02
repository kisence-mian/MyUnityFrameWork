using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class LanguageManager  
{
    public const string c_DataFilePrefix = "LangData_";
    public const string c_configFileName = "LanguageConfig";
    public const string c_defaultLanguageKey = "DefaultLanguage";

    public const string c_mainKey = "key";
    public const string c_valueKey = "value";

    static public SystemLanguage s_currentLanguage = SystemLanguage.ChineseSimplified;
    static public DataTable s_languageData;
	public static void Init()
    {
        //Debug.Log("当前语言: " + Application.systemLanguage);

        SetLanguage(Application.systemLanguage);
    }

    public static void SetLanguage(SystemLanguage l_lang)
    {
        s_currentLanguage = l_lang;

        string languageDataName = c_DataFilePrefix + l_lang.ToString();

        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            languageDataName = "Language/" + languageDataName;
        }
        #endif

        if(DataManager.GetIsExistData(languageDataName))
        {
            s_languageData = DataManager.GetData(languageDataName);
        }
        else
        {
            string defaultLanguage = ConfigManager.GetData(c_configFileName)[c_defaultLanguageKey].GetString();
            s_languageData = DataManager.GetData(defaultLanguage);
        }
    }
    public static string GetContent(string contentID, List<object> contentParams)
    {
        string content = null;

        if (s_languageData.ContainsKey(contentID))
        {
            content = s_languageData[contentID].GetString("value");
        }
        else
        {
            Debug.LogError("Dont find language : ->" + contentID + "<-");
            return "Dont find language : ->" + contentID + "<-";
        }

        if (contentParams == null || contentParams.Count == 0)
            return content;
        else
        {
            for (int i = 0; i < contentParams.Count; i++)
            {
                string replaceTmp = "{" + i + "}";
                content = content.Replace(replaceTmp, contentParams[i].ToString());
            }

            return content;
        }
    }
    public static string GetContent(string contentID, params object[] contentParams)
    {
        string content = null;

        if (s_languageData.ContainsKey(contentID))
        {
            content = s_languageData[contentID].GetString("value");
        }
        else
        {
            Debug.LogError("Dont find language : ->" + contentID + "<-");
            return "Dont find language : ->" + contentID + "<-";
        }

        if (contentParams == null || contentParams.Length == 0)
            return content;
        else
        { 
            for (int i = 0; i < contentParams.Length; i++)
            {
                string replaceTmp = "{" + i + "}";
                content = content.Replace(replaceTmp, contentParams[i].ToString());
            }

            return content;
        }
    }
}
