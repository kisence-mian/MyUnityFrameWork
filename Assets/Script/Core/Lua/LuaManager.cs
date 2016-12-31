using UnityEngine;
using System.Collections;
using LuaInterface;
using System;

public class LuaManager
{
    static LuaState state = new LuaState();

    public const string c_LuaConfigName = "LuaConfig";
    public const string c_LuaListKey = "LuaList";
    //public const string c_MainName = "LuaConfig";

    /// <summary>
    /// 这里仅仅初始化LuaState,热更新结束后调用StartLua正式启动Lua
    /// </summary>
    public static void Init()
    {
        try
        {
            state.Start();
            ApplicationManager.s_OnApplicationUpdate += Update;

        }
        catch (Exception e)
        {
            Debug.LogError("Lua Init Execption " + e.ToString());
        }
    }

    /// <summary>
    /// 加载全部Lua文件并执行
    /// </summary>
    public static void StartLua()
    {
        try
        {
            //取出所有的Lua文件并执行
            string[] luaList = ConfigManager.GetData(c_LuaConfigName)[c_LuaListKey].GetStringArray();

            for (int i = 0; i < luaList.Length; i++)
            {
                DoLuaFile(luaList[i]);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Lua Start Execption " + e.ToString());
        }
    }

    static void Update()
    {

    }

    public static void DoLuaFile(string fileName)
    {
        string content = ResourceManager.ReadTextFile(fileName);
        state.DoString(content, fileName);
    }

    static void UIEventHandle(UIEvent e)
    {

    }
}
