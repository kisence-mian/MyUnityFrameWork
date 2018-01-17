using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class HeapObjectPool
{
    #region string, object字典

    public static Dictionary<string, object> GetSODict()
    {
        return HeapObjectPool<Dictionary<string, object>>.GetObject();
    }

    public static void PutSODict(Dictionary<string, object> dict)
    {
        dict.Clear();
        HeapObjectPool<Dictionary<string, object>>.PutObject(dict);
    }

    #endregion
}

    #region HeapObject
public interface IHeapObjectInterface
{
    void OnInit();

    /// <summary>
    /// 取出时调用
    /// </summary>
    void OnPop();

    /// <summary>
    /// 放回时调用
    /// </summary>
    void OnPush();
}

#endregion

    #region HeapObjectPool

public static class HeapObjectPool<T> where T : new()
{
    static Stack<T> s_pool = new Stack<T>();

    public static int GetCount()
    {
        return s_pool.Count;
    }

    public static T GetObject()
    {
        T obj;
        IHeapObjectInterface heapObj;

        if (s_pool.Count >0)
        {
            obj = s_pool.Pop();
            heapObj = obj as IHeapObjectInterface;
        }
        else
        {
            obj = new T();
            heapObj = obj as IHeapObjectInterface;
            if(heapObj != null)
            {
                heapObj.OnInit();
            }
        }

        if (heapObj != null)
        {
            heapObj.OnPop();
        }

        return obj;
    }

    public static void PutObject(T obj)
    {
        IHeapObjectInterface heapObj = obj as IHeapObjectInterface;

        if (heapObj != null)
        {
            heapObj.OnPush();
        }

        s_pool.Push(obj);
    }
}

#endregion
