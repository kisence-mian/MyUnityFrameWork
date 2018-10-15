using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class LanguageDataEditorWindow : EditorWindow 
{
    const string c_DataPath = "Language";
  public  const string c_EditorConfigName = "LanguageEditorConfig";

    static List<string> s_languageList = new List<string>();

    //所有模块以及模块下的语言ID
    static Dictionary<string, List<string>> s_languageKeyDict = new Dictionary<string, List<string>>();

    //所有语言列表
    static List<string> s_languageKeyList = new List<string>();
    //所有文件（转换成全路径/）
    static List<string> s_languageFullKeyList = new List<string>();

    private int m_currentSelectIndex;
    private string m_currentLanguage;
    //当前语言数据
    private Dictionary<string,DataTable> m_langeuageDataDict = new Dictionary<string,DataTable>();

    SystemLanguage m_defaultLanguage = SystemLanguage.ChineseSimplified;

    static LanguageDataEditorWindow win =null;

    [MenuItem("Window/多语言编辑器 &5", priority = 600)]
    public static LanguageDataEditorWindow ShowWindow()
    {
         win= EditorWindow.GetWindow<LanguageDataEditorWindow>();
        return win;
    }
    FolderTreeView treeView;
    TreeViewState treeViewState = null;
    void OnEnable()
    {
        win = this;
        ResourcesConfigManager.Initialize();

        m_currentSelectIndex = 0;
        EditorGUIStyleData.Init();

        FindAllDataName();
        LoadEditorConfig();
        LoadConfig();

        if (treeViewState == null)
            treeViewState = new TreeViewState();

        treeView = new FolderTreeView(treeViewState);

        treeView.SetData(s_languageFullKeyList);
        treeView.dblclickItemCallBack = ModuleFileDblclickItemCallBack;
        treeView.selectCallBack = ModuleFileFolderSelectCallBack;
    }

   

    void OnProjectChange()
    {
        FindAllDataName();
        LoadEditorConfig();
        LoadConfig();
    }
    public int toolbarOption = 0;
    private string[] toolbarTexts = {"模块文件", "语言内容编辑", "语言设置" };
    void OnGUI()
    {
        titleContent.text = "多语言编辑器";
        //GUI.skin.label.fontSize = 11;
        if (!Application.isPlaying)
        {
            SelectLanguageGUI();
            DefaultLanguageGUI();
            SelectEditorModuleGUI();
            SearchValueGUI();
            toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
            switch (toolbarOption)
            {
                case 0:
                    EditorLanguageModuleFileGUI();
                    break;
                case 1:
                    EditorLanguageFieldGUI();
                    break;
                case 2:
                    AddLanguageGUI();

                    SetDefaultLanguageGUI(m_currentLanguage);
                    DeleteLanguageGUI();
                    break;
            }

            GUILayout.FlexibleSpace();
            SaveDataGUI();
           
            GenerateLanuageClassGUI();
            //CreateModuleByConfigData();
        }
        else
        {
            EditorGUILayout.LabelField("功能目前不可用");
        }
    }
    public string searchValue;
    private string searchModuleKey = "";
    private void SearchValueGUI()
    {
        searchValue = EditorDrawGUIUtil.DrawSearchField(searchValue);

        if(!string.IsNullOrEmpty( searchValue))
        {
            if (searchValue.Contains("/"))
            {
                string[] tempV = searchValue.Split('/');
                string key = tempV[tempV.Length - 1];
                string moduleName = searchValue.Replace('/', '_').Replace("_" + key, "");
                if (s_languageKeyDict.ContainsKey(moduleName))
                {
                    selectEditorModuleName = moduleName;
                    searchModuleKey = key;
                }
                else
                {
                    selectEditorModuleName = "";
                    searchModuleKey = "";
                }

            }
            else
                searchModuleKey = searchValue;
        }
        else
        {
            searchModuleKey = "";
        }
    }



    #region 加载/保存编辑器设置

    static void LoadEditorConfig()
    {
        s_languageKeyList.Clear();
        s_languageKeyDict.Clear();
        s_languageKeyDict = LanguageDataEditorUtils.LoadEditorConfig(c_EditorConfigName, ref s_languageKeyList);
        s_languageFullKeyList.Clear();

        foreach (var item in s_languageKeyDict)
        {
            string ss = item.Key.Replace("_", "/");
            s_languageFullKeyList.Add(ss);
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
            languageList.Add(item);
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
            EditorGUILayout.LabelField("默认语言", EditorGUIStyleData.WarnMessageLabel);
        }
    }

    void SetDefaultLanguageGUI(string languageName)
    {
        if (!string.IsNullOrEmpty(languageName ) )
        if (GUILayout.Button("设为默认语言"))
        {
            m_defaultLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), languageName);
        }
    }

    #endregion

    #region 选择语言

    void SelectLanguageGUI()
    {
        GUILayout.BeginHorizontal();
        string[] mask = s_languageList.ToArray();
        m_currentSelectIndex = EditorGUILayout.Popup("当前语言：", m_currentSelectIndex, mask);
        if (mask.Length != 0 
            && m_currentSelectIndex >= 0
            && m_currentSelectIndex < mask.Length)
        {
            LoadLanguage(mask[m_currentSelectIndex]);
        }
        else
        {
            Debug.Log("m_currentSelectIndex " + m_currentSelectIndex + " mask.Length " + mask.Length);
        }

        if(!string.IsNullOrEmpty( m_currentLanguage) && GUILayout.Button("加载上一次保存"))
        {

            DataManager.CleanCache();
            LanguageManager.IsInit = false;
            DataEditorWindow.Refresh();
            ConfigManager.CleanCache();

            LoadLanguage(m_currentLanguage);
            GUI.FocusControl("");
        }
        GUILayout.EndHorizontal();
    }

    void LoadLanguage(string LanguageName)
    {
        
            m_currentLanguage = LanguageName;

            if (!string.IsNullOrEmpty( LanguageName))
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

    #endregion

    #region 编辑语言字段

    private string selectEditorModuleName = "";
    private void SelectEditorModuleGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("选择编辑模块");
        GUILayout.FlexibleSpace();
        EditorGUILayout.SelectableLabel(selectEditorModuleName);
        GUILayout.EndHorizontal();
    }
    /// <summary>
    /// 模块文件中双击操作，选择文件
    /// </summary>
    /// <param name="t"></param>
    private void ModuleFileDblclickItemCallBack(FolderTreeViewItem t)
    {
        if (t.isDirectory)
            return;

        selectEditorModuleName = t.fullPath.Replace("/", "_"); ;
        toolbarOption = 1;
    }
    /// <summary>
    /// 模块文件中单击选择文件
    /// </summary>
    /// <param name="t"></param>
    private void ModuleFileFolderSelectCallBack(FolderTreeViewItem t)
    {
        //Debug.Log(t.fullPath+ " depth :" + t.depth +" isDir :"+t.isDirectory);
        if (t.isDirectory)
            return;

        selectItemFullName = t.fullPath.Replace("/","_");
    }
    Vector2 pos_editorField = Vector2.zero;
    private string selectItemFullName="";
    void EditorLanguageModuleFileGUI()
    {
       

        if (string.IsNullOrEmpty(m_currentLanguage))
            return;

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("▲多语言模块列表(双击选择文件)");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("新增模块", GUILayout.Width(70)))
        {
            AddLanguageModelGUI();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(9);
        GUIStyle style = "box";
        if (!string.IsNullOrEmpty(selectItemFullName))
        {
            style = "U2D.createRect";
        }
        GUILayout.BeginHorizontal(style);
        GUILayout.Label("选择的文件：" + selectItemFullName);
        if (!string.IsNullOrEmpty(selectItemFullName))
        {
            if (GUILayout.Button("删除", GUILayout.Width(40)))
            {
                
                if (EditorUtility.DisplayDialog("提示", "确定删除 :" + selectItemFullName, "OK", "Cancel"))
                {
                    if (selectItemFullName == selectEditorModuleName)
                        selectEditorModuleName = "";
                    s_languageKeyDict.Remove(selectItemFullName);

                    foreach (var lan in s_languageList)
                    {
                        string moduleName = LanguageManager.GetLanguageDataName(lan, selectItemFullName) + ".txt";
                        FileUtils.DeleteFile(Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + c_DataPath + "/" + lan + "/" + moduleName);
                    }
                    SaveData();
                    AssetDatabase.Refresh();
                    selectItemFullName = "";
                    OnEnable();
                }

            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(8);
        Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
        treeView.OnGUI(rect);
        //pos_editorField = EditorGUILayout.BeginScrollView(pos_editorField,"box", GUILayout.ExpandHeight(false));


        //int index = 0;
        //foreach (var item in s_languageKeyDict)
        //{
        //    EditorGUI.indentLevel = 2;

        //    GUIStyle style = "box";
        //    if (item.Key == selectEditorModuleName)
        //    {
        //        //GUI.color = Color.red;
        //        style = "U2D.createRect";
        //    }
           
        //    EditorGUILayout.BeginHorizontal(style);
        //    if (item.Key != selectEditorModuleName && GUILayout.Button("o",GUILayout.Width(20)))
        //    {
        //        selectEditorModuleName = item.Key;
        //        toolbarOption = 1;
        //    }
        //    GUILayout.Label(index+". " + item.Key);
        //    GUILayout.FlexibleSpace();
            
        //    GUILayout.Space(6);

            
        //    if (GUILayout.Button("删除",GUILayout.Width(40)))
        //    {
                
        //        if (EditorUtility.DisplayDialog("提示", "确定删除 :"+ item.Key, "OK", "Cancel"))
        //        {
        //            if (item.Key == selectEditorModuleName)
        //                selectEditorModuleName = "";
        //            s_languageKeyDict.Remove(item.Key);

        //            foreach (var lan in s_languageList)
        //            {
        //              string moduleName=  LanguageManager.GetLanguageDataName(lan, item.Key) + ".txt";
        //                FileUtils.DeleteFile(Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + c_DataPath + "/" + lan + "/" + moduleName);
        //            }
        //            SaveData();
        //            AssetDatabase.Refresh();
        //            break;
        //        }
               
        //    }

        //    EditorGUILayout.EndHorizontal();

        //    GUI.color = Color.white;

        //    EditorGUILayout.Space();

        //    EditorGUI.indentLevel = 1;

        //    index++;
        //}

        //EditorGUILayout.EndScrollView();

    }

    void AddLanguageModelGUI()
    {
        GeneralDataModificationWindow.OpenWindow(this, "新增模块", "", (value) =>
        {
            value = EditorDrawGUIUtil.DrawBaseValue("模块名", value);

            if (string.IsNullOrEmpty(value.ToString()))
                EditorGUILayout.HelpBox("名字不能为空", MessageType.Error);
            if (s_languageKeyDict.ContainsKey(value.ToString()))
                EditorGUILayout.HelpBox("名字重复", MessageType.Error);
            return value;
        }, (value) =>
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return false;
            if (s_languageKeyDict.ContainsKey(value.ToString()))
                return false;
            return true;
        }, (value) =>
         {
             selectEditorModuleName = value.ToString();
             s_languageKeyDict.Add(value.ToString(), new List<string>());

             DataTable data = new DataTable();
             data.TableKeys.Add(LanguageManager.c_mainKey);
             data.TableKeys.Add(LanguageManager.c_valueKey);
             data.SetDefault(LanguageManager.c_valueKey, "NoValue");

             m_langeuageDataDict.Add(selectEditorModuleName, data);

             SaveData();
             OnEnable();
         });

    }

   

    void EditorLanguageFieldGUI( )
    {
        if (string.IsNullOrEmpty(selectEditorModuleName))
            return;
        if (string.IsNullOrEmpty( m_currentLanguage))
            return;

        List<string> languageKeyList = s_languageKeyDict[selectEditorModuleName];

        DataTable data = null;
        if (m_langeuageDataDict.ContainsKey(selectEditorModuleName))
        {
            data = m_langeuageDataDict[selectEditorModuleName];
        }
        EditorGUI.indentLevel++;
        AddLangeuageFieldGUI(languageKeyList);

        EditorGUILayout.Space();

        EditorDrawGUIUtil.DrawScrollView(languageKeyList, () =>
         {
             for (int i = 0; i < languageKeyList.Count; i++)
             {
                 string key = languageKeyList[i];
                 if (!string.IsNullOrEmpty(searchModuleKey))
                     if (!key.Contains(searchModuleKey))
                         continue;

                 GUILayout.Space(5);
                 GUILayout.BeginVertical("HelpBox");
                 EditorGUILayout.BeginHorizontal();
               
                 string content = "";
                 if(data!=null )
                 {
                     if (!data.ContainsKey(key))
                     {
                         SingleData sd = new SingleData();
                         sd.Add(LanguageManager.c_mainKey, key);
                         sd.Add(LanguageManager.c_valueKey, "");
                         data.AddData(sd);
                     }
                     content = data[key].GetString(LanguageManager.c_valueKey);
                 }
               
                 if (GUILayout.Button("X",GUILayout.Width(20)))
                 {
                     if (EditorUtility.DisplayDialog("提示","确定删除key","OK","Cancel"))
                     {
                         languageKeyList.RemoveAt(i);
                         data.Remove(key);
                         return;
                     }
                    
                 }

                 GUILayout.Label(key);
                 GUILayout.FlexibleSpace();
                 if (GUILayout.Button("CopyPath"))
                 {
                     string tempContent = selectEditorModuleName.Replace('_', '/');
                     tempContent += "/" + key;
                     TextEditor tx = new TextEditor();
                     tx.text = tempContent;
                     tx.OnFocus();
                     tx.Copy();
                     ShowNotification(new GUIContent("已复制"));
                 }

                 EditorGUILayout.EndHorizontal();

                 content = EditorGUILayout.TextArea(content);
                 if (data != null)
                 {
                     data[key][LanguageManager.c_valueKey] = content;
                 }
                 GUILayout.EndVertical();

             }
         }, "box");
       

       
       
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
                    EditorGUILayout.LabelField("字段名重复！", EditorGUIStyleData.WarnMessageLabel);
                }
            }
        }
    }

    #endregion 

    #region 编辑语言

    void DeleteLanguageGUI()
    {
        if (string.IsNullOrEmpty(m_currentLanguage))
            return;
            if (GUILayout.Button("删除语言"))
            {
                if (EditorUtility.DisplayDialog("警告", "确定要删除该语言吗！", "是", "取消"))
                {
                    m_currentSelectIndex = 0;
                    s_languageKeyDict.Remove(m_currentLanguage);
                    Directory.Delete(Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + c_DataPath + "/" + m_currentLanguage, true);
                    AssetDatabase.Refresh();
                }
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
            SaveData();
        }
    }

    void SaveData()
    {
        SaveEditorConfig();
        SaveConfig();

        if (m_currentLanguage != "None")
        {
            if(m_langeuageDataDict.ContainsKey(selectEditorModuleName))
            {
                SaveData(GetLanguageSavePath(m_currentLanguage, selectEditorModuleName), m_langeuageDataDict[selectEditorModuleName]);
            }
        }
        LanguageManager.IsInit = false;    
        ConfigManager.CleanCache();
        DataEditorWindow.Refresh();
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
                EditorGUILayout.LabelField("已存在该语言",EditorGUIStyleData.WarnMessageLabel);
            }
        }

        EditorGUILayout.Space();
    }

    #endregion

    #region 生成模块类

    void GenerateLanuageClassGUI()
    {
        if(GUILayout.Button( "生成语言模块声明类"))
        {
            GenerateLanuageClass();
        }
    }

    void GenerateLanuageClass()
    {
        string content = "";
        string SavePath = Application.dataPath + "/Script/Generate/LanguageDef.cs";


        content += "public class LanguageDef\n";
        content += "{\n";

        //只创建模块名
        foreach (var item in s_languageKeyDict)
        {
            content += "\tpublic const string c_" + StringFilter(item.Key) + " = \"" + item.Key + "\";\n";

            List<string> list = item.Value;
            for (int i = 0; i < list.Count; i++)
            {
                content += "\tpublic const string c_" + StringFilter(item.Key) + "_" + StringFilter(list[i]) + " = \"" + list[i] + "\";\n";
            }

            content += "\n";
        }

        content += "}\n";

        EditorUtil.WriteStringByFile(SavePath, content);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 去掉所有不合法的字符
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    string StringFilter(string content)
    {
        return content.Replace("-","");
    }

    #endregion

    #region FindLanguageData

    private string m_directoryPath;
   

    void FindAllDataName()
    {
        AssetDatabase.Refresh();
        s_languageList = new List<string>();

        //s_languageList.Add("None");

        m_directoryPath = Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + c_DataPath;

        FileTool.CreatPath(m_directoryPath);

        FindConfigName(m_directoryPath);
    }

    public void FindConfigName(string path)
    {
        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            s_languageList.Add(FileTool.RemoveExpandName(PathTool.GetDirectoryRelativePath(m_directoryPath + "/", dires[i])));
        }
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
