using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        GameObject l_goTmp = (GameObject)ResourceManager.Load(l_gameObjectName);

        if (l_goTmp == null)
        {
            Debug.LogError("CreatGameObject error dont find :" + l_gameObjectName);
        }

        GameObject l_instanceTmp = Instantiate(l_goTmp);
        l_instanceTmp.name = l_gameObjectName;

        if(l_parent != null)
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

    public static GameObject GetInstanceByPool(string l_name,GameObject l_parent = null)
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

    public static void DestroyByPool(GameObject obj)
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


}
