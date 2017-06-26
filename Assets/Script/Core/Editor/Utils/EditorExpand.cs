using System.Collections;
using System.Collections.Generic;
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

    #endregion
}
