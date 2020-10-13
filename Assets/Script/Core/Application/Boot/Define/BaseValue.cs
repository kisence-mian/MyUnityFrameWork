using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

    /// <summary>
    /// 脚本变量值
    /// </summary>
    [System.Serializable]
    public class BaseValue
    {

        /// <summary>
        /// 变量名字
        /// </summary>
        public string name = "";
        /// <summary>
        /// 类型
        /// </summary>
        public string typeName = "";
        public bool isGeneric = false;
        public Type ValueType
        {
            get
            {
                if (isGeneric)
                {
                    string[] ss = typeName.Split('|');
                    Type t = ReflectionUtils.GetTypeByTypeFullName(ss[0]);
                    Type[] typeArguments = new Type[ss.Length - 1];
                    for (int i = 1; i < ss.Length; i++)
                    {
                        typeArguments[i - 1] = ReflectionUtils.GetTypeByTypeFullName(ss[i]);
                    }
                    return t.MakeGenericType(typeArguments);
                }
                else
                    return ReflectionUtils.GetTypeByTypeFullName(typeName);
            }
        }
        public string value = "";

        private object cacheObje = null;
        public BaseValue() { }
        public BaseValue(string vName, object vValue)
        {
            SetValue(vName, vValue);
        }
        public void SetValue(object vVlaue)
        {
            SetValue(name, vVlaue);
        }
        public void SetValue(string vName, object vValue)
        {
            if (vValue == null)
            {
                return;
            }
            cacheObje = vValue;

            this.name = vName;
            Type type = vValue.GetType();
            isGeneric = type.IsGenericType;
            if (isGeneric)
            {
                typeName += type.GetGenericTypeDefinition().FullName + "|";
                Type[] typeArguments = type.GetGenericArguments();
                for (int i = 0; i < typeArguments.Length; i++)
                {
                    Type t = typeArguments[i];
                    typeName += t.FullName;
                    if (i < typeArguments.Length - 1)
                        typeName += "|";
                }
            }
            else
                typeName = type.FullName;
            value = JsonUtils.ToJson(vValue);
        }

        public object GetValue()
        {
            object obj = null;
            if (string.IsNullOrEmpty(typeName) || (string.IsNullOrEmpty(value) && typeName != typeof(string).FullName))
                return obj;
            if (cacheObje != null)
                return cacheObje;
            try
            {

                obj = JsonUtils.FromJson(ValueType, value);
            }
            catch (Exception e)
            {
                Debug.LogError("Error name:" + name + " value : " + value + "  typeName:" + typeName + "\n" + e);

            }

            cacheObje = obj;
            return obj;
        }
        public bool EqualTo(BaseValue v)
        {
            if (v == null) return false;

            if (typeName.Equals(v.typeName) && value.Equals(v.value))
                return true;

            return false;
        }
    }



