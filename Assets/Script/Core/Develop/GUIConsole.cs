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
    static public OnGUICallback onGUICloseCallback = null;
    /// <summary>
    /// FPS计数器
    /// </summary>
    static private FPSCounter fpsCounter = null;
    /// <summary>
    /// 内存监视器
    /// </summary>
    //static private MemoryDetector memoryDetector = null;
    static private bool showGUI = false;
    static List<ConsoleMessage> entries = new List<ConsoleMessage>();
    static Vector2 scrollPos;
    //static bool scrollToBottom = true;
    static bool collapse;

#if !UNITY_EDITOR&&UNITY_ANDROID || UNITY_IOS
        static bool mTouching = false;
#endif

    const int margin = 3;
    const int offset = 0;
    static Rect windowRect = new Rect(margin + Screen.width * 0.6f - offset, margin, Screen.width * 0.6f - (2 * margin) + offset, Screen.height - (2 * margin));

    public static void Init()
    {
        fpsCounter = new FPSCounter();
        fpsCounter.Init();
        //memoryDetector = new MemoryDetector();
        //memoryDetector.Init();

        ApplicationManager.s_OnApplicationUpdate += Update;
        ApplicationManager.s_OnApplicationOnGUI += OnGUI;

        Application.logMessageReceivedThreaded += HandleLog;
        //Application.logMessageReceived += HandleLog;
    }

    ~GUIConsole()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
        //Application.logMessageReceived -= HandleLog;
    }

    static void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.F1))
            showGUI = !showGUI;
#elif UNITY_ANDROID || UNITY_IOS
			if (!mTouching && Input.touchCount >= 6)
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
        {
            if(onGUICloseCallback != null)
            {
                onGUICloseCallback();
            }
            return;
        }

        GUIUtil.SetGUIStyle();

        if (onGUICallback != null)
            onGUICallback();

        windowRect = new Rect(margin + Screen.width * 0.2f ,
                                margin,
                                Screen.width * 0.8f - (2 * margin),
                                Screen.height - (2 * margin));

        GUILayout.Window(100, windowRect, ConsoleWindow, "Console");
    }

    static int s_page = 0;
    const int c_perPageShowDebug = 50;

    /// <summary>
    /// A window displaying the logged messages.
    /// </summary>
    static void ConsoleWindow(int windowID)
    {
        //if (scrollToBottom)
        //{
        //    GUILayout.BeginScrollView(Vector2.up * entries.Count * 1000.0f);
        //} 
        //else
        //{
        //    
        //}

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        int startIndex = s_page * c_perPageShowDebug;
        int endIndex = startIndex + c_perPageShowDebug;

        if(endIndex > entries.Count)
        {
            endIndex = entries.Count;
        }

        // Go through each logged entry
        for (int i = startIndex; i < endIndex; i++)
        {
            ConsoleMessage entry = entries[i];

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

        GUILayout.Label("第" + (s_page + 1) + "页 共" + Mathf.Ceil(entries.Count / (float)c_perPageShowDebug) + "页");

        if (s_page > 0)
        {
            if (GUILayout.Button("上一页"))
            {
                s_page--;
            }
        }

        if(entries.Count > (s_page + 1) * c_perPageShowDebug)
        {
            if (GUILayout.Button("下一页"))
            {
                s_page++;
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("首页"))
        {
            s_page = 0;
        }

        if (GUILayout.Button("末页"))
        {
            s_page = entries.Count / c_perPageShowDebug;
        }

        GUILayout.EndHorizontal();

        // Clear button
        if (GUILayout.Button("Clear"))
        {
            entries.Clear();
        }

        //if (GUILayout.Button("Bottom:" + scrollToBottom))
        //{
        //    scrollToBottom = !scrollToBottom;
        //}

    }

    static void HandleLog(string message, string stackTrace, LogType type)
    {
        ConsoleMessage entry = new ConsoleMessage(message, stackTrace, type);
        entries.Add(entry);
    }
}
