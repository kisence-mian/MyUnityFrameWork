using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIStyleComponent), true)]
public class UIStyleComponentEditor : Editor 
{
    public string m_StyleName = "";
    public string m_createStyleName = "";
    public int m_currentStyle = 0;
    public UIStyleComponent comp;
    string[] styleList;

    public override void OnInspectorGUI()
    {
        comp = (UIStyleComponent)target;
        styleList = UIStyleConfigManager.GetUIStyleList();

        m_currentStyle = GetStyleID();
        m_currentStyle = EditorGUILayout.Popup("当前 UIStyle:",m_currentStyle, styleList);

        comp.m_styleID = styleList[m_currentStyle];

        m_StyleName = styleList[m_currentStyle];

        GUILayout.Space(15);

        if (m_StyleName != "None"   )
        {
            if (GUILayout.Button("套用Stlye模板"))
            {
                ApplyStyle(UIStyleConfigManager.GetData(m_StyleName));
            }

            if (GUILayout.Button("覆盖Stlye模板"))
            {
                if (EditorUtility.DisplayDialog("警告", "该模板已存在，是否覆盖？", "是", "否"))
                {
                    ReplaceStyle(UIStyleConfigManager.GetData(m_StyleName));
                }
            }
        }
        else
        {
            EditorGUILayout.BeginHorizontal();

            m_createStyleName = EditorGUILayout.TextField("创建 Style Name:", m_createStyleName);
            if(UIStyleConfigManager.GetData( m_createStyleName) != null)
            {
                if (GUILayout.Button("覆盖Stlye模板"))
                {
                    if (EditorUtility.DisplayDialog("警告", "该模板已存在，是否覆盖？", "是", "否"))
                    {
                        ReplaceStyle(UIStyleConfigManager.GetData(m_StyleName));
                    }
                }
            }
            else
            {
                if (GUILayout.Button("以此UI为模板创建UIStyle"))
                {
                    CreatStyle(m_createStyleName);
                    m_createStyleName = "";
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public void CreatStyle(string UIstyleName)
    {
        UIStyleInfo styleTmp = new UIStyleInfo();
        styleTmp.GetStyle(comp.gameObject);

        styleTmp.m_StyleInfoName = UIstyleName;

        UIStyleConfigManager.AddData(styleTmp.m_StyleInfoName, styleTmp);

        m_StyleName = styleTmp.m_StyleInfoName;

        m_currentStyle = UIStyleConfigManager.GetUIStyleList().Length - 1;
    }

    public void ApplyStyle(UIStyleInfo l_styleInfo)
    {
        l_styleInfo.ApplyStyle(comp.gameObject);
    }

    public void ReplaceStyle(UIStyleInfo l_styleInfo)
    {
        l_styleInfo = l_styleInfo.GetStyle(comp.gameObject);
    }

    int GetStyleID()
    {
        for (int i = 0; i < styleList.Length; i++)
        {
            if(comp.m_styleID.Equals(styleList[i]))
            {
                return i;
            }
        }

        return 0;
    }
}
