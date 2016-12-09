using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class ResourcesConfigManager 
{
    public const string c_configFileName = "ResourcesConfig";

    public const string c_relyBundleKey = "relyBundles";
    public const string c_bundlesKey = "AssetsBundles";

    static Dictionary<string, ResourcesConfig> m_relyBundleConfigs;
    static Dictionary<string, ResourcesConfig> m_bundleConfigs ;

    public static void Initialize()
    {
        Dictionary<string, SingleField> data = GetResourcesConfig();

        if (data == null
            || !data.ContainsKey(c_relyBundleKey) 
            || !data.ContainsKey(c_bundlesKey))
        {
            throw new Exception("RecourcesConfigManager Initialize Exception: " + c_configFileName + "file is not exits");
        }

        m_relyBundleConfigs = JsonTool.Json2Dictionary<ResourcesConfig>(data[c_relyBundleKey].GetString());
        m_bundleConfigs     = JsonTool.Json2Dictionary<ResourcesConfig>(data[c_bundlesKey].GetString());
    }

    public static ResourcesConfig GetBundleConfig(string bundleName)
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
            throw new Exception("RecourcesConfigManager GetBundleConfig : Dont find ->" + bundleName + "<- please check BundleConfig!");
        }
    }

    public static ResourcesConfig GetRelyBundleConfig(string bundleName)
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

    //资源路径数据不依赖任何其他数据
    static Dictionary<string, SingleField> GetResourcesConfig()
    {
        string dataJson = "";

        if (ResourceManager.m_gameLoadType == ResLoadType.Resource)
        {
            dataJson = ResourceIOTool.ReadStringByResource(
                PathTool.GetRelativelyPath(ConfigManager.c_directoryName,
                                            c_configFileName,
                                            ConfigManager.c_expandName));
        }
        else
        {
            ResLoadType type = ResLoadType.Streaming;

            if(RecordManager.GetData(AssetsBundleManager.c_HotUpdateRecordName).GetRecord(c_configFileName,false))
            {
                type = ResLoadType.Persistent;
            }

            dataJson = ResourceIOTool.ReadStringByBundle(
                PathTool.GetAbsolutePath(
                     type,
                     PathTool.GetRelativelyPath(
                                     ConfigManager.c_directoryName,
                                     c_configFileName,
                                     AssetsBundleManager.c_AssetsBundlesExpandName)));
        }

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

public class ResourcesConfig
{
    public string name;               //名称
    public string path;               //加载相对路径
    public string[] relyPackages;     //依赖包
    public string md5;                //md5
    //[System.NonSerialized]
    //public ResLoadType loadType;      //加载类型
}

