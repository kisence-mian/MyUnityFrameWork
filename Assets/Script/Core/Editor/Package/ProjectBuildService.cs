using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class ProjectBuildService : Editor
{
    #region 参数解析

    public static string ChannelName
    {
        get
        {
#if UNITY_ANDROID
            //这里遍历所有参数，找到 ChannelName 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("ChannelName"))
                {
                    return arg.Split("-"[0])[1];
                }
            }
            return "android";
#elif UNITY_IOS
            return "ios";
#else
            return "general";
#endif
        }
    }

    public static string ExportPath
    {
        get
        {
            //这里遍历所有参数，找到 ExportPath 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("ExportPath"))
                {
                    return arg.Split("-"[0])[1];
                }
            }
            return Application.dataPath + "/"+ ChannelName + "/"+ ApplicationMode+"/";
        }
    }

    public static AppMode ApplicationMode
    {
        get
        {
            //这里遍历所有参数，找到 AppMpde 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("AppMpde"))
                {
                    return (AppMode)Enum.Parse(typeof(AppMode), arg.Split("-"[0])[1]);
                }
            }
            return AppMode.Developing;
        }
    }

    public static bool isUseAssetsBundle
    {
        get
        {
            //这里遍历所有参数，找到 UseAssetsBundle 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("UseAssetsBundle"))
                {
                    return bool.Parse(arg.Split("-"[0])[1]);
                }
            }
            return false;
        }
    }

    public static string Version
    {
        get
        {
            return Application.version + "." + VersionService.LargeVersion + "." + VersionService.SmallVersion;
        }
    }

    #endregion

    #region 打包函数

    static void BuildForAndroid()
    {
        //if (projectName == "91")
        //{
        //    Function.CopyDirectory(Application.dataPath + "/91", Application.dataPath + "/Plugins/Android");
        //    PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "USE_SHARE");
        //}

        //切换渠道


        if(isUseAssetsBundle)
        {
            BundlePackage();
        }
        else
        {
            //不使用 Bundle 则删除 StreamingAssets 文件夹
            FileTool.DeleteDirectory(Application.dataPath + "/StreamingAssets");
        }

        string path = ExportPath + "/" + GetPackageName() + ".apk";
        BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
    }

    static void BundlePackage()
    {
        //自动增加小版本号
        VersionService.SmallVersion++;
        VersionService.CreateVersionFile();

        //打Bundle包
        PackageService.Package(PackageEditorConfigService.RelyPackages, PackageEditorConfigService.Bundles);

        //删除 Resources 文件夹
        FileTool.DeleteDirectory(Application.dataPath + "/Resources");
    }

    #endregion

    #region 功能函数

    //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }

    static string GetPackageName()
    {
        return Application.productName + "_" + Version + "_"+ ChannelName + "_" + ApplicationMode;
    }

    #endregion
}