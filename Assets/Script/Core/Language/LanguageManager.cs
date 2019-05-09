using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System;

public class LanguageManager
{
    public const string c_configFileName     = "LanguageConfig";
    public const string c_defaultModuleKey   = "default";

    public const string c_DataFilePrefix = "LangData_";
    public const string c_mainKey  = "key";
    public const string c_valueKey = "value";

    static public SystemLanguage s_currentLanguage = SystemLanguage.ChineseSimplified; //当前语言
    static public Dictionary<string,Dictionary<string,string>> s_languageDataDict = new Dictionary<string, Dictionary<string,string>>();//所有语言数据

    private static LanguageSettingConfig config;
    static private bool isInit = false;

    public static bool IsInit
    {
        get { return LanguageManager.isInit; }
        set { isInit = value; }
    }

    public static void Init()
    {
        //Debug.Log("1 当前语言: " + Application.systemLanguage + " isInit " + isInit);

        if (!isInit)
        {
            isInit = true;

            config = LanguageDataUtils.LoadEditorConfig();
            SetLanguage(ApplicationManager.Langguage);
        }
    }

    public static void SetLanguage(SystemLanguage lang)
    {  
        SystemLanguage oldLan = s_currentLanguage;
        if (config == null)
            return;
        if (config.gameExistLanguages.Contains(lang))
        {
            s_currentLanguage = lang;
        }
        else
        {
            //Debug.Log("当前语言不存在 " + lang);

            s_currentLanguage = config.defaultLanguage;
        }
        if (oldLan != s_currentLanguage)
        {
            s_languageDataDict.Clear();
        }

        GlobalEvent.DispatchEvent(LanguageEventEnum.LanguageChange, lang);
    }

    /// <summary>
    /// 兼容旧版本代码，不再建议使用
    /// </summary>
    [Obsolete]
    public static string GetContent(string contentID, List<object> contentParams)
    {
        return GetContent(c_defaultModuleKey, contentID, contentParams.ToArray());
    }

    /// <summary>
    /// 兼容旧版本代码，不再建议使用
    /// </summary>
    [Obsolete]
    public static string GetContent(string contentID, params object[] contentParams)
    {
        return GetContent(c_defaultModuleKey, contentID, contentParams);
    }

    public static string GetContent(string moduleName,string contentID, List<object> contentParams)
    {
        return GetContent(moduleName, contentID, contentParams.ToArray());
    }
    /// <summary>
    /// moduleName_key ： MiniGame/title_0
    /// </summary>
    /// <param name="fullKeyName"></param>
    /// <param name="contentParams"></param>
    /// <returns></returns>
    public static string GetContentByKey(string fullKeyName, params object[] contentParams)
    {
        Init();
        if (string.IsNullOrEmpty(fullKeyName))
            return "Error : key is null";
        int indexEnd = fullKeyName.LastIndexOf("/");
        if (indexEnd < 0)
            return "Error : Format is error";
        string key = fullKeyName.Substring(indexEnd + 1);
        string fullFileName = fullKeyName.Remove(indexEnd);
        if (string.IsNullOrEmpty(fullFileName) || string.IsNullOrEmpty(key))
            return "Error : key is null";

        Dictionary<string, string> contentDic = null;
        if (s_languageDataDict.ContainsKey(fullFileName))
        {
            contentDic = s_languageDataDict[fullFileName];
        }
        else
        {
            DataTable data = LoadDataTable(s_currentLanguage, fullFileName);

            contentDic = new Dictionary<string, string>();
            foreach (var item in data.TableIDs)
            {
                contentDic.Add(item, data[item].GetString(c_valueKey));
            }
            s_languageDataDict.Add(fullFileName, contentDic);
        }
        if (!contentDic.ContainsKey(key))
            return "Error:no key in File:" + fullFileName + "  key:" + key;
        string content = contentDic[key];
        if (contentParams != null && contentParams.Length > 0)
        {
            for (int i = 0; i < contentParams.Length; i++)
            {
                string replaceTmp = "{" + i + "}";
                if (contentParams[i] == null)
                    continue;
                content = content.Replace(replaceTmp, contentParams[i].ToString());
            }
        }
        if (ApplicationManager.Instance != null && ApplicationManager.Instance.showLanguageValue && ApplicationManager.Instance.m_AppMode == AppMode.Developing)
            content = "[" + content + "]";
        return content;
    }

    private static DataTable LoadDataTable(SystemLanguage language, string fullFileName)
    {
        if (Application.isPlaying)
        {

            string name = GetLanguageDataName(language, fullFileName);
            TextAsset text = ResourceManager.Load<TextAsset>(name);
            if (text == null)
            {
                Debug.LogError("Error： no Language file ：" + name);
                return null;
            }
            DataTable data = DataTable.Analysis(text.text);
            return data;
        }
        else
        {
            return LanguageDataUtils.LoadFileData(language, fullFileName);
        }
    }
    public static string GetContent(string moduleName, string contentID, params object[] contentParams)
    {
        string fullkey = moduleName.Replace('_', '/') + "/" + contentID;
        return GetContentByKey(fullkey,contentParams);
    }
    

    public static string GetLanguageDataName(SystemLanguage langeuageName, string fullkeyFileName)
    {
        string modelName = fullkeyFileName.Replace('/', '_');
        return c_DataFilePrefix + langeuageName + "_" + modelName;
    }

    public static void Release()
    {
        s_languageDataDict.Clear();
        isInit = false;
    }
}

public enum LanguageEventEnum
{
    LanguageChange,
}
