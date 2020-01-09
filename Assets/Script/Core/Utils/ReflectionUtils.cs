
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public static class ReflectionUtils
{
    public static Dictionary<string, object> GetClassOrStructData(object data, bool containsPropertyInfo = false)
    {
        Type type = data.GetType();
        FieldInfo[] fields = type.GetFields();
        Dictionary<string, object> dic = new Dictionary<string, object>();
        for (int i = 0; i < fields.Length; i++)
        {
            object v = fields[i].GetValue(data);
            string name = fields[i].Name;
            if (v == null)
                continue;
            dic.Add(name, v);
        }
        if (!containsPropertyInfo)
            return dic;
        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            PropertyInfo property = propertyInfos[i];
            if (!property.CanRead) continue;

            try
            {

                string name = property.Name;
                object v = property.GetValue(data, null);
                dic.Add(name, v);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                continue;
            }
        }
        return dic;
    }
    public static object SetClassOrStructData(Dictionary<string, object> dic, Type type, bool containsPropertyInfo = false, object instance = null)
    {
        object classObj = instance;
        if (classObj == null)
            classObj = Activator.CreateInstance(type);

        FieldInfo[] fields = type.GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            string name = fields[i].Name;
            if (dic.ContainsKey(name))
            {
                fields[i].SetValue(classObj, dic[name]);
            }
        }
        if (!containsPropertyInfo)
            return classObj;
        PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            PropertyInfo property = propertyInfos[i];
            if (!property.CanWrite) continue;
            try
            {
                string name = propertyInfos[i].Name;
                if (dic.ContainsKey(name))
                    propertyInfos[i].SetValue(classObj, dic[name], null);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                continue;
            }
        }
        return classObj;
    }

    /// <summary>
    /// 获取父类的所有子类
    /// </summary>
    /// <param name="parentType">父类Type</param>
    /// <returns></returns>
    public static Type[] GetChildTypes(Type parentType, bool isContainsAllChild = true)
    {
        List<Type> lstType = new List<Type>();
        Assembly assem = Assembly.GetAssembly(parentType);
        Type[] types = assem.GetTypes();
        //Debug.Log()
        foreach (Type tChild in types)
        {
            if (tChild.BaseType == parentType)
            {
                lstType.Add(tChild);
                if (isContainsAllChild)
                {
                    Type[] temp = GetChildTypes(tChild, isContainsAllChild);
                    if (temp.Length > 0)
                        lstType.AddRange(temp);
                }
            }
        }
        return lstType.ToArray();
    }
    //#if UNITY
    public readonly static Assembly Assembly_CSharp = Assembly.Load("Assembly-CSharp");

    public readonly static Assembly Assembly_UnityEngine = Assembly.Load("UnityEngine");
    //#endif
#if UNITY_EDITOR
    public readonly static Assembly Assembly_CSharp_Editor = Assembly.Load("Assembly-CSharp-Editor");
    public readonly static Assembly Assembly_UnityEditor = Assembly.Load("UnityEditor");
#endif
    /// <summary>
    /// 获取Type类
    /// </summary>
    /// <param name="typeFullName">type的全名</param>
    /// <returns></returns>
    public static Type GetTypeByTypeFullName(string typeFullName, bool isShowErrorLog = true)
    {
        if (string.IsNullOrEmpty(typeFullName))
            return null;

        Type type = Type.GetType(typeFullName);
        Assembly ass = Assembly.GetExecutingAssembly();
        type = ass.GetType(typeFullName);
        //#if UNITY
        if (type == null && Assembly_CSharp != null && ass != Assembly_CSharp)
        {
            type = Assembly_CSharp.GetType(typeFullName);
        }
        if (type == null && Assembly_UnityEngine != null && ass != Assembly_UnityEngine)
        {
            type = Assembly_UnityEngine.GetType(typeFullName);
        }
        //#endif
#if UNITY_EDITOR
        if (type == null && Assembly_CSharp_Editor != null && ass != Assembly_CSharp_Editor)
        {
            type = Assembly_CSharp_Editor.GetType(typeFullName);
        }
        if (type == null && Assembly_UnityEditor != null && ass != Assembly_UnityEditor)
        {
            type = Assembly_UnityEditor.GetType(typeFullName);
        }
#endif
        if (type == null)
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblys.Length; i++)
            {
                Assembly assembly = assemblys[i];
                if (assembly == ass) continue;
                //#if UNITY
                if (assembly == Assembly_CSharp) continue;
                if (assembly == Assembly_UnityEngine) continue;
                //#endif
#if UNITY_EDITOR
                if (assembly == Assembly_CSharp_Editor) continue;
                if (assembly == Assembly_UnityEditor) continue;
#endif
                type = assemblys[i].GetType(typeFullName);
                if (type != null)
                    break;
            }
        }
        if (type == null && isShowErrorLog)
            Debug.LogError("无法找到类型：" + typeFullName);
        return type;
    }
    public static Type GetTypefromAssemblyFullName(string AssemblyName, string fullName)
    {

        Assembly tmp = Assembly.Load(AssemblyName);
        return tmp.GetType(fullName);
    }


    private const BindingFlags flagsInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags flagsStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// 创建默认实例
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isDeepParameters">true：给构造方法传默认值，false：给构造方法传</param>
    /// <returns></returns>
    public static object CreateDefultInstanceAll(Type type, bool isDeepParameters = false)
    {
        object instance = null;
        string error = "";
        if (type.IsArray)
        {
            instance = Activator.CreateInstance(type, new object[] { 0 });

        }
        else if (type.IsValueType)
        {
            instance = Activator.CreateInstance(type);
        }
        else
        {
            ConstructorInfo[] construArrs = type.GetConstructors(flagsInstance);
            for (int i = 0; i < construArrs.Length; i++)
            {
                ConstructorInfo cInfo = construArrs[i];

                ParameterInfo[] pArr = cInfo.GetParameters();
                object[] parmsArr = new object[pArr.Length];
                for (int j = 0; j < parmsArr.Length; j++)
                {
                    ParameterInfo pf = pArr[j];
                    if (isDeepParameters)
                    {
                        parmsArr[j] = CreateDefultInstance(pf.ParameterType);
                    }
                    else
                        parmsArr[j] = null;
                }
                try
                {
                    instance = Activator.CreateInstance(type, flagsInstance, null, parmsArr, null);
                    if (instance == null)
                        continue;
                    else
                        break;
                }
                catch (Exception e)
                {
                    error += e.ToString() + "\n";
                    continue;
                }
            }
        }
        if (instance == null)
            Debug.LogError("Type :" + type + "\n" + error);
        return instance;
    }
    public static object CreateDefultInstance(Type type)
    {
        if (type == null)
        {
            Debug.LogError("Type不可为：null");
            return null;
        }
        object instance = CreateDefultInstanceAll(type, false); ;
        if (instance == null)
            instance = CreateDefultInstanceAll(type, true);
        if (instance == null)
            Debug.LogError("创建默认实例失败！Type:" + type.FullName);
        return instance;
    }

    public static void SetFieldInfo(Type t, object instance, string fieldName, object value)
    {
        BindingFlags flags = instance == null ? flagsStatic : flagsInstance;
        FieldInfo f = t.GetField(fieldName, flags);
        if (f == null)
        {
            Debug.LogError("获取失败：type:" + t + "  fieldName: " + fieldName);
            return;
        }
        try
        {
            f.SetValue(instance, value);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public static object GetFieldInfo(Type t, object instance, string fieldName)
    {
        BindingFlags flags = instance == null ? flagsStatic : flagsInstance;
        FieldInfo f = t.GetField(fieldName, flags);
        if (f == null)
        {
            Debug.LogError("获取失败：type:" + t + "  fieldName: " + fieldName);
            return null;
        }
        try
        {
            return f.GetValue(instance);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
    }
    public static void SetPropertyInfo(Type t, object instance, string propertyName, object value)
    {
        BindingFlags flags = instance == null ? flagsStatic : flagsInstance;
        PropertyInfo f = t.GetProperty(propertyName, flags);
        if (f == null)
        {
            Debug.LogError("获取失败：type:" + t + "  fieldName: " + propertyName);
            return;
        }
        if (!f.CanWrite)
        {
            Debug.LogError("属性不能写入：type:" + t + "  fieldName: " + propertyName);
            return;
        }
        try
        {
            f.SetValue(instance, value, null);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public static object GetPropertyInfo(Type t, object instance, string propertyName)
    {
        BindingFlags flags = instance == null ? flagsStatic : flagsInstance;
        PropertyInfo f = t.GetProperty(propertyName, flags);

        if (f == null)
        {
            Debug.LogError("获取失败：type:" + t + "  fieldName: " + propertyName);
            return null;
        }
        if (!f.CanRead)
        {
            Debug.LogError("属性不能读取：type:" + t + "  fieldName: " + propertyName);
            return null;
        }
        try
        {
            return f.GetValue(instance, null);
        }

        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
    }
    public static object InvokMethod(Type t, object instance, string methodName, ref object[] paras)
    {
        BindingFlags flags = instance == null ? flagsStatic : flagsInstance;
        MethodInfo temp = t.GetMethod(methodName, flags);
        if (temp == null)
        {
            Debug.LogError("获取方法失败：type:" + t + "  methodName: " + methodName);
            return null;
        }
        try
        {
            return temp.Invoke(instance, paras);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
    }

    /// <summary>
    /// 返回Type Name ，泛型返回： List<int>
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string GetTypeName4CodeFormat(Type t)
    {
        string typeName = t.Name;
        if (t.IsGenericType)
        {
            typeName = typeName.Remove(typeName.IndexOf('`'));
            Type[] gTypes = t.GetGenericArguments();
            string temp = "";
            for (int i = 0; i < gTypes.Length; i++)
            {
                Type tempType = gTypes[i];

                temp += GetTypeName4CodeFormat(tempType);
                if (i < gTypes.Length - 1)
                {
                    temp += ",";
                }
            }
            typeName = typeName + "<" + temp + ">";
        }

        return typeName;
    }
    /// <summary>
    /// 是否是代理类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsDelegate(Type type)
    {
        return type.BaseType == typeof(MulticastDelegate);
    }

    public static object DeepCopySelf(this object obj)
    {
        if (obj == null)
            return null;
        Type targetType = obj.GetType();
        if (targetType.IsValueType)
            return obj;
        if (obj is string)
            return obj;

        object targetDeepCopyObj;

        targetDeepCopyObj = CreateDefultInstance(targetType);   //创建引用对象   
        MemberInfo[] memberCollection = obj.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public);

        for (int i = 0; i < memberCollection.Length; i++)
        {
            MemberInfo member = memberCollection[i];
            //拷贝字段
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo field = (FieldInfo)member;
                object fieldValue = field.GetValue(obj);
                if (fieldValue is ICloneable)
                {
                    field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                }
                else
                {
                    field.SetValue(targetDeepCopyObj, fieldValue.DeepCopySelf());
                }

            }//拷贝属性
            else if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo myProperty = (PropertyInfo)member;
                if (myProperty.CanRead && myProperty.CanWrite)
                {

                    object propertyValue = myProperty.GetValue(obj, null);
                    if (propertyValue is ICloneable)
                    {
                        myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                    }
                    else
                    {
                        myProperty.SetValue(targetDeepCopyObj, propertyValue.DeepCopySelf(), null);
                    }

                }
            }
        }

        return targetDeepCopyObj;
    }
}