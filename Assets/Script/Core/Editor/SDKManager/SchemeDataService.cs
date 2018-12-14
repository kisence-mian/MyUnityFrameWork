using FrameWork.SDKManager;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SchemeDataService
{
    public const string c_SDKCachePath = ".SDKCache";

    private static List<SchemeData> configList;
    private static List<string> configNameList;

    public static List<SchemeData> ConfigList
    {
        get
        {
            if (configList == null)
            {
                ReloadEditorSchemeData();
            }

            return configList;
        }
    }

    public static List<string> ConfigNameList
    {
        get
        {
            if(configNameList == null)
            {
                ReloadEditorSchemeData();
            }

            return configNameList;
        }
    }

    #region 存取配置

    /// <summary>
    ///加载编辑器设置
    /// </summary>
    public static void ReloadEditorSchemeData()
    {
        configList = new List<SchemeData>();
        configNameList = new List<string>();
        configNameList.Add("None");

        Dictionary<string, object> editConfig = ConfigEditorWindow.GetEditorConfigData(SDKEditorWindow.s_editorConfigName);
        if (editConfig != null)
        {
            List<object> list = (List<object>)editConfig[SDKEditorWindow.s_schemeKey];

            for (int i = 0; i < list.Count; i++)
            {
                SchemeData tmp = JsonUtility.FromJson<SchemeData>(list[i].ToString());
                configList.Add(tmp);
                configNameList.Add(tmp.SchemeName);
            }
        }
    }

    public static void UpdateSchemeData(SchemeData data)
    {
        bool isUpdate = false;

        for (int i = 0; i < ConfigList.Count; i++)
        {
            if(ConfigList[i].SchemeName == data.SchemeName)
            {
                isUpdate = true;
                ConfigList[i] = data;
            }
        }

        if(!isUpdate)
        {
            Debug.LogError("更新失败 没有找到对应的方案 " + data.SchemeName);
        }
    }

    public static void SaveEditorSchemeData()
    {
        Dictionary<string, object> editConfig = new Dictionary<string, object>();
        List<string> list = new List<string>();

        for (int i = 0; i < ConfigList.Count; i++)
        {
            list.Add(JsonUtility.ToJson(ConfigList[i]));
        }

        editConfig.Add(SDKEditorWindow.s_schemeKey, list);
        ConfigEditorWindow.SaveEditorConfigData(SDKEditorWindow.s_editorConfigName, editConfig);
    }

    /// <summary>
    /// 将传入的SchemeData保存到游戏可以读取的地方
    /// </summary>
    /// <param name="schemeData"></param>
    public static void SaveGameSchemeConfig(SchemeData schemeData)
    {
        Debug.Log("SaveGameSchemeConfig " + schemeData.LoginScheme.Count + " " + schemeData.SchemeName);

        if(schemeData != null)
        {
            Dictionary<string, SingleField> config = new Dictionary<string, SingleField>();
            config.Add(SDKManager.c_KeyName, new SingleField(JsonUtility.ToJson(schemeData)));
            ConfigEditorWindow.SaveData(SDKManager.c_ConfigName, config);
            ConfigManager.CleanCache();
        }
        else
        {
            File.Delete( ConfigEditorWindow.GetConfigPath(SDKManager.c_ConfigName));
        }
    }

    public static SchemeData CreateSchemeData(

    string schemeName,
    bool useNewSDKManager,
    List<LoginInterface> loginScheme,
    List<ADInterface> ADScheme,
    List<PayInterface> payScheme,
    List<LogInterface> logScheme,
    List<OtherSDKInterface> otherScheme)
    {
        SchemeData schemeData = new SchemeData();

        schemeData.SchemeName = schemeName;
        schemeData.UseNewSDKManager = useNewSDKManager;

        for (int i = 0; i < loginScheme.Count; i++)
        {
            schemeData.LoginScheme.Add(SerializeConfig(loginScheme[i]));
        }

        for (int i = 0; i < ADScheme.Count; i++)
        {
            schemeData.ADScheme.Add(SerializeConfig(ADScheme[i]));
        }

        for (int i = 0; i < payScheme.Count; i++)
        {
            schemeData.PayScheme.Add(SerializeConfig(payScheme[i]));
        }

        for (int i = 0; i < logScheme.Count; i++)
        {
            schemeData.LogScheme.Add(SerializeConfig(logScheme[i]));
        }

        for (int i = 0; i < otherScheme.Count; i++)
        {
            schemeData.OtherScheme.Add(SerializeConfig(otherScheme[i]));
        }

        return schemeData;
    }

    static SDKConfigData SerializeConfig(SDKInterfaceBase sdkInterface)
    {
        SDKConfigData result = new SDKConfigData();

        if (sdkInterface != null)
        {
            result.SDKName = sdkInterface.GetType().Name;
            result.SDKContent = JsonUtility.ToJson(sdkInterface);
        }
        else
        {
            result.SDKName = "Null";
            result.SDKContent = "";
        }

        return result;
    }

    #endregion

    #region 切换方案

    /// <summary>
    /// 切换方案
    /// 由于自动打包会调用这里，所以将切换宏定义的代码也放在此处，注意！
    /// </summary>
    /// <param name="SchemeName"></param>
    public static void ChangeScheme(string SchemeName)
    {
        SchemeData data = SDKManager.LoadGameSchemeConfig();
        string oldSchemeName = "None";

        Debug.Log("ChangeScheme " + SchemeName);

        if (!IsExitsSchemeName(SchemeName))
        {
            Debug.Log("->" + SchemeName + "<- 方案不存在！ ");
            return;
        }

        if (data != null)
        {
            oldSchemeName = data.SchemeName;
        }

        //方案相同不切换
        if(SchemeName == oldSchemeName)
        {
            return;
        }

        //重新生成游戏内使用的配置
        SaveGameSchemeConfig(GetSchemeData(SchemeName));
        AssetDatabase.Refresh();
    }

    #region 功能函数

    public static bool IsExitsSchemeName(string name)
    {
        for (int i = 0; i < ConfigList.Count; i++)
        {
            if (ConfigList[i].SchemeName == name)
            {
                return true;
            }
        }

        return false;
    }

    public static int GetSchemeIndex(string name)
    {
        for (int i = 0; i < ConfigList.Count; i++)
        {
            if (ConfigList[i].SchemeName == name)
            {
                return i;
            }
        }

        return -1;
    }

    public static SchemeData GetSchemeData(string name)
    {
        if (name == "None")
            return null;

        for (int i = 0; i < ConfigList.Count; i++)
        {
            if (ConfigList[i].SchemeName == name)
            {
                return ConfigList[i];
            }
        }

        throw new System.Exception("GetSchemeData Error not find ->" + name + "<-");
    }
    #endregion

    #region 文件操作

    /// <summary>
    /// 把当前Plugin目录下的所有文件存在 .SDKCache文件夹下，并加上schemeName前缀
    /// </summary>
    /// <param name="schemeName">方案名</param>
    public static void UnloadPluginFile(string schemeName)
    {
        string oldPath = Application.dataPath + "/Plugins";
        string newPath = Application.dataPath + "/" + c_SDKCachePath + "/" + schemeName + "Plugins";

        Debug.Log("SavePluginFile ：oldPath: ->" + oldPath + "<- newPath: ->" + newPath + "<-");

        MoveFiles(oldPath, newPath);
    }

    /// <summary>
    /// 把当前schemeName目录下的所有文件存在 .SDKCache文件夹下
    /// </summary>
    /// <param name="schemeName">方案名</param>
    public static void UnloadSchemeFile(string schemeName)
    {
        string oldPath = Application.dataPath + "/" + schemeName;
        string newPath = Application.dataPath + "/" + c_SDKCachePath + "/" + schemeName;

        Debug.Log("SaveSchemeFile ：oldPath: ->" + oldPath + "<- newPath: ->" + newPath + "<-");

        MoveFiles(oldPath, newPath);
    }

    /// <summary>
    /// 加载 .SDKCache 目录下的所有Plugin文件，放回Plugins目录
    /// 与SavePluginFile是相反操作
    /// </summary>
    /// <param name="schemeName">方案名</param>
    public static void LoadPluginFile(string schemeName)
    {
        string oldPath = Application.dataPath + "/" + c_SDKCachePath + "/" + schemeName + "Plugins";
        string newPath =  Application.dataPath + "/Plugins";

        Debug.Log("LoadPluginFile ：oldPath: ->" + oldPath + "<- newPath: ->" + newPath + "<-");

        MoveFiles(oldPath,newPath);
    }

    /// <summary>
    /// 加载 .SDKCache 目录下的方案文件，放回项目目录
    /// 与SaveSchemeFile是相反操作
    /// </summary>
    /// <param name="schemeName">方案名</param>
    public static void LoadSchemeFile(string schemeName)
    {
        string oldPath = Application.dataPath + "/" + c_SDKCachePath + "/" + schemeName;
        string newPath =  Application.dataPath + "/" + schemeName;

        Debug.Log("LoadSchemeFile ：oldPath: ->" + oldPath + "<- newPath: ->" + newPath + "<-");

        MoveFiles(oldPath, newPath);
    }


    /// <summary>
    /// 清空newPath，并把oldPath的文件全部复制到newPath中
    /// </summary>
    /// <param name="oldPath">旧路径</param>
    /// <param name="newPath">新路径</param>
    public static void MoveFiles(string oldPath,string newPath)
    {
        //删除目标文件夹下所有文件
        if (Directory.Exists(newPath))
        {
            FileTool.SafeDeleteDirectory(newPath);
        }
        else
        {
            FileTool.CreatPath(newPath);
        }

        if (Directory.Exists(oldPath))
        {
            //把当前文件加下的文件拷贝到旧文件夹下
            FileTool.SafeCopyDirectory(oldPath, newPath);
            FileTool.SafeDeleteDirectory(oldPath);
        }
    }

    #endregion
    #endregion

    #region 增删方案

    /// <summary>
    /// 新增方案
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static SchemeData AddScheme(string name)
    {
        SchemeData data = new SchemeData();
        data.SchemeName = name;

        ConfigList.Add(data);
        ConfigNameList.Add(data.SchemeName);
        SaveEditorSchemeData();

        return data;
    }

    /// <summary>
    /// 删除方案
    /// </summary>
    /// <param name="data"></param>
    public static void DelectScheme(SchemeData data)
    {
        ConfigList.Remove(data);
        ConfigNameList.Remove(data.SchemeName);

        SaveEditorSchemeData();
        SaveGameSchemeConfig(null);

        string Path1 = Application.dataPath + "/" + c_SDKCachePath + "/" + data.SchemeName + "Plugins";
        string Path2 = Application.dataPath + "/" + c_SDKCachePath + "/" + data.SchemeName;
        string Path3 = Application.dataPath + "/Plugins";
        string Path4 = Application.dataPath + "/" + data.SchemeName;

        FileTool.SafeDeleteDirectory(Path1);
        FileTool.SafeDeleteDirectory(Path2);
        FileTool.SafeDeleteDirectory(Path3);
        FileTool.SafeDeleteDirectory(Path4);
    }

    #endregion
}
