using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class BundleConfigManager 
{
    public static string configFileName = "BundleConfig";

    public static string key_relyBundle   = "relyBundles";
    public static string key_bundles      = "AssetsBundles";

    static Dictionary<string, BundleConfig> relyBundleConfigs;
    static Dictionary<string, BundleConfig> bundleConfigs ;

    public static void Initialize()
    {
        Dictionary<string, object> data = ConfigManager.GetData(configFileName);

        if (data == null)
        {
            throw new Exception("BundleConfigManager Initialize Exception: " + configFileName + "file is not exits");
        }

        relyBundleConfigs = JsonTool.Json2Dictionary<BundleConfig>(data[key_relyBundle].ToString());
        bundleConfigs     = JsonTool.Json2Dictionary<BundleConfig>(data[key_bundles   ].ToString());
    }

    public static BundleConfig GetBundleConfig(string bundleName)
    {
        if (bundleConfigs == null)
        {
            throw new Exception("BundleConfigManager GetBundleConfig Exception: bundleConfigs is null  do you Initialize?");
        }

        if (bundleConfigs.ContainsKey(bundleName))
        {
            return bundleConfigs[bundleName];
        }
        else
        {
            throw new Exception("BundleConfigManager GetBundleConfig Exception: Dont find " + bundleName + " please check BundleConfig!");
        }
    }

    public static BundleConfig GetRelyBundleConfig(string bundleName)
    {
        if (relyBundleConfigs == null)
        {
            throw new Exception("BundleConfigManager GetRelyBundleConfig Exception: relyBundleConfigs is null do you Initialize?");
        }

        if (relyBundleConfigs.ContainsKey(bundleName))
        {
            return relyBundleConfigs[bundleName];
        }
        else
        {
            throw new Exception("BundleConfigManager GetRelyBundleConfig Exception: Dont find " + bundleName + " please check BundleConfig!");
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
    public ResLoadType loadType;      //加载绝对路径
}

