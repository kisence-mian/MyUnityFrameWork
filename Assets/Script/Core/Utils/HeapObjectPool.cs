using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class HeapObjectPool
{
    #region string, object字典

    public const float c_timerSpace = 0.5f;
    public const float c_safeTimerSpace = 5f;

    public static ApplicationVoidCallback OnUpdate;

    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += Update;
    }

    static void Update()
    {
        if (OnUpdate != null)
        {
            OnUpdate();
        }
    }

    public static Dictionary<string, object> GetSODict()
    {
        return SafeGetSODict();
    }

    public static Dictionary<string, object> SafeGetSODict()
    {
        Dictionary<string, object> dict = HeapObjectPoolTool<Dictionary<string, object>>.GetHeapObject();
        dict.Clear();

        return dict;
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

    //缓冲区是否从头开始使用了
    static bool s_isOverLength = false;
    //缓冲区的指针在哪个位置
    static int s_currentDictIndex = 0;

    static float s_timer = 0;
    static float s_safeTimer = 0;

    static void Init()
    {
        s_pool = new T[s_size];
        for (int i = 0; i < s_size; i++)
        {
            s_pool[i] = new T();
        }
        HeapObjectPool.OnUpdate += JudgeTimer;
    }

    public static void BeginSafeGet()
    {
        s_currentDictIndex = s_poolIndex -1;

        if (s_currentDictIndex <0)
        {
            s_currentDictIndex = s_size;
        }

        s_isOverLength = false;
    }

    static bool GetIsOverStack()
    {
        return (s_isOverLength && s_poolIndex >= s_currentDictIndex);
    }

    public static T GetHeapObject()
    {
        if (GetIsOverStack())
        {
            if (s_safeTimer == 0)
            {
                s_safeTimer = HeapObjectPool.c_safeTimerSpace;
            }
            return new T();
        }

        return GetObject();
    }

    public static T GetObject()
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
            s_isOverLength = true;
        }

        return obj;
    }

    static void JudgeTimer()
    {
        if(s_safeTimer != 0)
        {
            s_safeTimer -= Time.realtimeSinceStartup;
            if (s_safeTimer < 0)
            {
                s_safeTimer = 0;
                BeginSafeGet();
            }
        }
        else
        {
            s_timer -= Time.realtimeSinceStartup;
            if (s_timer < 0)
            {
                s_timer = HeapObjectPool.c_timerSpace;
                BeginSafeGet();
            }
        }
    }
}

#endregion
