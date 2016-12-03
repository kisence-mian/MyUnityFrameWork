using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class EditorUtilGUI 
{
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
        SingleField data = new SingleField(type, content, enumType);
        return EditorUtilGUI.FieldGUI_Type(data, labelContent);
    }

    public static string FieldGUI_Type(SingleField data, string labelContent = "字段内容")
    {
        string content = "";

        switch (data.m_type)
        {
            case FieldType.String:
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
}
