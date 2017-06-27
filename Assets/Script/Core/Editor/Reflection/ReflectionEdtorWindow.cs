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
    Type m_currentClass;
    MethodInfo m_method;

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
                //类
                AllClassGUI();
                break;

            case WindowStatus.Class:
                //类
                AllMethodGUI();
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
        else if (m_status == WindowStatus.Class)
        {
            content += "类视图：" + m_currentClass.Name;
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

    //Module[] m_module;


    void NameSpaceGUI()
    {
    }

    #endregion

    #region 类

    Type[] m_types;
    Vector2 m_classSpace = Vector2.zero;

    void AllClassGUI()
    {
        if (m_currentAssembly != null)
        {
            if(m_types == null )
            {
                m_types = m_currentAssembly.GetTypes();
            }

            m_classSpace = GUILayout.BeginScrollView(m_classSpace);

            for (int i = 0; i < m_types.Length; i++)
            {
                if(m_types[i].IsClass)
                {
                    if (GUILayout.Button(m_types[i].Name))
                    {
                        m_status = WindowStatus.Class;
                        m_currentClass = m_types[i];
                    }
                }
            }

            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("没有选中程序集");
        }

        if (GUILayout.Button("返回上一层"))
        {
            m_status = WindowStatus.AppDomain;
            m_currentAssembly = null;
            m_types = null;
        }
    }
    #endregion

    #region 方法

    void AllMethodGUI()
    {
        if (m_currentClass != null)
        {
            m_classSpace = GUILayout.BeginScrollView(m_classSpace);

            MethodInfo[] typesTmp = m_currentClass.GetMethods(BindingFlags.Public| BindingFlags.NonPublic| BindingFlags.Static| BindingFlags.Instance);

            for (int i = 0; i < typesTmp.Length; i++)
            {
                if (GUILayout.Button(typesTmp[i].Name))
                {
                    m_status = WindowStatus.Method;
                    m_method = typesTmp[i];
                }
            }

            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("没有选中程序集");
        }

        if (GUILayout.Button("返回上一层"))
        {
            m_status = WindowStatus.Assembly;
            m_currentClass = null;
        }
    }

    #endregion

    #region 搜索

    #endregion

    enum WindowStatus
    {
        AppDomain,
        Assembly,
        //NameSpace,
        Class,
        Method,
    }
}
