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

    [MenuItem("Window/Lua设置编辑器")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LuaEditorWindow));
    }

    void OnEnable()
    {
        EditorGUIStyleData.Init();
        LoadLuaConfig();
    }

    #region GUI
    void OnGUI()
    {
        titleContent.text = "Lua设置编辑器";

        if (!Directory.Exists(CustomSettings.saveDir))
        {
            InitLuaGUI();
        }
        else
        {
            LuaFileGUI();
            LuaWarpFileGUI();
            AutoLuaConfigGUI();
        }

        EditorGUILayout.Space();
    }

    #endregion

    #region 项目Lua初始化

    void InitLuaGUI()
    {
        if (GUILayout.Button("Lua 项目初始化"))
        {
            InitLua();
        }
    }

    void InitLua()
    {
        ToLuaMenu.GenLuaAll();
        //创建Lua目录
        Directory.CreateDirectory(PathTool.GetAbsolutePath(ResLoadLocation.Resource, c_LuaLibFilePath));
        Directory.CreateDirectory(PathTool.GetAbsolutePath(ResLoadLocation.Resource, c_LuaFilePath));

        string resPath = Application.dataPath + "/Script/Core/Editor/res/LuaLib";
        string aimPath = Application.dataPath + "/Resources/LuaLib";

        //复制lua初始库文件
        FileTool.CopyDirectory(resPath, aimPath);

        ProjectBuildService.SetScriptDefine("USE_LUA");
    }

#endregion

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
        if (Directory.Exists(CustomSettings.saveDir))
        {
            if (!File.Exists(CustomSettings.saveDir + "/LuaBinderCatch.cs"))
            {
                if (GUILayout.Button("清除Lua Warp脚本"))
                {
                    FileTool.DeleteDirectory(CustomSettings.saveDir);
                    CreateLuaBinder();
                    AssetDatabase.Refresh();
                }
            }

            if (GUILayout.Button("重新生成Lua Warp脚本"))
            {
                FileTool.DeleteDirectory(CustomSettings.saveDir);
                ToLuaMenu.GenLuaAll();
            }
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
