using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityRemoteConsole;
using System.Text;
using System.IO;
using System;
using LiteNetLibManager;

public class UnityRemoteConsoleSettingEditorWindow : EditorWindow
{

    [MenuItem("Window/UnityRemoteConsole/Setting", priority = 1002)]
    private static void OpenWindow()
    {
        UnityRemoteConsoleSettingEditorWindow win = GetWindow<UnityRemoteConsoleSettingEditorWindow>();
        win.autoRepaintOnSceneChange = true;
        win.wantsMouseMove = true;
        EditorWindow.FocusWindowIfItsOpen<UnityRemoteConsoleSettingEditorWindow>();
        win.Init();
    }
    private UnityRemoteConsoleSettingData config;
    private void OnEnable()
    {
        if (config == null)
            Init();
    }
    private void Init()
    {
        config = UnityRemoteConsoleSettingData.GetCofig();

        modifyPassword = false;
    }
    private void OnGUI()
    {
        int port = EditorGUILayout.IntField("Connect Port(1024-65535)", config.netPort);
        if (port >= 1024 && port <= 65535)
            config.netPort = port;

        GUILayout.Space(8);
        config.autoBoot = GUILayout.Toggle(config.autoBoot, "Auto Boot");

        LoginAccountGUI();

        SaveGUI();
       
    }

    private bool modifyPassword = false;
    private string pw = "";
    private string pwVerify = "";
    private void LoginAccountGUI()
    {
        GUILayout.Space(5);
        GUILayout.Box("Login Account");
        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
       // GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        GUILayout.Label("Key");
        config.loginKey = GUILayout.TextField(config.loginKey);
        GUILayout.Space(2);
        modifyPassword= EditorGUILayout.Toggle("Modify Password", modifyPassword);
        if (modifyPassword)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("password", GUILayout.Width(90));
            pw = EditorGUILayout.PasswordField(pw,GUILayout.Width(160));
            GUILayout.Label(">5 length");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("password again", GUILayout.Width(90));
            if (!string.IsNullOrEmpty(pwVerify)&&!string.IsNullOrEmpty(pw) && pw != pwVerify)
            {
                GUI.color = Color.red;
            }
            pwVerify = EditorGUILayout.PasswordField(pwVerify,GUILayout.Width(160));
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

           

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!string.IsNullOrEmpty(pwVerify) 
                && !string.IsNullOrEmpty(pw) 
                && pw == pwVerify 
                &&GUILayout.Button("Ok",GUILayout.Width(90)))
            {
                if (string.IsNullOrEmpty(pw))
                {
                    ShowNotification(new GUIContent("The password cannot be empty!"));
                    return;
                }
                if (pw.Length < 6)
                {
                    ShowNotification(new GUIContent("The password must be longer than 5!"));
                    return;
                }
                config.loginPassword = MD5Tool.GetObjectMD5(pw);
                Debug.Log(config.loginPassword);
                modifyPassword = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();


    }

    private void SaveGUI()
    {
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save", GUILayout.Width(120)))
        {
            if (modifyPassword)
            {
                ShowNotification(new GUIContent("Please save the password first!"));
                return;
            }
            UnityRemoteConsoleSettingData.SaveConfig(config);
            AssetDatabase.Refresh();

            ShowNotification(new GUIContent("Save Success!"));
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(8);
    }
}
