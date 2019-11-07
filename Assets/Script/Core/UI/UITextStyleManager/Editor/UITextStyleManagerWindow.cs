using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UITextStyleManagerWindow : EditorWindow {

    private static UITextStyleManagerWindow win;
    private  UITextStyleComponent component;
    
    public static void OpenWindow(UITextStyleComponent component)
    {
       

        win = GetWindow<UITextStyleManagerWindow>();
        win.component = component;
        win.Init();
    }

    private Dictionary<string, Dictionary<SystemLanguage, TextStyleData>> styleDataDic;
    private Text text;
    TextStyleData oldData;
    private void Init()
    {
        text = component.GetComponent<Text>();
        UITextStyleManager.Init();
        styleDataDic = UITextStyleManager.styleDataDic;
        oldData = UITextStyleManager.GetTextStyleDataFromText(text);
    }

    private string tmpName="";
    private SystemLanguage language;
    private void OnGUI()
    {
        EditorGUILayout.ObjectField(text,typeof(Text),true);
        GUILayout.Space(5);
        tmpName = EditorDrawGUIUtil.DrawBaseValue("Name",tmpName).ToString();
        language = (SystemLanguage)EditorDrawGUIUtil.DrawBaseValue("Language", language);
        if (GUILayout.Button("添加"))
        {
            if (string.IsNullOrEmpty(tmpName))
                return;
            if (UITextStyleManager.ContainsData(tmpName, language))
                return;
            else
            {
                Dictionary<SystemLanguage, TextStyleData> data=null;
                if (styleDataDic.ContainsKey(tmpName))
                {
                    data = styleDataDic[tmpName];
                }
                else
                {
                    data = new Dictionary<SystemLanguage, TextStyleData>();
                    styleDataDic.Add(tmpName, data);
                }
                TextStyleData sd = UITextStyleManager.GetTextStyleDataFromText(text);
                data.Add(language, sd);
            }
        }
        GUILayout.Space(6);

            EditorDrawGUIUtil.DrawScrollView(this, () =>
             {
                 foreach (var item in styleDataDic)
                 {
                     GUILayout.BeginHorizontal();
                     GUILayout.Box(item.Key);
                     GUILayout.FlexibleSpace();
                     if (GUILayout.Button("-"))
                     {
                         styleDataDic.Remove(item.Key);
                         return;
                     }
                     GUILayout.EndHorizontal();
                     foreach (var d in item.Value)
                     {
                         GUILayout.BeginHorizontal();
                         EditorDrawGUIUtil.DrawFoldout(d.Key, d.Key.ToString(), () =>
                           {
                               GUILayout.BeginVertical();
                               EditorDrawGUIUtil.DrawClassData("", d.Value);
                               if (GUILayout.Button("Show"))
                               {
                                   UITextStyleManager.SetText(text, item.Key, d.Key);
                               }
                               GUILayout.EndVertical();
                           });
                         GUILayout.FlexibleSpace();
                         if (GUILayout.Button("-"))
                         {
                             styleDataDic[item.Key].Remove(d.Key);
                             return;
                         }
                         GUILayout.EndHorizontal();
                     }
                 }
             });
       // EditorDrawGUIUtil.DrawDictionary("", styleDataDic);

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save"))
        {
            UITextStyleManager.SaveData(styleDataDic);
        }
        GUILayout.Space(5);
    }

    private void OnDestroy()
    {
        UITextStyleManager.SetText(text, oldData);
    }
}
