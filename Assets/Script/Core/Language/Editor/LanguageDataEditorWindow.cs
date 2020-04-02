using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class LanguageDataEditorWindow : EditorWindow
{
   
    private SystemLanguage currentLanguage = SystemLanguage.Unknown;
    private DataTable currentFileDataTable;
    private LanguageSettingConfig config;

    //所有文件（转换成全路径/）
    static List<string> s_languageFullKeyFileNameList = new List<string>();

    static LanguageDataEditorWindow win = null;

    [MenuItem("Window/多语言编辑器 &5", priority = 600)]
    public static LanguageDataEditorWindow ShowWindow()
    {
        win = EditorWindow.GetWindow<LanguageDataEditorWindow>();
        win.Init();
        return win;
    }
    FolderTreeView treeView;
    TreeViewState treeViewState = null;

    private void Init()
    {
        win = this;
       config= LanguageDataUtils.LoadRuntimeConfig();

        if(config == null)
        {
            return;
        }

        if (Application.isPlaying )
        {
            currentLanguage = LanguageManager.CurrentLanguage;
        }
        if (!config.gameExistLanguages.Contains(currentLanguage))
        {
            currentLanguage = config.defaultLanguage;
        }
        s_languageFullKeyFileNameList= LanguageDataEditorUtils.LoadLangusgeAllFileNames(currentLanguage);
        if (!string.IsNullOrEmpty(selectFullFileName))
        {
            currentFileDataTable = LanguageDataUtils.LoadFileData(currentLanguage, selectFullFileName);
        }
        if (treeViewState == null)
            treeViewState = new TreeViewState();

        treeView = new FolderTreeView(treeViewState);

        treeView.SetData(s_languageFullKeyFileNameList);
        treeView.dblclickItemCallBack = ModuleFileDblclickItemCallBack;
        treeView.selectCallBack = ModuleFileFolderSelectCallBack;
    }
    /// <summary>
    /// 检查修复多语言完整性
    /// </summary>
    private void CheckAllLanguageFileIntact()
    {
        List<string> defaultFullKeyNameList= LanguageDataEditorUtils.LoadLangusgeAllFileNames(config.defaultLanguage);
        StringBuilder logBuider = new StringBuilder();

        logBuider.Append("开始修复多语言完整....\n");
        logBuider.Append("基准语言:" + config.defaultLanguage + "\n");

        foreach (var nowLan in config.gameExistLanguages)
        {
            if (nowLan == config.defaultLanguage)
                continue;
            logBuider.Append("开始检查语言:" + nowLan + "\n");
            List<string> nowFullKeyNameList = LanguageDataEditorUtils.LoadLangusgeAllFileNames(nowLan);
            foreach (var fullKeyFileName in defaultFullKeyNameList)
            {
               

                DataTable dt = LanguageDataUtils.LoadFileData(config.defaultLanguage, fullKeyFileName);
                if (!nowFullKeyNameList.Contains(fullKeyFileName))
                {
                   
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    foreach (var id in dt.TableIDs)
                    {
                        dic.Add(id, "");
                    }
                    CreateLanguageNewFile(nowLan, fullKeyFileName, dic);

                    logBuider.Append(nowLan + "缺少文件:" + fullKeyFileName + " 已添加!\n");
                }
                else
                {
                    DataTable nowDT = LanguageDataUtils.LoadFileData(nowLan, fullKeyFileName);
                    List<string> addKeys = new List<string>();
                    foreach (var id in dt.TableIDs)
                    {
                        if (!nowDT.TableIDs.Contains(id))
                        {
                            addKeys.Add(id);
                        }
                    }
                    AddNewKey(nowLan, fullKeyFileName, addKeys.ToArray());
                    if (addKeys.Count > 0)
                    {
                        logBuider.Append(nowLan + "=>" + fullKeyFileName + "缺少字段:\n");
                        foreach (var item in addKeys)
                        {
                            logBuider.Append("\t" + item + "\n");
                        }
                        logBuider.Append("\n");
                    }
                }
            }
        }
        logBuider.Append("多语言完整性修复完成！\n");
        AssetDatabase.Refresh();
        Debug.Log(logBuider.ToString());
        ShowNotification(new GUIContent( "修复完成"));
    }
    void OnEnable()
    {
        Init();
    }
    public int toolbarOption = 0;
    private string[] toolbarTexts = { "模块文件", "语言内容编辑", "语言设置" };
    private bool richText = false;
    void OnGUI()
    {
        titleContent.text = "多语言编辑器";
        if (config == null|| config.gameExistLanguages.Count==0)
        {
            AddLanguageGUI();
            return;
        }
        richText = (bool)EditorDrawGUIUtil.DrawBaseValue("使用富文本显示：", richText);
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
                ConfigSettingGUI();
                AddLanguageGUI();
                DeleteLanguageGUI();
                break;
        }

        GUILayout.FlexibleSpace();
        SaveDataGUI();
    }

    public static void OpenAndSearchValue(string fullFlieKey)
    {
        LanguageDataEditorWindow w = ShowWindow();
        w.searchValue = fullFlieKey;
        w.toolbarOption = 1;
    }
    private string searchValue;
    private string searchKey = "";
    private void SearchValueGUI()
    {
        searchValue = EditorDrawGUIUtil.DrawSearchField(searchValue);

        if (!string.IsNullOrEmpty(searchValue))
        {
            if (searchValue.Contains("/"))
            {
                string[] tempV = searchValue.Split('/');
                string key = tempV[tempV.Length - 1];
                int indexEnd = searchValue.LastIndexOf("/");
                string moduleName = searchValue.Remove(indexEnd);
                if (s_languageFullKeyFileNameList.Contains(moduleName))
                {
                    if (selectFullFileName != moduleName)
                    {
                        selectFullFileName = moduleName;
                        Debug.Log("moduleName :" + moduleName);
                        currentFileDataTable = LanguageDataUtils.LoadFileData(currentLanguage, selectFullFileName);
                        Debug.Log("currentFileDataTable :" + currentFileDataTable );
                        Debug.Log("  keys:" + currentFileDataTable.TableIDs.Count);
                    }
                    searchKey = key;
                }
                else
                {
                    selectFullFileName = "";
                    searchKey = "";
                }

            }
            else
                searchKey = searchValue;
        }
        else
        {
            searchKey = "";
        }
    }

    #region 语言设置

    void DefaultLanguageGUI()
    {
        if (currentLanguage == config.defaultLanguage)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("默认语言");
        }
    }

    void ConfigSettingGUI()
    {
        config.useSystemLanguage = (bool)EditorDrawGUIUtil.DrawBaseValue("使用系统语言", config.useSystemLanguage);
        config.defaultLanguage = EditorDrawGUIUtil.DrawPopup("默认语言", config.defaultLanguage, config.gameExistLanguages);
        string lanNames = "";
        foreach (var item in config.gameExistLanguages)
        {
            lanNames += " " + item;
        }
        EditorGUILayout.LabelField("已含有语言：" + lanNames);
    }

    #endregion

    #region 选择语言

    void SelectLanguageGUI()
    {
        GUILayout.BeginHorizontal();
        currentLanguage = EditorDrawGUIUtil.DrawPopup("当前语言", currentLanguage, config.gameExistLanguages, (lan) =>
           {
               if (!string.IsNullOrEmpty(selectFullFileName))
                   currentFileDataTable = LanguageDataUtils.LoadFileData(lan, selectFullFileName);
               Debug.Log("切换到:" + lan);
               LanguageManager.SetLanguage(lan);
           });
        if (currentLanguage == SystemLanguage.Chinese)
            currentLanguage = SystemLanguage.ChineseSimplified;

        if (GUILayout.Button("加载上一次保存"))
        {

            DataManager.CleanCache();
            LanguageManager.IsInit = false;
            GlobalEvent.DispatchEvent(EditorEvent.LanguageDataEditorChange);
            currentFileDataTable = LanguageDataUtils.LoadFileData(currentLanguage, selectFullFileName);
            GUI.FocusControl("");
        }
        if (GUILayout.Button("检查修复多语言完整性"))
        {
            CheckAllLanguageFileIntact();
        }
            GUILayout.EndHorizontal();
    }
    #endregion

    #region 编辑语言字段

    private string selectFullFileName = "";
    private void SelectEditorModuleGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("选择编辑模块");
        GUILayout.FlexibleSpace();
        EditorGUILayout.SelectableLabel(selectFullFileName);
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

        selectFullFileName = t.fullPath;
        currentFileDataTable = LanguageDataUtils.LoadFileData(currentLanguage, selectFullFileName);
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

        selectItemFullName = t.fullPath;
    }
    private string selectItemFullName = "";
    void EditorLanguageModuleFileGUI()
    {
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
                    if (selectItemFullName == selectFullFileName)
                        selectFullFileName = "";
                    s_languageFullKeyFileNameList.Remove(selectItemFullName);


                    foreach (var lan in config.gameExistLanguages)
                    {
                        string path = LanguageDataUtils.GetLanguageSavePath(lan, selectItemFullName);
                        FileUtils.DeleteFile(path);
                    }
                   // SaveData();
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

    }

    void AddLanguageModelGUI()
    {
        GeneralDataModificationWindow.OpenWindow(this, "新增模块", "", (value) =>
        {
            value = EditorDrawGUIUtil.DrawBaseValue("模块名", value);
            string tempName = value.ToString();
            EditorGUILayout.HelpBox("下划线分割文件夹，如：AA_BB_CC", MessageType.Info);
            if (string.IsNullOrEmpty(tempName))
                EditorGUILayout.HelpBox("名字不能为空", MessageType.Error);
            if (s_languageFullKeyFileNameList.Contains(tempName.Replace('_','/')))
                EditorGUILayout.HelpBox("名字重复", MessageType.Error);
            return value;
        }, (value) =>
        {
            string tempName = value.ToString();
            if (string.IsNullOrEmpty(tempName))
                return false;
            if (s_languageFullKeyFileNameList.Contains(tempName.Replace('_', '/')))
                return false;
            return true;
        }, (value) =>
         {
             string fileName = value.ToString();
             CreateNewFile(fileName, null);
             string tempContent = fileName.Replace('_', '/');
             selectFullFileName = tempContent;
             //SaveData();
             Init();
         });

    }


    void EditorLanguageFieldGUI()
    {
        if (string.IsNullOrEmpty(selectFullFileName))
            return;
        if (currentFileDataTable == null)
            return;
        DataTable data = currentFileDataTable;
        List<string> languageKeyList = data.TableIDs;
        //Debug.Log("languageKeyList :" + languageKeyList.Count);
        AddLangeuageFieldGUI(languageKeyList);

        EditorGUILayout.Space();

        EditorDrawGUIUtil.DrawScrollView(languageKeyList, () =>
         {
             for (int i = 0; i < languageKeyList.Count; i++)
             {
                 string key = languageKeyList[i];
                 if (!string.IsNullOrEmpty(searchKey))
                     if (!key.Contains(searchKey))
                         continue;

                 GUILayout.Space(5);
                 GUILayout.BeginVertical("HelpBox");
                 EditorGUILayout.BeginHorizontal();

                 string content = "";
                 if (data != null)
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

                 if (GUILayout.Button("X", GUILayout.Width(20)))
                 {
                     if (EditorUtility.DisplayDialog("提示", "确定删除key", "OK", "Cancel"))
                     {
                         DeleteKey(selectFullFileName, key);
                         Init();
                         return;
                     }

                 }
                 string showKeyValue=key;
                 if (currentLanguage != config.defaultLanguage)
                 {
                     showKeyValue +=":"+ GetLanguageValue(config.defaultLanguage, selectFullFileName, key);
                 }
                 GUIStyle styleLable = "Label";
                 styleLable.wordWrap = true;
                 styleLable.richText = richText;
                 styleLable.alignment = TextAnchor.MiddleLeft;
               //  EditorGUILayout.SelectableLabel(key, styleLable);
                 EditorGUILayout.SelectableLabel(showKeyValue, styleLable);
                // GUILayout.FlexibleSpace();
                

                 EditorGUILayout.EndHorizontal();

                 GUILayout.BeginHorizontal();
                 GUIStyle style = "TextArea";
                 style.wordWrap = true;
                 style.richText = richText;
                 content = EditorGUILayout.TextArea(content, style);
                 if (data != null)
                 {
                     data[key][LanguageManager.c_valueKey] = content;
                 }
                 if (GUILayout.Button("CopyPath",GUILayout.Width(75)))
                 {
                     string tempContent = selectFullFileName;
                     tempContent += "/" + key;
                     TextEditor tx = new TextEditor();
                     tx.text = tempContent;
                     tx.OnFocus();
                     tx.Copy();
                     ShowNotification(new GUIContent("已复制"));
                 }
                 GUILayout.EndHorizontal();

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
            newField = EditorGUILayout.TextField("字段名", newField);

            if (newField != "" && !languageKeyList.Contains(newField))
            {
                if (GUILayout.Button("新增语言字段"))
                {
                    AddNewKeyAllLanguage(selectFullFileName, newField);
                    Init();
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

    #region 访问查询API
    /// <summary>
    /// 获取某种语言的多语言值
    /// </summary>
    /// <param name="language"></param>
    /// <param name="fullFileName">"/"格式的文件名</param>
    /// <param name="key"></param>
    /// <returns></returns>
    private string GetLanguageValue(SystemLanguage language,string fullFileName,string key)
    {
       DataTable data =  LanguageDataUtils.LoadFileData(language, fullFileName);

        if(data.ContainsKey(key)&& data[key].ContainsKey(LanguageManager.c_valueKey))
        return data[key][LanguageManager.c_valueKey];

        return "";
    } 

    #endregion

    #region 编辑语言

    void DeleteLanguageGUI()
    {
        if (GUILayout.Button("删除语言"))
        {
            SystemLanguage deleteLan = config.defaultLanguage;
            if (EditorUtility.DisplayDialog("警告", "确定要删除[" + deleteLan + "]语言吗！", "是", "取消"))
            {
               
                config.gameExistLanguages.Remove(deleteLan);
                if (config.gameExistLanguages.Count > 0)
                    config.defaultLanguage = config.gameExistLanguages[0];
                else
                    config.defaultLanguage = SystemLanguage.Unknown;

                if (!config.gameExistLanguages.Contains(currentLanguage))
                {
                    currentLanguage = config.defaultLanguage;
                }
                try
                {
                    currentFileDataTable = null;
                    selectFullFileName = "";
                    Directory.Delete(LanguageDataUtils.SavePathDir + deleteLan, true);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                SaveData();
                AssetDatabase.Refresh();

            }
        }
    }

    void AddMissLanguageGUI(DataTable data, string key)
    {
        if (GUILayout.Button("添加记录"))
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
            ShowNotification(new GUIContent("已保存"));
        }
    }

    void SaveData()
    {
        LanguageDataUtils. SaveEditorConfig(config);
        LanguageDataEditorUtils.SaveData(currentLanguage, selectFullFileName, currentFileDataTable);

        

        LanguageManager.Release();
        LanguageManager.SetLanguage(currentLanguage);

        GlobalEvent.DispatchEvent(EditorEvent.LanguageDataEditorChange);
        UnityEditor.AssetDatabase.Refresh();
    }

    #endregion

    #region 新增语言

    void AddLanguageGUI()
    {
        if (GUILayout.Button("新增语言"))
        {
            GeneralDataModificationWindow.OpenWindow(this, "新增语言", SystemLanguage.Afrikaans, (value) =>
             {
                 SystemLanguage lan = (SystemLanguage)value;
                 lan = (SystemLanguage)EditorDrawGUIUtil.DrawBaseValue("语言：", lan);

                 if (config!=null&& config.gameExistLanguages.Contains(lan))
                 {
                     EditorGUILayout.HelpBox("已存在", MessageType.Error);
                 }
                 if (lan == SystemLanguage.Chinese)
                     lan = SystemLanguage.ChineseSimplified;
                 if (lan == SystemLanguage.Unknown)
                     lan = SystemLanguage.ChineseSimplified;
                 return lan;
             }, (value) =>
             {
                 SystemLanguage lan = (SystemLanguage)value;
                 if (config != null && config.gameExistLanguages.Contains(lan))
                 {
                     return false;
                 }
                 return true;
             }, (value) =>
             {
                 if (config == null)
                     config = new LanguageSettingConfig();
                 SystemLanguage lan = (SystemLanguage)value;
                 config.gameExistLanguages.Add(lan);
                 CreateNewLangusge(lan);

             }
                 );
        }
    }

    private void CreateNewLangusge(SystemLanguage lan)
    {
        if (config == null)
        {
            config = new LanguageSettingConfig();
        }
        if (config.defaultLanguage == SystemLanguage.Unknown)
        {
            config.defaultLanguage = lan;
            config.gameExistLanguages.Add(lan);
            currentLanguage = lan;
        }
        else
        {
            foreach (var item in s_languageFullKeyFileNameList)
            {
                DataTable dt = LanguageDataUtils.LoadFileData(config.defaultLanguage, item);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var id in dt.TableIDs)
                {
                    dic.Add(id, "");
                }
                CreateLanguageNewFile(lan, item, dic);
            }
        }
        SaveData();
        AssetDatabase.Refresh();
        Init();
    }


    /// <summary>
    /// 新建多语言文件,给每种语言添加新文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="contentDic"></param>
    /// <returns>返回每个key对应的多语言访问key</returns>
    public Dictionary<string, string> CreateNewFile(string fileName, Dictionary<string, string> contentDic)
    {
        Dictionary<string, string> keyPaths = new Dictionary<string, string>();
        string tempContent = fileName.Replace('_', '/');
        //给每种语言添加文件
        foreach (var item in config.gameExistLanguages)
        {
            keyPaths = CreateLanguageNewFile(item, tempContent, contentDic);
        }
        UnityEditor.AssetDatabase.Refresh();
        return keyPaths;
    }

    public Dictionary<string, string> CreateLanguageNewFile(SystemLanguage language, string fullKeyFileName, Dictionary<string, string> contentDic)
    {
        Dictionary<string, string> keyPaths = new Dictionary<string, string>();

        DataTable data = new DataTable();
        data.TableKeys.Add(LanguageManager.c_mainKey);
        data.TableKeys.Add(LanguageManager.c_valueKey);
        data.SetDefault(LanguageManager.c_valueKey, "NoValue");
        if (contentDic != null)
        {
            foreach (var item in contentDic)
            {
                SingleData sd = new SingleData();
                sd.Add(LanguageManager.c_mainKey, item.Key);
                sd.Add(LanguageManager.c_valueKey, item.Value);
                data.AddData(sd);
                keyPaths.Add(item.Key, fullKeyFileName + "/" + item.Key);
            }

        }
        LanguageDataEditorUtils.SaveData(language, fullKeyFileName, data);
       
        return keyPaths;
    }
    private void AddNewKeyAllLanguage(string fullKeyFileName, string key)
    {
        foreach (var language in config.gameExistLanguages)
        {
            AddNewKey(language, fullKeyFileName,new string[] { key });
          
        }
        UnityEditor.AssetDatabase.Refresh();
    }
    private void AddNewKey(SystemLanguage language, string fullKeyFileName, string[] keys)
    {
        if (keys == null || keys.Length == 0)
            return;
        DataTable data = LanguageDataUtils.LoadFileData(language, fullKeyFileName);
        foreach (var key in keys)
        {
            if (data.ContainsKey(key))
                continue;
            SingleData sd = new SingleData();
            sd.Add(LanguageManager.c_mainKey, key);
            sd.Add(LanguageManager.c_valueKey, "");
            data.AddData(sd);
        }
        LanguageDataEditorUtils.SaveData(language, fullKeyFileName, data);

        UnityEditor.AssetDatabase.Refresh();
    }

    private void DeleteKey(string fullKeyFileName, string key)
    {
        foreach (var language in config.gameExistLanguages)
        {
            DataTable data = LanguageDataUtils. LoadFileData(language, fullKeyFileName);
            data.RemoveData(key);
            LanguageDataEditorUtils.SaveData(language, fullKeyFileName, data);
        }
        UnityEditor.AssetDatabase.Refresh();
    }

    #endregion
}
