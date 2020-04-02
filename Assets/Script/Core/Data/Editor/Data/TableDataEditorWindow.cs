using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TableDataEditorWindow : EditorWindow
{
    private static TableDataEditorWindow win;
    [MenuItem("Window/数据编辑器1 &2", priority = 501)]
    public static void ShowWindow()
    {
        win = EditorWindow.GetWindow<TableDataEditorWindow>(false, "数据编辑器1");
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
        editor.Init(this);
    }

}
