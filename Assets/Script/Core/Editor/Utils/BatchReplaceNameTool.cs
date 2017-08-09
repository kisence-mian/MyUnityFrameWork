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

    string m_content = "";
    string m_replace = "";
    Object[] selects;
    Vector3 pos = Vector3.zero;

    Vector3 pos2 = Vector3.zero;

    void OnGUI()
    {
        titleContent.text = "批量修改名称";

        pos = GUILayout.BeginScrollView(pos);

        EditorGUILayout.LabelField("已选列表：");
        EditorGUI.indentLevel++;

        for (int i = 0; i < selects.Length; i++)
        {
            EditorGUILayout.ObjectField(selects[i], typeof(Object));
        }

        EditorGUI.indentLevel--;
        GUILayout.EndScrollView();
        EditorGUILayout.Space();

        m_content = EditorGUILayout.TextField("replace content:", m_content);
        m_replace = EditorGUILayout.TextField("replace to:", m_replace);

        EditorGUILayout.Space();

        if(m_content != "")
        {
            EditorGUILayout.LabelField("预览：");
            EditorGUI.indentLevel++;

            pos2 = GUILayout.BeginScrollView(pos2);

            for (int i = 0; i < selects.Length; i++)
            {
                string tmp = selects[i].name;

                tmp = tmp.Replace(m_content, m_replace);

                EditorGUILayout.LabelField(tmp);
            }
            GUILayout.EndScrollView();
            EditorGUI.indentLevel--;

            if (GUILayout.Button("Repalce!"))
            {
                ChangeName(selects, m_content, m_replace);
            }
        }
    }

    private void Update()
    {
        selects = Selection.GetFiltered(typeof(GameObject), SelectionMode.Unfiltered);
        EnterLogic(selects, m_content);

        Repaint();
    }

    void EnterLogic(Object[] objs, string newName)
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Enter");
            ChangeName(objs, newName, m_replace);
        }
    }

    void ChangeName(Object[] objs, string newName,string replaceTo)
    {
        Undo.RecordObjects(objs, "ReplaceName->" + newName);
        for (int i = 0; i < objs.Length; i++)
        {
            string tmp = objs[i].name;

            tmp = tmp.Replace(newName, replaceTo);
            objs[i].name = tmp;
        }
    }
}
