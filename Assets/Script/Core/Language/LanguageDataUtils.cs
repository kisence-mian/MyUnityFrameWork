using HDJ.Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LanguageDataUtils
{
    public const string SavePathDir = "Assets/Resources/Data/Language/";
    public static LanguageSettingConfig LoadEditorConfig()
    {
        LanguageSettingConfig config;
        string json = FileUtils.LoadTextFileByPath(SavePathDir + LanguageManager.c_configFileName + ".txt");
        if (!string.IsNullOrEmpty(json))
            config = JsonUtils.FromJson<LanguageSettingConfig>(json);
        else
        {
            config = new LanguageSettingConfig();

            //config.defaultLanguage = SystemLanguage.ChineseSimplified;
            //config.gameExistLanguages.Add(SystemLanguage.ChineseSimplified);
        }
        return config;
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

