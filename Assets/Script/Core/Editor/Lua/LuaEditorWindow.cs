using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class LuaEditorWindow : EditorWindow
{
    string m_path = "Lua";
    List<string> m_LuaFileList;
    Dictionary<string, SingleField> m_luaConfig;

    [MenuItem("Window/Lua设置编辑器")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LuaEditorWindow));
    }

    void OnEnable()
    {
        EditorGUIStyleData.Init();
        InitLuaConfig();

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
        Directory.CreateDirectory(PathTool.GetAbsolutePath(ResLoadType.Resource,m_path));

        //复制lua初始库文件

    }

    #endregion

    #region Lua信息检视
    bool m_isFold = false;
    Vector2 m_pos = Vector2.zero;
    void LuaFileGUI()
    {
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

            
            //EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region ULua原生功能

    void LuaWarpFileGUI()
    {
        if (Directory.Exists(CustomSettings.saveDir))
        {
            if (GUILayout.Button("清除Lua Warp脚本"))
            {
                Directory.Delete(CustomSettings.saveDir, true);
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("重新生成Lua Warp脚本"))
            {
                Directory.Delete(CustomSettings.saveDir, true);
                ToLuaMenu.GenLuaAll();
            }
        }
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

    void InitLuaConfig()
    {
        if (ConfigManager.GetIsExistConfig(LuaManager.c_LuaConfigName))
        {
            m_luaConfig = ConfigManager.GetData(LuaManager.c_LuaConfigName);
        }
        else
        {
            m_luaConfig = new Dictionary<string, SingleField>();
        }

        InitLuaList();
    }

    void InitLuaList()
    {
        m_LuaFileList = new List<string>();

        if (m_luaConfig.ContainsKey(LuaManager.c_LuaListKey))
        {
            m_LuaFileList.AddRange(m_luaConfig[LuaManager.c_LuaListKey].GetStringArray());
        }
        else
        {
            m_LuaFileList = new List<string>();
        }
    }

    void SaveLuaConfig()
    {
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

        ConfigManager.SaveData(LuaManager.c_LuaConfigName, m_luaConfig);
    }

    void GetLuaFileList()
    {
        m_LuaFileList= new List<string>();
        FindConfigName(PathTool.GetAbsolutePath(ResLoadType.Resource, m_path));
    }

    public void FindConfigName(string path)
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
            FindConfigName(dires[i]);
        }
    }

    #endregion
}
