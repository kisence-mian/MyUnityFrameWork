using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TableDataEditor2Window : EditorWindow
{
    private static TableDataEditor2Window win;
    [MenuItem("Window/数据编辑器2", priority = 501)]
    public static void ShowWindow()
    {
        win = EditorWindow.GetWindow<TableDataEditor2Window>(false, "数据编辑器2");
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
    }
    TableDataEditor editor = new TableDataEditor();
    private string chooseFileName = "";
    private void OnEnable()
    {
        if (editor == null)
            editor = new TableDataEditor();
        editor.Init(this);

        GlobalEvent.AddEvent(EditorEvent.LanguageDataEditorChange, Refresh);
    }


    private void OnGUI()
    {
        chooseFileName= editor.OnGUI(chooseFileName);
    }

    private void OnDestroy()
    {
        editor.OnDestroy();
        GlobalEvent.RemoveEvent(EditorEvent.LanguageDataEditorChange, Refresh);
    }

    private void Refresh(params object[] args)
    {
        //Debug.Log("LanguageDataEditorChange");
        editor.Init(this);
    }
}
