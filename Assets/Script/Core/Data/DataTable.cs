using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class DataTable :Dictionary<string ,SingleData>
{
    //默认值
    public Dictionary<string, string> defaultValue;

    /// <summary>
    /// 单条记录所拥有的字段名
    /// </summary>
    public List<string> TableKeys;
    /// <summary>
    /// 将文本解析为表单数据
    /// </summary>
    /// <param name="stringData">文本</param>
    /// <returns>表单数据</returns>
	public static DataTable Analysis(string stringData)
    {
        try
        {
            DataTable data = new DataTable();
            string[] line = stringData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //第一行作为Key
            data.TableKeys = new List<string>();
            string[] rowKeys = ConvertStringArray(line[0]);
            for (int i = 0; i < rowKeys.Length; i++)
            {
                if (!rowKeys[i].Equals(""))
                {
                     data.TableKeys.Add(rowKeys[i]);
                }
            }

            //第二行作为默认值
            data.defaultValue = new Dictionary<string, string>();
            string[] rowDefaultValue = ConvertStringArray(line[1]);
            for (int i = 0; i < rowDefaultValue.Length; i++)
            {
                if (!rowDefaultValue[i].Equals(""))
                {
                    data.defaultValue.Add(data.TableKeys[i], rowDefaultValue[i]);
                }
            }

            //从第三行开始，解析数据
            for (int i = 2; i < line.Length; i++)
            {
                SingleData dataTmp = new SingleData();
                dataTmp.data = data;
                string[] row = ConvertStringArray(line[i]);

                for (int j = 0; j < row.Length; j++)
                {
                    if (!row[j].Equals(""))
                    {
                        dataTmp.Add(data.TableKeys[j], row[j]);
                    }
                }

                //第一个数据作为这一个记录的Key
                data.Add(row[0], dataTmp);
            }

            return data;
        }
        catch (Exception e)
        {
            throw new Exception("Analysis: Don't convert value to DataTable:" + "\n" + e.ToString()); // throw  
        }
    }
    public static string Serialize(DataTable data)
    {
        StringBuilder build = new StringBuilder();
        //key
        for (int i = 0; i < data.TableKeys.Count; i++)
        {
            build.Append(data.TableKeys[i]);
            if (i != data.TableKeys.Count - 1)
            {
                build.Append(",");
            }
            else
            {
                build.Append("\n");
            }
        }

        //defauleValue
        for (int i = 0; i < data.TableKeys.Count; i++)
        {
            string defauleValueTmp = "";

            if (data.defaultValue.ContainsKey(data.TableKeys[i]))
            {
                defauleValueTmp = data.defaultValue[data.TableKeys[i]];
            }

            build.Append(defauleValueTmp);
            if (i != data.TableKeys.Count - 1)
            {
                build.Append(",");
            }
            else
            {
                build.Append("\n");
            }
        }

        //value
        foreach (string k in data.Keys)
        {
            SingleData dataTmp = data[k];
            for (int i = 0; i < data.TableKeys.Count; i++)
            {
                string valueTmp = "";

                if (dataTmp.ContainsKey(data.TableKeys[i]))
                {
                    valueTmp = dataTmp[data.TableKeys[i]];
                }

                build.Append(valueTmp);
                if (i != data.TableKeys.Count - 1)
                {
                    build.Append(",");
                }
                else
                {
                    build.Append("\n");
                }
            }
        }

        return build.ToString();
    }

    public static string[] ConvertStringArray(string lineContent)
    {
        List<string> result = new List<string>();
        int startIndex = 0;
        bool state = true; //逗号状态和引号状态

        for (int i = 0; i < lineContent.Length;i++)
        {
            if(state)
            {
                if(lineContent[i] == '\t')
                {
                    result.Add(lineContent.Substring(startIndex, i - startIndex));
                    startIndex = i + 1;
                }
                else if (lineContent[i] == '\"')
                {
                    //转为引号状态
                    state = false;
                }
            }
            else
            {
                if (lineContent[i] == '\"')
                {
                    //转为逗号状态
                    state = true;
                }
            }
        }

        result.Add(lineContent.Substring(startIndex, lineContent.Length - startIndex));
        return result.ToArray();
    }


}
public class SingleData : Dictionary<string, string>
{
    public DataTable data;
    public int GetInt(string key)
    {
        if (this.ContainsKey(key))
        {
            return int.Parse(this[key]);
        }

        if (data.defaultValue.ContainsKey(key))
        {
            return int.Parse(data.defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public float GetFloat(string key)
    {
        if (this.ContainsKey(key))
        {
            return float.Parse(this[key]);
        }

        if (data.defaultValue.ContainsKey(key))
        {
            return float.Parse(data.defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public string GetString(string key)
    {
        if (this.ContainsKey(key))
        {
            return this[key];
        }

        if (data.defaultValue.ContainsKey(key))
        {
            return data.defaultValue[key];
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public Vector2 GetVector2(string key)
    {
        if (this.ContainsKey(key))
        {
            return ParseVector2(this[key]);
        }

        if (data.defaultValue.ContainsKey(key))
        {
            return ParseVector2(data.defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public Vector3 GetVector3(string key)
    {
        if (this.ContainsKey(key))
        {
            return ParseVector3(this[key]);
        }

        if (data.defaultValue.ContainsKey(key))
        {
            return ParseVector3(data.defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public Color GetColor(string key)
    {
        if (this.ContainsKey(key))
        {
            return ParseColor(this[key]);
        }

        if (data.defaultValue.ContainsKey(key))
        {
            return ParseColor(data.defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }
    
    public Vector2 ParseVector2(string value)
    {
        try
        {
            string[] values = value.Split('|');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);

            return new Vector2(x, y);
        }
        catch(Exception e)
        {
            throw new Exception("ParseVector2: Don't convert value to Vector2 value:" + value + "\n" + e.ToString()); // throw  
        }
    }

    public Vector3 ParseVector3(string value)
    {
        try
        {
            string[] values = value.Split('|');
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = float.Parse(values[2]);

            return new Vector3(x, y, z);
        }
        catch (Exception e)
        {
            throw new Exception("ParseVector3: Don't convert value to Vector3 value:" + value + "\n" + e.ToString()); // throw  
        }
    }

    public Color ParseColor(string value)
    {
        try
        {
            string[] values = value.Split('|');
            float r = float.Parse(values[0]);
            float g = float.Parse(values[1]);
            float b = float.Parse(values[2]);
            float a = 1;

            if (values.Length > 3)
            {
                a = float.Parse(values[3]);
            }

            return new Color(r, g, b,a);
        }
        catch (Exception e)
        {
            throw new Exception("ParseColor: Don't convert value to Color value:" + value + "\n" + e.ToString()); // throw  
        }
    }
}

