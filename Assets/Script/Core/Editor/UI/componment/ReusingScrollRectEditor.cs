using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ReusingScrollRect))]
public class ReusingScrollRectEditor : ScrollRectEditor
{
    ReusingScrollRect m_rsr;

    public override void OnInspectorGUI()
    {
        m_rsr = (ReusingScrollRect)target;

        m_rsr.m_isInversion = EditorGUILayout.Toggle("IsInversion: ", m_rsr.m_isInversion);

        base.OnInspectorGUI();
    }
}
