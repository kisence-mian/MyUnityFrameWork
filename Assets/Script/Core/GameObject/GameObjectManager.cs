using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameObjectManager :MonoBehaviour
{
    static Dictionary<string, List<GameObject>> s_objectPool = new Dictionary<string, List<GameObject>>();
    private static Transform s_poolParent;

    public static Transform s_PoolParent
    {
        get
        {
            if (s_poolParent == null)
            {
                GameObject instancePool = new GameObject();
                instancePool.name = "ObjectPool";
                s_poolParent = instancePool.transform;
                DontDestroyOnLoad(s_poolParent);
            }

            return s_poolParent;
        }
    }

    /// <summary>
    /// 加载一个对象并把它实例化
    /// </summary>
    /// <param name="l_gameObjectName">对象名</param>
    /// <param name="l_parent">对象的父节点,可空</param>
    /// <returns></returns>
    public static GameObject CreatGameObject(string l_gameObjectName,GameObject l_parent = null)
    {
        GameObject l_goTmp = ResourceManager.Load<GameObject>(l_gameObjectName);

        if (l_goTmp == null)
        {
            throw new Exception("CreatGameObject error dont find :" + l_gameObjectName);
        }

        return CreatGameObject(l_goTmp, l_parent);
    }

    public static GameObject CreatGameObject(GameObject l_prefab, GameObject l_parent = null)
    {
        if (l_prefab == null)
        {
            throw new Exception("CreatGameObject error : l_prefab  is null");
        }

        GameObject l_instanceTmp = Instantiate(l_prefab);
        l_instanceTmp.name = l_prefab.name;

        if (l_parent != null)
        {
            l_instanceTmp.transform.SetParent(l_parent.transform);
        }

        return l_instanceTmp;
    }


    public static bool IsExist(string l_objectName)
    {
        if (s_objectPool.ContainsKey(l_objectName) && s_objectPool[l_objectName].Count > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 从对象池取出一个对象，如果没有，则直接创建它
    /// </summary>
    /// <param name="l_name">对象名</param>
    /// <param name="l_parent">要创建到的父节点</param>
    /// <returns>返回这个对象</returns>
    public static GameObject CreatGameObjectByPool(string l_name,GameObject l_parent = null)
    {
        if (IsExist(l_name))
        {
            GameObject go = s_objectPool[l_name][0];
            s_objectPool[l_name].RemoveAt(0);

            go.SetActive(true);

            if (l_parent == null)
            {
                go.transform.SetParent(null);
            }
            else
            {
                go.transform.SetParent(l_parent.transform);
            }

            return go;
        }
        else
        {
            return CreatGameObject(l_name, l_parent);
        }
    }

    public static GameObject CreatGameObjectByPool(GameObject l_prefab, GameObject l_parent = null)
    {
        if (IsExist(l_prefab.name))
        {
            GameObject go = s_objectPool[l_prefab.name][0];
            s_objectPool[l_prefab.name].RemoveAt(0);

            go.SetActive(true);

            if (l_parent == null)
            {
                go.transform.SetParent(null);
            }
            else
            {
                go.transform.SetParent(l_parent.transform);
            }

            return go;
        }
        else
        {
            return CreatGameObject(l_prefab, l_parent);
        }
    }

    /// <summary>
    /// 将一个对象放入对象池
    /// </summary>
    /// <param name="obj">目标对象</param>
    public static void DestroyGameobjectByPool(GameObject obj)
    {
        string key = obj.name.Replace("(Clone)", "");

        if (s_objectPool.ContainsKey(key) == false)
        {
            s_objectPool.Add(key, new List<GameObject>());
        }

        s_objectPool[key].Add(obj);

        obj.SetActive(false);
        obj.name = key;
        obj.transform.SetParent(s_PoolParent);
    }

    public static void DestroyGameobjectByPool(GameObject go,float time)
    {
        Timer.DelayCallBack(time, (object[] obj) =>
        {
            DestroyGameobjectByPool(go);
        });
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public static void CleanPool()
    {
        foreach (string name in s_objectPool.Keys)
        {
            CleanPoolByName(name);
        }
    }

    /// <summary>
    /// 清除掉某一个对象的所有对象池缓存
    /// </summary>
    public static void CleanPoolByName(string name)
    {
        if (s_objectPool.ContainsKey(name))
        {
            List<GameObject> l_objList = s_objectPool[name];

            for (int i = 0; i < l_objList.Count; i++)
            {
                Destroy(l_objList[i]);
            }

            s_objectPool.Remove(name);
        }
    }


}
