using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

[CustomEditor(typeof(ApplicationManager))]
public class ApplicationManagerComponentEditor : Editor 
{
    ApplicationManager m_app;

    int m_currentSelectIndex = 0;
    string[] m_statusList;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        m_app = (ApplicationManager)target;

        m_statusList = GetStatusList();
        m_currentSelectIndex = GetStatusIndex();
        //m_app.showLanguageValue = EditorGUILayout.Toggle("Show Language Value", m_app.showLanguageValue);

        m_currentSelectIndex = EditorGUILayout.Popup("First Status:", m_currentSelectIndex, m_statusList);

        m_app.m_Status = m_statusList[m_currentSelectIndex];
        m_app.m_globalLogic = GetGlobaLogic();
        if (Application.isPlaying)
            GUILayout.Label("CurrentStatus: " + m_app.currentStatus);
        GUILayout.Space(10);
        if (m_app.m_globalLogic.Count != 0)
        {
            GUILayout.Label("Global Logic:");
            //EditorGUI.indentLevel = 2;
            for (int i = 0; i < m_app.m_globalLogic.Count; i++)
            {
                GUILayout.Label("  " + m_app.m_globalLogic[i]);
            }
        }
    }
   

    public string[] GetStatusList()
    {
        List<string> listTmp = new List<string>();

        Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(typeof(IApplicationStatus)))
            {
                listTmp.Add(types[i].Name);
            }
        }

        if (listTmp.Count == 0)
        {
            listTmp.Add("None");
        }

        return listTmp.ToArray();
    }

    public List<string> GetGlobaLogic()
    {
        List<string> listTmp = new List<string>();

        Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(typeof(IApplicationGlobalLogic)))
            {
                listTmp.Add(types[i].Name);
            }
        }
        return listTmp;
    }

    public int GetStatusIndex()
    {
        for (int i = 0; i < m_statusList.Length; i++)
        {
            if (m_app.m_Status.Equals(m_statusList[i]))
            {
                return i;
            }
        }
        return 0;
    }
}
