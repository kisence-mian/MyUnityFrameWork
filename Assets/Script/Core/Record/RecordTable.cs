using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RecordTable : Dictionary<string, SingleField>
{
    #region 解析
    public static RecordTable Analysis(string data)
    {
        RecordTable result = new RecordTable();
        Dictionary<string, SingleField> tmp = JsonTool.Json2Dictionary<SingleField>(data);

        

        List<string> keys = new List<string>(tmp.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            //Debug.Log("Key: " + keys[i]);

            result.Add(keys[i],tmp[keys[i]]);
        }

        return result;
    }

    public static string Serialize(RecordTable table)
    {
        return JsonTool.Dictionary2Json<SingleField>(table); ;
    }

    #endregion

    #region 取值封装

    public SingleField GetRecord(string key)
    {
        if(this.ContainsKey(key))
        {
            return this[key];
        }
        else
        {
            throw new Exception("RecordTable Not Find " + key);
        }
    }

    public string GetRecord(string key, string defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetString() ;
        }
        else
        {
            return new SingleField(defaultValue).GetString();
        }
    }

    public bool GetRecord(string key, bool defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetBool();
        }
        else
        {
            return new SingleField(defaultValue).GetBool();
        }
    }

    public int GetRecord(string key, int defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetInt();
        }
        else
        {
            return new SingleField(defaultValue).GetInt();
        }
    }

    public float GetRecord(string key, float defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetFloat();
        }
        else
        {
            return new SingleField(defaultValue).GetFloat();
        }
    }

    public Vector2 GetRecord(string key, Vector2 defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetVector2();
        }
        else
        {
            return new SingleField(defaultValue).GetVector2();
        }
    }

    public Vector3 GetRecord(string key, Vector3 defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetVector3();
        }
        else
        {
            return new SingleField(defaultValue).GetVector3();
        }
    }

    public Color GetRecord(string key, Color defaultValue)
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetColor();
        }
        else
        {
            return new SingleField(defaultValue).GetColor();
        }
    }

    #endregion

    #region 存值封装

    public void SetRecord(string key,string value)
    {
        if(this.ContainsKey(key))
        {
            this[key] = new SingleField(value);
        }
        else
        {
            this.Add(key, new SingleField(value));
        }
    }

    public void SetRecord(string key, int value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = new SingleField(value);
        }
        else
        {
            this.Add(key, new SingleField(value));
        }
    }

    public void SetRecord(string key, bool value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = new SingleField(value);
        }
        else
        {
            this.Add(key, new SingleField(value));
        }
    }

    public void SetRecord(string key, float value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = new SingleField(value);
        }
        else
        {
            this.Add(key, new SingleField(value));
        }
    }

    public void SetRecord(string key, Vector2 value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = new SingleField(value);
        }
        else
        {
            this.Add(key, new SingleField(value));
        }
    }

    public void SetRecord(string key, Vector3 value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = new SingleField(value);
        }
        else
        {
            this.Add(key, new SingleField(value));
        }
    }

    public void SetRecord(string key, Color value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = new SingleField(value);
        }
        else
        {
            this.Add(key, new SingleField(value));
        }
    }

    #endregion
}
