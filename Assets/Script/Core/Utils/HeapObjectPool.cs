using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class HeapObjectPool
{
    #region string, object字典

    const int c_SODictSize = 200;
    static Dictionary<string, object>[] s_SODict;
    static int s_SODictIndex = 0;

    /// <summary>
    /// 获取string, object字典
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, object> GetSODict()
    {
        InitSODict();
        Dictionary<string, object> dict = s_SODict[s_SODictIndex];
        dict.Clear();

        s_SODictIndex++;

        if (s_SODictIndex >= s_SODict.Length)
        {
            s_SODictIndex = 0;
        }

        return dict;
    }

    static void InitSODict()
    {
        if (s_SODict == null)
        {
            s_SODict = new Dictionary<string, object>[c_SODictSize];
            for (int i = 0; i < c_SODictSize; i++)
            {
                s_SODict[i] = new Dictionary<string, object>();
            }
        }
    }

    #endregion

    #region object列表

    const int c_ObjListSize = 20;
    static List<object>[] s_ObjListPool;
    static int s_ObjListIndex = 0;

    /// <summary>
    /// 获取string, object字典
    /// </summary>
    /// <returns></returns>
    public static List<object> GetObjList()
    {
        InitObjList();
        List<object> dict = s_ObjListPool[s_ObjListIndex];
        dict.Clear();

        s_ObjListIndex++;

        if (s_ObjListIndex >= s_ObjListPool.Length)
        {
            s_ObjListIndex = 0;
        }

        return dict;
    }

    static void InitObjList()
    {
        if (s_ObjListPool == null)
        {
            s_ObjListPool = new List<object>[c_ObjListSize];
            for (int i = 0; i < c_ObjListSize; i++)
            {
                s_ObjListPool[i] = new List<object>();
            }
        }
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

public class HeapObjectPoolTool<T> where T : new()
{
    static T[] s_pool;
    static int s_poolIndex = 0;
    static int s_size = 500;

    public static void SetSize(int size)
    {
        if (s_size != size)
        {
            s_size = size;
            Init();
        }
    }

    static void Init()
    {
        s_pool = new T[s_size];
        for (int i = 0; i < s_size; i++)
        {
            s_pool[i] = new T();
        }
    }

    public static T GetHeapObject()
    {
        if (s_pool == null)
        {
            Init();
        }

        T obj = s_pool[s_poolIndex];

        s_poolIndex++;

        if (s_poolIndex >= s_pool.Length)
        {
            s_poolIndex = 0;
        }

        return obj;
    }
}

#endregion
