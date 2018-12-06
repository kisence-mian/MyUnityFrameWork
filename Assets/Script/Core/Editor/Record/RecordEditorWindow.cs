using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
public class RecordEditorWindow : EditorWindow
{
    UILayerManager m_UILayerManager;

    [MenuItem("Window/持久数据编辑器 &3", priority = 501)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(RecordEditorWindow));
    }

    List<String> m_recordNameList = new List<string>();

    string m_currentRecordName;
    RecordTable m_currentRecord;

    void OnEnable()
    {
        m_currentSelectIndex = 0;
        EditorGUIStyleData.Init();

        FindAllRecordName();
    }

    void OnGUI()
    {
        titleContent.text = "持久数据编辑器";

        EditorGUILayout.BeginVertical();

        SelectRecordGUI();

        RecordEditorGUI();

        DeleteRecordGUI();

        AddRecordGUI();

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
        FindAllRecordName();
    }

    #region GUI

    #region 数据加载与新增

    int m_currentSelectIndex = 0;
    void SelectRecordGUI()
    {
        string[] mask = m_recordNameList.ToArray();
        m_currentSelectIndex = EditorGUILayout.Popup("当前数据：", m_currentSelectIndex, mask);

        LoadRecord(mask[m_currentSelectIndex]);
    }

    void LoadRecord(string recordName)
    {
        if (m_currentRecordName != recordName)
        {
            m_currentRecordName = recordName;

            if (m_currentRecordName != "None")
                m_currentRecord = RecordManager.GetData(recordName);
        }
    }
    bool isRecordFold;
    string recordName = "";
    void AddRecordGUI()
    {
        EditorGUI.indentLevel = 0;

        isRecordFold = EditorGUILayout.Foldout(isRecordFold, "新增数据");
        
        if (isRecordFold)
        {
            EditorGUI.indentLevel = 1;

            recordName = EditorGUILayout.TextField("数据名", recordName);

            if (!m_recordNameList.Contains(recordName) && recordName != "")
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("新增", GUILayout.Width(position.width - 60)))
                {
                    RecordTable dict = new RecordTable();
                    RecordManager.SaveData(recordName, dict);
                    FindAllRecordName();

                    LoadRecord(recordName);

                    recordName = "";
                }

                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (m_recordNameList.Contains(recordName))
                {
                    EditorGUILayout.LabelField("已存在该数据");
                }
            }
        }

        EditorGUILayout.Space();
    }

    void DeleteRecordGUI()
    {
        if (m_currentRecordName != "None")
        {
            if (GUILayout.Button("删除数据"))
            {
                if (EditorUtility.DisplayDialog("警告", "确定要删除该数据吗！", "是", "取消"))
                {
                    File.Delete(Application.persistentDataPath + "/" + RecordManager.c_directoryName + "/" + m_currentRecordName + ".json");
                    AssetDatabase.Refresh();
                    FindAllRecordName();
                }
            }
        }

        if (GUILayout.Button("清空数据"))
        {
            if (EditorUtility.DisplayDialog("警告", "确定要清空所有持久化数据吗！", "是", "取消"))
            {
                FileTool.DeleteDirectory(Application.persistentDataPath + "/" + RecordManager.c_directoryName);
                FindAllRecordName();
            }
        }
    }

    void CleanCacheGUI()
    {
        if (GUILayout.Button("清空缓存"))
        {
            RecordManager.CleanCache();
        }
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

            if (newType == FieldType.Enum)
            {
                int newIndex = EditorGUILayout.Popup("枚举类型", m_newTypeIndex, EditorTool.GetAllEnumType());

                if (newIndex != m_newTypeIndex)
                {
                    m_newTypeIndex = newIndex;
                    isNewType = true;
                }
            }

            if (content.m_type != newType)
            {
                isNewType = true;
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
                    m_currentRecord.Add(fieldName, content);

                    fieldName = "";
                    content = new SingleField();
                    newType = content.m_type;
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

    void RecordItemGUI(Dictionary<string, SingleField> dict,string key)
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
            EditorGUILayout.LabelField("字段类型：", data.m_type.ToString() + "/" + data.m_enumType);
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
    void RecordEditorGUI()
    {
        if (m_currentRecord != null
            && m_currentRecordName != "None")
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel = 1;
            pos = EditorGUILayout.BeginScrollView(pos,GUILayout.ExpandHeight(false));

            List<string> keys = new List<string>(m_currentRecord.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                RecordItemGUI(m_currentRecord, keys[i]);
            }

            EditorGUILayout.Space();

            AddFieldGUI(m_currentRecord);

            EditorGUILayout.Space();
            if (GUILayout.Button("保存"))
            {
                RecordManager.SaveData(m_currentRecordName, m_currentRecord);
                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndScrollView();
        }
    }

    #endregion

    #endregion

    #region FindRecord

    void FindAllRecordName()
    {
        AssetDatabase.Refresh();
        m_recordNameList = new List<string>();

        m_recordNameList.Add("None");

        FindRecordName(Application.persistentDataPath + "/" + RecordManager.c_directoryName);

        if (m_currentSelectIndex >= m_recordNameList.Count)
        {
            m_currentSelectIndex = m_recordNameList.Count - 1;
        }
    }

    public void FindRecordName(string path)
    {
        FileTool.CreatPath(path);

        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            if (item.EndsWith("." + RecordManager.c_expandName))
            {
                string recordName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                m_recordNameList.Add(recordName);
            }
        }

        //string[] dires = Directory.GetDirectories(path);
        //for (int i = 0; i < dires.Length; i++)
        //{
        //    FindRecordName(dires[i]);
        //}
    }

    #endregion

}


