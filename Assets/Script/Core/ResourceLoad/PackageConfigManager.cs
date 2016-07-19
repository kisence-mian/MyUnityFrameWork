using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public static class PackageConfigManager 
{
    public static string configFileName = "PackageConfig";

    public static string key_relyPackages = "relyBundles";
    public static string key_bundles      = "AssetsBundles";

    static Dictionary<string, PackageConfig> relyPackageConfigs = new Dictionary<string, PackageConfig>();
    static Dictionary<string, PackageConfig> PackageConfigs = new Dictionary<string, PackageConfig>();

    static PackageConfigManager()
    {
        Dictionary<string, object> data = ConfigManager.GetConfigData(configFileName);

        relyPackageConfigs = JsonTool.Json2Dictionary<PackageConfig>(data[key_relyPackages].ToString());
        relyPackageConfigs = JsonTool.Json2Dictionary<PackageConfig>(data[key_relyPackages].ToString());
    }

    public static PackageConfig GetPackageConfig(string bundleName)
    {
        if (PackageConfigs.ContainsKey(bundleName) )
        {
            return PackageConfigs[bundleName];
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
    public ResLoadType loadType;      //加载绝对路径
}

