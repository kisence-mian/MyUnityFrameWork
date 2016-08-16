using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class BundleConfigManager 
{
    public static string configFileName = "BundleConfig";

    public static string key_relyBundle = "relyBundles";
    public static string key_bundles      = "AssetsBundles";

    static Dictionary<string, BundleConfig> relyBundleConfigs = new Dictionary<string, BundleConfig>();
    static Dictionary<string, BundleConfig> bundleConfigs = new Dictionary<string, BundleConfig>();


    public static void Initialize()
    {
        Dictionary<string, object> data = ConfigManager.GetData(configFileName);

        relyBundleConfigs = JsonTool.Json2Dictionary<BundleConfig>(data[key_relyBundle].ToString());
        bundleConfigs     = JsonTool.Json2Dictionary<BundleConfig>(data[key_bundles   ].ToString());
    }

    public static BundleConfig GetBundleConfig(string bundleName)
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

    public static BundleConfig GetRelyBundleConfig(string bundleName)
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

public class BundleConfig
{
    public string name;               //名称
    public string path;               //加载相对路径
    public string[] relyPackages;     //依赖包
    public string md5;                //md5
    //[System.NonSerialized]
    public ResLoadType loadType;      //加载绝对路径
}

