using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class DataTable : Dictionary<string, SingleData>
{
    const char c_split   = '\t';
    const char c_newline = '\n';

    const string c_defaultValueTableTitle = "default";
    const string c_noteTableTitle         = "note";
    const string c_fieldTypeTableTitle    = "type";

    /// <summary>
    /// 默认值
    /// </summary>
    public Dictionary<string, string> m_defaultValue = new Dictionary<string,string>();

    /// <summary>
    /// 储存每个字段是什么类型
    /// </summary>
    public Dictionary<string, FieldType> m_tableTypes = new Dictionary<string,FieldType>();

    /// <summary>
    /// 单条记录所拥有的字段名
    /// </summary>
    public List<string> TableKeys = new List<string>();

    /// <summary>
    /// 数据所有的Key
    /// </summary>
    public List<string> TableIDs;

    /// <summary>
    /// 将文本解析为表单数据
    /// </summary>
    /// <param name="stringData">文本</param>
    /// <returns>表单数据</returns>
    public static DataTable Analysis(string stringData)
    {
        try
        {
            int lineIndex = 0;
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

            string[] LineData;
            for (lineIndex = 1; lineIndex < line.Length; lineIndex++)
            {
                LineData = ConvertStringArray(line[lineIndex]);

                //注释忽略
                if (LineData[0].Equals(c_noteTableTitle))
                {
                    //nothing
                }
                //默认值
                else if (LineData[0].Equals(c_defaultValueTableTitle))
                {
                    AnalysisDefaultValue(data, LineData);
                }
                //数据类型
                else if (LineData[0].Equals(c_fieldTypeTableTitle))
                {
                    AnalysisFieldType(data, LineData);
                }
                //数据正文
                else
                {
                    break;
                }
            }    

            data.TableIDs = new List<string>();

            //开始解析数据
            for (int i = lineIndex; i < line.Length; i++)
            {
                SingleData dataTmp = new SingleData();
                dataTmp.data = data;
                string[] row = ConvertStringArray(line[i]);

                for (int j = 0; j < data.TableKeys.Count; j++)
                {
                    if (!row[j].Equals(""))
                    {

                        //Debug.Log("j:" + j + "  " + data.TableKeys.Count + "  " + row.Length);
                        dataTmp.Add(data.TableKeys[j], row[j]);
                    }
                }

                //第一个数据作为这一个记录的Key
                //data.Add(row[0], dataTmp);
                //data.TableIDs.Add(row[0]);

                data.AddData(dataTmp);
            }

            return data;
        }
        catch (Exception e)
        {
            throw new Exception("Analysis: Don't convert value to DataTable:" + "\n" + e.ToString()); // throw  
        }
    }

    public static void AnalysisDefaultValue(DataTable l_data,string[] l_lineData)
    {
        l_data.m_defaultValue = new Dictionary<string, string>();

        for (int i = 0; i < l_lineData.Length && i < l_data.TableKeys.Count; i++)
        {
            if (!l_lineData[i].Equals(""))
            {
                l_data.m_defaultValue.Add(l_data.TableKeys[i], l_lineData[i]);
            }
        }
    }

    public static void AnalysisFieldType(DataTable l_data, string[] l_lineData)
    {
        l_data.m_tableTypes = new Dictionary<string, FieldType>();

        for (int i = 1; i < l_lineData.Length && i < l_data.TableKeys.Count; i++)
        {
            if (!l_lineData[i].Equals(""))
            {
                l_data.m_tableTypes.Add(l_data.TableKeys[i], (FieldType)Enum.Parse(typeof(FieldType), l_lineData[i]));
            }
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
                build.Append(c_split);
            }
            else
            {
                build.Append(c_newline);
            }
        }

        //type
        List<string> type = new List<string>(data.m_tableTypes.Keys);
        if (type.Count > 0)
        {
            build.Append(c_fieldTypeTableTitle);
            build.Append(c_split);
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];
                string typeString = "";

                if (data.m_tableTypes.ContainsKey(key))
                {
                    typeString = data.m_tableTypes[key].ToString();
                }
                else
                {
                    typeString = FieldType.String.ToString();
                }

                build.Append(typeString);

                if (i != data.TableKeys.Count - 1)
                {
                    build.Append(c_split);
                }
                else
                {
                    build.Append(c_newline);
                }
            }
        }

        //defauleValue
        List<string> defaultValue = new List<string>(data.m_defaultValue.Keys);

        if (defaultValue.Count >0)
        {
            build.Append(c_defaultValueTableTitle);
            build.Append(c_split);
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];
                string defauleValueTmp = "";

                if (data.m_defaultValue.ContainsKey(key))
                {
                    defauleValueTmp = data.m_defaultValue[key];
                }
                else
                {
                    defauleValueTmp = "";
                }

                build.Append(defauleValueTmp);

                if (i != data.TableKeys.Count - 1)
                {
                    build.Append(c_split);
                }
                else
                {
                    build.Append(c_newline);
                }
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
                    build.Append(c_split);
                }
                else
                {
                    build.Append(c_newline);
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

        for (int i = 0; i < lineContent.Length; i++)
        {
            if (state)
            {
                if (lineContent[i] == c_split)
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

    public FieldType GetFieldType(string key)
    {
        //主键只能是String类型
        if (key == TableKeys[0])
        {
            return FieldType.String;
        }

        if(m_tableTypes.ContainsKey(key))
        {
            return m_tableTypes[key];
        }
        else
        {
            return FieldType.String;
        }
    }

    public void SetFieldType(string key,FieldType type )
    {
        //主键只能是String类型
        if (key == TableKeys[0])
        {
            return;
        }

        if (m_tableTypes.ContainsKey(key))
        {
            m_tableTypes[key] = type;
        }
        else
        {
            m_tableTypes.Add(key,type);
        }
    }

    public string GetDefault(string key)
    {
        if(m_defaultValue.ContainsKey(key))
        {
            return m_defaultValue[key];
        }
        else
        {
            return null;
        }
    }

    public void SetDefault(string key,string value)
    {
        if (!m_defaultValue.ContainsKey(key))
        {
            m_defaultValue.Add(key, value);
        }
        else
        {
            m_defaultValue[key] = value;
        }
    }

    public void AddData(SingleData data)
    {
        if(data.ContainsKey(TableKeys[0]))
        {
            Add(data[TableKeys[0]], data);

            TableIDs.Add(data[TableKeys[0]]);
        }
        else
        {
            throw new Exception("Add SingleData fail!");
        }
    }

    public void RemoveData(string key)
    {
        if (ContainsKey(key))
        {
            Remove(key);
            TableIDs.Remove(key);
        }
        else
        {
            throw new Exception("Add SingleData fail!");
        }
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

        if (data.m_defaultValue.ContainsKey(key))
        {
            return int.Parse(data.m_defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public float GetFloat(string key)
    {
        if (this.ContainsKey(key))
        {
            return float.Parse(this[key]);
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            return float.Parse(data.m_defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public bool GetBool(string key)
    {
        if (this.ContainsKey(key))
        {
            return bool.Parse(this[key]);
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            return bool.Parse(data.m_defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public string GetString(string key)
    {
        if (this.ContainsKey(key))
        {
            return this[key];
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            return data.m_defaultValue[key];
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public Vector2 GetVector2(string key)
    {
        if (this.ContainsKey(key))
        {
            return ParseTool.String2Vector2(this[key]);
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            return ParseTool.String2Vector2(data.m_defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public Vector3 GetVector3(string key)
    {
        if (this.ContainsKey(key))
        {
            return ParseTool.String2Vector3(this[key]);
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            return ParseTool.String2Vector3(data.m_defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }

    public Color GetColor(string key)
    {
        if (this.ContainsKey(key))
        {
            return ParseTool.String2Color(this[key]);
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            return ParseTool.String2Color(data.m_defaultValue[key]);
        }

        throw new Exception("Don't Exist Value or DefaultValue by " + key); // throw  
    }
}

