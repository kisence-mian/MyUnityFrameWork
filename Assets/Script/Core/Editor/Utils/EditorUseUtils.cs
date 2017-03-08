using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class EditorUseUtils  {
    public static object DrawObjectDataEditorDefultOneField(string name, object value)
    {
        if (value == null)
            return value;
        Type type = value.GetType();
        object obj = null;
        if (type == typeof(int))
        {
            obj=EditorGUILayout.IntField(new GUIContent(name ), (int)value);
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
           obj=EditorGUILayout.FloatField(new GUIContent(name), (float)value);
        }
        else if (type == typeof(bool))
        {
           obj= EditorGUILayout.Toggle(new GUIContent(name ), (bool)value);
        }
        else if (type.FullName == typeof(string).FullName)
        {
           obj=EditorGUILayout.TextField(new GUIContent(name), value.ToString());
        }
        else if (type.BaseType.FullName == typeof(UnityEngine.Object).FullName || type.BaseType.FullName == typeof(UnityEngine.Component).FullName )
        {
           obj= EditorGUILayout.ObjectField(name , (UnityEngine.Object)value,type, true);
            // Debug.Log("GameObject type : " + f.FieldType.Name);
        }
        else if (type.BaseType == typeof(Enum))
        {
           obj= EditorGUILayout.EnumPopup(new GUIContent(name), (Enum)Enum.Parse(type, value.ToString()));
            // Debug.Log("Enum type : " + f.FieldType.Name);
        }
        else if (type.FullName == typeof(Vector3).FullName)
        {
           obj= EditorGUILayout.Vector3Field(new GUIContent(name ), (Vector3)value);
        }
        else if (type.FullName == typeof(Vector2).FullName)
        {
            obj = EditorGUILayout.Vector2Field (new GUIContent(name), (Vector2)value);
        }
        else if (type.FullName == typeof(UnityEngine.Color).FullName)
        {
            obj = EditorGUILayout.ColorField(new GUIContent(name), (Color)value);
        }
        else if (type.Name == typeof(List<>).Name)
        {
           obj = DrawList(name, value);
        }
        else if (type.IsClass)
        {
            obj = DrawClassData(type.Name, value);
        }
        else
        {
            obj = value;
        }

        return obj;
    }
    public static object DrawObjectDataEditorDefultOneField(object data, FieldInfo field)
    {
        if (data == null || field == null)
            return null;
        object d = DrawObjectDataEditorDefultOneField(field.Name, field.GetValue(data));
        field.SetValue(data, d);
        return d;
    }


    public static object DrawClassData(string className,object obj)
    {
        Assembly tmp = Assembly.Load("Assembly-CSharp");
        Type t = tmp.GetType(className);
       
        if (obj == null)
            obj = Activator.CreateInstance(t);
        else
        {
            if (t.Name != obj.GetType().Name)
            {
                obj = Activator.CreateInstance(t);
            }
        }
        return DrawClassData(obj);
    }
    public static object DrawClassData(object obj)
    {
        if (obj == null)
            return null;
        Type t =obj.GetType();
        FieldInfo[] fs = t.GetFields();
        GUILayout.BeginVertical("box");
        foreach (FieldInfo f in fs)
        {
            obj = DrawInternalVariableGUI(obj, f);
        }
        GUILayout.EndVertical();
        return obj;
    }

    public static string DrawPopup(string name, string selectedStr, List<string> displayedOptions)
    {
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
            da = DrawPopup("", da.ToString(),displayedOptions);
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
    public static object DrawList(string name, object obj)
    {
        if (obj == null)
            return null;
        Type type = obj.GetType();
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
              da =  DrawObjectDataEditorDefultOneField("", da);
             methodInfo1.Invoke(obj, new object[] { i,da });

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

    public static object DrawInternalVariableGUI(object obj, FieldInfo f)
    {
        bool isShow = true;
        foreach (Attribute a in f.GetCustomAttributes(true))
        {
            NoneShowInEditorGUIAttribute ns = a as NoneShowInEditorGUIAttribute;
            if (ns != null)
            {
                isShow = false;
                break;
            }
        }
        if (!isShow) return obj;

        object value = EditorUseUtils.DrawObjectDataEditorDefultOneField(f.Name, f.GetValue(obj));
        f.SetValue(obj, value);
        return obj;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class NoneShowInEditorGUIAttribute : Attribute
{
    public NoneShowInEditorGUIAttribute()
    {

    }
}