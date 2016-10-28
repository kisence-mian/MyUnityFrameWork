using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigTable :  Dictionary<string, SingleData> 
{

}

[SerializeField]
public class SingleField
{
    public FieldType m_type;

    public string m_content;

    #region 构造函数

    public SingleField()
    {
        m_type = FieldType.String;
        m_content = "";
    }

    public SingleField(FieldType type,string content)
    {
        m_type = type;
        m_content = content;

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

    public SingleField(float contrnt)
    {
        m_type = FieldType.Float;
        m_content = contrnt.ToString();
    }

    public SingleField(bool contrnt)
    {
        m_type = FieldType.Bool;
        m_content = contrnt.ToString();
    }

    public SingleField(Vector2 contrnt)
    {
        m_type = FieldType.Vector2;
        m_content = contrnt.ToSaveString();
    }

    public SingleField(Vector3 contrnt)
    {
        m_type = FieldType.Vector3;
        m_content = contrnt.ToSaveString();
    }

    public SingleField(Color contrnt)
    {
        m_type = FieldType.Color;
        m_content = contrnt.ToSaveString();
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
        }
    }

    #endregion

    #region 取值封装

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
}
