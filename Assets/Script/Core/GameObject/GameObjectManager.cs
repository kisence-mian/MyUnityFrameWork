using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameObjectManager
{
    static Vector3 s_OutOfRange = new Vector3(9000, 9000, 9000);

    private static Transform s_poolParent;
    public static Transform PoolParent
    {
        get
        {
            if (s_poolParent == null)
            {
                GameObject instancePool = new GameObject("ObjectPool");
                s_poolParent = instancePool.transform;
                if (Application.isPlaying)
                    UnityEngine.Object.DontDestroyOnLoad(s_poolParent);
            }

            return s_poolParent;
        }
    }

    #region 旧版本对象池
    private static Dictionary<string, Dictionary<int, GameObject>> createPools = new Dictionary<string, Dictionary<int, GameObject>>();
    private static Dictionary<string, Dictionary<int, GameObject>> recyclePools = new Dictionary<string, Dictionary<int, GameObject>>();

    public static Dictionary<string, Dictionary<int, GameObject>> GetCreatePool()
    {
        return createPools;
    }
    public static Dictionary<string, Dictionary<int, GameObject>> GetRecyclePool()
    {
        return recyclePools;
    }
    /// <summary>
    /// 加载一个对象并把它实例化
    /// </summary>
    /// <param name="gameObjectName">对象名</param>
    /// <param name="parent">对象的父节点,可空</param>
    /// <returns></returns>
    private static GameObject NewGameObject(string gameObjectName, GameObject parent = null)
    {
        GameObject goTmp = ResourceManager.Load<GameObject>(gameObjectName);

        if (goTmp == null)
        {
            throw new Exception("CreateGameObject error dont find :" + gameObjectName);
        }

        return ObjectInstantiate(goTmp, parent);
    }

    private static GameObject ObjectInstantiate(GameObject prefab, GameObject parent = null)
    {
        if (prefab == null)
        {
            throw new Exception("CreateGameObject error : prefab  is null");
        }
        Transform transform = parent == null ? null : parent.transform;
        GameObject instanceTmp = GameObject.Instantiate(prefab, transform);
        instanceTmp.name = prefab.name;
        return instanceTmp;
    }


    public static bool IsExist(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            Debug.LogError("GameObjectManager objectName is null!");
            return false;
        }

        if ((recyclePools.ContainsKey(objectName) && recyclePools[objectName].Count > 0)
            || (createPools.ContainsKey(objectName)&& createPools[objectName].Count>0))
        {
            return true;
        }

        return false;
    }

    //判断是否在对象池中
    public static bool IsExist(GameObject go)
    {
        if ((recyclePools.ContainsKey(go.name) && recyclePools[go.name].Count > 0)
            || (createPools.ContainsKey(go.name) && createPools[go.name].Count > 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

   
    public static GameObject CreateGameObject(string name, GameObject parent = null, bool isSetActive = true)
    {
        return GetNewObject(true, name, null, parent, isSetActive);
    }

    public static GameObject CreateGameObject(GameObject prefab, GameObject parent = null, bool isSetActive = true)
    {
        return GetNewObject(true, null, prefab, parent, isSetActive);
    }
    /// <summary>
    /// 从对象池取出一个对象，如果没有，则直接创建它
    /// </summary>
    /// <param name="name">对象名</param>
    /// <param name="parent">要创建到的父节点</param>
    /// <returns>返回这个对象</returns>
    public static GameObject CreateGameObjectByPool(string name, GameObject parent = null, bool isSetActive = true)
    {
        return GetNewObject(false, name, null, parent, isSetActive);
    }

    public static GameObject CreateGameObjectByPool(GameObject prefab, GameObject parent = null, bool isSetActive = true)
    {
        return GetNewObject(false, null, prefab, parent, isSetActive);
    }
    private static List<int> objIDs = new List<int>();
    private static GameObject GetNewObject(bool isAlwaysNew, string objName, GameObject prefab, GameObject parent = null, bool isSetActive = true)
    {
        GameObject go = null;
        string name = objName;
        if (string.IsNullOrEmpty(name))
        {
            name = prefab.name;
        }

        if (!isAlwaysNew && IsExist(name))
        {
            if (!recyclePools.ContainsKey(name))
            {
                if (prefab != null)
                {
                    go = ObjectInstantiate(prefab, parent);
                }
                else
                {
                    go = NewGameObject(name, parent);
                }
            }
            else
            {
                objIDs.Clear();
                objIDs.AddRange(recyclePools[name].Keys);
                int id = objIDs[0];
                go = recyclePools[name][id];
                recyclePools[name].Remove(id);
                if (recyclePools[name].Count == 0)
                    recyclePools.Remove(name);
            }
           
        }
        else
        {
            if (prefab == null && !string.IsNullOrEmpty(objName))
            {
                go = NewGameObject(name, parent);
               
            }
            else if (prefab != null && string.IsNullOrEmpty(objName))
            {
                go = ObjectInstantiate(prefab, parent);
            }
        }
        if (go == null)
        {
            Debug.LogError("GameObjectManager 加载失败：" + name);
            return go;
        }
        if (createPools.ContainsKey(name))
        {
            createPools[name].Add(go.GetInstanceID(), go);
        }
        else
        {
            createPools.Add(name, new  Dictionary<int, GameObject>() { { go.GetInstanceID(), go } });
        }
        AssetsUnloadHandler.MarkUseAssets(name);
        PoolObject po = go.GetComponent<PoolObject>();
        if (po)
        {
            try
            {
                po.OnFetch();
            }
            catch(Exception e)
            {
                Debug.LogError("GetNewObject Error: " + e.ToString());
            }
        }

        if (isSetActive)
            go.SetActive(true);

        if (parent == null)
        {
            go.transform.SetParent(null);
        }
        else
        {
            go.transform.SetParent(parent.transform);
        }
        return go;
    }

    /// <summary>
    /// 将一个对象放入对象池
    /// </summary>
    /// <param name="go"></param>
    /// <param name="isSetInactive">是否将放入的物体设为不激活状态（obj.SetActive(false)）</param>
    public static void DestroyGameObjectByPool(GameObject go, bool isSetInactive = true)
    {

        if (go == null)
            return;

        string key = go.name.Replace("(Clone)", "");
        if (recyclePools.ContainsKey(key) == false)
        {
            recyclePools.Add(key, new  Dictionary<int, GameObject>());
        }

        if (recyclePools[key].ContainsKey(go.GetInstanceID()))
        {
            Debug.LogError("DestroyGameObjectByPool:-> Repeat Destroy GameObject !" + go);
            return;
        }

        recyclePools[key].Add(go.GetInstanceID(), go);

        if (isSetInactive)
            go.SetActive(false);
        else
        {
            go.transform.position = s_OutOfRange;
        }

        go.name = key;
        go.transform.SetParent(PoolParent);
        PoolObject po = go.GetComponent<PoolObject>();
        if (po)
        {
            po.OnRecycle();
        }


        if (createPools.ContainsKey(key) && createPools[key].ContainsKey(go.GetInstanceID()))
        {
            createPools[key].Remove(go.GetInstanceID());
            //ResourceManager.DestoryAssetsCounter(go.name);
        }
        else
        {
            Debug.LogError("创建池不存在GameObject：" + go + " 不能回收！");
        }

    }
    /// <summary>
    /// 立即摧毁克隆体
    /// </summary>
    /// <param name="go"></param>
    public static void DestroyGameObject(GameObject go)
    {
        if (go == null)
            return;

        string key = go.name.Replace("(Clone)", "");

        PoolObject po = go.GetComponent<PoolObject>();
        if (po)
        {
            po.OnObjectDestroy();
        }

        if (createPools.ContainsKey(key) && createPools[key].ContainsKey(go.GetInstanceID()))
        {
            createPools[key].Remove(go.GetInstanceID());

            if (createPools[key].Count == 0)
            {
                createPools.Remove(key);
            }

        }
        ResourceManager.DestoryAssetsCounter(go.name);
        UnityEngine.Object.Destroy(go);
    }

    public static void DestroyGameObjectByPool(GameObject go, float time)
    {
        Timer.DelayCallBack(time, (object[] obj) =>
        {
            if (go != null)//应对调用过CleanPool()
                DestroyGameObjectByPool(go);
        });
    }

    private static List<string> removeObjList = new List<string>();
    /// <summary>
    /// 清空对象池
    /// </summary>
    public static void CleanPool()
    {
        //Debug.LogWarning("清空对象池");
        removeObjList.Clear();

        foreach (string name in createPools.Keys)
        {

            if (createPools[name].Count == 0)
            {
                removeObjList.Add(name);
                //Debug.Log("Pool DestoryAssetsCounter :" + name);
            }
        }

        foreach (var item in removeObjList)
        {
            createPools.Remove(item);
        }

        foreach (var name in recyclePools.Keys)
        {
            Dictionary<int, GameObject> l_objList = recyclePools[name];

            foreach (var go in l_objList.Values)
            {
                PoolObject po = go.GetComponent<PoolObject>();
                if (po)
                {
                    po.OnObjectDestroy();
                }
                ResourceManager.DestoryAssetsCounter(name);
                UnityEngine.Object.Destroy(go);
            }
            l_objList.Clear();

        }
        recyclePools.Clear();

    }

    /// <summary>
    /// 清除掉某一个对象的所有对象池缓存
    /// </summary>
    public static void CleanPoolByName(string name)
    {
        Debug.Log("CleanPool :" + name);
        if (recyclePools.ContainsKey(name))
        {
            Dictionary<int, GameObject> l_objList = recyclePools[name];

            foreach (var go in l_objList.Values)
            {

                PoolObject po = go.GetComponent<PoolObject>();
                if (po)
                {
                    po.OnObjectDestroy();
                }

                GameObject.Destroy(go);
            }
            l_objList.Clear();
            recyclePools.Remove(name);
        }

        if (createPools[name].Count == 0)
        {
            createPools.Remove(name);
            ResourceManager.DestoryAssetsCounter(name);
        }
    }

    #endregion

 

   
}
