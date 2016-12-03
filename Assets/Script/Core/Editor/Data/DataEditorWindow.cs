using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
public class DataEditorWindow : EditorWindow
{
    UILayerManager m_UILayerManager;

    [MenuItem("Window/数据编辑器", priority = 500)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DataEditorWindow));
    }

    List<String> m_dataNameList = new List<string>();

    string m_currentDataName;
    DataTable m_currentData;
    Dictionary<string, bool> m_foldList = new Dictionary<string, bool>();

    void OnEnable()
    {
        //if (!Application.isPlaying)
        //{
        m_currentSelectIndex = 0;
        EditorGUIStyleData.Init();

        FindAllDataName();
        //}
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
            FindAllDataName();

            if (m_currentDataName != null
                && m_currentDataName != ""
                && m_currentDataName != "None")
            {
                LoadData(m_currentDataName);
            }
        }
    }

    #region GUI

    void OnGUI()
    {
        titleContent.text = "数据编辑器";

        EditorGUILayout.BeginVertical();

        SelectDataGUI();

        DataGUI();

        AddDataGUI();

        CleanCatchGUI();

        EditorGUILayout.EndVertical();
    }

    #region 数据文件相关

    int m_currentSelectIndex = 0;
    void SelectDataGUI()
    {
        string[] mask = m_dataNameList.ToArray();
        m_currentSelectIndex = EditorGUILayout.Popup("当前数据：", m_currentSelectIndex, mask);
        if (mask.Length !=0 )
        {
            LoadData(mask[m_currentSelectIndex]);
        }
    }

    void LoadData(string dataName)
    {
        if (m_currentDataName != dataName)
        {
            m_currentDataName = dataName;

            if (m_currentDataName != "None")
                m_currentData = DataManager.GetData(dataName); 
        }
    }
    bool isConfigFold;
    string dataName = "";
    string mainKey = "";

    void AddDataGUI()
    {
        EditorGUI.indentLevel = 0;

        isConfigFold = EditorGUILayout.Foldout(isConfigFold, "新增数据");
        
        if (isConfigFold)
        {
            EditorGUI.indentLevel = 1;

            dataName = EditorGUILayout.TextField("数据名", dataName);

            mainKey  = EditorGUILayout.TextField("主键名", mainKey);

            if (!m_dataNameList.Contains(dataName) && dataName != "" && mainKey != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    DataTable data = new DataTable();
                    data.TableKeys.Add(mainKey);

                    DataManager.SaveData(dataName, data);
                    AssetDatabase.Refresh();

                    LoadData(dataName);

                    dataName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (m_dataNameList.Contains(dataName))
                {
                    EditorGUILayout.LabelField("已存在该配置");
                }

                if (mainKey == "")
                {
                    EditorGUILayout.LabelField("主键不能为空");
                }
            }
        }

        EditorGUILayout.Space();
    }

    void DeleteDataGUI()
    {
        if (m_currentDataName != "None")
        {
            if (GUILayout.Button("删除数据"))
            {
                if (EditorUtility.DisplayDialog("警告", "确定要删除该数据吗！", "是", "取消"))
                {
                    File.Delete(Application.dataPath + "/Resources/" + DataManager.c_directoryName + "/" + m_currentDataName + ".txt");
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    void CleanCatchGUI()
    {
        if (GUILayout.Button("清除缓存"))
        {
            DataManager.CleanCatch();
        }
    }

    #endregion

    #region 记录相关

    Vector2 pos = Vector3.zero;

    bool isFoldList = true;
    void DataGUI()
    {
        if (m_currentData != null
            && m_currentDataName != "None")
        {
            EditorGUI.indentLevel = 1;
            isFoldList = EditorGUILayout.Foldout(isFoldList, "记录列表");
            if (isFoldList)
            {
                pos = EditorGUILayout.BeginScrollView(pos, GUILayout.ExpandHeight(false));

                List<string> keys = m_currentData.TableIDs;

                for (int i = 0; i < keys.Count; i++)
                {
                    EditorGUI.indentLevel = 2;
                    DataItemGUI(m_currentData, keys[i]);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel = 1;
            AddDataGUI(m_currentData);

            EditorGUI.indentLevel = 1;
            EditorDataGUI();

            SaveDataGUI();

            DeleteDataGUI();

            EditorGUILayout.Space();
        }

    }


    bool isFold = false;
    string mianKey = "";
    SingleData content = new SingleData();

    Vector2 AddDataPos = Vector2.zero; 
    void AddDataGUI(DataTable dict)
    {
        isFold = EditorGUILayout.Foldout(isFold, "新增记录");
        if (isFold)
        {
            EditorGUI.indentLevel++;

            string key = dict.TableKeys[0];

            AddDataPos = EditorGUILayout.BeginScrollView(AddDataPos, GUILayout.ExpandHeight(false));

            EditorGUILayout.LabelField("<主键>字段名", key);
            mianKey = EditorUtilGUI.FieldGUI_TypeValue(FieldType.String, mianKey,null);

            if (mianKey == "")
            {
                EditorGUILayout.LabelField("主键不能为空！");
            }
            else if (dict.ContainsKey(mianKey))
            {
                EditorGUILayout.LabelField("重复的主键！", EditorGUIStyleData.s_WarnMessageLabel);
            }

            EditorGUILayout.Space();

            EditorDataGUI(dict, content);

            if (!dict.ContainsKey(mianKey) && mianKey != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    content.Add(key, mianKey);

                    m_currentData.AddData(content);
                    content = new SingleData();
                    mianKey = "";
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
        }


    }

    void DataItemGUI(DataTable table, string key)
    {
        if (!m_foldList.ContainsKey(key))
        {
            m_foldList.Add(key, false);
        }

        EditorGUILayout.BeginHorizontal();

        m_foldList[key] = EditorGUILayout.Foldout(m_foldList[key], key);

        if (GUILayout.Button("删除记录", GUILayout.Width(EditorGUIStyleData.s_ButtonWidth_small)))
        {
            table.RemoveData(key);
            return;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel ++;

        if (m_foldList[key])
        {
            SingleData data = table[key];

            List<string> keys = table.TableKeys;

            //这里只显示主键
            for (int i = 0; i < keys.Count; i++)
            {
                string keyTmp = keys[i];
               
                if (i == 0)
                {
                    EditorGUILayout.LabelField("["+ keyTmp+"]");

                    EditorGUI.indentLevel++;
                    //EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("<主键>字段名", keyTmp);
                    EditorGUILayout.LabelField("字段值", data[keyTmp]);

                    EditorGUI.indentLevel--;
                    //EditorGUI.indentLevel--;
                }
            }
            //显示其他键
            EditorDataGUI(table, data);
        }
    }

    SingleData EditorDataGUI(DataTable table, SingleData data)
    {
        try
        {
            List<string> keys = table.TableKeys;
            for (int i = 0; i < keys.Count; i++)
            {
                string keyTmp = keys[i];
                FieldType type = table.GetFieldType(keyTmp);

                if (i != 0)
                {
                    bool cancelDefault = false;
                    EditorGUILayout.BeginHorizontal();
                    
                    if (data.ContainsKey(keyTmp))
                    {
                        EditorGUILayout.LabelField("[" + keyTmp + "]");

                        if (GUILayout.Button("使用默认值"))
                        {
                            data.Remove(keyTmp);
                            EditorGUILayout.EndHorizontal();

                            continue;
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("[" + keyTmp + "] (默认值)");
                        if (GUILayout.Button("取消默认值"))
                        {
                            cancelDefault = true;
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    //EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;

                    //非默认值情况
                    if (data.ContainsKey(keyTmp))
                    {
                        EditorGUILayout.LabelField("字段名", keyTmp);
                        EditorGUILayout.LabelField("注释", table.GetNote(keyTmp));

                        string newContent = EditorUtilGUI.FieldGUI_TypeValue(type, data[keyTmp], table.GetEnumType(keyTmp));

                        if (newContent != data[keyTmp])
                        {
                            data[keyTmp] = newContent;
                        }
                    }
                    //如果是默认值则走这里
                    else
                    {
                        EditorGUILayout.LabelField("字段名", keyTmp);
                        EditorGUILayout.LabelField("注释", table.GetNote(keyTmp));
                        string newContent = "";

                        if (table.m_defaultValue.ContainsKey(keyTmp))
                        {
                            newContent = new SingleField(type, table.GetDefault(keyTmp), table.GetEnumType(keyTmp)).m_content;
                        }
                        else
                        {
                            newContent = new SingleField(type, null, table.GetEnumType(keyTmp)).m_content;
                        }

                        if (type != FieldType.Enum)
                        {
                            EditorGUILayout.LabelField("字段类型", type.ToString());
                        }
                        else
                        {
                            EditorGUILayout.LabelField("字段类型", type.ToString() + "/" + table.GetEnumType(keyTmp));
                        }

                        EditorGUILayout.LabelField("(默认)字段内容", new SingleField(type, newContent, table.GetEnumType(keyTmp)).GetShowString());

                        if (cancelDefault)
                        {
                            data.Add(keyTmp, newContent);
                        }
                    }

                    EditorGUI.indentLevel--;
                    //EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }
        catch(Exception e)
        {
            EditorGUILayout.TextArea(e.ToString(),EditorGUIStyleData.s_ErrorMessageLabel);
        }

        return data;
    }

    #endregion

    #region 字段相关

    bool m_isEditorFold = false;
    FieldType m_editorNewType;
    int m_editorNewEnumIndex = 0;
    string m_editorNoteContent = "";
    Vector2 m_EditorPos = Vector2.zero;
    void EditorDataGUI()
    {
        m_isEditorFold = EditorGUILayout.Foldout(m_isEditorFold, "编辑数据");
        EditorGUI.indentLevel ++;

        if (m_isEditorFold)
        {
            List<string> keys = m_currentData.TableKeys;
            m_EditorPos = EditorGUILayout.BeginScrollView(m_EditorPos, GUILayout.ExpandHeight(false));
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                FieldType type = m_currentData.GetFieldType(key);
                int EnumTypeIndex = EditorTool.GetAllEnumTypeIndex(m_currentData.GetEnumType(key));

                if (i == 0)
                {
                    EditorGUILayout.LabelField("<主键>字段名", key);
                    EditorGUILayout.LabelField("字段类型", m_currentData.GetFieldType(keys[i]).ToString());
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("字段名", key);

                    if(GUILayout.Button("删除字段"))
                    {
                        if (EditorUtility.DisplayDialog("警告", "确定要删除该字段吗？", "是", "取消"))
                        {
                            DeleteField(m_currentData, key);
                            continue;
                        }
                    }

                    EditorGUILayout.EndHorizontal();

                    bool isNewType = false;

                    m_editorNoteContent = EditorGUILayout.TextField("注释", m_currentData.GetNote(key));
                    m_currentData.SetNote(key, m_editorNoteContent);

                    m_editorNewType = (FieldType)EditorGUILayout.EnumPopup("字段类型", type);

                    if (m_editorNewType == FieldType.Enum)
                    {
                        m_editorNewEnumIndex = EditorGUILayout.Popup("枚举类型", EnumTypeIndex, EditorTool.GetAllEnumType());

                        if (EnumTypeIndex != m_editorNewEnumIndex)
                        {
                            isNewType = true;
                        }
                    }

                    if (type != m_editorNewType)
                    {
                        isNewType = true;
                    }

                    if (isNewType)
                    {
                        //弹出警告并重置数据
                        if (EditorUtility.DisplayDialog("警告", "改变字段类型会重置该字段的所有数据和默认值\n是否继续？", "是", "取消"))
                        {
                            m_currentData.SetFieldType(key, m_editorNewType, EditorTool.GetAllEnumType()[m_editorNewEnumIndex]);
                            ResetDataField(m_currentData, key, m_editorNewType, EditorTool.GetAllEnumType()[m_editorNewEnumIndex]);

                             type = m_editorNewType;
                             EnumTypeIndex = m_editorNewEnumIndex;
                             content = new SingleData();
                        }
                    }

                    string newContent;
                    if (type == FieldType.Enum)
                    {
                        newContent = EditorUtilGUI.FieldGUI_Type(type, EditorTool.GetAllEnumType()[EnumTypeIndex], m_currentData.GetDefault(key), "默认值");
                    }
                    else
                    {
                        newContent = EditorUtilGUI.FieldGUI_Type(type, null, m_currentData.GetDefault(key), "默认值");
                    }

                    m_currentData.SetDefault(key, newContent);
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();

            AddFieldGUI();
        }
    }

    void DeleteFieldGUI()
    {
    }

    void DeleteField(DataTable table,string fieldName)
    {
        table.TableKeys.Remove(fieldName);
    }

    bool m_isAddFoldField = false;
    string m_newFieldName = "";
    FieldType m_newAddType = FieldType.String;
    string m_newFieldDefaultValue = "";
    int m_newEnumTypeIndex = 0;
    string m_addNoteContent = "";
    void AddFieldGUI()
    {
        EditorGUILayout.Space();
        m_isAddFoldField = EditorGUILayout.Foldout(m_isAddFoldField, "新增字段");
        EditorGUI.indentLevel ++;

        if (m_isAddFoldField)
        {
            m_newFieldName = EditorGUILayout.TextField("字段名", m_newFieldName);
            FieldType typeTmp = (FieldType)EditorGUILayout.EnumPopup("字段类型", m_newAddType);

            bool isNewFieldType = false;

            if (typeTmp != m_newAddType)
            {
                m_newAddType = typeTmp;
                isNewFieldType = true;
            }

            m_addNoteContent = EditorGUILayout.TextField("注释", m_addNoteContent);

            if (typeTmp == FieldType.Enum) 
            {
                int newEnumTypeIndex = EditorGUILayout.Popup("枚举类型", m_newEnumTypeIndex, EditorTool.GetAllEnumType());

                if (newEnumTypeIndex != m_newEnumTypeIndex)
                {
                    m_newEnumTypeIndex = newEnumTypeIndex;
                    isNewFieldType = true;
                }
            }

            //更改字段类型重设初始值
            if (isNewFieldType)
            {
                if (typeTmp == FieldType.Enum)
                {
                    m_newFieldDefaultValue = new SingleField(m_newAddType, null, EditorTool.GetAllEnumType()[m_newEnumTypeIndex]).m_content;
                }
                else
                {
                    m_newFieldDefaultValue = new SingleField(m_newAddType, null, null).m_content;
                }
            }

            //是否是一个合理的字段名
            bool isShowButton = true;

            if (m_newFieldName == "")
            {
                isShowButton = false;
            }

            if (m_currentData.TableKeys.Contains(m_newFieldName))
            {
                isShowButton = false;
                EditorGUILayout.TextField("字段名不能重复！",EditorGUIStyleData.s_WarnMessageLabel);
            }

            m_newFieldDefaultValue = EditorUtilGUI.FieldGUI_Type(m_newAddType, EditorTool.GetAllEnumType()[m_newEnumTypeIndex], m_newFieldDefaultValue, "默认值");

            if (isShowButton)
            {
                if (GUILayout.Button("新增字段")) 
                {
                    if (m_newAddType == FieldType.Enum)
                    {
                        AddField(m_currentData, m_newFieldName, m_newAddType, m_newFieldDefaultValue, EditorTool.GetAllEnumType()[m_newEnumTypeIndex], m_addNoteContent);
                    }
                    else
                    {
                        AddField(m_currentData, m_newFieldName, m_newAddType, m_newFieldDefaultValue, null, m_addNoteContent);
                    }
                    
                    m_newFieldName = "";
                    m_newFieldDefaultValue = "";
                    m_addNoteContent = "";
                    m_newAddType = FieldType.String;
                    m_newEnumTypeIndex = 0;
                }
            }

        }
    }

    void AddField(DataTable table, string fieldName,FieldType type ,string value,string enumType,string note)
    {
        table.TableKeys.Add(fieldName);
        table.SetFieldType(fieldName, type, enumType);
        table.SetDefault(fieldName, value);
        table.SetNote(fieldName, note);
    }

    void ResetDataField(DataTable data,string key,FieldType type,string enumType)
    {
        string newContent = new SingleField(type, null, enumType).m_content;

        for (int i = 0; i < data.TableIDs.Count; i++)
        {
            SingleData tmp = data[data.TableIDs[i]];

            if (tmp.ContainsKey(key))
            {
                tmp[key] = newContent;
            }
        }

        data.SetDefault(key,newContent);
    }

    void SaveDataGUI()
    {
        if (GUILayout.Button("保存"))
        {
            DataManager.SaveData(m_currentDataName, m_currentData);
            AssetDatabase.Refresh();
            LoadData(m_currentDataName);
        }
    }

    #endregion

    #endregion

    #region FindData

    void FindAllDataName()
    {
        AssetDatabase.Refresh();
        m_dataNameList = new List<string>();

        m_dataNameList.Add("None");

        FindConfigName(Application.dataPath + "/Resources/" + DataManager.c_directoryName  );
    }

    public void FindConfigName(string path)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            
            if (item.EndsWith(".txt"))
            {
                string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                m_dataNameList.Add(configName);
            }
        }

        //string[] dires = Directory.GetDirectories(path);
        //for (int i = 0; i < dires.Length; i++)
        //{
        //    FindConfigName(dires[i]);
        //}
    }

    #endregion


}


