using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(UIWindowBase),true)]
public class UIWindowBaseComponmentEditor : Editor
{
    UIWindowBase m_ui;
    Canvas canvas;
    string[] list;
    int selectIndex = 0;

    public override void OnInspectorGUI()
    {

        if (m_ui == null)
        {
            m_ui = (UIWindowBase)target;
        }
        if (canvas == null)
        {
            canvas = m_ui.gameObject.GetComponent<Canvas>();
        }

        list = UIManager.GetCameraNames();
        selectIndex = GetIndex(m_ui.cameraKey);

        selectIndex = EditorGUILayout.Popup("Camera Key", selectIndex, list);
        if (list.Length != 0)
            m_ui.cameraKey = list[selectIndex];

        base.OnInspectorGUI();
        if (!Application.isPlaying && GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkAllScenesDirty();
            canvas.sortingLayerName = m_ui.m_UIType.ToString();
        }
    }

    public int GetIndex(string current)
    {
        if (string.IsNullOrEmpty(current))
            return 0;
        for (int i = 0; i < list.Length; i++)
        {
            if (current.Equals(list[i]))
            {
                return i;
            }
        }
        return 0;
    }
}
