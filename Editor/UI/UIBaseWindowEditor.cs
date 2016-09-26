using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIWindowBase),true)]
public class UIBaseWindowEditor : Editor 
{
    public string m_StyleName = "";
    public int m_currentStyle = 0;

    public override void OnInspectorGUI()
    {
        //string[] a = UIStyleConfigManager.GetUIStyleList();
        //m_currentStyle = EditorGUILayout.Popup("当前 UIStyle:",m_currentStyle, a);

        //if (GUILayout.Button("生成Stlye模板"))
        //{
        //    CreatStyleTmp();
        //}

        //if (GUILayout.Button("套用Stlye模板"))
        //{
        //    if (m_StyleName!= "")
        //    {
        //        AppStyleTmp(UIStyleConfigManager.GetData(m_StyleName));
        //    }
        //}
        base.OnInspectorGUI();
    }

    public void CreatStyleTmp()
    {
        UIStyleInfo styleTmp = new UIStyleInfo();
        styleTmp.GetStyle(((UIWindowBase)target).gameObject);

        UIStyleConfigManager.AddData(styleTmp.m_StyleInfoName, styleTmp);

        m_StyleName = styleTmp.m_StyleInfoName;
    }

    public void AppStyleTmp(UIStyleInfo l_styleInfo)
    {
        //l_styleInfo.ApplyStyle(((UIWindowBase)target).gameObject);
    }
}
