

using UnityEngine;
using System;
using System.Collections.Generic;
#if USE_LUA
using LuaInterface;
#endif

public class LuaManager
{
    public const string c_LuaConfigName = "LuaConfig";
    public const string c_LuaLibraryListKey = "LuaLibList";
    public const string c_LuaListKey = "LuaList";

#if USE_LUA

    private static LuaState s_state = new LuaState();

    public static LuaState LuaState
    {
        get { return s_state; }
    }



    public static bool s_isUpdate = false;

    /// <summary>
    /// 这里仅仅初始化LuaState,热更新结束后调用StartLua正式启动Lua
    /// </summary>
    public static void Init()
    {

        try
        {
            s_state.Start();
            LuaBinder.Bind(s_state);

            LoadLua();
            LaunchLua();

            ApplicationManager.s_OnApplicationUpdate += Update;
        }
        catch (Exception e)
        {
            Debug.LogError("Lua Init Execption " + e.ToString());
        }

    }

    /// <summary>
    /// 加载全部Lua文件
    /// </summary>
    public static void LoadLua()
    {
        //Debug.Log("LoadLua");

        try
        {
            Dictionary<string,SingleField> data = ConfigManager.GetData(c_LuaConfigName);

            //先取出所有库文件执行
            string[] luaLibList = data[c_LuaLibraryListKey].GetStringArray();
            for (int i = 0; i < luaLibList.Length; i++)
            {
                DoLuaFile(luaLibList[i]);
            }

            //再取出所有的Lua文件并执行
            string[] luaList = data[c_LuaListKey].GetStringArray();
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

    /// <summary>
    /// 启动Lua
    /// </summary>
    public static void LaunchLua()
    {
        //Debug.Log("LaunchLua");
        try
        {
            s_state.GetFunction("Main").Call();
            s_isUpdate = true;
            s_updateFunction = s_state.GetFunction("LuaUpdate");
        }
        catch (Exception e)
        {
            Debug.LogError("Lua Lunch Execption " + e.ToString());
        }
    }

    public static object[] CallFunction(string functionName,params object[] objs)
    {
        var function = LuaState.GetFunction(functionName);

        if(function != null)
        {
            return function.Call(objs);
        }
        else
        {
            throw new Exception("找不到Lua函数 ->" + functionName + "<-");
        }
    }

    static LuaFunction s_updateFunction;

    static void Update()
    {
        if(s_isUpdate)
        {
            s_updateFunction.Call(Time.deltaTime * 1000);
        }
    }

    public static void DoLuaFile(string fileName)
    {
        string content = ResourceManager.LoadText(fileName);
        s_state.DoString(content, fileName);
    }
#endif
}

