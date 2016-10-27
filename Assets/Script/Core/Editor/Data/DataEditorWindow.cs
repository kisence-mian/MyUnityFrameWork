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

    void OnGUI()
    {
        titleContent.text = "数据编辑器";

        EditorGUILayout.BeginVertical();

        SelectDataGUI();

        DataGUI();

        DeleteDataGUI();

        AddConfigGUI();


        EditorGUILayout.EndVertical();
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

    #region 配置加载与新增

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
    string configName = "";
    void AddConfigGUI()
    {
        EditorGUI.indentLevel = 0;

        isConfigFold = EditorGUILayout.Foldout(isConfigFold, "新增配置");
        
        if (isConfigFold)
        {
            EditorGUI.indentLevel = 1;

            configName = EditorGUILayout.TextField("配置名", configName);

            if (!m_dataNameList.Contains(configName) && configName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    Dictionary<string,SingleField> dict = new Dictionary<string,SingleField>();
                    ConfigManager.SaveData(configName, dict);

                    LoadData(configName);

                    configName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (m_dataNameList.Contains(configName))
                {
                    EditorGUILayout.LabelField("已存在该配置");
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

    #region 字段修改与新增

    bool isFold = false;
    string fieldName = "";
    FieldType newType;
    SingleField content = new SingleField();
    void AddDataGUI(DataTable dict)
    {
        EditorGUI.indentLevel = 1;
        isFold = EditorGUILayout.Foldout(isFold,"新增数据");
        if(isFold)
        {
            //EditorGUI.indentLevel = 2;

            //fieldName = EditorGUILayout.TextField("字段名",fieldName);
            //newType = (FieldType)EditorGUILayout.EnumPopup("字段类型", content.m_type);

            //if (content.m_type != newType)
            //{
            //    content.m_type = newType;
            //    content.Reset();
            //}

            //content.m_content = EditorUtilGUI.SingleFieldGUI(content);

            //if (!dict.ContainsKey(fieldName) && fieldName != "")
            //{
            //    EditorGUILayout.BeginHorizontal();
            //    EditorGUILayout.Space();

            //    if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
            //    {
            //        m_currentData.Add(fieldName, content.m_content);

            //        fieldName = "";
            //        content = new SingleField();
            //        newType = content.m_type;
            //    }

            //    EditorGUILayout.Space();
            //    EditorGUILayout.EndHorizontal();
            //}
            //else
            //{
            //    if (dict.ContainsKey(fieldName))
            //    {
            //        EditorGUILayout.LabelField("已存在该字段");
            //    }
            //}
        }
    }

    void DataItemGUI(DataTable table,string key)
    {
        if (!m_foldList.ContainsKey(key))
        {
            m_foldList.Add(key, false);
        }

        m_foldList[key] = EditorGUILayout.Foldout(m_foldList[key], key);

        EditorGUI.indentLevel = 2;

        if (m_foldList[key])
        {

            SingleData data = table[key];

            List<string> keys = new List<string>(data.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                EditorGUILayout.LabelField("字段名",table.TableKeys[i]);

                string newContent = DataFieldGUI(table.GetFieldType(keys[i]), data[keys[i]]);

                if (newContent != data[keys[i]])
                {
                    data[keys[i]] = newContent;
                }

                EditorGUILayout.Space();
            }
        }

    }

    string DataFieldGUI(FieldType type,string content)
    {
        SingleField data = new SingleField(type, content);
        FieldGUI(data);

        return data.m_content;
    }

    void FieldGUI(SingleField data)
    {
        string newContent = "";
        EditorGUILayout.LabelField("字段类型：", data.m_type.ToString());

        newContent = EditorUtilGUI.SingleFieldGUI(data);

        if (data.GetString() != newContent)
        {
            data.m_content = newContent;
        }
    }

    Vector2 pos = Vector3.zero;
    void DataGUI()
    {
        if (m_currentData != null
            && m_currentDataName != "None")
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel = 1;
            pos = EditorGUILayout.BeginScrollView(pos,GUILayout.ExpandHeight(false));

            List<string> keys = m_currentData.TableIDs;

            for (int i = 0; i < keys.Count; i++)
            {
                EditorGUI.indentLevel = 1;
                DataItemGUI(m_currentData, keys[i]);
            }

            EditorGUILayout.Space();

            AddDataGUI(m_currentData);

            EditorGUILayout.Space();
            if (GUILayout.Button("保存"))
            {
                DataManager.SaveData(m_currentDataName, m_currentData);
            }

            EditorGUILayout.EndScrollView();
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


