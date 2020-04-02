using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameConsoleController;
using System.Text;
using System.IO;
using System;

public class GameConsolePanelEditorWindow : EditorWindow
{
   


    [MenuItem("Window/GameConsoleController/ConfigWindow", priority = 1002)]
    private static void OpenWindow()
    {
        GameConsolePanelEditorWindow win = GetWindow<GameConsolePanelEditorWindow>();
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
        EditorWindow.FocusWindowIfItsOpen<GameConsolePanelEditorWindow>();
        win.Init();
    }
    private GameConsolePanelSettingConfig config;
    private void OnEnable()
    {
        if (config == null)
            Init();
    }
    private void Init()
    {
        config = GameConsolePanelSettingConfig.GetCofig();
    }
    private void OnGUI()
    {
        config.netPort = EditorGUILayout.IntField("Port", config.netPort);

        GUILayout.Space(8);
        config.autoBoot = GUILayout.Toggle(config.autoBoot, "Auto Boot");
        GUILayout.Space(5);
        GUILayout.Label("Login Key");
        config.loginKey = GUILayout.TextField( config.loginKey);
        GUILayout.Space(2);
        GUILayout.Label("Login Password");
        config.loginPassword = GUILayout.TextField( config.loginPassword);

      
       // GUILayout.Label("Auto Boot");
       

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Save",GUILayout.Width(150)))
        {
            GameConsolePanelSettingConfig.SaveConfig(config);
            AssetDatabase.Refresh();

            ShowNotification(new GUIContent("Save Success!"));
        }
        GUILayout.Space(8);
    }

  
}
