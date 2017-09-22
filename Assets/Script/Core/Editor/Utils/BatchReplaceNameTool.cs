using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class BatchReplaceNameTool : EditorWindow
{
    [MenuItem("Tools/批量修改名称")]

    public static void ShowWindow()
    {
        GetWindow(typeof(BatchReplaceNameTool));
    }

    bool selectChild = false;
    string m_content = "";
    string m_replace = "";
    Object[] selects;
    List<GameObject> selectList = new List<GameObject>();
    Vector3 pos = Vector3.zero;

    Vector3 pos2 = Vector3.zero;

    void OnGUI()
    {
        titleContent.text = "批量修改名称";

        pos = GUILayout.BeginScrollView(pos);

        selectChild = EditorGUILayout.Toggle("包括选中子节点", selectChild);
        EditorGUILayout.LabelField("已选列表：");
        EditorGUI.indentLevel++;

        for (int i = 0; i < selectList.Count; i++)
        {
            EditorGUILayout.ObjectField(selectList[i], typeof(Object),true);
        }

        EditorGUI.indentLevel--;
        GUILayout.EndScrollView();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("预览：");
        EditorGUI.indentLevel++;

        pos2 = GUILayout.BeginScrollView(pos2);

        for (int i = 0; i < selectList.Count; i++)
        {
            string tmp = selectList[i].name;

            if (m_content != "")
            {
                tmp = tmp.Replace(m_content, m_replace);
            }

            EditorGUILayout.LabelField(tmp);
        }
        GUILayout.EndScrollView();
        EditorGUI.indentLevel--;


        m_content = EditorGUILayout.TextField("replace content:", m_content);
        m_replace = EditorGUILayout.TextField("replace to:", m_replace);

        EditorGUILayout.Space();

        if (GUILayout.Button("Repalce!"))
        {
            if (m_content != "")
            {
                ChangeName(selectList, m_content, m_replace);
            }
        }
    }

    private void Update()
    {
        selects = Selection.GetFiltered(typeof(GameObject), SelectionMode.Unfiltered);

        selectList.Clear();
        for (int i = 0; i < selects.Length; i++)
        {
            if(selectChild)
            {
                GameObject go = (GameObject)selects[i];
                AddChild(go);
            }
            else
            {
                selectList.Add((GameObject)selects[i]);
            }
        }

        Repaint();
    }

    void AddChild(GameObject go)
    {
        if(!selectList.Contains(go))
        {
            selectList.Add(go);

            foreach (Transform tf in go.transform)
            {
                AddChild(tf.gameObject);
            }
        }
    }

    void ChangeName(List<GameObject> list, string newName,string replaceTo)
    {
        Undo.RecordObjects(list.ToArray(), "ReplaceName->" + newName);
        for (int i = 0; i < list.Count; i++)
        {
            string tmp = list[i].name;

            tmp = tmp.Replace(newName, replaceTo);
            list[i].name = tmp;
        }
    }
}
