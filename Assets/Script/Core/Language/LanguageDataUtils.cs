using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LanguageDataUtils
{
    public const string SavePathDir = "Assets/Resources/Data/Language/";
    //public static LanguageSettingConfig LoadEditorConfig()
    //{

    //    if(ResourcesConfigManager.GetIsExitRes(LanguageManager.c_configFileName))
    //    {
    //        LanguageSettingConfig config;

    //        string json = ResourceManager.Load<TextAsset>(LanguageManager.c_configFileName).text; 

    //        if (!string.IsNullOrEmpty(json))
    //            config = JsonUtils.FromJson<LanguageSettingConfig>(json);
    //        else
    //        {
    //            config = null;

    //            //config.defaultLanguage = SystemLanguage.ChineseSimplified;
    //            //config.gameExistLanguages.Add(SystemLanguage.ChineseSimplified);
    //        }
    //        return config;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}
    public static LanguageSettingConfig LoadRuntimeConfig()
    {

        if (ResourcesConfigManager.GetIsExitRes(LanguageManager.c_configFileName))
        {
            LanguageSettingConfig config;

            string json = ResourceManager.LoadText(LanguageManager.c_configFileName);
            ResourceManager.DestoryAssetsCounter(LanguageManager.c_configFileName);
            if (!string.IsNullOrEmpty(json))
                config = JsonUtils.FromJson<LanguageSettingConfig>(json);
            else
            {
                config = null;

                //config.defaultLanguage = SystemLanguage.ChineseSimplified;
                //config.gameExistLanguages.Add(SystemLanguage.ChineseSimplified);
            }
            return config;
        }
        else
        {
            return null;
        }
    }
    public static void SaveEditorConfig(LanguageSettingConfig config)
    {
        string json = JsonUtils.ToJson(config);
        FileUtils.CreateTextFile(SavePathDir + LanguageManager.c_configFileName + ".txt", json);
    }

    public static DataTable LoadFileData(SystemLanguage language, string fullKeyFileName)
    {
        if (string.IsNullOrEmpty(fullKeyFileName))
            return null;

        string path = GetLanguageSavePath(language, fullKeyFileName);
        string text = FileUtils.LoadTextFileByPath(path);
        //Debug.Log("path :" + path);
        //Debug.Log("Text :" + text);
        return DataTable.Analysis(text);
    }
    public static string GetLanguageSavePath(SystemLanguage langeuageName, string fullkeyFileName)
    {
        return LanguageDataUtils.SavePathDir + langeuageName + "/" + LanguageManager.GetLanguageDataName(langeuageName, fullkeyFileName) + ".txt";
    }
}

