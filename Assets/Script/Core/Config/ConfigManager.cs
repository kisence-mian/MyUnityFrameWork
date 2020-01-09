using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FrameWork;
using System.Text;
using System;

/// <summary>
/// 配置管理器，只读
/// </summary>
public static class ConfigManager 
{
    public const string c_directoryName = "Config";
    public const string c_expandName    = "json";

    /// <summary>
    /// 配置缓存
    /// </summary>
    static Dictionary<string, Dictionary<string, SingleField>> s_configCache = new Dictionary<string,Dictionary<string, SingleField>>();

    public static bool GetIsExistConfig(string ConfigName)
    {
        return ResourcesConfigManager.GetIsExitRes(ConfigName);
    }

    public static Dictionary<string, SingleField> GetData(string ConfigName)
    {
        if (s_configCache.ContainsKey(ConfigName))
        {
            return s_configCache[ConfigName];
        }

        string dataJson = "";

        //#if UNITY_EDITOR
        //if (!Application.isPlaying)
        //{
        //    dataJson = ResourceIOTool.ReadStringByResource(
        //        PathTool.GetRelativelyPath(c_directoryName,
        //                    ConfigName,
        //                    c_expandName));
        //}

        ////#else
        //else
        //{
        //    dataJson = ResourceManager.LoadText(ConfigName);
        //}
        //#endif
        dataJson = ResourceManager.LoadText(ConfigName);

        if (dataJson == "")
        {
            throw new Exception("ConfigManager GetData not find " + ConfigName);
        }
        else
        {
            Dictionary<string, SingleField> config = JsonTool.Json2Dictionary<SingleField>(dataJson);

            s_configCache.Add(ConfigName, config);
            return config;
        }
    }

    public static SingleField GetData(string ConfigName,string key)
    {
        return GetData(ConfigName)[key];
    }

    public static void CleanCache()
    {
        foreach (var item in s_configCache.Keys)
        {
            ResourceManager.DestoryAssetsCounter(item);
        }
        s_configCache.Clear();
    }
}
