using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System;
using System.Collections.Generic;


	/// <summary>
	/// 控制台GUI输出类
	/// 包括FPS，内存使用情况，日志GUI输出
	/// </summary>
public class GUIConsole
{

    struct ConsoleMessage
    {
        public readonly string message;
        public readonly string stackTrace;
        public readonly LogType type;

        public ConsoleMessage(string message, string stackTrace, LogType type)
        {
            this.message = message;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }

    /// <summary>
    /// Update回调
    /// </summary>
    public delegate void OnUpdateCallback();
    /// <summary>
    /// OnGUI回调
    /// </summary>
    public delegate void OnGUICallback();

    static public OnUpdateCallback onUpdateCallback = null;
    static public OnGUICallback onGUICallback = null;
    /// <summary>
    /// FPS计数器
    /// </summary>
    static private FPSCounter fpsCounter = null;
    /// <summary>
    /// 内存监视器
    /// </summary>
    static private MemoryDetector memoryDetector = null;
    static private bool showGUI = false;
    static List<ConsoleMessage> entries = new List<ConsoleMessage>();
    static Vector2 scrollPos;
    static bool scrollToBottom = true;
    static bool collapse;
    static bool mTouching = false;

    const int margin = 20;
    static Rect windowRect = new Rect(margin + Screen.width * 0.5f, margin, Screen.width * 0.5f - (2 * margin), Screen.height - (2 * margin));

    static GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    static GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
    static GUIContent scrollToBottomLabel = new GUIContent("ScrollToBottom", "Scroll bar always at bottom");


    public static void Init()
    {
        fpsCounter = new FPSCounter();
        memoryDetector = new MemoryDetector();
        //        this.showGUI = App.Instance().showLogOnGUI;
        ApplicationManager.s_OnApplicationUpdate += Update;
        ApplicationManager.s_OnApplicationOnGUI += OnGUI;
        Application.logMessageReceived += HandleLog;
    }

    ~GUIConsole()
    {
        Application.logMessageReceived -= HandleLog;
    }


    static void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.F1))
            showGUI = !showGUI;
#elif UNITY_ANDROID || UNITY_IOS
			if (!mTouching && Input.touchCount >= 3)
			{
				mTouching = true;
				showGUI = !showGUI;
			} else if (Input.touchCount == 0){
				mTouching = false;
			}
#endif

        if (onUpdateCallback != null)
            onUpdateCallback();
    }

    static void OnGUI()
    {
        if (!showGUI)
            return;

        if (onGUICallback != null)
            onGUICallback();

        //if (GUI.Button (new Rect (100, 100, 200, 100), "清空数据")) {
        //    PlayerPrefs.DeleteAll ();
        //    #if UNITY_EDITOR
        //    EditorApplication.isPlaying = false;
        //    #else
        //    Application.Quit();
        //    #endif
        //}
        windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
    }


    /// <summary>
    /// A window displaying the logged messages.
    /// </summary>
    static void ConsoleWindow(int windowID)
    {
        if (scrollToBottom)
        {
            GUILayout.BeginScrollView(Vector2.up * entries.Count * 100.0f);
        }
        else
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
        }
        // Go through each logged entry
        for (int i = 0; i < entries.Count; i++)
        {
            ConsoleMessage entry = entries[i];
            // If this message is the same as the last one and the collapse feature is chosen, skip it
            if (collapse && i > 0 && entry.message == entries[i - 1].message)
            {
                continue;
            }
            // Change the text colour according to the log type
            switch (entry.type)
            {
                case LogType.Error:
                case LogType.Exception:
                    GUI.contentColor = Color.red;
                    break;
                case LogType.Warning:
                    GUI.contentColor = Color.yellow;
                    break;
                default:
                    GUI.contentColor = Color.white;
                    break;
            }
            if (entry.type == LogType.Exception)
            {
                GUILayout.Label(entry.message + " || " + entry.stackTrace);
            }
            else
            {
                GUILayout.Label(entry.message);
            }
        }
        GUI.contentColor = Color.white;
        GUILayout.EndScrollView();
        GUILayout.BeginHorizontal();
        // Clear button
        if (GUILayout.Button(clearLabel))
        {
            entries.Clear();
        }
        // Collapse toggle
        collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
        scrollToBottom = GUILayout.Toggle(scrollToBottom, scrollToBottomLabel, GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();
        // Set the window to be draggable by the top title bar
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }

    static void HandleLog(string message, string stackTrace, LogType type)
    {
        ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
        entries.Add(entry);
    }
}
