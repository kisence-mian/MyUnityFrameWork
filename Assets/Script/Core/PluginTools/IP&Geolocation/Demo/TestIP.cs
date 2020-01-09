using UnityEngine;
using System.Collections;
using System;

public class TestIP : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Application.logMessageReceived += LogMessageReceived;
    }

    private void LogMessageReceived(string condition, string stackTrace, LogType type)
    {
        logStr += "[" + type + "]" + condition+"\n";
    }

    string showDetail = "";
    private Vector2 pos;

    private string logStr = "";
    void OnGUI()
    {
        GUIStyle style = "box";
        style.fontSize = 35;
        style.alignment = TextAnchor.UpperLeft;
        style.wordWrap = true;
        if (GUILayout.Button("Get IPGeolocation", GUILayout.Height(75),GUILayout.Width(Screen.width)))
        {
            showDetail = "Start Get IP。。。。";
            IPGeolocationManager.GetIPGeolocation((detail) =>
            {
                if (detail == null)
                    showDetail = "Get IP failed!";
                else
                    showDetail = JsonUtils.ToJson(detail);
               // Debug.Log(showDetail);
            });
        }
        if (GUILayout.Button("Clear", GUILayout.Height(75), GUILayout.Width(Screen.width)))
        {
            showDetail = "";
            logStr = "";
        }

          
       pos = GUILayout.BeginScrollView(pos);
        GUILayout.Box(showDetail, style);
        GUILayout.Label(logStr, style);
        GUILayout.EndScrollView();
    }
}
