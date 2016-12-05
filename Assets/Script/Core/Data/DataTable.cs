using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class DataTable : Dictionary<string, SingleData>
{
    const char c_split   = '\t';
    const string c_newline = "\r\n";

    const string c_defaultValueTableTitle = "default";
    const string c_noteTableTitle         = "note";
    const string c_fieldTypeTableTitle    = "type";

    const char c_EnumSplit = '|';

    public string m_tableName;

    /// <summary>
    /// 默认值
    /// </summary>
    public Dictionary<string, string> m_defaultValue = new Dictionary<string,string>();

    /// <summary>
    /// 注释
    /// </summary>
    public Dictionary<string, string> m_noteValue = new Dictionary<string, string>();

    /// <summary>
    /// 储存每个字段是什么类型
    /// </summary>
    public Dictionary<string, FieldType> m_tableTypes = new Dictionary<string,FieldType>();
    /// <summary>
    /// 如果是枚举类型，这里储存二级类型
    /// </summary>
    public Dictionary<string, string> m_tableEnumTypes = new Dictionary<string, string>();

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
            string[] line = stringData.Split(c_newline.ToCharArray());

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
                if (line[lineIndex] != "" && line[lineIndex] != null)
                {
                    LineData = ConvertStringArray(line[lineIndex]);

                    //注释
                    if (LineData[0].Equals(c_noteTableTitle))
                    {
                        AnalysisNoteValue(data, LineData);
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
            }    

            data.TableIDs = new List<string>();

            //开始解析数据
            for (int i = lineIndex; i < line.Length; i++)
            {
                SingleData dataTmp = new SingleData();
                dataTmp.data = data;

                if (line[i] != "" && line[i] != null)
                {
                    string[] row = ConvertStringArray(line[i]);

                    for (int j = 0; j < data.TableKeys.Count; j++)
                    {
                        if (!row[j].Equals(""))
                        {
                            dataTmp.Add(data.TableKeys[j], row[j]);
                        }
                    }

                    //第一个数据作为这一个记录的Key
                    data.AddData(dataTmp);
                }

            }

            return data;
        }
        catch (Exception e)
        {
            throw new Exception("Analysis: Don't convert value to DataTable:" + "\n" + e.ToString()); // throw  
        }
    }

    /// <summary>
    /// 解析注释
    /// </summary>
    /// <param name="l_data"></param>
    /// <param name="l_lineData"></param>
    public static void AnalysisNoteValue(DataTable l_data, string[] l_lineData)
    {
        l_data.m_noteValue = new Dictionary<string, string>();

        for (int i = 0; i < l_lineData.Length && i < l_data.TableKeys.Count; i++)
        {
            if (!l_lineData[i].Equals(""))
            {
                l_data.m_noteValue.Add(l_data.TableKeys[i], l_lineData[i]);
            }
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
                string[] content = l_lineData[i].Split(c_EnumSplit);
                l_data.m_tableTypes.Add(l_data.TableKeys[i], (FieldType)Enum.Parse(typeof(FieldType), content[0]));

                if (content.Length >1)
                {
                    l_data.m_tableEnumTypes.Add(l_data.TableKeys[i], content[1]);
                }
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

                    if (data.m_tableEnumTypes.ContainsKey(key))
                    {
                        typeString += c_EnumSplit + data.m_tableEnumTypes[key];
                    }
                }
                //默认字符类型
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

        //note
        List<string> noteValue = new List<string>(data.m_noteValue.Keys);
        if (noteValue.Count > 0)
        {
            build.Append(c_noteTableTitle);
            build.Append(c_split);
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];
                string defauleNoteTmp = "";

                if (data.m_noteValue.ContainsKey(key))
                {
                    defauleNoteTmp = data.m_noteValue[key];
                }
                else
                {
                    defauleNoteTmp = "";
                }

                build.Append(defauleNoteTmp);

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

    public void SetFieldType(string key,FieldType type ,string enumType)
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

        //存储二级类型
        if (enumType != null)
        {
            if (m_tableEnumTypes.ContainsKey(key))
            {
                m_tableEnumTypes[key] = enumType;
            }
            else
            {
                m_tableEnumTypes.Add(key, enumType);
            }
        }
    }

    public string GetEnumType(string key)
    {
        if (m_tableEnumTypes.ContainsKey(key))
        {
            return m_tableEnumTypes[key];
        }
        else
        {
            return null;
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

    public void SetNote(string key, string note)
    {
        if (!m_noteValue.ContainsKey(key))
        {
            m_noteValue.Add(key, note);
        }
        else
        {
            m_noteValue[key] = note;
        }
    }

    public string GetNote(string key)
    {
        if (!m_noteValue.ContainsKey(key))
        {
            return null;
        }
        else
        {
            return m_noteValue[key];
        }
    }

    public void AddData(SingleData data)
    {
        data.m_SingleDataName = data[TableKeys[0]];

        if(data.ContainsKey(TableKeys[0]))
        {
            Add(data[TableKeys[0]], data);

            TableIDs.Add(data[TableKeys[0]]);
        }
        else
        {
            throw new Exception("Add SingleData fail! The dataTable dont have MainKeyKey!");
        }
    }

    public void SetData(SingleData data)
    {
        //主键
        string mainKey = TableKeys[0];

        if (data.ContainsKey(mainKey))
        {
            string key = data[mainKey];
            if (ContainsKey(key))
            {
                this[key] = data;
            }
            else
            {
                Add(key, data);
                TableIDs.Add(key);
            }
        }
        else
        {
            throw new Exception("Add SingleData fail! The dataTable dont have MainKeyKey!");
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
    public string m_SingleDataName;
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

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-");// throw  
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

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-"); // throw  
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

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-"); // throw  
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

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-");// throw  
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

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-"); // throw  
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

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-"); // throw  
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

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-"); // throw  
    }

    public T GetEnum<T>(string key) where T:struct
    {
        if (this.ContainsKey(key))
        {
            return (T)Enum.Parse(typeof(T) ,this[key]);
        }

        if (data.m_defaultValue.ContainsKey(key))
        {
            return (T)Enum.Parse(typeof(T), data.m_defaultValue[key]); ;
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataName + "<-"); // throw  
    }
}

