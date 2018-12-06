using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using FrameWork;

public class ConfigEditorWindow : EditorWindow
{
    UILayerManager m_UILayerManager;

    [MenuItem("Window/配置编辑器 &4", priority = 502)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConfigEditorWindow));
    }

    List<String> m_configNameList = new List<string>();

    string m_currentConfigName;
    Dictionary<string, SingleField> m_currentConfig;

    void OnEnable()
    {
        
        m_currentSelectIndex = 0;
        EditorGUIStyleData.Init();

        FindAllConfigName();
    }

    void OnGUI()
    {
        titleContent.text = "配置编辑器";

        EditorGUILayout.BeginVertical();

        SelectConfigGUI();

        ConfigEditorGUI();

        DeleteConfigGUI();

        AddConfigGUI();

        CleanCacheGUI();


        EditorGUILayout.EndVertical();
    }

    //当选择改变时
    void OnSelectionChange()
    {

    }

    //当工程改变时
    void OnProjectChange()
    {
        if (!Application.isPlaying)
        {
            FindAllConfigName();

            if (m_currentConfigName != null
            && m_currentConfigName != ""
            && m_currentConfigName != "None")
            {
                LoadConfig(m_currentConfigName);
            }
        }
    }

    #region GUI

    #region 配置加载与新增

    int m_currentSelectIndex = 0;
    void SelectConfigGUI()
    {
        string[] mask = m_configNameList.ToArray();
        m_currentSelectIndex = EditorGUILayout.Popup("当前配置：", m_currentSelectIndex, mask);
        if (mask.Length != 0)
        {
            LoadConfig(mask[m_currentSelectIndex]);
        }
    }

    void LoadConfig(string configName)
    {
        if (m_currentConfigName != configName)
        {
            m_currentConfigName = configName;

            if (m_currentConfigName != "None")
                m_currentConfig = ConfigManager.GetData(configName);
        }
    }
    bool isConfigFold;
    string configName = "";
    void AddConfigGUI()
    {
        EditorGUI.indentLevel = 0;

        isConfigFold = EditorGUILayout.Foldout(isConfigFold, "新增配置");
        
        if (isConfigFold)
        {
            EditorGUI.indentLevel = 1;

            configName = EditorGUILayout.TextField("配置名", configName);

            if (!m_configNameList.Contains(configName) && configName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    Dictionary<string,SingleField> dict = new Dictionary<string,SingleField>();
                    SaveData(configName, dict);

                    LoadConfig(configName);

                    configName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (m_configNameList.Contains(configName))
                {
                    EditorGUILayout.LabelField("已存在该配置");
                }
            }
        }

        EditorGUILayout.Space();
    }

    void DeleteConfigGUI()
    {
        if (m_currentConfigName != "None")
        {
            if (GUILayout.Button("删除配置"))
            {
                if (EditorUtility.DisplayDialog("警告", "确定要删除该配置吗！", "是", "取消"))
                {
                    File.Delete(Application.dataPath + "/Resources/" + ConfigManager.c_directoryName + "/" + m_currentConfigName + ".json");
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    public void CleanCacheGUI()
    {
        ConfigManager.CleanCache();
    }

    #endregion

    #region 字段修改与新增

    bool isFold = false;
    string fieldName = "";
    FieldType newType;
    SingleField content = new SingleField();
    int m_newTypeIndex = 0;
    void AddFieldGUI(Dictionary<string, SingleField> dict)
    {
        EditorGUI.indentLevel = 1;
        isFold = EditorGUILayout.Foldout(isFold,"新增字段");
        if(isFold)
        {
            EditorGUI.indentLevel = 2;

            bool isNewType = false;

            fieldName = EditorGUILayout.TextField("字段名",fieldName);

            newType = (FieldType)EditorGUILayout.EnumPopup("字段类型", content.m_type);

            if (content.m_type != newType)
            {
                isNewType = true;
            }

            if (newType == FieldType.Enum)
            {
                int newIndex = EditorGUILayout.Popup("枚举类型", m_newTypeIndex, EditorTool.GetAllEnumType());

                if (newIndex != m_newTypeIndex)
                {
                    m_newTypeIndex = newIndex;
                    isNewType = true;
                }
            }

            if (isNewType)
            {
                content.m_type = newType;
                content.m_enumType = EditorTool.GetAllEnumType()[m_newTypeIndex];
                content.Reset();
            }

            content.m_content = EditorUtilGUI.FieldGUI_Type(content);

            if (!dict.ContainsKey(fieldName) && fieldName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    m_currentConfig.Add(fieldName, content);

                    fieldName = "";
                    content = new SingleField();
                    newType = content.m_type;
                    m_newTypeIndex = 0;
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (dict.ContainsKey(fieldName))
                {
                    EditorGUILayout.LabelField("已存在该字段");
                }
            }
        }
    }

    void ConfigItemGUI(Dictionary<string, SingleField> dict,string key)
    {
        EditorGUI.indentLevel = 2;
        string newContent = "";
        SingleField data = dict[key];

        EditorGUILayout.LabelField(key);
        
        EditorGUI.indentLevel = 3;

        EditorGUILayout.BeginHorizontal();

        if (data.m_type != FieldType.Enum)
        {
            EditorGUILayout.LabelField("字段类型：", data.m_type.ToString());
        }
        else
        {
            EditorGUILayout.LabelField("字段类型：", data.m_type.ToString() +"/"+ data.m_enumType);
        }

        if(GUILayout.Button("删除字段"))
        {
            if (EditorUtility.DisplayDialog("警告", "确定要删除该字段吗！", "是", "取消"))
            {
                dict.Remove(key);
                return;
            }
        }

        EditorGUILayout.EndHorizontal();

        newContent = EditorUtilGUI.FieldGUI_Type(data);

        if (data.GetString() != newContent)
        {
            data.m_content = newContent;
        }

        EditorGUILayout.Space();
    }


    Vector2 pos = Vector3.zero;
    void ConfigEditorGUI()
    {
        if (m_currentConfig != null
            && m_currentConfigName != "None")
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel = 1;
            pos = EditorGUILayout.BeginScrollView(pos,GUILayout.ExpandHeight(false));

            List<string> keys = new List<string>(m_currentConfig.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                try
                {
                    ConfigItemGUI(m_currentConfig, keys[i]);
                }
                catch(Exception e)
                {
                    GUILayout.Label(e.ToString(), EditorGUIStyleData.ErrorMessageLabel);
                }

            }

            EditorGUILayout.Space();

            AddFieldGUI(m_currentConfig);

            EditorGUILayout.Space();
            if (GUILayout.Button("保存"))
            {
                SaveData(m_currentConfigName, m_currentConfig);
                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    #endregion

    #endregion

    #region FindConfig

    void FindAllConfigName()
    {
        AssetDatabase.Refresh();
        m_configNameList = new List<string>();

        m_configNameList.Add("None");

        FindConfigName(Application.dataPath + "/Resources/" + ConfigManager.c_directoryName);
    }

    public void FindConfigName(string path)
    {
        FileTool.CreatPath(path);

        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            if (item.EndsWith(".json"))
            {
                string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                m_configNameList.Add(configName);
            }
        }
    }

    public static string GetConfigPath(string configName)
    {
        return PathTool.GetAbsolutePath(ResLoadLocation.Resource,
            PathTool.GetRelativelyPath(ConfigManager.c_directoryName,
                                                configName,
                                                ConfigManager.c_expandName));
    }

    #endregion

    #region 保存配置

    public static void SaveData(string ConfigName, Dictionary<string, SingleField> data)
    {
        EditorUtil.WriteStringByFile(PathTool.GetAbsolutePath(ResLoadLocation.Resource,
            PathTool.GetRelativelyPath(ConfigManager.c_directoryName,
                                                ConfigName,
                                                ConfigManager.c_expandName)),
                                        JsonTool.Dictionary2Json<SingleField>(data));

        UnityEditor.AssetDatabase.Refresh();
    }
    public static Dictionary<string, object> GetEditorConfigData(string ConfigName)
    {
      //  UnityEditor.AssetDatabase.Refresh();

        string dataJson = ResourceIOTool.ReadStringByFile(PathTool.GetEditorPath(ConfigManager.c_directoryName, ConfigName, ConfigManager.c_expandName));

        if (dataJson == "")
        {
            return null;
        }
        else
        {
            return Json.Deserialize(dataJson) as Dictionary<string, object>;
        }
    }

    public static void SaveEditorConfigData(string ConfigName, Dictionary<string, object> data)
    {
        string configDataJson = Json.Serialize(data);

        EditorUtil.WriteStringByFile(PathTool.GetEditorPath(ConfigManager.c_directoryName, ConfigName, ConfigManager.c_expandName), configDataJson);

        UnityEditor.AssetDatabase.Refresh();
    }

    #endregion


}


