using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public static class BundleConfigManager 
{
    public static string configFileName = "BundleConfig";

    public static string key_relyBundle = "relyBundles";
    public static string key_bundles      = "AssetsBundles";

    static Dictionary<string, PackageConfig> relyBundleConfigs = new Dictionary<string, PackageConfig>();
    static Dictionary<string, PackageConfig> bundleConfigs = new Dictionary<string, PackageConfig>();


    public static void Initialize()
    {
        Dictionary<string, object> data = ConfigManager.GetData(configFileName);

        relyBundleConfigs = JsonTool.Json2Dictionary<PackageConfig>(data[key_relyBundle].ToString());
        bundleConfigs     = JsonTool.Json2Dictionary<PackageConfig>(data[key_bundles   ].ToString());
    }

    public static PackageConfig GetBundleConfig(string bundleName)
    {
        if (bundleConfigs.ContainsKey(bundleName) )
        {
            return bundleConfigs[bundleName];
        }
        else
        {
            return null;
        }
    }

    public static PackageConfig GetRelyBundleConfig(string bundleName)
    {
        if (relyBundleConfigs.ContainsKey(bundleName))
        {
            return relyBundleConfigs[bundleName];
        }
        else
        {
            return null;
        }
    }
}

public class PackageConfig
{
    public string name;               //名称
    public string path;               //加载相对路径
    public string[] relyPackages;     //依赖包
    public string md5;                //md5
    //[System.NonSerialized]
    public ResLoadType loadType;      //加载绝对路径
}

