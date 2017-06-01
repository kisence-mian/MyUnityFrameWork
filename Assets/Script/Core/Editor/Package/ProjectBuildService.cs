using System;
using System.Collections.Generic;
using System.IO;
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
            return "Android";
#elif UNITY_IOS
            return "Ios";
#else
            return "General";
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
                    return arg.Split("-"[0])[1] + "/" + ChannelName + "/" + ApplicationMode + "/";
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
                if (arg.StartsWith("AppMode"))
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

    public static bool isUseLua
    {
        get
        {
            //这里遍历所有参数，找到 UseLua 开头的参数， 然后把-符号 后面的字符串返回，
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg.StartsWith("UseLua"))
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
        //输出日志
        PrintDebug();

        //使用Lua
        SetLua(isUseLua);

        //发布模式
        SetApplicationMode(ApplicationMode);

        //切换渠道
        ChangeChannel(ChannelName);

        //使用Resource或者使用Bundle
        UseResourcesOrBundle(isUseAssetsBundle);

        //打包
        string path = ExportPath + "/" + GetPackageName() + ".apk";

        BuildPipeline.BuildPlayer(GetBuildScenes(), path, BuildTarget.Android, BuildOptions.None);
    }

    static void PrintDebug()
    {
        string debugString = "";

        debugString += ">>>============================================================自动打包输出============================================================<<<\n";

        foreach (string arg in Environment.GetCommandLineArgs())
        {
            debugString += "参数：" + arg + "\n";
        }

        debugString += "\n";

        debugString += "是否使用 Bundle 打包: " + isUseAssetsBundle + "\n";
        debugString += "是否使用 Lua : " + isUseLua + "\n";
        debugString += "渠道名: " + ChannelName + "\n";
        debugString += "发布模式: " + ApplicationMode + "\n";
        debugString += "导出路径: " + ExportPath + "\n";
        debugString += ">>>====================================================================================================================================<<<\n";

        Debug.Log(debugString);
    }

    static void SetApplicationMode(AppMode mode)
    {
        string appModeDefine = "";

        switch(mode)
        {
            case AppMode.Developing:
                appModeDefine = "APPMODE_DEV"; break;
            case AppMode.QA:
                appModeDefine = "APPMODE_QA"; break;
            case AppMode.Release:
                appModeDefine = "APPMODE_REL"; break;
        }

        SetScriptDefine(appModeDefine);
    }

    static void SetLua(bool useLua)
    {
        if(useLua)
        {
            SetScriptDefine("USE_LUA");
        }
    }

    /// <summary>
    /// 切换渠道
    /// </summary>
    static void ChangeChannel(string channelName)
    {

#if UNITY_ANDROID
        SchemeDataService.ChangeScheme(channelName);
#endif

    }

    /// <summary>
    ///打包或者使用Bundle流程
    /// </summary>
    static void UseResourcesOrBundle(bool useBundle)
    {
        if (useBundle)
        {
            BundlePackage();
        }
        else
        {
            if (File.Exists(Application.dataPath + "/StreamingAssets"))
            {
                //不使用 Bundle 则删除 StreamingAssets 文件夹
                FileTool.DeleteDirectory(Application.dataPath + "/StreamingAssets");
            }
        }
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
        return Application.productName + "_" + Version + "_"+ ChannelName + "_" + GetModeName(ApplicationMode) +"_"+ GetTimeString();
    }

    static string GetTimeString()
    {
        DateTime date = DateTime.Now;

        return date.Year + string.Format("{0:d2}", date.Month) + string.Format("{0:d2}", date.Day) + "_" + string.Format("{0:d2}", date.Hour) + string.Format("{0:d2}", date.Minute);
    }

    static string GetModeName(AppMode mode)
    {
        switch (mode)
        {
            case AppMode.Developing:
                return "Dev"; ;
            case AppMode.QA:
                return "QA"; ;
            case AppMode.Release:
                return "Rel"; ;
            default: return "unknow";
        }
    }

    public static void SetScriptDefine(string symbols)
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
#if UNITY_ANDROID
        targetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
        targetGroup = BuildTargetGroup.iOS;
#endif
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        if (!define.Contains(symbols))
        {
            define += ";" + symbols;
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, define);
    }

    #endregion
}