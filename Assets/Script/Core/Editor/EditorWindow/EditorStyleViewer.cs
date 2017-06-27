using UnityEngine;
using System.Collections;
using UnityEditor;
public class EditorStyleViewer : EditorWindow
{
    Vector2 scrollPosition = new Vector2(0, 0);
    string search = "";
    [MenuItem("Window/GUIStyle查看器",priority = 1101)]
    public static void Init()
    {
        EditorWindow.GetWindow(typeof(EditorStyleViewer));
    }
    private int toolbarOption = 0;
    private string[] toolbarTexts = { "全部GUIStyle", "搜索" };
    void OnGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));
        switch (toolbarOption)
        {
            case 0:
                GUILayout.Space(5);
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                foreach (GUIStyle style in GUI.skin.customStyles)
                {
                        ShowStyleGUI(style);                   
                }
                break;
            case 1:
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.Label("Click a right button to copy its Name to your Clipboard", "MiniBoldLabel");
                GUILayout.FlexibleSpace();
                GUILayout.Label("Search:");
                search = EditorGUILayout.TextField(search);

                GUILayout.EndHorizontal();
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                foreach (GUIStyle style in GUI.skin.customStyles)
                {
                    if (style.name.ToLower().Contains(search.ToLower()))
                    {
                        ShowStyleGUI(style);
                    }
                }
                break;

                
        }
        GUILayout.EndScrollView();

    }

    void ShowStyleGUI(GUIStyle style)
    {
        GUILayout.BeginHorizontal("box");

        GUILayout.Space(40);
        EditorGUILayout.SelectableLabel(style.name, style);
        GUILayout.Space(40);

        EditorGUILayout.SelectableLabel("", style,GUILayout.Height(40),GUILayout.Width(40));

        GUILayout.Space(40);

        GUILayout.FlexibleSpace();
        EditorGUILayout.SelectableLabel(style.name);
        GUILayout.Space(6);
        if (GUILayout.Button("复制到剪贴板"))
        {
           //  EditorGUIUtility.systemCopyBuffer = style.name;
            TextEditor tx = new TextEditor();
            tx.text = style.name;
            tx.OnFocus();
            tx.Copy();
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(11);
    }
}