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

    static List<string> s_languageList = new List<string>();

    //所有模块以及模块下的语言ID
    static Dictionary<string, List<string>> s_languageKeyDict = new Dictionary<string, List<string>>();

    //所有语言列表
    static List<string> s_languageKeyList = new List<string>();

    private int m_currentSelectIndex;
    private string m_currentLanguage;
    //当前语言数据
    private Dictionary<string,DataTable> m_langeuageDataDict = new Dictionary<string,DataTable>();

    SystemLanguage m_defaultLanguage = SystemLanguage.ChineseSimplified;

    [MenuItem("Window/多语言编辑器", priority = 600)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LanguageDataEditorWindow));
    }

    void OnEnable()
    {
        ResourcesConfigManager.Initialize();

        m_currentSelectIndex = 0;
        EditorGUIStyleData.Init();

        FindAllDataName();
        LoadEditorConfig();
        LoadConfig();
    }

    void OnProjectChange()
    {
        FindAllDataName();
        LoadEditorConfig();
        LoadConfig();
    }

    void OnGUI()
    {
        titleContent.text = "多语言编辑器";

        if (!Application.isPlaying)
        {
            SelectLanguageGUI();
            DefaultLanguageGUI();
            EditorLanguageFieldGUI();
            EditorLanguageGUI();
            SaveDataGUI();
            AddLanguageGUI();
        }
        else
        {
            EditorGUILayout.LabelField("功能目前不可用");
        }
    }

    #region 加载/保存编辑器设置

    static void LoadEditorConfig()
    {
        s_languageKeyList.Clear();
        s_languageKeyDict.Clear();

        Dictionary<string, object> config = ConfigEditorWindow.GetEditorConfigData(c_EditorConfigName);

        if (config == null)
        {
            config = new Dictionary<string, object>();
        }

        foreach (var item in config)
        {
            List<string> list = new List<string>();
            List<object> ObjList = (List<object>)item.Value;
            for (int i = 0; i < ObjList.Count; i++)
            {
                list.Add(ObjList[i].ToString());

                s_languageKeyList.Add(item.Key + "/" + ObjList[i].ToString());
            }

            s_languageKeyDict.Add(item.Key, list);
        }
    }

    void LoadConfig()
    {
        if (ConfigManager.GetIsExistConfig(LanguageManager.c_configFileName))
        {
            Dictionary<string, SingleField> config = ConfigManager.GetData(LanguageManager.c_configFileName);

            if (config.ContainsKey(LanguageManager.c_defaultLanguageKey))
            {
                m_defaultLanguage = config[LanguageManager.c_defaultLanguageKey].GetEnum<SystemLanguage>();
            }
        }
    }

    void SaveEditorConfig()
    {
        Dictionary<string, object> config = new Dictionary<string, object>();

        foreach (var item in s_languageKeyDict)
        {
            config.Add(item.Key, item.Value);
        }

        ConfigEditorWindow.SaveEditorConfigData(c_EditorConfigName, config);
    }

    void SaveConfig()
    {
        Dictionary<string, SingleField> config = new Dictionary<string, SingleField>();

        //保存默认语言
        config.Add(LanguageManager.c_defaultLanguageKey, new SingleField(m_defaultLanguage.ToString()));

        //保存语言列表
        List<string> languageList = new List<string>();
        foreach (var item in s_languageList)
        {
            if (item != "None")
            {
                languageList.Add(item);
            }
        }
        config.Add(LanguageManager.c_languageListKey, new SingleField(languageList));

        //保存模块列表
        List<string> moduleList = new List<string>();
        foreach (var item in s_languageKeyDict)
        {
            moduleList.Add(item.Key);
        }
        config.Add(LanguageManager.c_moduleListKey, new SingleField(moduleList));

        ConfigEditorWindow.SaveData(LanguageManager.c_configFileName, config);
    }

    #endregion

    #region 语言设置

    void DefaultLanguageGUI()
    {
        if (m_currentLanguage == m_defaultLanguage.ToString())
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("默认语言", EditorGUIStyleData.s_WarnMessageLabel);
        }
    }

    void SetDefaultLanguageGUI(string languageName)
    {
        if (GUILayout.Button("设为默认语言"))
        {
            m_defaultLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageName);
        }
    }

    #endregion

    #region 选择语言

    void SelectLanguageGUI()
    {
        string[] mask = s_languageList.ToArray();
        m_currentSelectIndex = EditorGUILayout.Popup("当前语言：", m_currentSelectIndex, mask);
        if (mask.Length != 0)
        {
            LoadLanguage(mask[m_currentSelectIndex]);
        }
    }

    void LoadLanguage(string LanguageName)
    {
        if (m_currentLanguage != LanguageName)
        {
            m_currentLanguage = LanguageName;

            if (LanguageName != "None")
            {
                m_langeuageDataDict.Clear();

                foreach (var item in s_languageKeyDict)
                {
                    string savePath = GetLanguageSavePath(LanguageName, item.Key);

                    if (GetIsExistDataEditor(savePath))
                    {
                        DataTable data = DataManager.GetData(savePath);
                         m_langeuageDataDict.Add(item.Key, data);
                    }
                }
            }
        }
    }

    #endregion

    #region 编辑语言字段

    Dictionary<string, bool> LanguageFieldFold = new Dictionary<string, bool>();
    Vector2 pos_editorField = Vector2.zero;
    bool isFoldLanguageField = false;

    void EditorLanguageFieldGUI()
    {
        EditorGUI.indentLevel = 1;
        pos_editorField = EditorGUILayout.BeginScrollView(pos_editorField, GUILayout.ExpandHeight(false));

        isFoldLanguageField = EditorGUILayout.Foldout(isFoldLanguageField, "多语言模块列表");

        if (isFoldLanguageField)
        {
            foreach (var item in s_languageKeyDict)
            {
                EditorGUI.indentLevel = 2;
                if (!LanguageFieldFold.ContainsKey(item.Key))
                {
                    LanguageFieldFold.Add(item.Key, false);
                }
                EditorGUILayout.BeginHorizontal();

                LanguageFieldFold[item.Key] = EditorGUILayout.Foldout(LanguageFieldFold[item.Key], item.Key);

                if(GUILayout.Button("删除模块"))
                {
                    s_languageKeyDict.Remove(item.Key);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                if (LanguageFieldFold[item.Key])
                {
                    EditorLanguageFieldGUI(item.Value);
                }
            }

            EditorGUILayout.Space();

            EditorGUI.indentLevel = 1;
            AddLanguageModelGUI();
        }

        EditorGUILayout.EndScrollView();
        
    }
    string m_newModelName = "";
    bool isAddLanguageModelFold = false;
    void AddLanguageModelGUI()
    {
        EditorGUI.indentLevel ++;
        isAddLanguageModelFold = EditorGUILayout.Foldout(isAddLanguageModelFold, "新增模块");

        if (isAddLanguageModelFold)
        {
            EditorGUI.indentLevel++;
            m_newModelName = EditorGUILayout.TextField("模块名", m_newModelName);

            if (!s_languageKeyDict.ContainsKey(LanguageManager.c_defaultModuleKey.ToString()))
            {
                if (GUILayout.Button("新增默认模块"))
                {
                    s_languageKeyDict.Add(LanguageManager.c_defaultModuleKey, new List<string>());
                }
            }

            if (m_newModelName != "" && !s_languageKeyDict.ContainsKey(m_newModelName))
            {
                if (GUILayout.Button("新增模块"))
                {
                    s_languageKeyDict.Add(m_newModelName, new List<string>());
                    m_newModelName = "";
                }
                EditorGUILayout.Space();
            }
            else
            {
                if (s_languageKeyDict.ContainsKey(m_newModelName))
                {
                    EditorGUILayout.LabelField("模块名重复！", EditorGUIStyleData.s_WarnMessageLabel);
                }
            }
        }
    }

    void EditorLanguageFieldGUI(List<string> languageKeyList)
    {
        EditorGUI.indentLevel++;
        for (int i = 0; i < languageKeyList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", languageKeyList[i]);

            if (GUILayout.Button("删除字段"))
            {
                languageKeyList.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        AddLangeuageFieldGUI(languageKeyList);
    }

    string newField = "";

    void AddLangeuageFieldGUI(List<string> languageKeyList)
    {
        EditorGUILayout.LabelField("新增字段");
        if (true)
        {
            EditorGUI.indentLevel = 3;
            newField = EditorGUILayout.TextField("字段名",newField);

            if (newField != "" && !languageKeyList.Contains(newField))
            {
                if (GUILayout.Button("新增语言字段"))
                {
                    languageKeyList.Add(newField);
                    newField = "";
                }
                EditorGUILayout.Space();
            }
            else
            {
                if (languageKeyList.Contains(newField))
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

    Dictionary<string, bool> m_EditorLanguageGUIFoldDict = new Dictionary<string, bool>();

    void DeleteLanguageGUI()
    {
        if(GUILayout.Button("删除语言"))
        {
            if (EditorUtility.DisplayDialog("警告", "确定要删除该语言吗！", "是", "取消"))
            {
                m_currentSelectIndex = 0;
                s_languageKeyDict.Remove(m_currentLanguage);
                Directory.Delete(Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + c_DataPath + "/" + m_currentLanguage,true);
                AssetDatabase.Refresh();
            }
        }
    }

    void EditorLanguageGUI()
    {
        if (m_currentLanguage != "None")
        {
            EditorGUI.indentLevel = 1;
            isFoldList = EditorGUILayout.Foldout(isFoldList, "语言数据：");
            if (isFoldList)
            {
                pos = EditorGUILayout.BeginScrollView(pos, GUILayout.ExpandHeight(false));

                foreach (var item in s_languageKeyDict)
                {
                    EditorGUI.indentLevel = 2;
                    if (!m_langeuageDataDict.ContainsKey(item.Key))
                    {
                        DataTable data = new DataTable();
                        data.TableKeys.Add(LanguageManager.c_mainKey);
                        data.TableKeys.Add(LanguageManager.c_valueKey);
                        data.SetDefault(LanguageManager.c_valueKey, "NoValue");

                        m_langeuageDataDict.Add(item.Key, data);
                    }

                    if (!m_EditorLanguageGUIFoldDict.ContainsKey(item.Key))
                    {
                        m_EditorLanguageGUIFoldDict.Add(item.Key, false);
                    }

                    m_EditorLanguageGUIFoldDict[item.Key] = EditorGUILayout.Foldout(m_EditorLanguageGUIFoldDict[item.Key], item.Key);
                    if (m_EditorLanguageGUIFoldDict[item.Key])
                    {
                        EditorLanguageGUI(item.Key, m_langeuageDataDict[item.Key]);
                    }
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();

            }

            SetDefaultLanguageGUI(m_currentLanguage);
            DeleteLanguageGUI();
        }
    }

    void EditorLanguageGUI(string modelName, DataTable languageData)
    {
        EditorGUI.indentLevel ++;
        for (int i = 0; i < s_languageKeyDict[modelName].Count; i++)
        {
            LanguageItemGUI(languageData, s_languageKeyDict[modelName][i]);
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
        if (GUILayout.Button("保存"))
        {
            SaveEditorConfig();
            SaveConfig();

            if (m_currentLanguage != "None")
            {
                foreach (var item in m_langeuageDataDict)
                {
                    SaveData(GetLanguageSavePath(m_currentLanguage,item.Key) ,item.Value);
                }
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

            if (!s_languageList.Contains(m_selectLanguage.ToString()) )
            {
                if (GUILayout.Button("新增"))
                {
                    s_languageList.Add(m_selectLanguage.ToString());
                    LoadLanguage(m_selectLanguage.ToString());
                    isFoldAddLanguage = false;
                }
            }
            else
            {
                EditorGUILayout.LabelField("已存在该语言",EditorGUIStyleData.s_WarnMessageLabel);
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
        s_languageList = new List<string>();

        s_languageList.Add("None");

        m_directoryPath = Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + c_DataPath;

        FileTool.CreatPath(m_directoryPath);

        FindConfigName(m_directoryPath);
    }

    public void FindConfigName(string path)
    {
        //string[] allUIPrefabName = Directory.GetFiles(path);
        //foreach (var item in allUIPrefabName)
        //{
        //    if (item.EndsWith(".txt"))
        //    {
        //        //string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
        //        s_dataNameList.Add(FileTool.RemoveExpandName(PathTool.GetDirectoryRelativePath(m_directoryPath + "/", item)));
        //    }
        //}

        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            s_languageList.Add(FileTool.RemoveExpandName(PathTool.GetDirectoryRelativePath(m_directoryPath + "/", dires[i])));

            //FindConfigName(dires[i]);
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

    #region 功能函数

    string GetLanguageSavePath(string langeuageName, string modelName)
    {
        return c_DataPath + "/" + langeuageName + "/" + LanguageManager.GetLanguageDataName(langeuageName, modelName);
    }

    public static bool GetIsExistDataEditor(string DataName)
    {
        return "" != ResourceIOTool.ReadStringByResource(
                        PathTool.GetRelativelyPath(DataManager.c_directoryName,
                                                    DataName,
                                                    DataManager.c_expandName));
    }

    public static void SaveData(string ConfigName, DataTable data)
    {
        EditorUtil.WriteStringByFile(
            PathTool.GetAbsolutePath(
                ResLoadLocation.Resource,
                PathTool.GetRelativelyPath(
                    DataManager.c_directoryName,
                    ConfigName,
                    DataManager.c_expandName)),
            DataTable.Serialize(data));

        UnityEditor.AssetDatabase.Refresh();
    }

    #endregion
}
