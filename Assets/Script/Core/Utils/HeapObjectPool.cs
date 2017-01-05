using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeapObjectPool
{
    #region string, object字典

    const int c_SODictSize = 50;
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
    const int c_PlayerInfoListSize = 20;
    static List<object>[] s_ObjListPool;
    static List<PlayerInfo>[] s_PlayerInfoListPool;
    static int s_ObjListIndex = 0;
    static int s_PlayerInfoListIndex = 0;

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

    public static List<PlayerInfo> GetPlayerInfoList()
    {
        InitPlayerInfoList();
        List<PlayerInfo> dict = s_PlayerInfoListPool[s_PlayerInfoListIndex];
        dict.Clear();

        s_PlayerInfoListIndex++;

        if (s_PlayerInfoListIndex >= s_PlayerInfoListPool.Length)
        {
            s_PlayerInfoListIndex = 0;
        }

        return dict;
    }

    static void InitObjList()
    {
        if (s_ObjListPool == null)
        {
            s_ObjListPool = new List<object>[c_ObjListSize];
            for (int i = 0; i < c_SODictSize; i++)
            {
                s_ObjListPool[i] = new List<object>();
            }
        }
    }

    static void InitPlayerInfoList()
    {
        if (s_PlayerInfoListPool == null)
        {
            s_PlayerInfoListPool = new List<PlayerInfo>[c_PlayerInfoListSize];
            for (int i = 0; i < c_PlayerInfoListSize; i++)
            {
                s_PlayerInfoListPool[i] = new List<PlayerInfo>();
            }
        }
    }

    #endregion

    #region Mini Josn 优化

    #endregion
}

#region HeapObjectPool

public class HeapObjectPoolTool<T> where T : new()
{
    static T[] s_pool;
    static int s_poolIndex = 0;
    static int s_size = 20;

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
