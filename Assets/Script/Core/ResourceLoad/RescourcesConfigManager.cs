using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class RescourcesConfigManager 
{
    public const string c_configFileName = "BundleConfig";

    public const string c_relyBundleKey = "relyBundles";
    public const string c_bundlesKey = "AssetsBundles";

    static Dictionary<string, BundleConfig> m_relyBundleConfigs;
    static Dictionary<string, BundleConfig> m_bundleConfigs ;

    public static void Initialize()
    {
        Dictionary<string, SingleField> data = GetResourcesConfig();

        if (data == null
            || !data.ContainsKey(c_relyBundleKey) 
            || !data.ContainsKey(c_bundlesKey))
        {
            throw new Exception("RecourcesConfigManager Initialize Exception: " + c_configFileName + "file is not exits");
        }

        m_relyBundleConfigs = JsonTool.Json2Dictionary<BundleConfig>(data[c_relyBundleKey].GetString());
        m_bundleConfigs     = JsonTool.Json2Dictionary<BundleConfig>(data[c_bundlesKey].GetString());
    }

    public static BundleConfig GetBundleConfig(string bundleName)
    {
        if (m_bundleConfigs == null)
        {
            throw new Exception("RecourcesConfigManager GetBundleConfig : bundleConfigs is null  do you Initialize?");
        }

        if (m_bundleConfigs.ContainsKey(bundleName))
        {
            return m_bundleConfigs[bundleName];
        }
        else
        {
            throw new Exception("RecourcesConfigManager GetBundleConfig : Dont find " + bundleName + " please check BundleConfig!");
        }
    }

    public static BundleConfig GetRelyBundleConfig(string bundleName)
    {
        if (m_relyBundleConfigs == null)
        {
            throw new Exception("RecourcesConfigManager GetRelyBundleConfig Exception: relyBundleConfigs is null do you Initialize?");
        }

        if (m_relyBundleConfigs.ContainsKey(bundleName))
        {
            return m_relyBundleConfigs[bundleName];
        }
        else
        {
            throw new Exception("RecourcesConfigManager GetRelyBundleConfig Exception: Dont find " + bundleName + " please check BundleConfig!");
        }
    }


    public static Dictionary<string, SingleField> GetResourcesConfig()
    {
        string dataJson = "";

        ResLoadType type = ResLoadType.Streaming;

        dataJson = ResourceIOTool.ReadStringByResource(
            PathTool.GetRelativelyPath(ConfigManager.c_directoryName,
                                        c_configFileName,
                                        ConfigManager.c_expandName));

        if (dataJson == "")
        {
            throw new Exception("ResourcesConfig not find " + c_configFileName);
        }
        else
        {
            return JsonTool.Json2Dictionary<SingleField>(dataJson);
        }
    }
}

public class BundleConfig
{
    public string name;               //名称
    public string path;               //加载相对路径
    public string[] relyPackages;     //依赖包
    public string md5;                //md5
    //[System.NonSerialized]
    //public ResLoadType loadType;      //加载类型
}

