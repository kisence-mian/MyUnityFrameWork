using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 打包设置管理器
/// </summary>
public static class PackageConfigManager
{
    static Dictionary<string, PackageConfig> PackageConfigs = new Dictionary<string, PackageConfig>();

    static PackageConfigManager()
    {

    }

    public static PackageConfig GetPackageConfig(string bundleName)
    {
        if (!PackageConfigs.ContainsKey(bundleName))
        {
            Debug.LogError("Can't find " + bundleName + " packageConfig !");
            return new PackageConfig();
        }
        else
        {
            return PackageConfigs[bundleName];
        }
    }
}

public struct PackageConfig
{
    public string path;
    public List<string> relyPackages;
}
