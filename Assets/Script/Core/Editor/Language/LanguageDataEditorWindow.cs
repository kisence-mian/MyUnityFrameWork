using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LanguageDataEditorWindow : EditorWindow 
{
    const string c_DataPath = "Language";
    const string c_EditorConfigName = "LanguageEditorConfig";

    const string c_ListKey = "LanguageKeys";

    static List<string> s_dataNameList = new List<string>();
    static Dictionary<string, object> s_editorConfig;
    static List<string> s_languageKeyList = new List<string>();

    private int m_currentSelectIndex;

    private string m_currentDataName;
    private DataTable m_currentData;

    string m_defaultLanguage = "";

    [MenuItem("Window/多语言编辑器", priority = 600)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LanguageDataEditorWindow));
    }

    void OnEnable()
    {
        m_currentSelectIndex = 0;
        EditorGUIStyleData.Init();

        FindAllDataName();
        LoadEditorConfig();
        LoadLanguageConfig();
    }

    void OnProjectChange()
    {
        FindAllDataName();
        LoadEditorConfig();
        LoadLanguageConfig();
    }

    void OnGUI()
    {
        titleContent.text = "多语言编辑器";

        SelectLanguageGUI();
        DefaultLanguageGUI();
        EditorLanguageFieldGUI();
        EditorLanguageGUI();
        SaveDataGUI();
        AddLanguageGUI();
    }

    #region 加载/保存编辑器设置

    static void LoadEditorConfig()
    {
        s_editorConfig =  ConfigManager.GetEditorConfigData(c_EditorConfigName);

        if (s_editorConfig == null)
        {
            s_editorConfig = new Dictionary<string, object>();
        }

        if(s_editorConfig.ContainsKey(c_ListKey))
        {
            string m_KeyContent = s_editorConfig[c_ListKey].ToString();
            string[] m_stringArrary = m_KeyContent.Split('|');

            s_languageKeyList = new List<string>();
            s_LanguageKeyCatch = new Dictionary<string, string>();

            for (int i = 0; i < m_stringArrary.Length; i++)
            {
                if (m_stringArrary[i] != "" && !s_LanguageKeyCatch.ContainsKey(m_stringArrary[i]))
                {
                    s_languageKeyList.Add(m_stringArrary[i]);
                    s_LanguageKeyCatch.Add(m_stringArrary[i], "");
                }
            }
        }
    }

    void SaveEditorConfig()
    {
        string content = "";

        for (int i = 0; i < s_languageKeyList.Count; i++)
        {
            content += s_languageKeyList[i];
            if(i!= s_languageKeyList.Count - 1)
            {
                content += "|";
            }
        }

        if (s_editorConfig.ContainsKey(c_ListKey))
        {
            s_editorConfig[c_ListKey] = content;
        }
        else
        {
            s_editorConfig.Add(c_ListKey, content);
        }

        ConfigManager.SaveEditorConfigData(c_EditorConfigName, s_editorConfig);
    }

    #endregion

    #region 语言设置

    void DefaultLanguageGUI()
    {
        if (m_currentDataName == m_defaultLanguage)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("默认语言", EditorGUIStyleData.s_WarnMessageLabel);
        }
    }

    void SetDefaultLanguageGUI(string languageName)
    {
        if (GUILayout.Button("设为默认语言"))
        {
            m_defaultLanguage = languageName;

            Dictionary<string, SingleField> config = new Dictionary<string, SingleField>();

            config.Add(LanguageManager.c_defaultLanguageKey, new SingleField(languageName));
            ConfigManager.SaveData(LanguageManager.c_configFileName, config);
        }
    }

    void LoadLanguageConfig()
    {
        if(ConfigManager.GetIsExistConfig(LanguageManager.c_configFileName))
        {
            Dictionary<string,SingleField> config = ConfigManager.GetData(LanguageManager.c_configFileName);

            if(config.ContainsKey(LanguageManager.c_defaultLanguageKey))
            {
                m_defaultLanguage = config[LanguageManager.c_defaultLanguageKey].GetString();
            }
        }
    }

    #endregion

    #region 选择语言

    void SelectLanguageGUI()
    {
        string[] mask = s_dataNameList.ToArray();
        m_currentSelectIndex = EditorGUILayout.Popup("当前语言：", m_currentSelectIndex, mask);
        if (mask.Length != 0)
        {
            LoadLanguage(mask[m_currentSelectIndex]);
        }
    }

    void LoadLanguage(string LanguageName)
    {
        if (m_currentDataName != LanguageName)
        {
            m_currentDataName = LanguageName;

            if (m_currentDataName != "None")
                m_currentData = DataManager.GetData(c_DataPath + "/" + LanguageName); 
        }
    }

    #endregion

    #region 编辑语言字段

    static Dictionary<string,string> s_LanguageKeyCatch = new Dictionary<string,string>();

    Vector2 pos_editorField = Vector2.zero;

    bool isFoldLanguageField = false;

    void EditorLanguageFieldGUI()
    {
        EditorGUI.indentLevel = 1;

        isFoldLanguageField = EditorGUILayout.Foldout(isFoldLanguageField, "字段列表");
        if (isFoldLanguageField)
        {
            EditorGUI.indentLevel = 2;

            pos_editorField = EditorGUILayout.BeginScrollView(pos_editorField, GUILayout.ExpandHeight(false));

            for (int i = 0; i < s_languageKeyList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("",s_languageKeyList[i]);

                if(GUILayout.Button("删除字段"))
                {
                    s_languageKeyList.RemoveAt(i);
                    i--;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();

            AddLangeuageFieldGUI();

        }
    }

    string newField = "";
    bool isFoldLanguageNewField = false;

    void AddLangeuageFieldGUI()
    {
        isFoldLanguageNewField = EditorGUILayout.Foldout(isFoldLanguageNewField, "新增字段");
        if (isFoldLanguageNewField)
        {
            EditorGUI.indentLevel = 3;
            newField = EditorGUILayout.TextField("字段名",newField);

            if (newField != "" && !s_LanguageKeyCatch.ContainsKey(newField))
            {
                if (GUILayout.Button("新增语言字段"))
                {
                    s_languageKeyList.Add(newField);
                    s_LanguageKeyCatch.Add(newField, "");
                    newField = "";
                }
                EditorGUILayout.Space();
            }
            else
            {
                if (s_LanguageKeyCatch.ContainsKey(newField))
                {
                    EditorGUILayout.LabelField("字段名重复！", EditorGUIStyleData.s_WarnMessageLabel);
                }
            }
        }
    }

    #endregion 

    #region 编辑语言

    private bool isFoldList;
    private Vector2 pos = Vector2.zero;

    void EditorLanguageGUI()
    {
        if (m_currentData != null
            && m_currentDataName != "None")
        {
            EditorGUI.indentLevel = 1;
            isFoldList = EditorGUILayout.Foldout(isFoldList, "记录列表");
            if (isFoldList)
            {
                pos = EditorGUILayout.BeginScrollView(pos, GUILayout.ExpandHeight(false));

                ResetLanguageKeyList(m_currentData);

                for (int i = 0; i < s_languageKeyList.Count; i++)
                {
                    EditorGUI.indentLevel = 2;
                    LanguageItemGUI(m_currentData, s_languageKeyList[i]);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();
            }

            SetDefaultLanguageGUI(m_currentDataName);
        }
    }

    void ResetLanguageKeyList(DataTable data)
    {
        List<string> keys = m_currentData.TableIDs;

        for (int i = 0; i < keys.Count; i++)
        {
            if (!s_LanguageKeyCatch.ContainsKey(keys[i]))
            {
                s_LanguageKeyCatch.Add(keys[i], "");
                s_languageKeyList.Add(keys[i]);
            }
        }
    }

    void LanguageItemGUI(DataTable data, string key)
    {

        if (data.ContainsKey(key))
        {
            GUILayout.BeginHorizontal();
            try
            {
                string contentTmp = null;

                if (data[key].ContainsKey((LanguageManager.c_valueKey)))
                {
                    contentTmp = EditorGUILayout.TextField(key, data[key].GetString(LanguageManager.c_valueKey));
                }
                else
                {
                    contentTmp = EditorGUILayout.TextField(key, "");
                }

                if(contentTmp != "")
                {
                    data[key][LanguageManager.c_valueKey] = contentTmp;
                }
            }
            catch (Exception e)
            {
                EditorGUILayout.TextArea("Error: " + e.ToString(), EditorGUIStyleData.s_ErrorMessageLabel);
                EditorGUILayout.Space();
            }

            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("缺少 " + key + " 字段", EditorGUIStyleData.s_WarnMessageLabel);

            AddMissLanguageGUI(data, key);

            EditorGUILayout.EndHorizontal();
        }

    }
 

    void AddMissLanguageGUI(DataTable data,string key)
    {
        if(GUILayout.Button("添加记录"))
        {
            SingleData newData = new SingleData();
            newData.Add(LanguageManager.c_valueKey, "");
            newData.Add(LanguageManager.c_mainKey, key);

            data.AddData(newData);
        }
    }

    void SaveDataGUI()
    {
        if (m_currentDataName != "None")
        {
            if (GUILayout.Button("保存"))
            {
                DataManager.SaveData(c_DataPath + "/" + m_currentDataName, m_currentData);
                SaveEditorConfig();
            }
        }
    }

    #endregion

    #region 新增语言

    private bool isFoldAddLanguage = false;
    private SystemLanguage m_selectLanguage;

    void AddLanguageGUI()
    {
        EditorGUI.indentLevel = 0;

        isFoldAddLanguage = EditorGUILayout.Foldout(isFoldAddLanguage, "新增语言");

        if (isFoldAddLanguage)
        {
            EditorGUI.indentLevel = 1;

            m_selectLanguage = (SystemLanguage)EditorGUILayout.EnumPopup("语言类型", m_selectLanguage);

            if (!s_dataNameList.Contains(m_selectLanguage.ToString()) )
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    DataTable data = new DataTable();
                    data.TableKeys.Add(LanguageManager.c_mainKey);
                    data.TableKeys.Add(LanguageManager.c_valueKey);
                    data.SetDefault(LanguageManager.c_valueKey,"NoValue");

                    string dataName = LanguageManager.c_DataFilePrefix + m_selectLanguage.ToString();
                    string savePath = c_DataPath + "/" + dataName;

                    DataManager.SaveData(savePath, data);
                    AssetDatabase.Refresh();

                    LoadLanguage(dataName);
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("已存在该语言");
            }
        }

        EditorGUILayout.Space();
    }

    #endregion

    #region FindLanguageData

    private string m_directoryPath;
    void FindAllDataName()
    {
        AssetDatabase.Refresh();
        s_dataNameList = new List<string>();

        s_dataNameList.Add("None");

        m_directoryPath = Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + c_DataPath;

        FileTool.CreatPath(m_directoryPath);

        FindConfigName(m_directoryPath);
    }

    public void FindConfigName(string path)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            if (item.EndsWith(".txt"))
            {
                //string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                s_dataNameList.Add(FileTool.RemoveExpandName(PathTool.GetDirectoryRelativePath(m_directoryPath + "/", item)));
            }
        }

        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            FindConfigName(dires[i]);
        }
    }

    public static List<string> GetLanguageKeyList()
    {
        if(s_languageKeyList != null)
        {
            LoadEditorConfig();
        }
        return s_languageKeyList;
    }

    #endregion
}
