using System;
using System.Collections.Generic;
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
    MethodInfo m_currentMethod;

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

        switch (m_status)
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
                ClassGUI();
                break;

            case WindowStatus.Method:
                MethodDetailGUI();

                break;

            default: break;
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
                toolbarOption = 0;
                return;
            }

            content += "类视图：" +  m_currentClass.FullName;
        }
        else if (m_status == WindowStatus.Method)
        {
            if (m_currentMethod == null)
            {
                m_status = WindowStatus.Class;
                toolbarOption = 0;
                return;
            }

            content += "方法详细视图：" + m_currentClass.FullName + " " + m_currentMethod.Name;
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

    Vector2 m_classSpace = Vector2.zero;

    void AllClassGUI()
    {
        if (m_currentAssembly != null)
        {
            Type[] types;

            //if (isRoot)
            //{
                types = m_currentAssembly.GetTypes();
            //}
            //else
            //{
            //    types = m_typeStack.Peek().;
            //}

            m_classSpace = GUILayout.BeginScrollView(m_classSpace);

            for (int i = 0; i < types.Length; i++)
            {
                if(types[i].IsClass)
                {
                    if (m_searchClassContent == "" ||
                          (m_searchClassContent != "" && types[i].Name.ToLower().Contains(m_searchClassContent.ToLower())))
                    {
                        ClassButtonGUI(types[i]);
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

        string[] m_nameArrary = classType.FullName.Split('.');

        for (int i = 0; i < m_nameArrary.Length; i++)
        {
            if(i != m_nameArrary.Length - 1)
            {
                content3 += "<color=grey>" + m_nameArrary[i] + "</color>.";
            }
            else
            {
                content3 += "<color=#11FF11>" + m_nameArrary[i] + "</color>";
            }
        }

        GUILayout.Label(content, EditorGUIStyleData.RichText, GUILayout.Width(60));
        GUILayout.Label(content2, EditorGUIStyleData.RichText, GUILayout.Width(60));
        GUILayout.Label(content3, EditorGUIStyleData.RichText);

        if (GUILayout.Button("查看详细信息", GUILayout.Width(200)))
        {
            m_status = WindowStatus.Class;
            m_currentClass = classType;
            m_searchTmp = "";
            toolbarOption = 0;
        }

        GUILayout.EndHorizontal();
    }

    int toolbarOption = 0;
    string[] toolbarTexts = {"方法", "字段" , "属性"};
    void ClassGUI()
    {
        toolbarOption = GUILayout.Toolbar(toolbarOption, toolbarTexts, GUILayout.Width(Screen.width));

        switch(toolbarOption)
        {
            case 0:
                AllMethodGUI();
                break;

            case 1:
                AllFieleGUI();
                break;

            case 2:
                AllPropertyGUI();
                break;
        }
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
            EditorGUILayout.LabelField("没有选中类");
        }

        if(m_callContent != "")
        {
            EditorGUILayout.TextArea(m_callContent);
            if(GUILayout.Button("关闭", "toolbarbutton"))
            {
                m_callContent = "";
            }

            if (GUILayout.Button("复制到剪贴板"))
            {
                TextEditor tx = new TextEditor();
                tx.text = m_callContent;
                tx.OnFocus();
                tx.Copy();
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

        content3 += "<color=#11FF11>" + GetTypeName(method.ReturnType) + "</color> <color=white>" + method.Name + "</color>";

        if(method.IsGenericMethod)
        {
            Type[] types = method.GetGenericArguments();

            content3 += "<color=grey><</color>";

            for (int i = 0; i < types.Length; i++)
            {
                content3 += "<color=#11FF11>"+ GetTypeName(types[i]) + "</color>";

                if(i!= types.Length - 1)
                {
                    content3 += ",";
                }
            }
            content3 += "<color=grey>></color>";
        }

        content3 += " <color=grey>(</color>";

        ParameterInfo[] infos = method.GetParameters();
        for (int i = 0; i < infos.Length; i++)
        {
            content3 += GetParmPrdfix(infos[i],true) + "<color=#11FF11>" + GetTypeName(infos[i].ParameterType) + "</color> <color=#FF77FF>" + infos[i].Name +"</color>" + GetParmPostfix(infos[i],true);

            if(i != infos.Length - 1)
            {
                content3 += " <color=grey>,</color> ";
            }
        }

        content3 += "<color=grey>)</color>";

        GUILayout.Label(content, EditorGUIStyleData.RichText,GUILayout.Width(55));
        GUILayout.Label(content2, EditorGUIStyleData.RichText, GUILayout.Width(50));
        GUILayout.Label(content3, EditorGUIStyleData.RichText);  

        if(!method.IsPublic)
        {
            if (GUILayout.Button("生成调用代码", GUILayout.Width(200)))
            {
                m_callContent = GenerateCallCode(m_currentAssembly, m_currentClass, method);
                GUI.FocusControl("Search");
            }
        }

        if (GUILayout.Button("查看详细信息", GUILayout.Width(200)))
        {
            m_status = WindowStatus.Method;
            m_currentMethod = method;
        }

        GUILayout.EndHorizontal();
    }

    void MethodDetailGUI()
    {
        if(m_currentMethod != null)
        {
            m_classSpace = GUILayout.BeginScrollView(m_classSpace);

            GUILayout.Label("特性：", "WhiteLargeLabel");
            object[] objs = m_currentMethod.GetCustomAttributes(false);
            //特性
            for (int i = 0; i < objs.Length; i++)
            {
                GUILayout.BeginHorizontal("box");
                string content = "<color=#11FF11>[" + objs[i] + "]</color>";
                GUILayout.Label(content, EditorGUIStyleData.RichText);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("返回上一层"))
            {
                m_status = WindowStatus.Class;
                m_currentMethod = null;
                m_searchTmp = m_searchMethodContent;
            }
        }
        else
        {
            GUILayout.Label("没有选中方法");
        }
    }

    string GenerateCallCode(Assembly ab,Type type,MethodInfo method)
    {
        string content = "public static ";

        content += GetTypeName(method.ReturnType) + " " + method.Name + "Reflection";

        if (method.IsGenericMethod)
        {
            Type[] types = method.GetGenericArguments();

            content += "<";

            for (int i = 0; i < types.Length; i++)
            {
                content += GetTypeName(types[i]);

                if (i != types.Length - 1)
                {
                    content += ",";
                }
            }
            content += ">";
        }

        content += "(";
        ParameterInfo[] infos = method.GetParameters();

        if (!method.IsStatic)
        {
            content += "this " + type.Name + " instance";

            if(infos.Length > 0)
            {
                content += ",";
            }
        }

        for (int i = 0; i < infos.Length; i++)
        {
            content += GetParmPrdfix(infos[i],false) +  GetTypeName(infos[i].ParameterType) + " " + infos[i].Name + GetParmPostfix(infos[i],false);

            if(i != infos.Length - 1 )
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
        content += "\tMethodInfo mi = type.GetMethod(\"" + method.Name + "\", flags);\n";

        if (method.IsGenericMethod)
        {
            content += "\tmi = mi.MakeGenericMethod(";

            Type[] types = method.GetGenericArguments();

            for (int i = 0; i < types.Length; i++)
            {
                content += "typeof(" + GetTypeName(types[i]) + ")";

                if (i != types.Length - 1)
                {
                    content += ",";
                }

            }

            content += ");\n";
        }


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
            content += "return (" + GetTypeName(method.ReturnType) + ")";
        }

        if (method.IsStatic)
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

        content += "}";

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
        else if(info.ParameterType.IsByRef)
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
            content = "bool";
        }
        else if (info.ParameterType == typeof(int))
        {
            content = "int";
        }
        else if (info.ParameterType == typeof(long))
        {
            content = "long";
        }
        else if (info.ParameterType == typeof(float))
        {
            content = "float";
        }
        else if (info.ParameterType == typeof(double))
        {
            content = "double";
        }
        else if (info.ParameterType == typeof(string))
        {
            content = "\"string\"";
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

        if(info.IsByRef)
        {
            info = info.GetElementType();
        }

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
            if(info.IsGenericType)
            {
                Type[] types = info.GetGenericArguments();
                content = info.Name.Split('`')[0] + "<";

                for (int i = 0; i < types.Length; i++)
                {
                    content += GetTypeName(types[i]);

                    if(i != types.Length - 1)
                    {
                        content += ",";
                    }
                }

                content += ">";
            }
            else
            {
                content = info.Name;
            }
        }

        return content;
    }


    #endregion

    #region 字段

    void AllFieleGUI()
    {
        if (m_currentClass != null)
        {
            m_classSpace = GUILayout.BeginScrollView(m_classSpace);

            FieldInfo[] typesTmp = m_currentClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            for (int i = 0; i < typesTmp.Length; i++)
            {
                if (m_searchMethodContent == "" ||
                        (m_searchMethodContent != "" && typesTmp[i].Name.ToLower().Contains(m_searchMethodContent.ToLower())))
                {
                    FieldGUI(typesTmp[i]);
                }
            }

            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("没有选中类");
        }

        if (m_callContent != "")
        {
            EditorGUILayout.TextArea(m_callContent);
            if (GUILayout.Button("关闭", "toolbarbutton"))
            {
                m_callContent = "";
            }

            if (GUILayout.Button("复制到剪贴板"))
            {
                TextEditor tx = new TextEditor();
                tx.text = m_callContent;
                tx.OnFocus();
                tx.Copy();
            }
        }

        if (GUILayout.Button("返回上一层"))
        {
            m_status = WindowStatus.Assembly;
            m_currentClass = null;
            m_searchTmp = m_searchClassContent;
        }
    }

    void FieldGUI(FieldInfo info)
    {
        GUILayout.BeginHorizontal("box");

        string content = "";
        string content2 = "";
        string content3 = "";

        if (!info.IsPublic)
        {
            content += "<color=red>Private</color> ";
        }

        if (info.IsLiteral)
        {
            content2 += "<color=#9999FF>Const</color> ";
        }
        else
        {
            if (!info.IsStatic)
            {
                content2 += "<color=yellow>Static</color> ";
            }
        }

        content3 += "<color=#11FF11>" + GetTypeName(info.FieldType) + "</color> <color=white>" + info.Name + "</color>";

        GUILayout.Label(content, EditorGUIStyleData.RichText, GUILayout.Width(55));
        GUILayout.Label(content2, EditorGUIStyleData.RichText, GUILayout.Width(50));
        GUILayout.Label(content3, EditorGUIStyleData.RichText);

        //if (GUILayout.Button("查看详细信息", GUILayout.Width(200)))
        //{
        //    m_searchAssemblyContent = "";
        //}

        if(!info.IsPublic)
        {
            if (GUILayout.Button("生成调用代码", GUILayout.Width(200)))
            {
                m_callContent = GenerateSetValueCode(m_currentAssembly, m_currentClass, info);
                GUI.FocusControl("Search");
            }
        }

        GUILayout.EndHorizontal();
    }

    string GenerateSetValueCode(Assembly assembly,Type type,FieldInfo field)
    {
        string content = "";

        //常量不能被修改
        if (!field.IsLiteral)
        {
            //生成Set方法
            content += "public static void Set_" + field.Name + "Reflection(";

            if (field.IsStatic)
            {
                content += "this " + GetTypeName(type) + " instance,";
            }

            content += GetTypeName(field.FieldType) + " " + field.Name + ")\n";

            content += "{\n";

            //flags
            content += "\tBindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;\n";

            //获取程序集
            content += "\tAssembly ab = Assembly.Load(\"" + assembly.FullName.Split(',')[0] + "\");\n";
            //获取Type
            content += "\tType type = ab.GetType(\"" + type.FullName + "\");\n";
            //获取FieldInfo
            content += "\tFieldInfo fi = type.GetField(\"" + field.Name + "\", flags);\n\n";
            //调用
            content += "\t";

            if (field.IsStatic)
            {
                content += "fi.SetValue(null," + field.Name + ");\n";
            }
            else
            {
                content += "fi.SetValue(instance," + field.Name + ");";
            }

            content += "}";
            content += "\n\n";
        }

        //生成Get方法
        content += "public static " + GetTypeName(field.FieldType) + " Get_" + field.Name + "()\n";
        content += "{\n";

        //flags
        content += "\tBindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;\n";
        //获取程序集
        content += "\tAssembly ab = Assembly.Load(\"" + assembly.FullName.Split(',')[0] + "\");\n";
        //获取Type
        content += "\tType type = ab.GetType(\"" + type.FullName + "\");\n";
        //获取FieldInfo
        content += "\tFieldInfo fi = type.GetField(\"" + field.Name + "\", flags);\n\n";
        //调用
        content += "\treturn ("+GetTypeName(field.FieldType) +")";

        if (field.IsStatic)
        {
            content += "fi.GetValue(null);\n";
        }
        else
        {
            content += "fi.GetValue(instance);\n";
        }

        content += "}";

        return content;
    }

    #endregion

    #region  属性

    void AllPropertyGUI()
    {
        if (m_currentClass != null)
        {
            m_classSpace = GUILayout.BeginScrollView(m_classSpace);

            PropertyInfo[] typesTmp = m_currentClass.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            for (int i = 0; i < typesTmp.Length; i++)
            {
                if (m_searchMethodContent == "" ||
                        (m_searchMethodContent != "" && typesTmp[i].Name.ToLower().Contains(m_searchMethodContent.ToLower())))
                {
                    PropertyGUI(typesTmp[i]);
                }
            }

            GUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("没有选中类");
        }

        if (m_callContent != "")
        {
            EditorGUILayout.TextArea(m_callContent);
            if (GUILayout.Button("关闭", "toolbarbutton"))
            {
                m_callContent = "";
            }

            if (GUILayout.Button("复制到剪贴板"))
            {
                TextEditor tx = new TextEditor();
                tx.text = m_callContent;
                tx.OnFocus();
                tx.Copy();
            }
        }

        if (GUILayout.Button("返回上一层"))
        {
            m_status = WindowStatus.Assembly;
            m_currentClass = null;
            m_searchTmp = m_searchClassContent;
        }
    }

    void PropertyGUI(PropertyInfo info)
    {
        GUILayout.BeginHorizontal("box");

        string content = "";
        string content2 = "";
        //string content3 = "";

        if(info.CanRead)
        {
            if (info.CanWrite)
            {
                content = "<color=#88FF88>Read&Write</color>";
            }
            else
            {
                content = "<color=yellow>ReadOnly</color>";
            }
        }
        else
        {
            if (info.CanWrite)
            {
                content = "<color=#00FF99>WriteOnly</color>";
            }
            else
            {
                content = "<color=red>No</color>";
            }
        }

        content2 += "<color=#11FF11>" + GetTypeName(info.PropertyType) + "</color> <color=white>" + info.Name + "</color>";

        GUILayout.Label(content, EditorGUIStyleData.RichText, GUILayout.Width(100));
        GUILayout.Label(content2, EditorGUIStyleData.RichText);

        GUILayout.EndHorizontal();
    }


#endregion

    enum WindowStatus
    {
        AppDomain,
        Assembly,
        Class,
        Method,
        Field,
        Property,
    }
}
