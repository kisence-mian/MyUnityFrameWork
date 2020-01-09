using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class String2MD5Window : EditorWindow
{
    [MenuItem("Window/String转MD5(1003)", priority = 1003)]
    private static void OpenWindow()
    {
        String2MD5Window win = GetWindow<String2MD5Window>();
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
        EditorWindow.FocusWindowIfItsOpen<String2MD5Window>();
        win.Init();
    }

    private void Init()
    {
       
    }

    string inputText = "";
    string resText = "";
    private void OnGUI()
    {
        GUILayout.FlexibleSpace();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
      inputText =  EditorDrawGUIUtil.DrawBaseValue("输入字符串：", inputText).ToString();
        if (GUILayout.Button("转换"))
        {
            resText= MD5Utils.GetObjectMD5(inputText);
        }
        GUILayout.EndHorizontal();

        EditorDrawGUIUtil.DrawBaseValue("MD5：", resText);
        GUILayout.FlexibleSpace();
    }
}
