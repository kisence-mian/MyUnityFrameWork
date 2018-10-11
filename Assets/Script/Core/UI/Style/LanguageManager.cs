using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System;

public class LanguageManager
{
    public const string c_configFileName     = "LanguageConfig";
    public const string c_defaultLanguageKey = "DefaultLanguage";
    public const string c_languageListKey    = "languageFields";
    public const string c_moduleListKey      = "Module";
    public const string c_defaultModuleKey   = "default";

    public const string c_DataFilePrefix = "LangData_";
    public const string c_mainKey  = "key";
    public const string c_valueKey = "value";

    static public List<string> s_LanguageList = new List<string>();                    //语言列表
    static public List<string> s_modelList = new List<string>();                       //模块列表
    static public SystemLanguage s_defaultlanguage = SystemLanguage.ChineseSimplified; //默认语言

    static public SystemLanguage s_currentLanguage = SystemLanguage.ChineseSimplified; //当前语言
    static public Dictionary<string,DataTable> s_languageDataDict = new Dictionary<string,DataTable>();//所有语言数据

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

            Debug.Log("当前语言: " + Application.systemLanguage);
            LoadConfig();
            SetLanguage(Application.systemLanguage);
        }
    }

    public static void SetLanguage(SystemLanguage lang)
    {
        s_currentLanguage = lang;

        if (s_LanguageList.Contains(lang.ToString()))
        {
            Loadlanguage(lang);
        }
        else
        {
            //Debug.Log("当前语言不存在 " + lang);

            Loadlanguage(s_defaultlanguage);
        }

        GlobalEvent.DispatchEvent(LanguageEventEnum.LanguageChange, lang);
    }

    static void LoadConfig()
    {
        s_LanguageList.Clear();
        s_modelList.Clear();
        //读取配置
        Dictionary<string, SingleField> config = ConfigManager.GetData(c_configFileName);

        //读取默认语言
        s_defaultlanguage = config[c_defaultLanguageKey].GetEnum<SystemLanguage>();

        //读取语言列表
        string[] languageList = config[c_languageListKey].GetStringArray();
        for (int i = 0; i < languageList.Length; i++)
        {
			 s_LanguageList.Add(languageList[i]);
        }

        //读取模块列表
        string[] modelList = config[c_moduleListKey].GetStringArray();
        for (int i = 0; i < modelList.Length; i++)
        {
            s_modelList.Add(modelList[i]);
        }
    }

    static void Loadlanguage(SystemLanguage language)
    {
        s_languageDataDict.Clear();

        for (int i = 0; i < s_modelList.Count; i++)
        {
            s_languageDataDict.Add(s_modelList[i], DataManager.GetData(GetLanguageDataSaveName(language.ToString(), s_modelList[i])));
        }
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
    /// <param name="moduleName_key"></param>
    /// <param name="contentParams"></param>
    /// <returns></returns>
    public static string GetContentByKey(string moduleName_key, params object[] contentParams)
    {
        if (string.IsNullOrEmpty(moduleName_key))
            return "Error : key is null";
        string[] res = ChangeKey2ModuleID(moduleName_key);
        if (res == null)
            return "Error : format is error！";
        return GetContent(res[0], res[1], contentParams); 
    }
    public static bool HaveKey(string moduleName_key)
    {
        Init();
        if (string.IsNullOrEmpty(moduleName_key))
            return false;
        string[] res = ChangeKey2ModuleID(moduleName_key);
        if (res == null)
            return false;
        string moduleName = res[0];
        string contentID = res[1];

        if (!s_languageDataDict.ContainsKey(moduleName))
        {
            //Debug.Log("s_languageDataDict.ContainsKey》》》 moduleName :" + moduleName + "  contentID :" + contentID);
            return false;
        }

        DataTable data = s_languageDataDict[moduleName];

        if (!data.ContainsKey(contentID))
        {
            //Debug.Log("data.ContainsKeyy》》》 moduleName :" + moduleName + "  contentID :" + contentID);
            return false;
        }

        return true;
    }
    private static string[] ChangeKey2ModuleID(string moduleName_key)
    {
        if (!moduleName_key.Contains("/"))
        {
            Debug.LogError("多语言key格式错误 ：" + moduleName_key);
            return null;
        }
        string[] ss = moduleName_key.Split('/');
        string moduleName = "";
        string contentID = "";
        if (ss.Length > 2)
        {
            for (int i = 0; i < ss.Length - 1; i++)
            {
                moduleName += ss[i];
                if (i != ss.Length - 2)
                    moduleName += "_";
            }
            contentID = ss[ss.Length - 1];
        }
        else
        {
            moduleName = ss[0];
            contentID = ss[1];
        }
        string[] data = new string[] { moduleName, contentID };
        return data;
    }
    public static string GetContent(string moduleName, string contentID, params object[] contentParams)
    {
        Init();

        string content = null;

        if (!s_languageDataDict.ContainsKey(moduleName))
        {
            Debug.LogError("Dont find language moduleName:->" + moduleName + "<- contentID: ->" + contentID + "<-");
            return "Dont find language : ->" + contentID + "<-";
        }

        DataTable data = s_languageDataDict[moduleName];

        if (data.ContainsKey(contentID))
        {
            content = data[contentID].GetString("value");
        }
        else
        {
            Debug.LogError("Dont find language moduleName:->" + moduleName + "<- contentID: ->" + contentID + "<-");
            return "Dont find language : ->" + contentID + "<-";
        }

        if (contentParams != null && contentParams.Length > 0)         
        {
            for (int i = 0; i < contentParams.Length; i++)
            {
                string replaceTmp = "{" + i + "}";
                content = content.Replace(replaceTmp, contentParams[i].ToString());
            }      
        }
        if (ApplicationManager.Instance!=null && ApplicationManager.Instance.showLanguageValue && ApplicationManager.Instance.m_AppMode == AppMode.Developing)
            content = "[" + content + "]";
        return content;
    }

    public static string GetLanguageDataSaveName(string langeuageName, string modelName)
    {
        if(Application.isPlaying)
        {
            return GetLanguageDataName(langeuageName, modelName);
        }
        else
        {
            return "Language" + "/" + langeuageName + "/" + GetLanguageDataName(langeuageName, modelName);
        }

    }

    public static string GetLanguageDataName(string langeuageName, string modelName)
    {
        return c_DataFilePrefix + langeuageName + "_" + modelName;
    }
}

public enum LanguageEventEnum
{
    LanguageChange,
}
