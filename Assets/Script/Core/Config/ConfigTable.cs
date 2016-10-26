using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfigTable :  Dictionary<string, SingleData> 
{

}

[SerializeField]
public class SingleConfig
{
    public ConfigType m_type;

    public string m_content;

    #region 构造函数

    public SingleConfig()
    {
        m_type = ConfigType.String;
        m_content = "";
    }

    public SingleConfig(string contrnt)
    {
        m_type = ConfigType.String;
        m_content = contrnt;
    }

    public SingleConfig(int contrnt)
    {
        m_type = ConfigType.Int;
        m_content = contrnt.ToString();
    }

    public SingleConfig(float contrnt)
    {
        m_type = ConfigType.Float;
        m_content = contrnt.ToString();
    }

    public SingleConfig(bool contrnt)
    {
        m_type = ConfigType.Bool;
        m_content = contrnt.ToString();
    }

    public SingleConfig(Vector2 contrnt)
    {
        m_type = ConfigType.Vector2;
        m_content = contrnt.ToSaveString();
    }

    public SingleConfig(Vector3 contrnt)
    {
        m_type = ConfigType.Vector3;
        m_content = contrnt.ToSaveString();
    }

    public SingleConfig(Color contrnt)
    {
        m_type = ConfigType.Color;
        m_content = contrnt.ToSaveString();
    }

    #endregion

    #region ReSet

    public void Reset()
    {
        switch (m_type)
        {
            case ConfigType.Bool:
                m_content = false.ToString();
                break;
            case ConfigType.Vector2:
                m_content = Vector2.zero.ToSaveString();
                break;
            case ConfigType.Vector3:
                m_content = Vector3.zero.ToSaveString();
                break;
            case ConfigType.Color:
                m_content = Color.white.ToSaveString();
                break;
            case ConfigType.Float:
                m_content = (0.0f).ToString();
                break;
            case ConfigType.Int:
                m_content = (0).ToString();
                break;
        }
    }

    #endregion

    #region 取值封装

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

public enum ConfigType
{
    String,
    Bool,
    Int,
    Float,
    Vector2,
    Vector3,
    Color,
}
