using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;



public static class DataConfigUtils
{
    /// <summary>
    /// 将字段类型（FieldType）转换为对应 数据类型（Type）
    /// </summary>
    /// <param name="fieldValueType"></param>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static Type ConfigFieldValueType2Type(FieldType fieldValueType,string enumType, List<char> arraySplitFormat)
    {
        Type t = null;
        switch (fieldValueType)
        {
            case FieldType.String:
                t = typeof(string);
                break;
            case FieldType.Bool:
                t = typeof(bool);
                break;
            case FieldType.Int:
                t = typeof(int);
                break;
            case FieldType.Float:
                t = typeof(float);
                break;
            case FieldType.Vector2:
                t = typeof(Vector2);
                break;
            case FieldType.Vector3:
                t = typeof(Vector3);
                break;
            case FieldType.Color:
                t = typeof(Color);
                break;
            case FieldType.Enum:
                t =  ReflectionUtils.GetTypeByTypeFullName(enumType);
                break;
            case FieldType.StringArray:
                t = ParseTool.CreateInterleavedType(typeof(string), arraySplitFormat.Count + 1);
               // t = typeof(string[]);
                break;
            case FieldType.IntArray:
              //  t = typeof(int[]);
                t = ParseTool.CreateInterleavedType(typeof(int), arraySplitFormat.Count + 1);
                break;
            case FieldType.FloatArray:
                //  t = typeof(float[]);
                t = ParseTool.CreateInterleavedType(typeof(float), arraySplitFormat.Count + 1);
                break;
            case FieldType.BoolArray:
                // t = typeof(bool[]);
                t = ParseTool.CreateInterleavedType(typeof(bool), arraySplitFormat.Count + 1);
                break;
            case FieldType.Vector2Array:
                // t = typeof(Vector2[]);
                t = ParseTool.CreateInterleavedType(typeof(Vector2), arraySplitFormat.Count + 1);
                break;
            case FieldType.Vector3Array:
                // t = typeof(Vector3[]);
                t = ParseTool.CreateInterleavedType(typeof(Vector3), arraySplitFormat.Count + 1);
                break;
            default:
                break;
        }

        return t;
    }
    /// <summary>
    /// 具体数据类型转换为存储的String字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ObjectValue2TableString(object value, List<char> arraySplitFormat)
    {
        string res = "";
        if (value == null)
            return res;
        
        Type t = value.GetType();
        if (t.IsArray)
        {
            //  PropertyInfo pL= t.GetProperty("Length");
            //int length= (int) pL.GetValue(value, null);
            //  MethodInfo mth= t.GetMethod("GetValue", new Type[] { typeof(int) });
            //  for (int i = 0; i < length; i++)
            //  {
            //      object v = mth.Invoke(value, new object[] { i });
            //      res += SingleData2String(v);
            //      if (i < length - 1)
            //          res += "|";
            //  }
            res = ParseTool.ArrayObject2String(value, arraySplitFormat.ToArray());
        }
        else
        {
            res = SingleData2String(value);
        }

        return res;

    }
    /// <summary>
    /// 非数组转换
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string SingleData2String(object value)
    {
        Type t = value.GetType();
        if (t == typeof(Vector2))
        {
            Vector2 v2 = (Vector2)value;
            return v2.x + "," + v2.y;
        }
        if (t == typeof(Vector3))
        {
            Vector3 v3 = (Vector3)value;
            return v3.x + "," + v3.y + "," + v3.z;
        }
        if (t == typeof(Color))
        {
            Color c = (Color)value;
            return c.r + "," + c.g + "," + c.b + "," + c.a;
        }
        else
            return value.ToString();
    }
    /// <summary>
    /// 原始表格字符串类型转换为具体的数据
    /// </summary>
    /// <param name="v"></param>
    /// <param name="fieldValueType"></param>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static object TableString2ObjectValue( string v, FieldType fieldValueType, string enumType,List<char> m_ArraySplitFormat)
    {
        object t = null;
        switch (fieldValueType)
        {
            case FieldType.String:
                t = v;
                break;
            case FieldType.Bool:
                t = bool.Parse(v);
                break;
            case FieldType.Int:
                t = int.Parse(v);
                break;
            case FieldType.Float:
                t = float.Parse(v);
                break;
            case FieldType.Vector2:
                t = ParseTool.String2Vector2(v);
                break;
            case FieldType.Vector3:
                t = ParseTool.String2Vector3(v);
                break;
            case FieldType.Color:
                t = ParseTool.String2Color(v);
                break;
            case FieldType.Enum:
               Type type = ConfigFieldValueType2Type(fieldValueType,enumType, m_ArraySplitFormat);
                try
                {
                    t = Enum.Parse(type, v);
                }
                catch (Exception e)
                {
                    t = Enum.GetValues(type).GetValue(0);
                   // Debug.LogError("fieldValueType:" + fieldValueType + " enumType:" + enumType + " type:" + type + " v:" + v + "\n"+e);
                    throw e;
                }
             
                break;
            case FieldType.StringArray:

                //t = ParseTool.String2StringArray(v);
                //break;
            case FieldType.IntArray:
                //t = ParseTool.String2IntArray(v);
                //break;
            case FieldType.FloatArray:
                //t = ParseTool.String2FloatArray(v);
                //break;
            case FieldType.BoolArray:
                //t = ParseTool.String2BoolArray(v);
                //break;
            case FieldType.Vector2Array:
                //t = ParseTool.String2Vector2(v);
                //break;
            case FieldType.Vector3Array:
                //t = ParseTool.String2Vector3Array(v);
                t = ParseTool.String2Array(fieldValueType, v, m_ArraySplitFormat.ToArray());
            break;
            default:
              
                break;
        }

        return t;

    }


    #region 自动生成代码

  public static   void CreatAllClass(List<String> configFileNames)
    {
        for (int i = 0; i < configFileNames.Count; i++)
        {

            CreatDataCSharpFile(configFileNames[i], DataManager.GetData(configFileNames[i]));

        }

        UnityEditor.AssetDatabase.Refresh();
    }
    /// <summary>
    /// 创建数据表对应的具体数据类
    /// </summary>
    /// <param name="dataName"></param>
    /// <param name="data"></param>
    public static  void CreatDataCSharpFile(string dataName, DataTable data)
    {
        if (dataName.Contains("/"))
        {
            string[] tmp = dataName.Split('/');
            dataName = tmp[tmp.Length - 1];
        }

        string className = dataName + "Generate";
        string content = "";

        content += "using System;\n";
        content += "using UnityEngine;\n\n";

        content += @"//" + className + "类\n";
        content += @"//该类自动生成请勿修改，以避免不必要的损失";
        content += "\n";

        content += "public class " + className + " : DataGenerateBase \n";
        content += "{\n";

        content += "\tpublic string m_key;\n";

        //type
        List<string> type = new List<string>(data.m_tableTypes.Keys);

        //Debug.Log("type count: " + type.Count);

        if (type.Count > 0)
        {
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];
                string enumType = null;

                if (data.m_tableEnumTypes.ContainsKey(key))
                {
                    enumType = data.m_tableEnumTypes[key];
                }
                char[] m_ArraySplitFormat = new char[0];
                if (data.m_ArraySplitFormat.ContainsKey(key))
                {
                    m_ArraySplitFormat = data.m_ArraySplitFormat[key];
                }
                string note = ";";

                if (data.m_noteValue.ContainsKey(key))
                {
                    note = @"; //" + data.m_noteValue[key];
                }

                content += "\t";

                if (data.m_tableTypes.ContainsKey(key))
                {
                    //访问类型 + 字段类型  + 字段名
                    content += "public " + OutPutFieldName(data.m_tableTypes[key], enumType, m_ArraySplitFormat) + " m_" + key + note;
                }
                //默认字符类型
                else
                {
                    //访问类型 + 字符串类型 + 字段名 
                    content += "public " + "string" + " m_" + key + note;
                }

                content += "\n";
            }
        }

        content += "\n";

        content += "\tpublic override void LoadData(string key) \n";
        content += "\t{\n";
        content += "\t\tDataTable table =  DataManager.GetData(\"" + dataName + "\");\n\n";
        content += "\t\tif (!table.ContainsKey(key))\n";
        content += "\t\t{\n";
        content += "\t\t\tthrow new Exception(\"" + className + " LoadData Exception Not Fond key ->\" + key + \"<-\");\n";
        content += "\t\t}\n";
        content += "\n";
        content += "\t\tSingleData data = table[key];\n\n";

        content += "\t\tm_key = key;\n";

        if (type.Count > 0)
        {
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];

                content += "\t\t";

                
                string enumType = null;

                if (data.m_tableEnumTypes.ContainsKey(key))
                {
                    enumType = data.m_tableEnumTypes[key];
                }
                char[] m_ArraySplitFormat = new char[0];
                if (data.m_ArraySplitFormat.ContainsKey(key))
                {
                    m_ArraySplitFormat = data.m_ArraySplitFormat[key];
                }

                if (data.m_tableTypes.ContainsKey(key))
                {
                    content += "m_" + key + " = data." + OutPutFieldFunction(data.m_tableTypes[key], enumType, m_ArraySplitFormat) + "(\"" + key + "\")";
                }
                //默认字符类型
                else
                {
                    content += "m_" + key + " = data." + OutPutFieldFunction(FieldType.String, enumType, m_ArraySplitFormat) + "(\"" + key + "\")";
                    Debug.LogWarning("字段 " + key + "没有配置类型！");
                }

                content += ";\n";
            }
        }

        content += "\t}\n";
        content += "\t public override void LoadData(DataTable table,string key) \n";
        content += "\t{\n";

        content += "\t\tSingleData data = table[key];\n\n";

        content += "\t\tm_key = key;\n";

        if (type.Count > 0)
        {
            for (int i = 1; i < data.TableKeys.Count; i++)
            {
                string key = data.TableKeys[i];

                content += "\t\t";

                string enumType = null;

                if (data.m_tableEnumTypes.ContainsKey(key))
                {
                    enumType = data.m_tableEnumTypes[key];
                }
                char[] m_ArraySplitFormat = new char[0];
                if (data.m_ArraySplitFormat.ContainsKey(key))
                {
                    m_ArraySplitFormat = data.m_ArraySplitFormat[key];
                }
                if (data.m_tableTypes.ContainsKey(key))
                {
                    content += "m_" + key + " = data." + OutPutFieldFunction(data.m_tableTypes[key], enumType, m_ArraySplitFormat) + "(\"" + key + "\")";
                }
                //默认字符类型
                else
                {
                    content += "m_" + key + " = data." + OutPutFieldFunction(FieldType.String, enumType, m_ArraySplitFormat) + "(\"" + key + "\")";
                    Debug.LogWarning("字段 " + key + "没有配置类型！");
                }

                content += ";\n";
            }
        }

        content += "\t}\n";

        content += "}\n";

        string SavePath = Application.dataPath + "/Script/DataClassGenerate/" + className + ".cs";

        EditorUtil.WriteStringByFile(SavePath, content.ToString());
    }
  


    static string OutPutFieldFunction(FieldType fileType, string enumType,char[] m_ArraySplitFormat)
    {
        string arrayFun = "";
        for (int i = 0; i < m_ArraySplitFormat.Length; i++)
        {
            arrayFun += "[]";
        }
       

        switch (fileType)
        {
            case FieldType.Bool: return "GetBool";
            case FieldType.Color: return "GetColor";
            case FieldType.Float: return "GetFloat";
            case FieldType.Int: return "GetInt";
            case FieldType.String: return "GetString";
            case FieldType.Vector2: return "GetVector2";
            case FieldType.Vector3: return "GetVector3";
            case FieldType.Enum: return "GetEnum<" + enumType + ">";

            case FieldType.StringArray:
                arrayFun = "string" + arrayFun;
                break;
            case FieldType.IntArray:
                arrayFun = "int" + arrayFun;
                break;
            case FieldType.FloatArray:
                arrayFun = "float" + arrayFun;
                break;
            case FieldType.BoolArray:
                arrayFun = "bool" + arrayFun;
                break;
            case FieldType.Vector2Array:
                arrayFun = "Vector2" + arrayFun;
                break;
            case FieldType.Vector3Array:
                arrayFun = "Vector3" + arrayFun;
                break;
          
        }
        arrayFun = "GetArray<"+ arrayFun+">";
        return arrayFun;
    }

   static  string OutPutFieldName(FieldType fileType, string enumType, char[] m_ArraySplitFormat)
    {
        string arrayFun = "";
        for (int i = 0; i < m_ArraySplitFormat.Length; i++)
        {
            arrayFun += "[]";
        }
        switch (fileType)
        {
            case FieldType.Bool: return "bool";
            case FieldType.Color: return "Color";
            case FieldType.Float: return "float";
            case FieldType.Int: return "int";
            case FieldType.String: return "string";
            case FieldType.Vector2: return "Vector2";
            case FieldType.Vector3: return "Vector3";
            case FieldType.Enum: return enumType;

            case FieldType.StringArray: return "string[]"+ arrayFun;
            case FieldType.IntArray: return "int[]" + arrayFun;
            case FieldType.FloatArray: return "float[]" + arrayFun;
            case FieldType.BoolArray: return "bool[]" + arrayFun;
            case FieldType.Vector2Array: return "Vector2[]" + arrayFun;
            case FieldType.Vector3Array: return "Vector3[]" + arrayFun;
            default: return "";
        }
    }

    #endregion
}

