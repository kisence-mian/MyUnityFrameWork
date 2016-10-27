using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorUtilGUI 
{
    public static string SingleFieldGUI(SingleField data)
    {
        string content = "";

        switch (data.m_type)
        {
            case FieldType.String:
                content = EditorGUILayout.TextField("字段内容", data.GetString());
                break;

            case FieldType.Int:
                content = EditorGUILayout.IntField("字段内容", data.GetInt()).ToString();
                break;

            case FieldType.Float:
                content = EditorGUILayout.FloatField("字段内容", data.GetFloat()).ToString();
                break;

            case FieldType.Bool:
                content = EditorGUILayout.Toggle("字段内容", data.GetBool()).ToString();
                break;

            case FieldType.Vector3:
                content = EditorGUILayout.Vector3Field("字段内容", data.GetVector3()).ToSaveString();
                break;

            case FieldType.Vector2:
                content = EditorGUILayout.Vector2Field("字段内容", data.GetVector2()).ToSaveString();
                break;

            case FieldType.Color:
                content = EditorGUILayout.ColorField("字段内容", data.GetColor()).ToSaveString();
                break;
        }

        return content;
    }
}
