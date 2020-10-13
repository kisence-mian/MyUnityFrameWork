using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using HDJ.Framework.Core;
using System;

public class BootConfigBuildPreprocess : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("OnPreprocessBuild");

        GameBootConfig config = GameBootConfig.LoadConfig();
        if (config != null)
        {
            config.buildTime = DateTime.Now.Ticks;
            GameBootConfig.Save(config);
            AssetDatabase.Refresh();
            Debug.Log("OnPreprocessBuild GameBootConfig:" + config.buildTime);
        }
    }
}