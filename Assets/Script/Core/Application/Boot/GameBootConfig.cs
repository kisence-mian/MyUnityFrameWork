using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 游戏模块设置配置文件
/// </summary>
public class GameBootConfig
{
    #region 设置启动参数
    //[ShowGUIName("游戏运行模式")]
    ///// <summary>
    ///// 游戏运行模式
    ///// </summary>
    //public AppMode gameRunType = AppMode.Developing;

    [ShowGUIName("后台运行模式")]
    public bool runInBackground = true;

    [NoShowInEditor]
    public Dictionary<string, ClassValue> allAppModuleSetting = new Dictionary<string, ClassValue>();
    #endregion

    #region 其他参数
    /// <summary>
    /// 当打包是写入打包时间,取值DateTime.Now.Ticks
    /// </summary>
    public long buildTime;
    #endregion


    #region 读取写入
    public const string ConfigFileName = "GameBootConfig";
    public static GameBootConfig LoadConfig()
    {
        string jsonData = null;
        if (Application.isEditor)
        {
            jsonData = ResourcesLoadConfig();

        }
        else
        {
            string fileName = ConfigFileName;
            string persistentDataPath = Application.persistentDataPath + "/Configs/" + fileName + ".txt";
            jsonData = ResourcesLoadConfig();
            if (!File.Exists(persistentDataPath))
            {
                if (string.IsNullOrEmpty(jsonData))
                {
                    return null;
                }
                else
                {
                    Debug.Log("GameBootConfig写入沙盒");
                    FileUtils.CreateTextFile(persistentDataPath, jsonData);
                }
            }
            else
            {
                //比较包里的配置和沙河路径的配置buildTime，当不一致时 以包里的覆盖沙盒的，否则使用沙盒路径的（便于保存修改）
                GameBootConfig resConfig = JsonUtils.FromJson<GameBootConfig>(jsonData);

                string perJsonData = FileUtils.LoadTextFileByPath(persistentDataPath);
                GameBootConfig perConfig = JsonUtils.FromJson<GameBootConfig>(perJsonData);

                if (perConfig == null || perConfig.buildTime != resConfig.buildTime)
                {
                    Debug.Log("GameBootConfig覆盖写入：" + resConfig.buildTime);
                    FileUtils.CreateTextFile(persistentDataPath, jsonData);
                    return resConfig;
                }
                else
                {
                    return perConfig;
                }
            }
        }
        return JsonUtils.FromJson<GameBootConfig>(jsonData);
    }
    private static string ResourcesLoadConfig()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(ConfigFileName);
        if (textAsset != null)
        {
            return textAsset.text;
        }
        else
        {
            return null;
        }
    }
    private const string SavePathDir = "Assets/GameCofig/Resources/";
    public static void Save(GameBootConfig config)
    {
        string json = JsonUtils.ToJson(config);
        FileUtils.CreateTextFile(SavePathDir + ConfigFileName + ".txt", json);
    }
    #endregion

}