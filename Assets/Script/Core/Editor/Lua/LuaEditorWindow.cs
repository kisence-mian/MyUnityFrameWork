using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class LuaEditorWindow : EditorWindow
{
    const string c_LuaFilePath = "Lua";
    const string c_LuaLibFilePath = "LuaLib";

    List<string> m_LuaFileList;
    List<string> m_LuaLibFileList;
    Dictionary<string, SingleField> m_luaConfig;

    [MenuItem("Window/Lua设置编辑器 &7")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LuaEditorWindow));
    }

    void OnEnable()
    {
        EditorGUIStyleData.Init();
#if USE_LUA
        LoadLuaConfig();
#endif
    }

    #region GUI
    void OnGUI()
    {
        titleContent.text = "Lua设置编辑器";

        if (!GetIsInit())
        {
            InitLuaGUI();
        }
        else
        {
#if USE_LUA
            LuaFileGUI();
            LuaWarpFileGUI();
            AutoLuaConfigGUI();
#endif

        }

        EditorGUILayout.Space();
    }

    #endregion

    #region 项目Lua初始化

    bool GetIsInit()
    {
#if USE_LUA
        return true;
#else
        return false;
#endif
    }

    void InitLuaGUI()
    {
        if (GUILayout.Button("Lua 项目初始化"))
        {
            //InitLua();
        }
    }

#if !USE_LUA
}
#endif

#if USE_LUA

    void InitLua()
    {
        //创建导出列表文件
        CreateLuaExportFile();

        //创建空LuaBinder文件
        CreateEmptyLuaBinder();

        ToLuaMenu.ClearLuaFiles();
        //创建Lua目录
        Directory.CreateDirectory(PathTool.GetAbsolutePath(ResLoadLocation.Resource, c_LuaLibFilePath));
        Directory.CreateDirectory(PathTool.GetAbsolutePath(ResLoadLocation.Resource, c_LuaFilePath));

        string resPath = Application.dataPath + "/Script/Core/Editor/res/LuaLib";
        string aimPath = Application.dataPath + "/Resources/LuaLib";

        string pluginsResPath = Application.dataPath + "/Script/Core/Lua/ToLua/PluginsRes";
        string pluginsPath = Application.dataPath + "/Lua/Plugins";

        //复制lua初始库文件
        FileTool.CopyDirectory(resPath, aimPath);

        //拷贝LuaPlugins文件
        FileTool.CopyDirectory(pluginsResPath, pluginsPath);

        //修改文件名 dllx -> dll
        FileTool.ChangeFileName(pluginsPath + "/x86/tolua.dllx", pluginsPath + "/x86/tolua.dll");
        FileTool.ChangeFileName(pluginsPath + "/x86_64/tolua.dllx", pluginsPath + "/x86_64/tolua.dll");

        FileTool.ChangeFileName(pluginsPath + "/CString.dllx", pluginsPath + "/x86_64/CString.dll");
        FileTool.ChangeFileName(pluginsPath + "/Debugger.dllx", pluginsPath + "/x86_64/Debugger.dll");

        //初始Warp
        ToLuaMenu.GenLuaAll();

        //自动生成Lua配置文件
        GetLuaFileList();
        SaveLuaConfig();

        //创建启动文件
        string luaMainPath = Application.dataPath + "/Resources/Lua/luaMain.txt";
        string content = "print(\"lua is launch!\");";

        ResourceIOTool.WriteStringByFile(luaMainPath, content);

        //设置宏
        ProjectBuildService.SetScriptDefine("USE_LUA");

        AssetDatabase.Refresh();

        string info = "Lua初始化完成，\n";
        info += "请先生成luaWarp文件 （Window -> Lua设置管理器 -> 重新生成Lua Warp脚本）\n";
        info += "再重新生成打包设置（Window -> 打包设置管理器 -> 自动添加Resources目录下的资源并保存）\n";

        Debug.Log(info);
    }

    void CreateLuaExportFile()
    {
        string LoadPath = Application.dataPath + "/Script/Core/Editor/res/LuaExportListTemplate.txt";
        string SavePath = Application.dataPath + "/Script/UI/Editor/Lua/LuaExportList.cs";

        string ExportListContent = ResourceIOTool.ReadStringByFile(LoadPath);

        EditorUtil.WriteStringByFile(SavePath, ExportListContent);
    }

    static void CreateEmptyLuaBinder()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using LuaInterface;");
        sb.AppendLine();
        sb.AppendLine("public static class LuaBinder");
        sb.AppendLine("{");
        sb.AppendLine("\tpublic static void Bind(LuaState L)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tthrow new LuaException(\"Please generate LuaBinder files first!\");");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        string filePath = CustomSettings.saveDir + "/LuaBinder.cs";

        EditorUtil.WriteStringByFile(filePath, sb.ToString());
    }
#endif
#endregion
#if USE_LUA

#region 读取Lua配置
void LoadLuaConfig()
    {
        if (ConfigManager.GetIsExistConfig(LuaManager.c_LuaConfigName))
        {
            m_luaConfig = ConfigManager.GetData(LuaManager.c_LuaConfigName);
        }
        else
        {
            m_luaConfig = new Dictionary<string, SingleField>();
        }

        LoadLuaList();
    }

    void LoadLuaList()
    {
        if (m_luaConfig.ContainsKey(LuaManager.c_LuaListKey))
        {
            m_LuaFileList = new List<string>();
            m_LuaLibFileList = new List<string>();

            m_LuaFileList.AddRange(m_luaConfig[LuaManager.c_LuaListKey].GetStringArray());
            m_LuaLibFileList.AddRange(m_luaConfig[LuaManager.c_LuaLibraryListKey].GetStringArray());
        }
        else
        {
            m_LuaFileList = new List<string>();
            m_LuaLibFileList = new List<string>();
        }
    }
#endregion

#region Lua信息检视

    bool m_isFold = false;
    bool m_isFoldLib = false;
    Vector2 m_pos = Vector2.zero;
    Vector2 m_posLib = Vector2.zero;
    void LuaFileGUI()
    {
        m_isFoldLib = EditorGUILayout.Foldout(m_isFoldLib, "Lua库列表");

        if (m_isFoldLib)
        {
            m_posLib = EditorGUILayout.BeginScrollView(m_posLib,GUILayout.ExpandHeight(false));
            EditorGUI.indentLevel = 1;
            if (m_LuaLibFileList != null)
            {
                for (int i = 0; i < m_LuaLibFileList.Count; i++)
                {
                    EditorGUILayout.LabelField(m_LuaLibFileList[i]);
                }
            }
            EditorGUILayout.EndScrollView();
        }


        EditorGUI.indentLevel = 0;
        m_isFold = EditorGUILayout.Foldout(m_isFold, "Lua列表");
        m_pos = EditorGUILayout.BeginScrollView(m_pos);
        if (m_isFold)
        {
            EditorGUI.indentLevel = 1;

            
            if (m_LuaFileList != null)
            {
                for (int i = 0; i < m_LuaFileList.Count; i++)
                {
                    EditorGUILayout.LabelField(m_LuaFileList[i]);
                }
            }

            
        }
        EditorGUILayout.EndScrollView();
        EditorGUI.indentLevel--;
    }

#endregion

#region ULua原生功能

    void LuaWarpFileGUI()
    {
        if (!File.Exists(CustomSettings.saveDir + "/LuaBinderCatch.cs"))
        {
            if (GUILayout.Button("清除Lua Warp脚本"))
            {
                FileTool.CreatFilePath(CustomSettings.saveDir);
                FileTool.DeleteDirectory(CustomSettings.saveDir);
                CreateLuaBinder();
                AssetDatabase.Refresh();
            }
        }

        if (GUILayout.Button("重新生成Lua Warp脚本"))
        {
            FileTool.CreatPath(CustomSettings.saveDir);
            FileTool.DeleteDirectory(CustomSettings.saveDir);
            ToLuaMenu.GenLuaAll();
        }
    }

    /// <summary>
    /// 生成LuaBinder文件
    /// </summary>
    void CreateLuaBinder()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using LuaInterface;");
        sb.AppendLine();
        sb.AppendLine("public static class LuaBinder");
        sb.AppendLine("{");
        sb.AppendLine("\tpublic static void Bind(LuaState L)");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tthrow new LuaException(\"Please generate LuaBinder files first!\");");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        EditorUtil.WriteStringByFile(CustomSettings.saveDir + "/LuaBinderCatch.cs", sb.ToString());
    }

#endregion

#region 自动生成Lua设置

    void AutoLuaConfigGUI()
    {
        if(GUILayout.Button("自动生成Lua配置文件"))
        {
            GetLuaFileList();
            SaveLuaConfig();
        }
    }

    void SaveLuaConfig()
    {
        //Lua库文件
        string luaLibConfig = "";
        for (int i = 0; i < m_LuaLibFileList.Count; i++)
        {
            luaLibConfig += m_LuaLibFileList[i];
            if (i != m_LuaLibFileList.Count - 1)
            {
                luaLibConfig += "|";
            }
        }

        //Lua文件
        string luaConfig = "";
        for (int i = 0; i < m_LuaFileList.Count; i++)
        {
            luaConfig += m_LuaFileList[i];
            if (i != m_LuaFileList.Count -1)
            {
                luaConfig += "|";
            }
        }

        if (!m_luaConfig.ContainsKey(LuaManager.c_LuaListKey))
        {
            m_luaConfig.Add(LuaManager.c_LuaListKey, new SingleField(luaConfig));
        }
        else
        {
            m_luaConfig[LuaManager.c_LuaListKey].m_content = luaConfig;
        }

        if (!m_luaConfig.ContainsKey(LuaManager.c_LuaLibraryListKey))
        {
            m_luaConfig.Add(LuaManager.c_LuaLibraryListKey, new SingleField(luaLibConfig));
        }
        else
        {
            m_luaConfig[LuaManager.c_LuaLibraryListKey].m_content = luaLibConfig;
        }

        ConfigEditorWindow.SaveData(LuaManager.c_LuaConfigName, m_luaConfig);
    }

    void GetLuaFileList()
    {
        m_LuaLibFileList = new List<string>();
        m_LuaFileList= new List<string>();

        FindLuaLibFile(PathTool.GetAbsolutePath(ResLoadLocation.Resource, c_LuaLibFilePath));
        FindLuaFile(PathTool.GetAbsolutePath(ResLoadLocation.Resource, c_LuaFilePath));
    }

    public void FindLuaFile(string path)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            if (item.EndsWith(".txt"))
            {
                string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                m_LuaFileList.Add(configName);
            }
        }

        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            FindLuaFile(dires[i]);
        }
    }

    public void FindLuaLibFile(string path)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            if (item.EndsWith(".txt"))
            {
                string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                m_LuaLibFileList.Add(configName);
            }
        }

        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            FindLuaLibFile(dires[i]);
        }
    }

#endregion
}

#endif