using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VersionService
{
    private static int largeVersion = 1;  
    private static int smallVersion = 1;

    static bool isInit = false;

    ///大版本号  
    public static int LargeVersion
    {
        get
        {
            if(!isInit)
            {
                isInit = true;
                AnalysisVersionFile();
            }

            return largeVersion;
        }

        set
        {
            if (!isInit)
            {
                isInit = true;
                AnalysisVersionFile();
            }

            largeVersion = value;
        }
    }

    ///小版本号
    public static int SmallVersion
    {
        get
        {
            if (!isInit)
            {
                isInit = true;
                AnalysisVersionFile();
            }

            return smallVersion;
        }

        set
        {
            if (!isInit)
            {
                isInit = true;
                AnalysisVersionFile();
            }

            smallVersion = value;
        }
    }

    //生成版本文件
    public static void CreateVersionFile()
    {
        Dictionary<string, object> VersionData = new Dictionary<string, object>();

        VersionData.Add(HotUpdateManager.c_largeVersionKey, largeVersion);
        VersionData.Add(HotUpdateManager.c_smallVersonKey, smallVersion);

        EditorUtil.WriteStringByFile(
            PathTool.GetAbsolutePath(ResLoadLocation.Resource, HotUpdateManager.c_versionFileName + ".json"),
            FrameWork.Json.Serialize(VersionData));

        AssetDatabase.Refresh();
    }

    //解析版本号文件
    static void AnalysisVersionFile()
    {
        string version = ResourceIOTool.ReadStringByFile(PathTool.GetAbsolutePath(ResLoadLocation.Resource, HotUpdateManager.c_versionFileName + ".json"));

        Dictionary<string, object> VersionData = null;
        if (version == "")
        {
            VersionData = null;
        }
        else
        {
            VersionData = (Dictionary<string, object>)FrameWork.Json.Deserialize(version);
        }

        if (VersionData == null)
        {
            largeVersion = -1;
            smallVersion = -1;
            return;
        }

        if (VersionData.ContainsKey(HotUpdateManager.c_largeVersionKey))
        {
            largeVersion = int.Parse(VersionData[HotUpdateManager.c_largeVersionKey].ToString());
        }
        else
        {
            largeVersion = -1;
        }

        if (VersionData.ContainsKey(HotUpdateManager.c_smallVersonKey))
        {
            smallVersion = int.Parse(VersionData[HotUpdateManager.c_smallVersonKey].ToString());
        }
        else
        {
            smallVersion = -1;
        }
    }
}
