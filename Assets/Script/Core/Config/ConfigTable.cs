using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConfigTable :  Dictionary<string, SingleData> 
{

}

[SerializeField]
public class SingleField
{
    public FieldType m_type;

    public string m_content;
    public string m_enumType;

    #region 构造函数

    public SingleField()
    {
        m_type = FieldType.String;
        m_content = "";
    }

    public SingleField(FieldType type,string content,string enumType)
    {
        m_type = type;
        m_content = content;
        m_enumType = enumType;

        if (content == null)
        {
            Reset();
        }
    }

    public SingleField(string contrnt)
    {
        m_type = FieldType.String;
        m_content = contrnt;
    }

    public SingleField(int contrnt)
    {
        m_type = FieldType.Int;
        m_content = contrnt.ToString();
    }

    public SingleField(float content)
    {
        m_type = FieldType.Float;
        m_content = content.ToString();
    }

    public SingleField(bool content)
    {
        m_type = FieldType.Bool;
        m_content = content.ToString();
    }

    public SingleField(Vector2 content)
    {
        m_type = FieldType.Vector2;
        m_content = content.ToSaveString();
    }

    public SingleField(Vector3 content)
    {
        m_type = FieldType.Vector3;
        m_content = content.ToSaveString();
    }

    public SingleField(Color content)
    {
        m_type = FieldType.Color;
        m_content = content.ToSaveString();
    }

    public SingleField(List<string> content)
    {
        m_type = FieldType.StringArray;
        m_content = content.ToSaveString();
    }

    public SingleField(Enum content)
    {
        m_type = FieldType.Enum;
        m_enumType = content.GetType().Name;
        m_content = content.ToString();
    }

    #endregion

    #region ReSet

    public void Reset()
    {
        switch (m_type)
        {
            case FieldType.Bool:
                m_content = false.ToString();
                break;
            case FieldType.Vector2:
                m_content = Vector2.zero.ToSaveString();
                break;
            case FieldType.Vector3:
                m_content = Vector3.zero.ToSaveString();
                break;
            case FieldType.Color:
                m_content = Color.white.ToSaveString();
                break;
            case FieldType.Float:
                m_content = (0.0f).ToString();
                break;
            case FieldType.Int:
                m_content = (0).ToString();
                break;
            case FieldType.Enum:

                if (m_enumType != "" && m_enumType != null)
                {
                    m_content = Enum.GetValues(GetEnumType()).GetValue(0).ToString();
                }
                else
                {
                    throw new Exception("EnumType is Null! ");
                }
                break;
        }
    }

    #endregion

    #region 取值封装

    /// <summary>
    /// 显示在编辑器UI上的字符串
    /// </summary>
    /// <returns></returns>
    public string GetShowString()
    {
        switch (m_type)
        {
            case FieldType.Bool:
                return  GetBool().ToString();
            case FieldType.Vector2:
                return  GetVector2().ToString();
            case FieldType.Vector3:
                return  GetVector3().ToString();
            case FieldType.Color:
                return  GetColor().ToString();
            case FieldType.Float:
                return  GetFloat().ToString();
            case FieldType.Int:
                return  GetInt().ToString();
            case FieldType.Enum:
                return GetEnum().ToString();
        }

        return m_content;
    }

    public int GetInt()
    {
        return int.Parse(m_content);
    }

    public float GetFloat()
    {
        return float.Parse(m_content);
    }

    public bool GetBool()
    {
        return bool.Parse(m_content);
    }

    public string GetString()
    {
        return m_content;
    }

    public string[] GetStringArray()
    {
        return ParseTool.String2StringArray(m_content);
    }

    public Vector2 GetVector2()
    {
       return ParseTool.String2Vector2(m_content);
    }

    public Vector3 GetVector3()
    {
        return ParseTool.String2Vector3(m_content);
    }

    public Color GetColor()
    {
        return ParseTool.String2Color(m_content);
    }

    public T GetEnum<T>() where T:struct
    {
        return (T)Enum.Parse(typeof(T), m_content);
    }

    public Enum GetEnum() 
    {
        if (m_content == null && m_content == "")
        {
            throw new Exception("GetEnum Fail Content is null !");
        }

        if (GetEnumType() == null)
        {
            throw new Exception("GetEnum Fail GetEnumType() is null !　->" + m_enumType + "<-");
        }

        try
        {
            return (Enum)Enum.Parse(GetEnumType(), m_content);
        }
        catch(Exception e)
        {
            throw new Exception("Enum Parse Fail! EnumType is ->" + m_enumType + "<-  GetEnumType() is ->" + GetEnumType() + "<- Content is ->" + m_content + "<-\n" + e.ToString());
        }


    }

    public Type GetEnumType()
    {
        return Type.GetType(m_enumType);
    }

    #endregion
}

public enum FieldType
{
    String,
    Bool,
    Int,
    Float,
    Vector2,
    Vector3,
    Color,
    Enum,

    StringArray,
    IntArray,
    FloatArray,
    BoolArray,
    Vector2Array,
    Vector3Array,
}
