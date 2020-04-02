using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;
using LiteNetLibManager;

namespace GameConsoleController
{
    /// <summary>
    /// Json处理工具类
    /// </summary>
    public static class SimpleJsonUtils
    {
        private static Type list_Type = typeof(List<>);
        private static Type dictionary_Type = typeof(Dictionary<,>);
        private static BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        private static Type notJsonSerialized_Type = typeof(NotJsonSerializedAttribute);
        public static string ToJson(object data)
        {
            try
            {
                object temp = ChangeObjectToJsonObject(data);
                if (null == temp)
                    return "";
                return SimpleJsonTool.SerializeObject(temp);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return null;
        }
        public static T FromJson<T>(string json)
        {
            try
            {
                object obj = FromJson(typeof(T), json);
                return obj == null ? default(T) : (T)obj;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return default(T);
        }
        public static object FromJson(Type type, string json)
        {
            try
            {
                object jsonObj = SimpleJsonTool.DeserializeObject(json);
                return ChangeJsonDataToObjectByType(type, jsonObj);
            }
            catch (Exception e)
            {
                Debug.LogError("Json:"+json+"\n" +e);
            }
            return null;
        }

        #region   List<T>
        /// <summary>
        /// Json转换List<T>
        /// </summary>
        /// <param name="json"></param>
        /// <param name="itemType">T的type</param>
        /// <returns></returns>
        private static object JsonToList(string json, Type itemType)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            object obj = SimpleJsonTool.DeserializeObject(json);
            object res = JsonObjectToList(obj, itemType);
            return res;
        }
        private static object JsonObjectToList(object obj, Type itemType)
        {
            IList<object> listData = obj as IList<object>;
            Type listType = list_Type.MakeGenericType(itemType);
            object list = ReflectionTool.CreateDefultInstance(listType);
            if (listData == null || listData.Count == 0)
                return list;

            MethodInfo addMethod = listType.GetMethod("Add");
            for (int i = 0; i < listData.Count; i++)
            {
                object obj0 = listData[i];
                obj0 = ChangeJsonDataToObjectByType(itemType, obj0);
                if (obj0 == null)
                    continue;
                addMethod.Invoke(list, new object[] { obj0 });
            }
            return list;
        }


        /// <summary>
        /// List<T>转换为Json
        /// </summary>
        /// <param name="datas">List<T></param>
        /// <returns>json</returns>
        private static string ListToJson(object datas)
        {
            object temp = ListArrayToJsonObject(datas, true);
            return SimpleJsonTool.SerializeObject(temp);

        }
        private static object ListArrayToJsonObject(object datas, bool isList)
        {
            Type type = datas.GetType();
            PropertyInfo pro = null;
            if (isList)
            {
                pro = type.GetProperty("Count");
            }
            else
            {
                pro = type.GetProperty("Length");
            }

            int count = (int)pro.GetValue(datas, null);
            MethodInfo methodInfo = null;
            if (isList)
                methodInfo = type.GetMethod("get_Item", flags);
            else
                methodInfo = type.GetMethod("GetValue", new Type[] { typeof(int) });
            List<object> temp = new List<object>();
            for (int i = 0; i < count; i++)
            {
                object da = methodInfo.Invoke(datas, new object[] { i });
                da = ChangeObjectToJsonObject(da);
                if (null == da)
                    continue;
                temp.Add(da);
            }
            return temp;
        }
        #endregion

        #region Array

        /// <summary>
        /// Json转换为Array
        /// </summary>
        /// <param name="json"></param>
        /// <param name="itemType">数组的类型T[]的T类型</param>
        /// <returns></returns>
        private static object JsonToArray(string json, Type itemType)
        {
            object obj = SimpleJsonTool.DeserializeObject(json);
            return JsonObjectToArray(obj, itemType);
        }
        private static object JsonObjectToArray(object data, Type itemType)
        {
            object result = JsonObjectToList(data, itemType);
            MethodInfo method = result.GetType().GetMethod("ToArray");
            //Debug.Log("JsonObjectToArray : result:" + result.GetType().FullName);
            return method.Invoke(result, null);
        }
        /// <summary>
        /// Array转换为Json
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private static string ArrayToJson(object datas)
        {
            object temp = ListArrayToJsonObject(datas, false);
            return SimpleJsonTool.SerializeObject(temp);
        }
        #endregion
        #region class或struct
        /// <summary>
        /// class或struct转换为json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string ClassOrStructToJson(object data)
        {
            object jsonObject = ClassOrStructToJsonObject(data);
            return SimpleJsonTool.SerializeObject(jsonObject);
        }
        private static object ClassOrStructToJsonObject(object data)
        {
            IDictionary<object, object> dic = new Dictionary<object, object>();
            Type type = data.GetType();

            FieldInfo[] fields = type.GetFields(flags);

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo f = fields[i];

                if (ReflectionTool.IsDelegate(f.FieldType))
                    continue;
                if (CheckHaveNotJsonSerializedAttribute(f))
                    continue;
                try
                {

                    object v = f.GetValue(data);
                    string name = f.Name;
                    if (v == null)
                        continue;
                    v = ChangeObjectToJsonObject(v);
                    dic.Add(name, v);
                }
                catch
                {
                    continue;
                }
            }

            PropertyInfo[] propertys = type.GetProperties(flags);
            for (int i = 0; i < propertys.Length; i++)
            {
                PropertyInfo p = propertys[i];
                if (CheckPropertyInfo(p))
                {
                    try
                    {
                        object v = p.GetValue(data, null);
                        if (v == null)
                            continue;
                        v = ChangeObjectToJsonObject(v);
                        dic.Add(p.Name, v);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("property :" + p.Name + "\n" + e);
                        continue;
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// json转换为class或struct
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type">class或struct的type</param>
        /// <returns></returns>
        private static object JsonToClassOrStruct(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            object obj = SimpleJsonTool.DeserializeObject(json);
            return JsonObjectToClassOrStruct(obj, type);
        }
        private static object JsonObjectToClassOrStruct(object jsonObj, Type type)
        {
            IDictionary<object, object> dic = (IDictionary<object, object>)jsonObj;
            object instance = ReflectionTool.CreateDefultInstance(type);
            if (dic == null || instance == null)
            {
                return null;
            }
            foreach (var item in dic)
            {
                string key = item.Key.ToString();
                object value = item.Value;
                FieldInfo f = type.GetField(key, flags);
                if (f != null)
                {
                    value = ChangeJsonDataToObjectByType(f.FieldType, value);
                    f.SetValue(instance, value);
                }
                else
                {
                    PropertyInfo p = type.GetProperty(key, flags);
                    if (CheckPropertyInfo(p))
                    {
                        try
                        {
                            value = ChangeJsonDataToObjectByType(p.PropertyType, value);
                            p.SetValue(instance, value, null);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("property :" + p.Name + "\n" + e);
                            continue;
                        }
                    }
                }
            }
            return instance;
        }
        #endregion
        #region Dictionary
        /// <summary>
        /// Dictionary<k,v>转换为json 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string DictionaryToJson(object data)
        {
            object obj = DictionaryToJsonObject(data);
            return SimpleJsonTool.SerializeObject(obj);
        }

        private static object DictionaryToJsonObject(object data)
        {
            Type type = data.GetType();
            PropertyInfo p = type.GetProperty("Count");
            int count = (int)p.GetValue(data, null);

            MethodInfo GetEnumeratorMe = type.GetMethod("GetEnumerator");
            PropertyInfo current = GetEnumeratorMe.ReturnParameter.ParameterType.GetProperty("Current");
            MethodInfo moveNext = GetEnumeratorMe.ReturnParameter.ParameterType.GetMethod("MoveNext");

            Type[] typeArguments = type.GetGenericArguments();
            Type KeyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(typeArguments);
            PropertyInfo keyProperty = KeyValuePairType.GetProperty("Key");
            PropertyInfo valueProperty = KeyValuePairType.GetProperty("Value");

            object enumerator = GetEnumeratorMe.Invoke(data, null);

            Dictionary<object, object> dataDic = new Dictionary<object, object>();
            for (int i = 0; i < count; i++)
            {
                moveNext.Invoke(enumerator, null);
                object kv = current.GetValue(enumerator, null);
                object key = keyProperty.GetValue(kv, null);
                object value = valueProperty.GetValue(kv, null);
                key = ChangeObjectToJsonObject(key);
                value = ChangeObjectToJsonObject(value);
                if (key == null || value == null)
                    continue;
                dataDic.Add(key, value);
            }

            return dataDic;
        }
        /// <summary>
        /// json转换为Dictionary<k,v>
        /// </summary>
        /// <param name="json"></param>
        /// <param name="keyType">key的type</param>
        /// <param name="valueType">value的type</param>
        /// <returns></returns>
        private static object JsonToDictionary(string json, Type keyType, Type valueType)
        {
            object obj = SimpleJsonTool.DeserializeObject(json);
            return JsonObjectToDictionary(obj, keyType, valueType);
        }
        private static object JsonObjectToDictionary(object data, Type keyType, Type valueType)
        {
            IDictionary<object,object> iDic = data as IDictionary<object, object>;
            //Debug.Log(iDic.Count);
            Type dicType = dictionary_Type.MakeGenericType(keyType, valueType);
            object tempDic = Activator.CreateInstance(dicType);
            MethodInfo addDicMe = dicType.GetMethod("Add", flags);

            if (iDic != null)
            {
                foreach (var item in iDic)
                {
                    object key = item.Key;
                    object value = item.Value;
                    key = ChangeJsonDataToObjectByType(keyType, key);
                    value = ChangeJsonDataToObjectByType(valueType, value);
                    addDicMe.Invoke(tempDic, new object[] { key, value });
                }
            }
            return tempDic;
        }
        #endregion

        #region Other
        private static bool IsSupportBaseValueParseJson(Type t)
        {
            //p.PropertyType.IsPrimitive IsPrimitive 表示是否为基元类型之一，则为 true；否则为 false。 基元类型是 Boolean、 Byte、 SByte、 Int16、 UInt16、 Int32、 UInt32、 Int64、 UInt64、 Char、 Double和 Single。
            if (t.IsPrimitive 
                || t == typeof(decimal) 
                || t == typeof(string) 
                || t.IsEnum
                || t==typeof(DateTime)
                || t == typeof(DateTimeOffset)
                || t == typeof(Guid)
                || t == typeof(Uri))
                return true;
            return false;
        }
        private static object ChangeJsonDataToObjectByType(Type type, object data)
        {
            object value = null;
            if (data == null)
                return value;
            if (IsSupportBaseValueParseJson(type))
            {
               value = SimpleJsonTool.DeserializeObject(data,type);
            }
            else if (type.IsArray)
            {
                try
                {
                    value = JsonObjectToArray(data, type.GetElementType());
                }
                catch (Exception e)
                {
                    Debug.LogError("Array无法转换类型， data：" + data.GetType().FullName + "  type.GetElementType(): " + type.GetElementType().FullName);
                    Debug.LogError(e);
                }
            }
            else if (type.IsGenericType)
            {
                if (list_Type.Name == type.Name)
                {
                    value = JsonObjectToList(data, type.GetGenericArguments()[0]);
                }
                else if (dictionary_Type.Name == type.Name)
                {
                    Type[] ts = type.GetGenericArguments();
                    value = JsonObjectToDictionary(data, ts[0], ts[1]);
                }
                else
                {
                    value = JsonObjectToClassOrStruct(data, type);
                }
            }
            else
            {
                if (type.IsClass || type.IsValueType)
                {
                    value = JsonObjectToClassOrStruct(data, type);
                }
            }
            if (value == null)
                return value;
            try
            {
                value = ChangeType(value, type);
            }
            catch (Exception e)
            {
                Debug.LogError("无法转换类型， type：" + type.FullName + "  valueType: " + value.GetType().FullName + "\n " + e);
            }
            return value;
        }
        static public object ChangeType(object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            //if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (!(value is IConvertible)) return value;

            if (type == typeof(byte))
            {
                return Convert.ToByte(value);
            }
            else if (type == typeof(int))
                return Convert.ToInt32(value);
            else if (type == typeof(long))
                return Convert.ToInt64(value);
            else if (type == typeof(bool))
                return Convert.ToBoolean(value);
            else if (type == typeof(float))
                return Convert.ToSingle(value);
            else if (type == typeof(double))
                return Convert.ToDouble(value);
            else if (type == typeof(decimal))
                return Convert.ToDecimal(value);
            else if (type == typeof(char))
                return Convert.ToChar(value);
            else if (type == typeof(short))
                return Convert.ToInt16(value);
            else if (type == typeof(sbyte))
                return Convert.ToSByte(value);
            else if (type == typeof(ushort))
                return Convert.ToUInt16(value);
            else if (type == typeof(uint))
                return Convert.ToUInt32(value);
            else if (type == typeof(ulong))
                return Convert.ToUInt64(value);

            return Convert.ChangeType(value, type);
        }
        /// <summary>
        /// 检测是否字段或属性添加了 NotJsonSerializedAttribute 特性
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private static bool CheckHaveNotJsonSerializedAttribute(MemberInfo member)
        {
            object[] attrs = member.GetCustomAttributes(false);
            bool isSerialized = false;
            foreach (var att in attrs)
            {
                if (att.GetType() == notJsonSerialized_Type)
                {
                    isSerialized = true;
                    break;
                }
            }
            return isSerialized;
        }
        /// <summary>
        /// 检查属性是否能用于序列化
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool CheckPropertyInfo(PropertyInfo property)
        {
            if (property == null)
                return false;
            if (!(property.CanRead && property.CanWrite))
                return false;
            //索引器
            if (property.GetIndexParameters().Length > 0)
            {
                return false;
            }
            if (ReflectionTool.IsDelegate(property.PropertyType))
                return false;


            return true;
        }
        private static object ChangeObjectToJsonObject(object data)
        {
            if (null == data)
                return data;
            Type t = data.GetType();
            if (ReflectionTool.IsDelegate(t))
                return null;
            object value = data;
            if (!IsSupportBaseValueParseJson(t))
            {
                if (t.IsArray)
                    value = ListArrayToJsonObject(data, false);
                else if (t.IsClass || t.IsGenericType)
                {
                    if (list_Type.Name == t.Name)
                        value = ListArrayToJsonObject(data, true);
                    else if (dictionary_Type.Name == t.Name)
                        value = DictionaryToJsonObject(data);
                    else
                        value = ClassOrStructToJsonObject(data);
                }
                else
                {
                    value = ClassOrStructToJsonObject(data);
                }
            }
            return value;
        }
        #endregion
    }



    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NotJsonSerializedAttribute : Attribute
    {

    }
}