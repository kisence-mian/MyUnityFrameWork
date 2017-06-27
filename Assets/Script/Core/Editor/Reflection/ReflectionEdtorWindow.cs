using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ReflectionEdtorWindow : EditorWindow
{
    [MenuItem("Window/反射查看器",priority =1100)]
    public static void ShowWindow()
    {
        GetWindow(typeof(ReflectionEdtorWindow));
    }

    void OnEnable()
    {
        EditorGUIStyleData.Init();
    }

    void OnGUI()
    {
        titleContent.text = "反射查看器";

        MainGUI();



        //命名空间

        //类

        //方法

        //生成调用代码

        //搜索 或者 指定
    }

    #region 主面板

    WindowStatus m_status = WindowStatus.AppDomain;
    Assembly m_currentAssembly;

    void MainGUI()
    {
        //顶部面板
        TopPanel();

        switch(m_status)
        {
            case WindowStatus.AppDomain:
                //程序集
                AllAssemvlyGUI();
                break;

            case WindowStatus.Assembly:
                NameSpaceGUI();
                break;

            default:break;
        }
    }

    void TopPanel()
    {
        string content = "";

        if(m_status == WindowStatus.AppDomain)
        {
            content += "应用程序视图：";
            content += AppDomain.CurrentDomain.FriendlyName;

        }
        else if(m_status == WindowStatus.Assembly)
        {
            content += "程序集视图：";

        }

        EditorGUILayout.LabelField(content);
        EditorGUILayout.Space();
    }

    #endregion

    #region 程序集

    Assembly[] m_asb;
    Vector2 m_assemvlyPos = Vector2.zero;

    void AllAssemvlyGUI()
    {
        if(m_asb == null)
        {
            m_asb = AppDomain.CurrentDomain.GetAssemblies();
        }

        m_assemvlyPos = GUILayout.BeginScrollView(m_assemvlyPos);

        for (int i = 0; i < m_asb.Length; i++)
        {
            if(GUILayout.Button(m_asb[i].FullName))
            {
                m_status = WindowStatus.Assembly;
                m_currentAssembly = m_asb[i];
            }
        }
        GUILayout.EndScrollView();
    }

    #endregion

    #region 命名空间

    Module[] m_module;
    Type[] m_types;
    Vector2 m_nameSpace = Vector2.zero;

    void NameSpaceGUI()
    {
        if(m_currentAssembly != null)
        {
            if(m_module == null)
            {
                m_module = m_currentAssembly.GetModules(true);
            }

            m_nameSpace = GUILayout.BeginScrollView(m_nameSpace);

            for (int i = 0; i < m_module.Length; i++)
            {
                EditorGUILayout.LabelField(m_module[i].Name);
            }

            for (int i = 0; i < m_currentAssembly.GetManifestResourceNames().Length; i++)
            {
                EditorGUILayout.LabelField(m_currentAssembly.GetManifestResourceNames()[i]);
            }

            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("没有选中程序集");
        }

        if(GUILayout.Button("返回上一层"))
        {
            m_status = WindowStatus.AppDomain;
            m_currentAssembly = null;
            m_module = null;
        }
    }

    #endregion

    #region 类

    #endregion

    #region 方法

    #endregion

    #region 搜索

    #endregion

    enum WindowStatus
    {
        AppDomain,
        Assembly,
        NameSpace,
        Class,
        Method,
    }
}
