using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器功能增强
/// </summary>
public class EditorExpand
{
    #region 宏定义
    /// <summary>
    /// 设置宏定义
    /// </summary>
    /// <param name="defines"></param>
    public static void SetDefine(string[] defines)
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
#if UNITY_ANDROID
        targetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
        targetGroup = BuildTargetGroup.iOS;
#elif UNITY_WEBGL
        targetGroup = BuildTargetGroup.WebGL;
#endif
        string define = "";

        for (int i = 0; i < defines.Length; i++)
        {
            if (!define.Contains(defines[i]))
            {
                define += defines[i];
            }

            if(i != defines.Length - 1)
            {
                define += ";";
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, define);
    }

    /// <summary>
    /// 设置宏定义
    /// </summary>
    /// <param name="defines"></param>
    public static void SetDefine(List<string> defines)
    {
        SetDefine(defines.ToArray());
    }

    public static void ChangeDefine(string[] addList,string[] removeList)
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;
#if UNITY_ANDROID
        targetGroup = BuildTargetGroup.Android;
#elif UNITY_IOS
        targetGroup = BuildTargetGroup.iOS;
#elif UNITY_WEBGL
        targetGroup = BuildTargetGroup.WebGL;
#endif

        string[] oldDefine = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';');
        List<string> defines = new List<string>();
        defines.AddRange(oldDefine);

        //去重添加
        for (int i = 0; i < addList.Length; i++)
        {
            if(!defines.Contains(addList[i]))
            {
                defines.Add(addList[i]);
            }
        }

        for (int i = 0; i < removeList.Length; i++)
        {
            defines.Remove(removeList[i]);
        }

        SetDefine(defines);
    }

    #endregion

    #region Sorting Layer

    public static void AddSortLayerIfNotExist(string name)
    {
        if(!isExistShortLayer(name))
        {
            int index = GetSortingLayerCount();
            AddSortingLayer();
            SetSortingLayerName(index,name);
        }
    }

    public static bool isExistShortLayer(string name)
    {
        bool isExist = false;

        string[] layers = get_sortingLayerNames();

        for (int i = 0; i < layers.Length; i++)
        {
            if (name == layers[i])
            {
                isExist = true;
            }
        }

        return isExist;
    }
    public static string GetSortingLayer(int index)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Assembly ab = Assembly.Load("UnityEditor");
        Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
        MethodInfo mi = type.GetMethod("GetSortingLayerName", flags);

        object[] objs = new object[1];
        objs[0] = index;

        return (string)mi.Invoke(null, objs);
    }

    public static int GetSortingLayerCount()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Assembly ab = Assembly.Load("UnityEditor");
        Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
        MethodInfo mi = type.GetMethod("GetSortingLayerCount", flags);

        return (int)mi.Invoke(null, null);
    }

    public static void SetSortingLayerName(int index, string name)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Assembly ab = Assembly.Load("UnityEditor");
        Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
        MethodInfo mi = type.GetMethod("SetSortingLayerName", flags);

        object[] objs = new object[2];
        objs[0] = index;
        objs[1] = name;

        mi.Invoke(null, objs);
    }

    public static void SetSortingLayerLocked(int index, bool locked)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Assembly ab = Assembly.Load("UnityEditor");
        Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
        MethodInfo mi = type.GetMethod("SetSortingLayerLocked", flags);

        object[] objs = new object[2];
        objs[0] = index;
        objs[1] = locked;

        mi.Invoke(null, objs);
    }

    public static bool GetSortingLayerLocked(int index)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Assembly ab = Assembly.Load("UnityEditor");
        Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
        MethodInfo mi = type.GetMethod("GetSortingLayerLocked", flags);

        object[] objs = new object[1];
        objs[0] = index;

        return (bool)mi.Invoke(null, objs);
    }

    public static void AddSortingLayer()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Assembly ab = Assembly.Load("UnityEditor");
        Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");
        MethodInfo mi = type.GetMethod("AddSortingLayer", flags);

        mi.Invoke(null, null);
    }

    public static string[] get_sortingLayerNames()
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        Assembly ab = Assembly.Load("UnityEditor");
        Type type = ab.GetType("UnityEditorInternal.InternalEditorUtility");

        MethodInfo mi = type.GetMethod("get_sortingLayerNames", flags);

        return (string[])mi.Invoke(null, null);
    }

    #endregion
}
