using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

public class EditorUtilGUI
{
    #region 常用数据类型

    public static string FieldGUI_TypeValue(FieldType type, string content,string enumType)
    {
        SingleField data = new SingleField(type, content, enumType);

        return EditorUtilGUI.FieldGUI_TypeValue(data);
    }

    public static string FieldGUI_TypeValue(SingleField data)
    {
        if (data.m_type != FieldType.Enum) 
        {
            EditorGUILayout.LabelField("字段类型", data.m_type.ToString());
        }
        else
        {
            EditorGUILayout.LabelField("字段类型", data.m_type.ToString() +"/"+ data.m_enumType);
        }

        return EditorUtilGUI.FieldGUI_Type(data);
    } 

    public static string FieldGUI_Type(FieldType type,string enumType,string content, string labelContent = "字段内容")
    {
        try
        {
            SingleField data = new SingleField(type, content, enumType);
            return EditorUtilGUI.FieldGUI_Type(data, labelContent);
        }
        catch(Exception e)
        {
            EditorGUILayout.LabelField("解析错误：", e.ToString(), "ErrorLabel");
            return content;
        }
    }

    public static string FieldGUI_Type(SingleField data, string labelContent = "字段内容")
    {
        string content = "";

        switch (data.m_type)
        {
            case FieldType.String:
            case FieldType.StringArray:
            case FieldType.BoolArray:
            case FieldType.IntArray:
            case FieldType.FloatArray:
            case FieldType.Vector2Array:
            case FieldType.Vector3Array:
                content = EditorGUILayout.TextField(labelContent, data.GetString());
                break;

            case FieldType.Int:
                content = EditorGUILayout.IntField(labelContent, data.GetInt()).ToString();
                break;

            case FieldType.Float:
                content = EditorGUILayout.FloatField(labelContent, data.GetFloat()).ToString();
                break;

            case FieldType.Bool:
                content = EditorGUILayout.Toggle(labelContent, data.GetBool()).ToString();
                break;

            case FieldType.Vector3:
                content = EditorGUILayout.Vector3Field(labelContent, data.GetVector3()).ToSaveString();
                break;

            case FieldType.Vector2:
                content = EditorGUILayout.Vector2Field(labelContent, data.GetVector2()).ToSaveString();
                break;

            case FieldType.Color: 
                content = EditorGUILayout.ColorField(labelContent, data.GetColor()).ToSaveString();
                break;
            case FieldType.Enum:
                if (data.m_enumType != "" && data.m_enumType != null)
                {
                    content = EditorGUILayout.EnumPopup(labelContent, data.GetEnum()).ToString();
                }
                break;
        }

        if (data.m_content != content)
        {
            data.m_content = content;
        }

        return content;
    }

    #endregion

    #region 反射类并显示在GUI
    public static object DrawObjectDataEditorDefultOneField(Type type,string name, object value)
    {
        object obj = null;
        if (type == typeof(int))
        {
            obj = EditorGUILayout.IntField(new GUIContent(name), (int)value);
        }
        else if (type == typeof(long))
        {
            obj = EditorGUILayout.LongField(new GUIContent(name), (long)value);
        }
        else if (type == typeof(double))
        {
            obj = EditorGUILayout.DoubleField(new GUIContent(name), (double)value);
        }
        else if (type == typeof(float))
        {
            obj = EditorGUILayout.FloatField(new GUIContent(name), (float)value);
        }
        else if (type == typeof(bool))
        {
            obj = EditorGUILayout.Toggle(new GUIContent(name), (bool)value);
        }
        else if (type.FullName == typeof(string).FullName)
        {
            obj = EditorGUILayout.TextField(new GUIContent(name), (string)value);
        }
        else if (type.BaseType.FullName == typeof(UnityEngine.Object).FullName || type.BaseType.FullName == typeof(UnityEngine.Component).FullName)
        {
            obj = EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, type, true);
        }
        else if (type.BaseType == typeof(Enum))
        {
            obj = EditorGUILayout.EnumPopup(new GUIContent(name), (Enum)Enum.Parse(type, value.ToString()));
        }
        else if (type.FullName == typeof(Vector3).FullName)
        {
            obj = EditorGUILayout.Vector3Field(new GUIContent(name), (Vector3)value);
        }
        else if (type.FullName == typeof(Vector2).FullName)
        {
            obj = EditorGUILayout.Vector2Field(new GUIContent(name), (Vector2)value);
        }
        else if (type.FullName == typeof(UnityEngine.Color).FullName)
        {
            obj = EditorGUILayout.ColorField(new GUIContent(name), (Color)value);
        }
        else if (type.Name == typeof(List<>).Name)
        {
            obj = DrawList(type,name, value);
        }
        else if (type.IsClass)
        {
            obj = DrawClassData(type,type.Name, value, name);
        }
        else
        {
            obj = value;
        }

        return obj;
    }


    public static object DrawClassData(Type type,string className, object obj, string fieldName = null)
    {
        if (obj == null)
        {
            if(type.GetConstructor(new Type[] { }) != null)
            {
                obj = Activator.CreateInstance(type);
            }
            else
            {
                return obj;
            }
        }
        else
        {
            if (type == null)
            {
                return obj;
            }

            if (type.Name != obj.GetType().Name)
            {
                obj = Activator.CreateInstance(type);
            }
        }
        return DrawClassData(obj, fieldName);
    }
    public static object DrawClassData(object obj, string fieldName = null)
    {
        if (obj == null)
            return null;

        Type t = obj.GetType();
        FieldInfo[] fs = t.GetFields();
        GUILayout.BeginVertical("box");
        if (fieldName != null)
        {
            GUILayout.Label(fieldName);
        }

        foreach (FieldInfo f in fs)
        {
            if (f.GetCustomAttributes(typeof(HideInInspector),true).Length == 0
                && f.IsStatic == false)
            {
                obj = DrawInternalVariableGUI(obj, f);
            }
        }

        GUILayout.EndVertical();
        return obj;
    }

    public static string DrawPopup(string name, string selectedStr, List<string> displayedOptions)
    {
        if (displayedOptions == null)
            return selectedStr;
        int selectedIndex = 0;

        if (displayedOptions.Contains(selectedStr))
        {
            selectedIndex = displayedOptions.IndexOf(selectedStr);
        }

        int sel = EditorGUILayout.Popup(name, selectedIndex, displayedOptions.ToArray());
        if (displayedOptions.Count == 0)
            return "";
        else
            return displayedOptions[sel];
    }
    public static object DrawStringPopupList(string name, object obj, List<string> displayedOptions)
    {
        if (obj == null)
            return null;
        Type type = obj.GetType();
        //Type t = type.GetGenericArguments()[0];

        MethodInfo methodInfo = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo methodInfo1 = type.GetMethod("set_Item", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo methodInfo2 = type.GetMethod("RemoveAt", BindingFlags.Instance | BindingFlags.Public);
        MethodInfo methodInfo3 = type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
        GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", GUILayout.Width(50)))
        {
            object temp = "";
            methodInfo3.Invoke(obj, new object[] { temp });
        }

        GUILayout.EndHorizontal();
        PropertyInfo pro = type.GetProperty("Count");
        int cout = (int)pro.GetValue(obj, null);
        GUILayout.BeginVertical("box");
        for (int i = 0; i < cout; i++)
        {
            object da = methodInfo.Invoke(obj, new object[] { i });
            GUILayout.BeginHorizontal();
            da = DrawPopup("", da.ToString(), displayedOptions);
            methodInfo1.Invoke(obj, new object[] { i, da });

            if (GUILayout.Button("-", GUILayout.Width(50)))
            {
                methodInfo2.Invoke(obj, new object[] { i });
                break;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        return obj;
    }
    public static object DrawList(Type type,string name, object obj)
    {
        try
        {
            Type t = type.GetGenericArguments()[0];

            MethodInfo methodInfo = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo methodInfo1 = type.GetMethod("set_Item", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo methodInfo2 = type.GetMethod("RemoveAt", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo methodInfo3 = type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                object temp = null;
                if (t.FullName == typeof(string).FullName)
                    temp = "";
                else
                    temp = Activator.CreateInstance(t);
                methodInfo3.Invoke(obj, new object[] { temp });
            }
            GUILayout.EndHorizontal();
            PropertyInfo pro = type.GetProperty("Count");
            int cout = (int)pro.GetValue(obj, null);
            GUILayout.BeginVertical("box");
            for (int i = 0; i < cout; i++)
            {
                object da = methodInfo.Invoke(obj, new object[] { i });
                GUILayout.BeginHorizontal();
                da = DrawObjectDataEditorDefultOneField(t, "", da);
                methodInfo1.Invoke(obj, new object[] { i, da });

                if (GUILayout.Button("-", GUILayout.Width(50)))
                {
                    methodInfo2.Invoke(obj, new object[] { i });
                    break;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        catch(Exception e)
        {
                Debug.Log(e.ToString());
        }
        return obj;
    }

    public static object DrawInternalVariableGUI(object obj, FieldInfo f)
    {
        object value = DrawObjectDataEditorDefultOneField(f.FieldType,f.Name, f.GetValue(obj));
        f.SetValue(obj, value);
        return obj;
    }

    public static string[] FindDataName(string path)
    {
        List<string> m_dataNameList = new List<string>();
        if (!Directory.Exists(path))
        {
            FileTool.CreatPath(path);
        }

        string[] allUIPrefabName = Directory.GetFileSystemEntries(path);
        foreach (var item in allUIPrefabName)
        {
            string[] tempFileName = new string[0];
            if (Directory.Exists(item))
            {
                tempFileName = FindDataName(item);
            }

            if (tempFileName.Length > 0)
            {
                foreach (string s in tempFileName)
                {
                    string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                    configName += "/" + s;
                    m_dataNameList.Add(configName);
                }
            }
            else
            //if (item.EndsWith(".txt"))
            {
                if (item.EndsWith(".meta"))
                    continue;
                string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                m_dataNameList.Add(configName);
            }
        }
        return m_dataNameList.ToArray();

    }

    /// <summary>
    /// 使用这个特性的字段不会显示在面板上
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class HideByGUIAttribute : System.Attribute
    {
    }

    #endregion
}
