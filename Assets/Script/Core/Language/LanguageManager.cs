using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System;

public class LanguageManager
{
    public const string c_configFileName     = "LanguageConfig";
    public const string c_defaultModuleKey = "default";

    public const string c_DataFilePrefix = "LangData_";
    public const string c_mainKey  = "key";
    public const string c_valueKey = "value";
    /// <summary>
    /// 当前语言
    /// </summary>
    public static SystemLanguage CurrentLanguage
    {
        get
        {
            Init();
            return s_currentLanguage;
        }
    }

    public static CallBack<SystemLanguage> OnChangeLanguage;

    static private SystemLanguage s_currentLanguage = SystemLanguage.ChineseSimplified; //当前语言
    static private Dictionary<string,string> s_languageDataDict = new Dictionary<string, string>();//所有语言数据

    private static LanguageSettingConfig config;
    static private bool isInit = false;

    public static bool IsInit
    {
        get { return isInit; }
        set { isInit = value; }
    }

    public static void Init()
    {
        if (!isInit)
        {
            isInit = true;
            if (config == null)
            {
                config = LanguageDataUtils.LoadRuntimeConfig();
            }
            if (config == null)
                return;
            s_currentLanguage = SetConfig();
            Debug.Log("使用语言：" + s_currentLanguage);
        }
    }

    private static SystemLanguage SetConfig()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
        if(systemLanguage== SystemLanguage.Chinese)
        {
            systemLanguage = SystemLanguage.ChineseSimplified;
        }
        Debug.Log("config.useSystemLanguage:" + config.useSystemLanguage+ " config.defaultLanguage:"+ config.defaultLanguage);
        if (config.useSystemLanguage)
        {
           
            if (config.gameExistLanguages.Contains(systemLanguage))
            {
                return systemLanguage;
            }
            else
            {
                if (config.gameExistLanguages.Contains(SystemLanguage.English))
                {
                    return SystemLanguage.English;
                }
            }
        }

        return config.defaultLanguage;
    }

    public static void SetLanguage(SystemLanguage lang)
    {
        Init();

        SystemLanguage oldLan = s_currentLanguage;
        if (config == null)
            return;
        if (lang == SystemLanguage.Chinese)
            lang = SystemLanguage.ChineseSimplified;

        if (config.gameExistLanguages.Contains(lang))
        {
            Debug.Log("切换语言：" + lang);
            s_currentLanguage = lang;
        }
        else
        {
            Debug.LogError("当前语言不存在 " + lang);
            return;
        }
        if (oldLan != s_currentLanguage)
        {
            s_languageDataDict.Clear();

            if (OnChangeLanguage != null)
            {
                OnChangeLanguage(s_currentLanguage);
            }
        }
    }

    ///// <summary>
    ///// 兼容旧版本代码，不再建议使用
    ///// </summary>
    //[Obsolete]
    //public static string GetContent(string contentID, List<object> contentParams)
    //{
    //    return GetContent(c_defaultModuleKey, contentID, contentParams.ToArray());
    //}

    ///// <summary>
    ///// 兼容旧版本代码，不再建议使用
    ///// </summary>
    //[Obsolete]
    //public static string GetContent(string contentID, params object[] contentParams)
    //{
    //    return GetContent(c_defaultModuleKey, contentID, contentParams);
    //}

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
            //string key = fullKeyName.Substring(indexEnd + 1);
            string fullFileName = fullKeyName.Remove(indexEnd);

            DataTable data = LoadDataTable(s_currentLanguage, fullFileName);

            foreach (var item in data.TableIDs)
            {
                try
                {
                    if(!s_languageDataDict.ContainsKey(fullFileName + "/" + item))
                    {
                        s_languageDataDict.Add(fullFileName + "/" + item, data[item].GetString(c_valueKey));
                    }

                }
                catch (Exception e)
                {

                    Debug.LogError("Find:"+ fullKeyName + "\n ContainsFullKeyName Error (" + fullFileName + "/" + item + ") -> (" + data[item].GetString(c_valueKey) +")\n"+e);
                }
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
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(content);
        if (contentParams != null && contentParams.Length > 0)
        {
            for (int i = 0; i < contentParams.Length; i++)
            {
                object pars = contentParams[i];
                if (pars == null)
                    continue;
                string replaceTmp = "{" + i + "}";
                stringBuilder.Replace(replaceTmp, pars.ToString());
                // content = content.Replace(replaceTmp, pars.ToString());
            }
        }
        if (ApplicationManager.Instance != null && ApplicationManager.Instance.showLanguageValue && ApplicationManager.Instance.m_AppMode == AppMode.Developing)
        {
            stringBuilder.Insert(0, "[");
            stringBuilder.Insert(stringBuilder.Length-1, "]");
        }

        return stringBuilder.ToString();
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
    /// <summary>
    /// 获得多语言保存的文件名
    /// </summary>
    /// <param name="langeuageName"></param>
    /// <param name="fullkeyFileName"></param>
    /// <returns></returns>
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

    //当前语言是否是汉语
    public static bool CurrentLanguageIsChinese()
    {
        bool isChinese = LanguageManager.CurrentLanguage == SystemLanguage.ChineseSimplified || LanguageManager.CurrentLanguage == SystemLanguage.ChineseTraditional || LanguageManager.CurrentLanguage == SystemLanguage.Chinese;
        return isChinese;
    }
}

