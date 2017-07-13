using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameObjectManager :MonoBehaviour
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
                if(Application.isPlaying)
                DontDestroyOnLoad(s_poolParent);
            }

            return s_poolParent;
        }
    }

    #region 旧版本对象池

    static Dictionary<string, List<GameObject>> s_objectPool = new Dictionary<string, List<GameObject>>();

    /// <summary>
    /// 加载一个对象并把它实例化
    /// </summary>
    /// <param name="gameObjectName">对象名</param>
    /// <param name="parent">对象的父节点,可空</param>
    /// <returns></returns>
    public static GameObject CreateGameObject(string gameObjectName,GameObject parent = null)
    {
        GameObject goTmp = ResourceManager.Load<GameObject>(gameObjectName);

        if (goTmp == null)
        {
            throw new Exception("CreateGameObject error dont find :" + gameObjectName);
        }

        return CreateGameObject(goTmp, parent);
    }

    public static GameObject CreateGameObject(GameObject prefab, GameObject parent = null)
    {
        if (prefab == null)
        {
            throw new Exception("CreateGameObject error : l_prefab  is null");
        }

        GameObject instanceTmp = Instantiate(prefab);
        instanceTmp.name = prefab.name;

        if (parent != null)
        {
            instanceTmp.transform.SetParent(parent.transform);
        }
        //l_instanceTmp.transform.localScale = Vector3.one;
        return instanceTmp;
    }


    public static bool IsExist(string objectName)
    {
        if (objectName == null)
        {
            Debug.LogError("GameObjectManager objectName is null!");
            return false;
        }

        if (s_objectPool.ContainsKey(objectName) && s_objectPool[objectName].Count > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 从对象池取出一个对象，如果没有，则直接创建它
    /// </summary>
    /// <param name="name">对象名</param>
    /// <param name="parent">要创建到的父节点</param>
    /// <returns>返回这个对象</returns>
    public static GameObject CreateGameObjectByPool(string name,GameObject parent = null,bool isSetActive = true)
    {
        if (IsExist(name))
        {
            GameObject go = s_objectPool[name][0];
            s_objectPool[name].RemoveAt(0);

            if(isSetActive)
                go.SetActive(true);

            if (parent == null)
            {
                go.transform.SetParent(null);
            }
            else
            {
                go.transform.SetParent(parent.transform);
            }

            //go.transform.localScale = Vector3.one;

            return go;
        }
        else
        {
            return CreateGameObject(name, parent);
        }
    }

    public static GameObject CreateGameObjectByPool(GameObject prefab, GameObject parent = null, bool isSetActive = true)
    {
        if (IsExist(prefab.name))
        {
            GameObject go = s_objectPool[prefab.name][0];
            s_objectPool[prefab.name].RemoveAt(0);

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
        else
        {
            return CreateGameObject(prefab, parent);
        }
    }

    /// <summary>
    /// 将一个对象放入对象池
    /// </summary>
    /// <param name="obj">目标对象</param>
    public static void DestroyGameObjectByPool(GameObject obj, bool isSetActive = true)
    {
        string key = obj.name.Replace("(Clone)", "");

        if (s_objectPool.ContainsKey(key) == false)
        {
            s_objectPool.Add(key, new List<GameObject>());
        }

        s_objectPool[key].Add(obj);

        if(isSetActive)
            obj.SetActive(false);
        else
        {
            obj.transform.position = s_OutOfRange;
        }

        obj.name = key;
        obj.transform.SetParent(PoolParent);
    }

    public static void DestroyGameObjectByPool(GameObject go,float time)
    {
        Timer.DelayCallBack(time, (object[] obj) =>
        {
            DestroyGameObjectByPool(go);
        });
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public static void CleanPool()
    {
        foreach (string name in s_objectPool.Keys)
        {
            if (s_objectPool.ContainsKey(name))
            {
                List<GameObject> l_objList = s_objectPool[name];

                for (int i = 0; i < l_objList.Count; i++)
                {
                    Destroy(l_objList[i]);
                }

                l_objList.Clear();

            }
        }

        s_objectPool.Clear();
    }

    /// <summary>
    /// 清除掉某一个对象的所有对象池缓存
    /// </summary>
    public static void CleanPoolByName(string name)
    {
        Debug.Log("CleanPool :" + name);

        if (s_objectPool.ContainsKey(name))
        {
            List<GameObject> l_objList = s_objectPool[name];

            for (int i = 0; i < l_objList.Count; i++)
            {
                Destroy(l_objList[i]);
            }

            l_objList.Clear();

            s_objectPool.Remove(name);
        }
    }

    #endregion

    #region 旧版本对象池 异步方法

    public static void CreateGameObjectByPoolAsync(string name, CallBack<GameObject> callback, GameObject parent = null, bool isSetActive = true)
    {
        ResourceManager.LoadAsync(name, (status, res) =>
        {
            if(status.isDone)
            {
                try
                {
                    callback(CreateGameObjectByPool(name, parent, isSetActive));
                }
                catch (Exception e)
                {
                    Debug.LogError("CreateGameObjectByPoolAsync Exception: " + e.ToString());
                }
            }
        });
    }

    #endregion

    #region 新版本对象池

    static Dictionary<string, List<PoolObject>> s_objectPool_new = new Dictionary<string, List<PoolObject>>();

    /// <summary>
    /// 加载一个对象并把它实例化
    /// </summary>
    /// <param name="gameObjectName">对象名</param>
    /// <param name="parent">对象的父节点,可空</param>
    /// <returns></returns>
    static PoolObject CreatePoolObject(string gameObjectName, GameObject parent = null)
    {
        GameObject go = ResourceManager.Load<GameObject>(gameObjectName);

        if (go == null)
        {
            throw new Exception("CreatPoolObject error dont find : ->" + gameObjectName + "<-");
        }

        GameObject instanceTmp = Instantiate(go);
        instanceTmp.name = go.name;

        PoolObject po = instanceTmp.GetComponent<PoolObject>();

        if (po == null)
        {
            throw new Exception("CreatPoolObject error : ->" + gameObjectName + "<- not is PoolObject !");
        }

        po.OnCreate();

        if (parent != null)
        {
            instanceTmp.transform.SetParent(parent.transform);
        }

        instanceTmp.SetActive(true);

        return po;
    }

    /// <summary>
    /// 把一个对象放入对象池
    /// </summary>
    /// <param name="gameObjectName"></param>
    public static void PutPoolObject(string gameObjectName)
    {
        DestroyPoolObject(CreatePoolObject(gameObjectName));
    }

    /// <summary>
    /// 预存入对象池
    /// </summary>
    /// <param name="name"></param>
    public static void PutPoolGameOject(string name)
    {
        DestroyGameObjectByPool(CreateGameObjectByPool(name));
    }

    public static bool IsExist_New(string objectName)
    {
        if (objectName == null)
        {
            Debug.LogError("IsExist_New error : objectName is null!");
            return false;
        }

        if (s_objectPool_new.ContainsKey(objectName) && s_objectPool_new[objectName].Count > 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 从对象池取出一个对象，如果没有，则直接创建它
    /// </summary>
    /// <param name="name">对象名</param>
    /// <param name="parent">要创建到的父节点</param>
    /// <returns>返回这个对象</returns>
    public static PoolObject GetPoolObject(string name, GameObject parent = null)
    {
        PoolObject po;
        if (IsExist_New(name))
        {
            po = s_objectPool_new[name][0];
            s_objectPool_new[name].RemoveAt(0);
            if (po && po.SetActive)
                po.gameObject.SetActive(true);

            if (parent == null)
            {
                po.transform.SetParent(null);
            }
            else
            {
                po.transform.SetParent(parent.transform);
            }
        }
        else
        {
            po = CreatePoolObject(name, parent);
        }

        po.OnFetch();

        return po;
    }

    /// <summary>
    /// 将一个对象放入对象池
    /// </summary>
    /// <param name="obj">目标对象</param>
    public static void DestroyPoolObject(PoolObject obj)
    {
        string key = obj.name.Replace("(Clone)", "");

        if (s_objectPool_new.ContainsKey(key) == false)
        {
            s_objectPool_new.Add(key, new List<PoolObject>());
        }

        s_objectPool_new[key].Add(obj);

        if (obj.SetActive)
            obj.gameObject.SetActive(false);
        else
            obj.transform.position = s_OutOfRange;

        obj.OnRecycle();

        obj.name = key;
        obj.transform.SetParent(PoolParent);
    }

    public static void DestroyPoolObject(PoolObject go, float time)
    {
        Timer.DelayCallBack(time, (object[] obj) =>
        {
            DestroyPoolObject(go);
        });
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public static void CleanPool_New()
    {
        foreach (string name in s_objectPool_new.Keys)
        {
            if (s_objectPool_new.ContainsKey(name))
            {
                List<PoolObject> objList = s_objectPool_new[name];

                for (int i = 0; i < objList.Count; i++)
                {
                    try
                    {
                        objList[i].OnObjectDestroy();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.ToString());
                    }

                    Destroy(objList[i].gameObject);
                }

                objList.Clear();
            }
        }

        s_objectPool_new.Clear();
    }

    /// <summary>
    /// 清除掉某一个对象的所有对象池缓存
    /// </summary>
    public static void CleanPoolByName_New(string name)
    {
        if (s_objectPool_new.ContainsKey(name))
        {
            List<PoolObject> objList = s_objectPool_new[name];

            for (int i = 0; i < objList.Count; i++)
            {
                try
                {
                    objList[i].OnObjectDestroy();
                }
                catch(Exception e)
                {
                    Debug.Log(e.ToString());
                }

                Destroy(objList[i].gameObject);
            }

            objList.Clear();
            s_objectPool_new.Remove(name);
        }
    }

    #endregion

    #region 新版本对象池 异步方法

    public static void CreatePoolObjectAsync(string name, CallBack<PoolObject> callback, GameObject parent = null)
    {
        ResourceManager.LoadAsync(name, (status,res) =>
        {
            try
            {
                callback(CreatePoolObject(name, parent));
            }
            catch(Exception e)
            {
                Debug.LogError("CreatePoolObjectAsync Exception: " + e.ToString());
            }
        });
    }

    #endregion
}
