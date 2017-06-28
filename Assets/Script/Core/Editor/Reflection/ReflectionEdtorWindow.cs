using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class ReflectionEdtorWindow : EditorWindow
{
    GUIStyle style;


    [MenuItem("Window/反射查看器",priority =1100)]
    public static void ShowWindow()
    {
        GetWindow(typeof(ReflectionEdtorWindow));
    }

    void OnEnable()
    {
        EditorGUIStyleData.Init();
        GUI.FocusControl("Search");
    }

    void OnGUI()
    {
        titleContent.text = "反射查看器";

        EditorGUIStyleData.Init();

        MainGUI();
    }

    #region 主面板

    WindowStatus m_status = WindowStatus.AppDomain;
    Assembly m_currentAssembly;
    Type m_currentClass;
    MethodInfo m_method;

    string m_searchTmp = "";

    string m_searchAssemblyContent = "";
    string m_searchClassContent  = "";
    string m_searchMethodContent = "";

    void InitStyle()
    {
        if (style == null)
        {
            style = new GUIStyle();
            style.richText = true;
        }
    }

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
            if(m_currentAssembly == null)
            {
                m_status = WindowStatus.AppDomain;
                return;
            }
            content += "程序集视图：" + m_currentAssembly.FullName;
        }
        else if (m_status == WindowStatus.Class)
        {
            if (m_currentClass == null)
            {
                m_status = WindowStatus.Assembly;
                return;
            }

            content += "类视图：" + m_currentClass.FullName;
        }

        GUILayout.Label(content, "PreLabel");

        GUILayout.BeginHorizontal();

        GUI.SetNextControlName("Search");
        GUILayout.Label("在当前页面下搜索:", GUILayout.Width(110));

        m_searchTmp = GUILayout.TextField(m_searchTmp, "SearchTextField");

        if(GUILayout.Button("清除", "SearchCancelButton", GUILayout.Width(50)))
        {
            m_searchTmp = "";
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (m_status == WindowStatus.AppDomain)
        {
            m_searchAssemblyContent = m_searchTmp;
        }
        else if (m_status == WindowStatus.Assembly)
        {
            m_searchClassContent = m_searchTmp;
        }
        else if (m_status == WindowStatus.Class)
        {
            m_searchMethodContent = m_searchTmp;
        }
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
            if(m_searchAssemblyContent == "" ||
                (m_searchAssemblyContent != "" && m_asb[i].FullName.ToLower().Contains(m_searchAssemblyContent.ToLower())))
            {
                AssemvlyGUI(m_asb[i]);
            }
        }
        GUILayout.EndScrollView();
    }

    void AssemvlyGUI(Assembly assembly)
    {
        GUILayout.BeginHorizontal("box");

        string content = "";
        string content2 = "";

        string[] splitTmp = assembly.FullName.Split(',');
        string name = splitTmp[0];

        string[] nameSplitTmp = name.Split('.');

        for (int i = 0; i < nameSplitTmp.Length; i++)
        {
            if(i != nameSplitTmp.Length - 1)
            {
                content += "<color=grey>" + nameSplitTmp[i] + "</color>.";
            }
            else
            {
                content += "<color=white>" + nameSplitTmp[i] + "</color> ";
            }
        }

        for (int i = 1; i < splitTmp.Length; i++)
        {
            string[] Tmp = splitTmp[i].Split('=');
            if(Tmp.Length > 1)
            {
                content2 += " <color=yellow>" + Tmp[0] + "</color> = <color=red>" + Tmp[1] + "</color> ";
            }
        }

        GUILayout.Label(content, EditorGUIStyleData.RichText, GUILayout.Width(400));
        GUILayout.Label(content2, EditorGUIStyleData.RichText);

        if (GUILayout.Button("查看详细信息", GUILayout.Width(200)))
        {
            m_status = WindowStatus.Assembly;
            m_currentAssembly = assembly;
            m_searchTmp = "";
        }

        GUILayout.EndHorizontal();
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
                    if (m_searchClassContent == "" ||
                          (m_searchClassContent != "" && m_types[i].Name.ToLower().Contains(m_searchClassContent.ToLower())))
                    {
                        ClassButtonGUI(m_types[i]);
                    }
                }
            }

            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("没有选中类");
        }

        if (GUILayout.Button("返回上一层"))
        {
            m_status = WindowStatus.AppDomain;
            m_currentAssembly = null;
            m_types = null;
            m_searchTmp = m_searchAssemblyContent;
        }
    }

    void ClassButtonGUI(Type classType)
    {
        GUILayout.BeginHorizontal("box");

        string content = "";
        string content2 = "";
        string content3 = "";

        if (!classType.IsPublic)
        {
            content += "<color=red>Private</color> ";
        }

        if (!classType.IsSealed)
        {
            content2 += "<color=blue>Sealed</color> ";
        }

        content3 += "<color=#11FF11>" + classType.Name + "</color> ";

        GUILayout.Label(content, EditorGUIStyleData.RichText, GUILayout.Width(60));
        GUILayout.Label(content2, EditorGUIStyleData.RichText, GUILayout.Width(60));
        GUILayout.Label(content3, EditorGUIStyleData.RichText);

        if (GUILayout.Button("查看详细信息", GUILayout.Width(200)))
        {
            m_status = WindowStatus.Class;
            m_currentClass = classType;
            m_searchTmp = "";
        }

        GUILayout.EndHorizontal();
    }

    #endregion

    #region 方法

    string m_callContent = "";

    void AllMethodGUI()
    {
        if (m_currentClass != null)
        {
            m_classSpace = GUILayout.BeginScrollView(m_classSpace);

            MethodInfo[] typesTmp = m_currentClass.GetMethods(BindingFlags.Public| BindingFlags.NonPublic| BindingFlags.Static| BindingFlags.Instance);

            for (int i = 0; i < typesTmp.Length; i++)
            {
                if (m_searchMethodContent == "" ||
                        (m_searchMethodContent != "" && typesTmp[i].Name.ToLower().Contains(m_searchMethodContent.ToLower())))
                {
                    MethodGUI(typesTmp[i]);
                }
            }

            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("没有选中方法");
        }

        if(m_callContent != "")
        {
            EditorGUILayout.TextArea(m_callContent);
            if(GUILayout.Button("关闭", "toolbarbutton"))
            {
                m_callContent = "";
            }
        }

        if (GUILayout.Button("返回上一层"))
        {
            m_status = WindowStatus.Assembly;
            m_currentClass = null;
            m_searchTmp = m_searchClassContent;
        }
    }

    void MethodGUI(MethodInfo method)
    {
        GUILayout.BeginHorizontal("box");

        string content = "";
        string content2 = "";
        string content3 = "";

        if (!method.IsPublic)
        {
            content += "<color=red>Private</color> ";
        }

        if (!method.IsStatic)
        {
            content2 += "<color=yellow>Static</color> ";
        }

        content3 += "<color=#11FF11>" + GetTypeName(method.ReturnType) + "</color> <color=white>" +  method.Name + "</color> <color=grey>(</color>";

        ParameterInfo[] infos = method.GetParameters();
        for (int i = 0; i < infos.Length; i++)
        {
            content3 += GetParmPrdfix(infos[i],true) + "<color=#11FF11>" + GetTypeName(infos[i].ParameterType) + "</color> " + infos[i].Name + GetParmPostfix(infos[i],true);

            if(i != infos.Length - 1)
            {
                content3 += " <color=grey>,</color> ";
            }
        }

        content3 += "<color=grey>)</color>";

        GUILayout.Label(content, EditorGUIStyleData.RichText,GUILayout.Width(55));
        GUILayout.Label(content2, EditorGUIStyleData.RichText, GUILayout.Width(50));
        GUILayout.Label(content3, EditorGUIStyleData.RichText);

        //if (GUILayout.Button("查看详细信息", GUILayout.Width(200)))
        //{
        //    m_searchAssemblyContent = "";
        //}

        if (GUILayout.Button("生成调用代码",GUILayout.Width(200)))
        {
            m_callContent = GenerateCallCode(m_currentAssembly,m_currentClass, method);
            GUI.FocusControl("Search");
        }

        GUILayout.EndHorizontal();
    }

    string GenerateCallCode(Assembly ab,Type type,MethodInfo method)
    {
        string content = "public ";

        if(method.IsStatic)
        {
            content += "static ";
        }

        content += GetTypeName(method.ReturnType) + " " + method.Name + "Reflection(";

        if (!method.IsStatic)
        {
            content += "this " + type.Name + " instance,";
        }

        ParameterInfo[] infos = method.GetParameters();
        for (int i = 0; i < infos.Length; i++)
        {
            content += GetParmPrdfix(infos[i],false) +  GetTypeName(infos[i].ParameterType) + " " + infos[i].Name + GetParmPostfix(infos[i],false);
            if (i != infos.Length - 1)
            {
                content += ",";
            }
        }

        content += ")\n";

        content += "{\n";

        //flags
        content += "\tBindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;\n";

        //获取程序集
        content += "\tAssembly ab = Assembly.Load(\""+ ab.FullName.Split(',')[0]+"\");\n";
        //获取Type
        content += "\tType type = ab.GetType(\"" + type.FullName + "\");\n";
        //获取Method
        content += "\tMethodInfo mi = type.GetMethod(\""+ method .Name+ "\", flags);\n";

        //调用
        if (infos.Length != 0)
        {
            content += "\n";
            content += "\tobject[] objs = new object["+ infos.Length + "];\n";
            for (int i = 0; i < infos.Length; i++)
            {
                content += "\tobjs[" + i + "] = " + infos[i].Name + ";\n";
            }
        }
        content += "\n";

        content += "\t";

        if (method.ReturnType != typeof(void))
        {
            content += "return (" + GetTypeName(method.ReturnType)+")";
        }

        if(method.IsStatic)
        {
            content += "mi.Invoke(null,";
        }
        else
        {
            content += "mi.Invoke(instance,";
        }

        if (infos.Length != 0)
        {
            content += "objs";
        }
        else
        {
            content += "null";
        }

        content += ");\n";

        content += "}\n";

        return content;
    }

    string GetParmPrdfix(ParameterInfo info,bool isHighLight)
    {
        string content = "";

        if (isHighLight)
        {
            content += "<color=blue>";
        }

        if (info.IsOut)
        {
            content +=  "out ";
        }

        if(info.Name.EndsWith("&"))
        {
            content += "ref ";
        }

        if (isHighLight)
        {
            content += "</color>";
        }

        return content;
    }

    string GetParmPostfix(ParameterInfo info, bool isHighLight)
    {
        string content = "";

        if(info.IsOptional)
        {
            if (isHighLight)
            {
                content = " <color=grey>=</color> <color=black>" + GetDefaultValue(info) + "</color>";
            }
            else
            {
                content = " = " + GetDefaultValue(info);
            }
        }

        return content;
    }

    string GetDefaultValue(ParameterInfo info)
    {
        string content = "";

        if (info.ParameterType == typeof(bool))
        {
            content = ((bool)info.DefaultValue).ToString();
        }
        else if (info.ParameterType == typeof(int))
        {
            content = ((int)info.DefaultValue).ToString();
        }
        else if (info.ParameterType == typeof(long))
        {
            content = ((long)info.DefaultValue).ToString();
        }
        else if (info.ParameterType == typeof(float))
        {
            content = ((float)info.DefaultValue).ToString() + "f";
        }
        else if (info.ParameterType == typeof(double))
        {
            content = ((double)info.DefaultValue).ToString() + "d";
        }
        else if (info.ParameterType == typeof(string))
        {
            content = "\""+((string)info.DefaultValue).ToString() + "\"";
        }
        else
        {
            content = info.ParameterType.Name;
        }

        return content;
    }

    string GetTypeName(Type info)
    {
        string content = "";

        if(info == typeof(bool))
        {
            content = "bool";
        }
        else if (info == typeof(bool[]))
        {
            content = "bool[]";
        }

        else if(info == typeof(int))
        {
            content = "int";
        }
        else if (info == typeof(int[]))
        {
            content = "int[]";
        }

        else if (info == typeof(long))
        {
            content = "long";
        }
        else if (info == typeof(long[]))
        {
            content = "long[]";
        }

        else if (info == typeof(float))
        {
            content = "float";
        }
        else if (info == typeof(float[]))
        {
            content = "float[]";
        }

        else if (info == typeof(double))
        {
            content = "double";
        }
        else if (info == typeof(double[]))
        {
            content = "double[]";
        }

        else if (info == typeof(string))
        {
            content = "string";
        }
        else if (info == typeof(string[]))
        {
            content = "string[]";
        }

        else if (info == typeof(void))
        {
            content = "void";
        }

        else if (info == typeof(object))
        {
            content = "object";
        }
        else if (info == typeof(object[]))
        {
            content = "object[]";
        }
        else
        {
            content = info.Name;
        }

        return content;
    }


    #endregion

    #region 搜索

    #endregion

    enum WindowStatus
    {
        AppDomain,
        Assembly,
        Class,
        Method,
    }
}
