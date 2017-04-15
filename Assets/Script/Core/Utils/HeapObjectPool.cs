using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class HeapObjectPool
{
    #region string, object字典

    /// <summary>
    /// 获取string, object字典
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, object> GetSODict()
    {
        Dictionary<string, object> dict = HeapObjectPoolTool<Dictionary<string, object>>.GetHeapObject();
        dict.Clear();

        return dict;
    }

    public static void PutSODict(Dictionary<string, object> dict)
    {
        HeapObjectPoolTool<Dictionary<string, object>>.PutHeapObject(dict);
    }

    #endregion

    #region object列表
    /// <summary>
    /// 获取string, object字典
    /// </summary>
    /// <returns></returns>
    public static List<object> GetObjList()
    {
        List<object> list = HeapObjectPoolTool<List<object>>.GetHeapObject();
        list.Clear();

        return list;
    }

    public static void PutObjList(List<object> list)
    {
        HeapObjectPoolTool<List<object>>.PutHeapObject(list);
    }

    #endregion

    #region HeapObjectPool

    static Dictionary<string, List<HeapObjectBase>> s_pool = new Dictionary<string, List<HeapObjectBase>>();

    public static void Init<T>(int count = 20) where T : HeapObjectBase, new()
    {
        string type = typeof(T).Name;

        JudgeNullPool(type);

        List<HeapObjectBase> list = s_pool[type];

        for (int i = 0; i < count; i++)
        {
            T tmp = new T();
            list.Add((HeapObjectBase)tmp);
        }
    }

    public static void Init(string type,int count = 20)
    {
        JudgeNullPool(type);

        List<HeapObjectBase> list = s_pool[type];

        for (int i = 0; i < count; i++)
        {
            Type t = Type.GetType(type);

            HeapObjectBase tmp = (HeapObjectBase)Activator.CreateInstance(t);
            list.Add(tmp);
        }
    }

    public static HeapObjectBase GetObject(string type)
    {
        JudgeNullPool(type);

        List<HeapObjectBase> list = s_pool[type];

        if (list.Count > 0)
        {
            HeapObjectBase tmp = list[0];
            list.RemoveAt(0);

            return tmp;
        }
        else
        {
            Init(type);
            return GetObject(type);
        }
    }

    public static T GetObject<T>() where T : HeapObjectBase, new()
    {
        string type = typeof(T).Name;

        JudgeNullPool(type);

        List<HeapObjectBase> list = s_pool[type];

        if (list.Count > 0)
        {
            HeapObjectBase tmp = list[0];
            list.RemoveAt(0);

            return (T)tmp;
        }
        else
        {
            Init<T>();
            return GetObject<T>();
        }
    }

    public static T GetObject<T>(string TypeName) where T : HeapObjectBase, new()
    {
        JudgeNullPool(TypeName);

        List<HeapObjectBase> list = s_pool[TypeName];

        if (list.Count > 0)
        {
            HeapObjectBase tmp = list[0];
            list.RemoveAt(0);

            return (T)tmp;
        }
        else
        {
            Init<T>();
            return GetObject<T>();
        }
    }

    public static void ReleaseObject(string type,HeapObjectBase obj)
    {
        JudgeNullPool(type);

        s_pool[type].Add(obj);
    }

    public static void ReleaseObject<T>(HeapObjectBase obj)
    {
        string type = typeof(T).Name;
        JudgeNullPool(type);

        s_pool[type].Add(obj);
    }

    static void JudgeNullPool(string type)
    {
        if (!s_pool.ContainsKey(type))
        {
            s_pool.Add(type, new List<HeapObjectBase>());
        }
    }

    #endregion
}

#region HeapObject
public class HeapObjectBase
{
    public virtual void OnCreate()
    {

    }

    public virtual void OnRelease()
    {

    }

    public void Release()
    {
        try
        {
            OnRelease();
        }
        catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
        HeapObjectPool.ReleaseObject(GetType().Name, this);
    }
}

#endregion

#region HeapObjectPoolTool

/// <summary>
/// 注意：为避免内存泄漏，GetObject的引用计数必须和Put的引用计数相等
/// </summary>
/// <typeparam name="T"></typeparam>
public class HeapObjectPoolTool<T> where T : new()
{
    static Stack<T> s_pool = new Stack<T>();

    public static T GetHeapObject()
    {
        if (s_pool.Count >0)
        {
            return s_pool.Pop();
        }
        else
        {
            return new T();
        }
    }

    public static void PutHeapObject(T obj)
    {
        s_pool.Push(obj);
    }
}

#endregion