using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public static class PackageConfigManager 
{
    static Dictionary<string, PackageConfig> relyPackageConfigs = new Dictionary<string, PackageConfig>();

    static Dictionary<string, PackageConfig> PackageConfigs = new Dictionary<string, PackageConfig>();

    static PackageConfigManager()
    {

    }

    public static PackageConfig GetPackageConfig(string bundleName)
    {
        if (PackageConfigs.ContainsKey(bundleName) )
        {
            return PackageConfigs[bundleName];
        }
        else
        {
            return new PackageConfig();
        }
    }
}

public struct PackageConfig
{
    public string path;
    public List<string> relyPackages;
}

