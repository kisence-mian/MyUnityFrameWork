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
    const char c_DataFieldAssetTypeSplit = '&';

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
    /// 数组分割符号（字段名,分割字符）
    /// </summary>
    public Dictionary<string, char[]> m_ArraySplitFormat = new Dictionary<string, char[]>();
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
    public List<string> TableIDs = new List<string>();

    /// <summary>
    /// 字段的用途区分
    /// </summary>
    public Dictionary<string, DataFieldAssetType> m_fieldAssetTypes = new Dictionary<string, DataFieldAssetType>();

    /// <summary>
    /// 将文本解析为表单数据
    /// </summary>
    /// <param name="stringData">文本</param>
    /// <returns>表单数据</returns>
    public static DataTable Analysis(string stringData)
    {
        string debugContent = "";
        int debugLineCount = 0;
        int debugRowCount = 0;

        string debugKey = "";
        string debugProperty = "";

        try
        {
            int lineIndex = 0;
            DataTable data = new DataTable();
            string[] line = stringData.Split(c_newline.ToCharArray());

            //第一行作为Key
            debugContent = "解析Key";
            data.TableKeys = new List<string>();
            string[] rowKeys = ConvertStringArray(line[0]);
            for (int i = 0; i < rowKeys.Length; i++)
            {
                debugRowCount = i;
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
                    debugLineCount = lineIndex;
                    LineData = ConvertStringArray(line[lineIndex]);

                    //注释
                    if (LineData[0].Equals(c_noteTableTitle))
                    {
                        debugContent = "解析注释";
                        AnalysisNoteValue(data, LineData);
                    }
                    //默认值
                    else if (LineData[0].Equals(c_defaultValueTableTitle))
                    {
                        debugContent = "解析默认值";
                        AnalysisDefaultValue(data, LineData);
                    }
                    //数据类型
                    else if (LineData[0].Equals(c_fieldTypeTableTitle))
                    {
                        debugContent = "解析类型";
                        AnalysisFieldType(data, LineData);
                    }
                    //数据正文
                    else
                    {
                        debugContent = "解析正文";
                        break;
                    }
                }
            }    

            data.TableIDs = new List<string>();

            //开始解析数据
            for (int i = lineIndex; i < line.Length; i++)
            {
                debugLineCount = i;
                SingleData dataTmp = new SingleData();
                dataTmp.data = data;

                if (line[i] != "" && line[i] != null)
                {
                    string[] row = ConvertStringArray(line[i]);

                    for (int j = 0; j < data.TableKeys.Count; j++)
                    {
                        debugRowCount = j;
                        debugKey = row[0];
                        if (!row[j].Equals(""))
                        {
                            debugProperty = data.TableKeys[j];
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
            throw new Exception("DataTable Analysis Error: 错误位置：" + debugContent + " 行:" + debugLineCount / 2 + " 列：" + debugRowCount + " key:->" + debugKey + "<- PropertyName：->" +debugProperty+ "<-\n" + e.ToString()); // throw  
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
                string field = l_data.TableKeys[i];

                string[] tempType = l_lineData[i].Split(c_DataFieldAssetTypeSplit);
                string[] content = tempType[0].Split(c_EnumSplit);

                try
                {
                    string fieldType = content[0];
                    if (fieldType.Contains("["))
                    {
                        string[] tempSS = fieldType.Split('[');
                        fieldType = tempSS[0];
                        string splitStr = tempSS[1].Replace("]", "");

                        l_data.m_ArraySplitFormat.Add(field, splitStr.ToCharArray());
                    }
                    l_data.m_tableTypes.Add(field, (FieldType)Enum.Parse(typeof(FieldType), fieldType));

                    if (content.Length > 1)
                    {
                        l_data.m_tableEnumTypes.Add(field, content[1]);
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("AnalysisFieldType Exception: " + content + "\n" + e.ToString());
                }

                if (tempType.Length > 1)
                {
                    l_data.m_fieldAssetTypes.Add(field, (DataFieldAssetType)Enum.Parse(typeof(DataFieldAssetType), tempType[1]));
                }
                else
                {
                    l_data.m_fieldAssetTypes.Add(field, DataFieldAssetType.Data);
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
        //Debug.Log("type count " + type.Count);
        build.Append(c_fieldTypeTableTitle);

        if (type.Count > 0)
        {
            build.Append(c_split);

            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];
                string typeString = "";
                
                if (data.m_tableTypes.ContainsKey(key))
                {
                    typeString = data.m_tableTypes[key].ToString();

                    if (data.m_ArraySplitFormat.ContainsKey(key))
                    {
                        typeString += "[";
                        foreach (var item in data.m_ArraySplitFormat[key])
                        {
                            typeString += item;
                        }
                        typeString += "]";
                    }

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

                if (data.m_fieldAssetTypes.ContainsKey(key))
                {
                    if (data.m_fieldAssetTypes[key] != DataFieldAssetType.Data)
                        typeString += "&" + data.m_fieldAssetTypes[key];
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
        else
        {
            build.Append(c_newline);
        }

        //note
        List<string> noteValue = new List<string>(data.m_noteValue.Keys);
        build.Append(c_noteTableTitle);

        if (noteValue.Count > 0)
        {
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
        else
        {
            build.Append(c_newline);
        }

        //defauleValue
        List<string> defaultValue = new List<string>(data.m_defaultValue.Keys);

        build.Append(c_defaultValueTableTitle);

        if (defaultValue.Count >0)
        {
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
        else
        {
            build.Append(c_newline);
        }

        //value
        foreach (string k in data.TableIDs)
        {
            SingleData dataTmp = data[k];
            for (int i = 0; i < data.TableKeys.Count; i++)
            {
                string valueTmp = "";
                string field = data.TableKeys[i];
                string defaultV="";
                if (data.m_defaultValue.ContainsKey(field))
                    defaultV = data.m_defaultValue[field];
                if (dataTmp.ContainsKey(field) && dataTmp[field]!= defaultV)
                {
                    valueTmp = dataTmp[field];
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
    public char[] GetArraySplitFormat(string key)
    {
        if (m_ArraySplitFormat.ContainsKey(key))
        {
            return m_ArraySplitFormat[key];
        }
        return new char[0];
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

    public void SetAssetTypes(string key, DataFieldAssetType type)
    {
        //主键只能是String类型
        if (key == TableKeys[0])
        {
            return;
        }

        if (m_fieldAssetTypes.ContainsKey(key))
        {
            m_fieldAssetTypes[key] = type;
        }
        else
        {
            m_fieldAssetTypes.Add(key, type);
        }
    }

    public SingleData GetLineFromKey(string key)
    {
        //主键只能是String类型
        SingleData _value = null;
        if (ContainsKey(key))
            _value = this[key];



        return _value;
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
        if(data.ContainsKey(TableKeys[0]))
        {
            data.m_SingleDataKey = data[TableKeys[0]];
            Add(data[TableKeys[0]], data);
            TableIDs.Add(data[TableKeys[0]]);
        }
        else
        {
            throw new Exception("Add SingleData fail! The dataTable dont have MainKey!");
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
    /// <summary>
    /// 该记录的key
    /// </summary>
    public string m_SingleDataKey;
    public int GetInt(string key)
    {
        string content = null;

        try
        {
            if (this.ContainsKey(key))
            {
                content = this[key];
                return ParseTool.GetInt(content);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                content = data.m_defaultValue[key];
                return ParseTool.GetInt(content);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetInt Error TableName is :->" + data.m_tableName + "<- key : ->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- content: ->" + content + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue TableName is :->" + data.m_tableName + "<- key : ->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<-");// throw  
    }

    public int[] GetIntArray(string key)
    {
        string content = null;

        try
        {
            if (this.ContainsKey(key))
            {
                content = this[key];
                return ParseTool.String2IntArray(content);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                content = data.m_defaultValue[key];
                return ParseTool.String2IntArray(content);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetIntArray Error TableName is :->" + data.m_tableName + "<- key : ->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- content: ->" + content + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue TableName is :->" + data.m_tableName + "<- key : ->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<-");// throw  
    }

    public float GetFloat(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.GetFloat(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.GetFloat(data.m_defaultValue[key]);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetFloat Error TableName is :->" + data.m_tableName + "<- key :->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public float[] GetFloatArray(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2FloatArray(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2FloatArray(data.m_defaultValue[key]);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetFloatArray Error TableName is :->" + data.m_tableName + "<- key :->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public bool GetBool(string key)
    {
        string content = null;

        try
        {
            if (this.ContainsKey(key))
            {
                content = this[key];
                return ParseTool.GetBool(content);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                content = data.m_defaultValue[key];
                return ParseTool.GetBool(content);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetBool Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- content: ->" + content + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public bool[] GetBoolArray(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2BoolArray(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2BoolArray(data.m_defaultValue[key]);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetBoolArray Error TableName is :->" + data.m_tableName + "<- key :->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public string GetString(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                //String 读取null 的改进，兼容旧代码
#if Compatibility
                return this[key];
#else
                return ParseTool.GetString(this[key]);
#endif
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
#if Compatibility
                return data.m_defaultValue[key];
#else
                return ParseTool.GetString(data.m_defaultValue[key]);
#endif
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetString Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-");// throw  
    }

    string StringFilter(string content)
    {
        if(content == "Null"
            || content == "null"
            || content == "NULL"
            || content == "nu11"
            || content == "none"
            || content == "nil"
            || content == "")
        {
            return null;
        }
        else
        {
            return content;
        }
    }

    public Vector2 GetVector2(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2Vector2(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2Vector2(data.m_defaultValue[key]);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetVector2 Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public Vector2[] GetVector2Array(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2Vector2Array(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2Vector2Array(data.m_defaultValue[key]);
            }

        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetVector2 Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }


    public Vector3[] GetVector3Array(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2Vector3Array(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2Vector3Array(data.m_defaultValue[key]);
            }

        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetVector3Array Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public Vector3 GetVector3(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2Vector3(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2Vector3(data.m_defaultValue[key]);
            }

        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetVector3 Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public Color GetColor(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2Color(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2Color(data.m_defaultValue[key]);
            }
        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetColor Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public T GetEnum<T>(string key) where T : struct
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return (T)Enum.Parse(typeof(T), this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return (T)Enum.Parse(typeof(T), data.m_defaultValue[key]);
            }

        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetEnum Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-"); // throw  
    }

    public string[] GetStringArray(string key)
    {
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2StringArray(this[key]);
            }

            if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2StringArray(data.m_defaultValue[key]);
            }

        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetStringArray Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-");// throw  
    }
    public T[] GetArray<T>(string key)
    {
        return (T[])GetArray(key);
    }
        public Array GetArray(string key)
    {
        
        try
        {
            if (this.ContainsKey(key))
            {
                return ParseTool.String2Array(data.GetFieldType(key), this[key], data.GetArraySplitFormat(key));
            }

            else if (data.m_defaultValue.ContainsKey(key))
            {
                return ParseTool.String2Array(data.GetFieldType(key), data.m_defaultValue[key], data.GetArraySplitFormat(key));
            }

        }
        catch (Exception e)
        {
            throw new Exception("SingleData GetStringArray2 Error TableName is :->" + data.m_tableName + "<- key->" + key + "<-  singleDataName : ->" + m_SingleDataKey + "<- \n" + e.ToString());
        }

        throw new Exception("Don't Exist Value or DefaultValue by ->" + key + "<- TableName is : ->" + data.m_tableName + "<- singleDataName : ->" + m_SingleDataKey + "<-");// throw  
    }
}

