using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameDataMonitorEditorWindow : EditorWindow
{

    [MenuItem("Window/游戏数据查看器", priority = 400)]
    public static void ShowWindow()
    {
        GameDataMonitorEditorWindow win= GetWindow<GameDataMonitorEditorWindow>();
        win.autoRepaintOnSceneChange = true;
        
    }
    
    void OnGUI()
    {
        titleContent.text = "游戏数据查看器";

        if(Application.isPlaying)
        {
            ViewGUI();
        }
        else
        {
            DescriptionGUI();
        }
    }

    #region 使用介绍

    void DescriptionGUI()
    {
        EditorGUILayout.TextArea("用法介绍：\n可以将游戏中的数据可视化的显示出来（类只显示public的字段)\nAPI:\nGameDataMonitor.PushData(string key,object obj)\nGameDataMonitor.RemoveData(string string)");
    }

    #endregion

    #region View

    Vector2 m_scrollPos = new Vector2();
    //Dictionary<string, bool> m_foldDict = new Dictionary<string, bool>();
    void ViewGUI()
    {
        GUILayout.Space(6);
        m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);

        foreach(var obj in GameDataMonitor.GameData)
        {
            EditorDrawGUIUtil.DrawFoldout(obj.Value, obj.Key+":"+obj.Value.description, () =>
            {
                EditorDrawGUIUtil.DrawClassData(obj.Key,obj.Value.showValue);
            });
           // ViewItemGUI(obj.Key,obj.Value);
        }

        GUILayout.EndScrollView();
    }

    void ViewItemGUI(string key,object obj)
    {
        //if (!m_foldDict.ContainsKey(key))
        //{
        //    m_foldDict.Add(key,false);
        //}

        //m_foldDict[key] = EditorGUILayout.Foldout(m_foldDict[key], key + ":");

        //if (m_foldDict[key])

       
        //{
        //    EditorGUI.indentLevel++;
        //    EditorUtilGUI.DrawClassData(obj, key);

        //    EditorGUI.indentLevel--;
        //}
    }

    #endregion
}
