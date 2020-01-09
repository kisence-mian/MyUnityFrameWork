using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

[CustomEditor(typeof(LanguageComponent))]
public class LanguageComponmentEditor : Editor
{
    LanguageComponent m_lc;
    List<string> m_languageList;
   
    public override void OnInspectorGUI()
    {
        if (m_lc == null)
        {
            m_lc = (LanguageComponent)target;
        }

        if (m_lc.m_text == null)
        {
            m_lc.m_text = m_lc.GetComponent<Text>();
            if (m_lc.m_text == null)
            {
                EditorGUILayout.LabelField("没有找到Text组件！", EditorGUIStyleData.ErrorMessageLabel);
                return;
            }
        }
        if (m_languageList == null)
            m_languageList = LanguageDataEditorUtils.GetLanguageAllFunKeyList();
        GUILayout.Space(6);
        m_lc.languageKey = EditorDrawGUIUtil.DrawBaseValue("多语言key", m_lc.languageKey).ToString();
        GUILayout.Space(6);
        m_lc.languageKey = EditorDrawGUIUtil.DrawPopup("多语言key", m_lc.languageKey, m_languageList);
        m_lc.ResetLanguage();
        GUILayout.Space(8);
        if (GUILayout.Button("刷新多语言key"))
        {
            m_languageList = LanguageDataEditorUtils.GetLanguageAllFunKeyList();
            m_lc.ResetLanguage();
        }

        if (GUILayout.Button("打开编辑"))
        {
            LanguageDataEditorWindow.OpenAndSearchValue(m_lc.languageKey);
        }
    }

}
