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

    [MenuItem("Window/数据编辑器")]
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
        m_currentSelectIndex = 0;
        EditorGUIStyleData.Init();

        FindAllDataName();
    }

    //当选择改变时
    void OnSelectionChange()
    {

    }

    //当工程改变时
    void OnProjectChange()
    {
        FindAllDataName();
    }

    #region GUI

    void OnGUI()
    {
        titleContent.text = "数据编辑器";

        EditorGUILayout.BeginVertical();

        SelectDataGUI();

        DataGUI();

        AddDataGUI();

        EditorGUILayout.EndVertical();
    }

    #region 数据文件相关

    int m_currentSelectIndex = 0;
    void SelectDataGUI()
    {
        string[] mask = m_dataNameList.ToArray();
        m_currentSelectIndex = EditorGUILayout.Popup("当前数据：", m_currentSelectIndex, mask);

        LoadData(mask[m_currentSelectIndex]);
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
            FieldType type = m_currentData.GetFieldType(key);

            AddDataPos = EditorGUILayout.BeginScrollView(AddDataPos, GUILayout.ExpandHeight(false));

            EditorGUILayout.LabelField("<主键>字段名", key);
            mianKey = EditorUtilGUI.FieldGUI_TypeValue(FieldType.String, mianKey);

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

            for (int i = 0; i < keys.Count; i++)
            {
                string keyTmp = keys[i];
                FieldType type = table.GetFieldType(keyTmp);

                if (i == 0)
                {
                    EditorGUILayout.LabelField("<主键>字段名", keyTmp);
                    EditorGUILayout.LabelField("字段值", data[keyTmp]);
                }
            }
            EditorDataGUI(table, data);
        }
    }

    SingleData EditorDataGUI(DataTable table, SingleData data)
    {
        List<string> keys = table.TableKeys;
        for (int i = 0; i < keys.Count; i++)
        {
            string keyTmp = keys[i];
            FieldType type = table.GetFieldType(keyTmp);

            if (i != 0)
            {
                if (data.ContainsKey(keyTmp))
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("字段名", keyTmp);

                    if (GUILayout.Button("使用默认值"))
                    {
                        data.Remove(keyTmp);
                        EditorGUILayout.EndHorizontal();

                        continue;
                    }

                    EditorGUILayout.EndHorizontal();

                    string newContent = EditorUtilGUI.FieldGUI_TypeValue(type, data[keyTmp]);

                    if (newContent != data[keyTmp])
                    {
                        data[keyTmp] = newContent;
                    }
                }
                else
                {
                    bool cancelDefault = false;

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("字段名", keyTmp);

                    if (GUILayout.Button("取消默认值"))
                    {
                        cancelDefault = true;
                    }

                    EditorGUILayout.EndHorizontal();

                    string newContent = "";

                    if (table.m_defaultValue.ContainsKey(keyTmp))
                    {
                        newContent = new SingleField(type, table.GetDefault(keyTmp)).m_content;
                    }
                    else
                    {
                        newContent = new SingleField(type, null).m_content;
                    }

                    EditorGUILayout.LabelField("字段类型", type.ToString());
                    EditorGUILayout.LabelField("(默认值)字段内容", new SingleField(type, newContent).GetShowString());

                    if (cancelDefault)
                    {
                        data.Add(keyTmp, newContent);
                    }
                }
            }

            EditorGUILayout.Space();
        }

        return data;
    }

    #endregion

    #region 字段相关

    bool isEditorFold = false;
    FieldType newType;
    Vector2 EditorPos = Vector2.zero;
    void EditorDataGUI()
    {
        isEditorFold = EditorGUILayout.Foldout(isEditorFold, "编辑数据");
        EditorGUI.indentLevel ++;

        if (isEditorFold)
        {
            List<string> keys = m_currentData.TableKeys;
            EditorPos = EditorGUILayout.BeginScrollView(EditorPos, GUILayout.ExpandHeight(false));
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                FieldType type = m_currentData.GetFieldType(key);

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


                    newType = (FieldType)EditorGUILayout.EnumPopup("字段类型", m_currentData.GetFieldType(keys[i]));

                    if (type != newType)
                    {
                        //弹出警告并重置数据
                        if (EditorUtility.DisplayDialog("警告", "改变字段类型会重置该字段的所有数据和默认值\n是否继续？", "是", "取消"))
                        {
                             m_currentData.SetFieldType(key, newType);
                             ResetDataField(m_currentData,key, newType);

                             type = newType;
                             content = new SingleData();
                        }
                    }

                    string newContent = EditorUtilGUI.FieldGUI_Type(type, m_currentData.GetDefault(key),"默认值");
                    m_currentData.SetDefault(key,newContent);
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

    bool isAddFoldField = false;
    string newFieldName = "";
    FieldType newAddType = FieldType.String;
    string newFieldDefaultValue = "";
    void AddFieldGUI()
    {
        isAddFoldField = EditorGUILayout.Foldout(isAddFoldField, "新增字段");
        EditorGUI.indentLevel ++;

        if (isAddFoldField)
        {
            newFieldName = EditorGUILayout.TextField("字段名", newFieldName);
            FieldType typeTmp = (FieldType)EditorGUILayout.EnumPopup("字段类型", newAddType);

            if (typeTmp != newAddType)
            {
                newAddType = typeTmp;
                newFieldDefaultValue = new SingleField(newAddType, null).m_content;
            }

            bool isShowButton = true;

            if (newFieldName == "")
            {
                isShowButton = false;
            }

            if (m_currentData.TableKeys.Contains(newFieldName))
            {
                isShowButton = false;
                EditorGUILayout.TextField("字段名不能重复！",EditorGUIStyleData.s_WarnMessageLabel);
            }

            newFieldDefaultValue = EditorUtilGUI.FieldGUI_Type(newAddType, newFieldDefaultValue, "默认值");

            if (isShowButton)
            {
                if (GUILayout.Button("新增字段"))
                {
                    AddField(m_currentData, newFieldName, newAddType, newFieldDefaultValue);

                    newFieldName = "";
                    newFieldDefaultValue = "";
                    newAddType = FieldType.String;
                }
            }

        }
    }

    void AddField(DataTable table, string fieldName,FieldType type ,string value)
    {
        table.TableKeys.Add(fieldName);
        table.SetFieldType(fieldName, type);
        table.SetDefault(fieldName, value);
    }

    void ResetDataField(DataTable data,string key,FieldType type)
    {
        string newContent = new SingleField(type,null).m_content;

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


