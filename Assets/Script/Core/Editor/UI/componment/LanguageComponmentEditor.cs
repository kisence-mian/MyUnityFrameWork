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
    string[] m_languageList;
    int m_currentSelectIndex = 0;

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
                EditorGUILayout.LabelField("没有找到Text组件！", EditorGUIStyleData.s_ErrorMessageLabel);
            }
        }

        m_languageList = LanguageDataEditorWindow.GetLanguageKeyList().ToArray();

        if (m_currentSelectIndex == 0 && m_lc.m_text != null)
        {
            m_currentSelectIndex = GetIndex(m_lc.m_text.text);
        }

        m_currentSelectIndex = EditorGUILayout.Popup("当前内容：", m_currentSelectIndex, m_languageList);
        m_lc.m_languageID = m_languageList[m_currentSelectIndex];


        base.OnInspectorGUI();
    }

    public int GetIndex(string content)
    {
        for (int i = 0; i < m_languageList.Length; i++)
        {
            if(m_languageList[i] == content)
            {
                return i;
            }
        }

        return 0;
    }

}
