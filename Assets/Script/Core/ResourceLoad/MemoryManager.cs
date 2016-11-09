using UnityEngine;
using System.Collections;
using System;

public class MemoryManager
{
    ///// <summary>
    ///// 是否允许动态加载
    ///// </summary>
    //public static bool s_allowDynamicLoad = false;

    ///// <summary>
    ///// 最大允许的内存使用量
    ///// </summary>
    //public static int s_MaxMemoryUse = 150;

    /// <summary>
    /// 释放一部分内存
    /// </summary>
    public static void FreeMemory()
    {
        //清空对象池
        GameObjectManager.CleanPool();

        //清空缓存的UI
        UIManager.DestroyAllHideUI();

        //GC
        GC.Collect();
    }

    ///// <summary>
    ///// 用于监控内存
    ///// </summary>
    ///// <param name="tag"></param>
    //public void JudgeMemorySize(string tag)
    //{

    //}
}
