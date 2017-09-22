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
        return JsonTool.Dictionary2Json<SingleField>(table);
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
            return defaultValue;
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
            return defaultValue;
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
            return defaultValue;
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
            return defaultValue;
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
            return defaultValue;
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
            return defaultValue;
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
            return defaultValue;
        }
    }

    public T GetEnumRecord<T>(string key, T defaultValue) where T:struct
    {
        if (this.ContainsKey(key))
        {
            return this[key].GetEnum<T>();
        }
        else
        {
            return defaultValue;
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

    public void SetEnumRecord(string key, Enum value)
    {
        if (this.ContainsKey(key))
        {
            this[key] = new SingleField(value.ToString());
        }
        else
        {
            this.Add(key, new SingleField(value.ToString()));
        }
    }

    #endregion
}
