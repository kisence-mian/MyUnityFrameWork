using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class DrawValueEitorGUI
{

    public static bool SupportValueType(object data)
    {
        if (data == null)
            return false;

        Type type = data.GetType();
        if (type == typeof(int))
        {

        }
        else if (type == typeof(short))
        {

        }
        else if (type == typeof(long))
        {

        }
        else if (type == typeof(double))
        {

        }
        else if (type == typeof(float))
        {

        }
        else if (type == typeof(bool))
        {

        }
        else if (type == typeof(string))
        {

        }
        else if (type == typeof(AnimationClip) ||
            type == typeof(Texture2D) ||
            type == typeof(Texture) ||
            type == typeof(Sprite) ||
            type == typeof(AnimatorController) ||
            type.BaseType == typeof(UnityEngine.Object) || type.BaseType == typeof(Component) || type.BaseType == typeof(MonoBehaviour))
        {

        }

        else if (type.BaseType == typeof(Enum))
        {
        }
        else if (type == typeof(Vector3))
        {

        }
        else if (type == typeof(Vector2))
        {

        }
        else if (type == typeof(Vector3Int))
        {

        }
        else if (type == typeof(Vector2Int))
        {

        }
        else if (type == typeof(Vector4))
        {

        }
        else if (type == typeof(Color))
        {
        }
        else
        {
            return false;
        }

        return true;
    }
    /// <summary>
    /// 绘制object
    /// </summary>
    /// <param name="showName"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static object DrawValue(Rect rect, object data, GUIStyle style)
    {
        if (data == null)
            return data;

        Type type = data.GetType();
        object obj = data;

        if (type == typeof(int))
        {
            obj = EditorGUI.IntField(rect, (int)data, style);
        }
        else if (type == typeof(short))
        {
            obj = (short)EditorGUI.IntField(rect, (short)data, style);
        }
        else if (type == typeof(long))
        {
            obj = EditorGUI.LongField(rect, (long)data, style);
        }
        else if (type == typeof(double))
        {
            obj = EditorGUI.DoubleField(rect, (double)data, style);
        }
        else if (type == typeof(float))
        {
            obj = EditorGUI.FloatField(rect, (float)data, style);
        }
        else if (type == typeof(bool))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.Toggle(rect, (bool)data, "BoldToggle");
        }
        else if (type == typeof(string))
        {
            obj = EditorGUI.TextArea(rect, data.ToString(), style);
        }
        else if (type == typeof(AnimationClip) ||
            type == typeof(Texture2D) ||
            type == typeof(Texture) ||
            type == typeof(Sprite) ||
            type == typeof(AnimatorController) ||
            type.BaseType == typeof(UnityEngine.Object) || type.BaseType == typeof(Component) || type.BaseType == typeof(MonoBehaviour))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.ObjectField(rect, (UnityEngine.Object)data, type, true);
        }

        else if (type.BaseType == typeof(Enum))
        {
            obj = EditorGUI.EnumPopup(rect, "", (Enum)Enum.Parse(type, data.ToString()), style);
        }
        else if (type == typeof(Vector3))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.Vector3Field(rect, "", (Vector3)data);
        }
        else if (type == typeof(Vector2))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.Vector2Field(rect, "", (Vector2)data);
        }
        else if (type == typeof(Vector3Int))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.Vector3IntField(rect, "", (Vector3Int)data);
        }
        else if (type == typeof(Vector2Int))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.Vector2IntField(rect, "", (Vector2Int)data);
        }
        else if (type == typeof(Vector4))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.Vector4Field(rect, "", (Vector4)data);
        }
        else if (type == typeof(Color))
        {
            GUI.Box(rect, "", style);
            obj = EditorGUI.ColorField(rect, (Color)data);
        }
        //else if (type.Name == typeof(List<>).Name)
        //{


        //}
        //else if (type.Name == typeof(Dictionary<,>).Name)
        //{

        //}
        //else if (type.IsArray)
        //{

        //}
        //else if ((type.IsClass && type != typeof(string)) || type.IsValueType)
        //{

        //}



        return obj;
    }
}