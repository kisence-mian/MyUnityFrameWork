using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using HDJ.Framework.Utils;


public static class DataConfigUtils
{
    public static Type ConfigFieldValueType2Type(FieldType fieldValueType,string enumType)
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
                t = typeof(string[]);
                break;
            case FieldType.IntArray:
                t = typeof(int[]);
                break;
            case FieldType.FloatArray:
                t = typeof(float[]);
                break;
            case FieldType.BoolArray:
                t = typeof(bool[]);
                break;
            case FieldType.Vector2Array:
                t = typeof(Vector2[]);
                break;
            case FieldType.Vector3Array:
                t = typeof(Vector3[]);
                break;
            default:
                break;
        }

        return t;
    }

    public static string ObjectValue2TableString(object value)
    {
        string res = "";
        if (value == null)
            return res;
        
        Type t = value.GetType();
        if (t.IsArray)
        {
            PropertyInfo pL= t.GetProperty("Length");
          int length= (int) pL.GetValue(value, null);
            MethodInfo mth= t.GetMethod("GetValue", new Type[] { typeof(int) });
            for (int i = 0; i < length; i++)
            {
                object v = mth.Invoke(value, new object[] { i });
                res += SingleData2String(v);
                if (i < length - 1)
                    res += "|";
            }

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
        else
            return value.ToString();
    }
    public static object TableString2ObjectValue( string v, FieldType fieldValueType, string enumType)
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
               Type type = ConfigFieldValueType2Type(fieldValueType,enumType);
              t=  Enum.Parse(type, v);
                break;
            case FieldType.StringArray:
                t = ParseTool.String2StringArray(v);
                break;
            case FieldType.IntArray:
                t = ParseTool.String2IntArray(v);
                break;
            case FieldType.FloatArray:
                t = ParseTool.String2FloatArray(v);
                break;
            case FieldType.BoolArray:
                t = ParseTool.String2BoolArray(v);
                break;
            case FieldType.Vector2Array:
                t = ParseTool.String2Vector2(v);
                break;
            case FieldType.Vector3Array:
                t = ParseTool.String2Vector3Array(v);
                break;
            default:
                break;
        }

        return t;

    }
}

