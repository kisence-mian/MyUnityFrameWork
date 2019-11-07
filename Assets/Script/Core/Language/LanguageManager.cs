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
    static public Dictionary<string,string> s_languageDataDict = new Dictionary<string, string>();//所有语言数据

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
            if (config == null)
            {
                config = LanguageDataUtils.LoadRuntimeConfig();
            }
            SetLanguage(ApplicationManager.Langguage);
        }
    }

    public static void SetLanguage(SystemLanguage lang)
    {  
        SystemLanguage oldLan = s_currentLanguage;
        if (config == null)
            return;
        if (lang == SystemLanguage.Chinese)
            lang = SystemLanguage.ChineseSimplified;

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

    public static bool ContainsFullKeyName(string fullKeyName)
    {
        if (string.IsNullOrEmpty(fullKeyName))
        {
            Debug.LogError("LanguageManager =>Error : key is null :" + fullKeyName);
            return false;
        }
        Init();


        if (s_languageDataDict.ContainsKey(fullKeyName))
        {
            return true;
        }
        else
        {
            int indexEnd = fullKeyName.LastIndexOf("/");
            if (indexEnd < 0)
            {
                Debug.LogError("LanguageManager => Error : Format is error :"+fullKeyName);
                return false;
            }
            string key = fullKeyName.Substring(indexEnd + 1);
            string fullFileName = fullKeyName.Remove(indexEnd);

            DataTable data = LoadDataTable(s_currentLanguage, fullFileName);

            foreach (var item in data.TableIDs)
            {
                s_languageDataDict.Add(fullFileName + "/" + item, data[item].GetString(c_valueKey));
            }
            return s_languageDataDict.ContainsKey(fullKeyName);
        }
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

        string content = null;

        if (ContainsFullKeyName(fullKeyName))
        {
            content = s_languageDataDict[fullKeyName];
        }
        else
        {
            Debug.LogError("LanguageManager => Error : no find key :" + fullKeyName);
            return "";
        }
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
    private static Dictionary<string, int> loadTextFileTimesDic = new Dictionary<string, int>();
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
            if (loadTextFileTimesDic.ContainsKey(name))
                loadTextFileTimesDic[name]++;
            else
            {
                loadTextFileTimesDic.Add(name, 1);
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

        foreach (var item in loadTextFileTimesDic)
        {
            ResourceManager.DestoryAssetsCounter(item.Key, item.Value);
        }
        loadTextFileTimesDic.Clear();
    }
}

public enum LanguageEventEnum
{
    LanguageChange,
}
