using HDJ.Framework.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadManager:MonoBehaviour
{
    private static PreloadManager instance = null;

    private static PreloadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("[PreloadManager]").AddComponent<PreloadManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    /// <summary>
    /// 进度显示 ：当前数量：最大数量
    /// </summary>
    public static CallBack<int,int> progressCallBack;

    public static void StartLoad(List<PreloadResourcesDataGenerate> otherResList=null)
    {
        Instance.Prepare(otherResList);
    }

    private int count;
    private int currentNum;
    private List<PreloadResourcesDataGenerate> queueRes = new List<PreloadResourcesDataGenerate>();

    private void Prepare(List<PreloadResourcesDataGenerate> otherResList)
    {
        List<PreloadResourcesDataGenerate> configs = DataGenerateManager<PreloadResourcesDataGenerate>.GetAllDataList();
        if (otherResList != null)
            queueRes.AddRange(otherResList);

        queueRes.AddRange(configs);

        count = queueRes.Count;
        currentNum = 0;
        LoadQueue();
    }

    private void LoadQueue()
    {
        if (currentNum >= count)
        {
            RunCallBack();
            Destroy();
            return;
        }
        PreloadResourcesDataGenerate da = queueRes[currentNum];
        currentNum++;
        //Debug.Log("da.m_key " + da.m_key);
        try
        {
            Type resType= ReflectionUtils.GetTypeByTypeFullName(da.m_ResType);
            ResourceManager.LoadAsync(da.m_key,resType, (LoadState loadState, object loadRes) =>
            {
                if (loadState.isDone)
                {
                    if (loadRes != null && loadRes is GameObject)
                    {
                        if (da.m_instantiateNum > 0)
                        {
                            GameObject prefab = (GameObject)loadRes;
                            for (int i = 0; i < da.m_instantiateNum; i++)
                            {
                                GameObject obj = GameObjectManager.CreateGameObject(prefab);
                                GameObjectManager.DestroyGameObjectByPool(obj, !da.m_createInstanceActive);
                            }
                        }
                    }
                    else
                    {
                        if (loadRes == null)
                        {
                            Debug.LogError("Error： 预加载失败  key：" + da.m_key);
                        }
                    }
                    RunCallBack();
                    LoadQueue();
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            LoadQueue();
        }
    }

    private void RunCallBack()
    {
        if (progressCallBack != null)
            progressCallBack(currentNum, count);
    }
    //private void Update()
    //{
    //    List<PreloadResourcesDataGenerate> datas = DataGenerateManager<PreloadResourcesDataGenerate>.GetAllDataList();
    //    //  Debug.Log("datas.Count :" + datas.Count);
    //    if (currentNum >= count)
    //    {
    //        RunCallBack();
    //        Destroy();
    //        return;
    //    }
    //    PreloadResourcesDataGenerate da = datas[currentNum];
    //    currentNum++;
    //    //Debug.Log("da.m_key " + da.m_key);
    //    ResourceManager.LoadAsync(da.m_key, (LoadState loadState, object loadRes) =>
    //         {
    //             if (loadState.isDone)
    //             {
    //                 if (loadRes != null && loadRes is GameObject)
    //                 {
    //                     if (da.m_instantiateNum > 0)
    //                     {
    //                         GameObject prefab = (GameObject)loadRes;
    //                         for (int i = 0; i < da.m_instantiateNum; i++)
    //                         {
    //                             GameObject obj = GameObjectManager.CreateGameObject(prefab);
    //                             GameObjectManager.DestroyGameObjectByPool(obj, !da.m_createInstanceActive);
    //                         }
    //                     }
    //                 }
    //                 else
    //                 {
    //                     if (loadRes == null)
    //                     {
    //                         Debug.LogError("Error： 预加载失败  key：" + da.m_key);
    //                     }
    //                 }
    //                 RunCallBack();
    //             }
    //         });


    //    //Debug.Log("loadRes " + loadRes);


    //}

    private void Destroy()
    {
        Destroy(instance.gameObject);
    }
}
