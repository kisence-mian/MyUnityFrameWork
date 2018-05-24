using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

public class MessageClass2JaveClassEditorWindow : Editor
{
    [MenuItem("Tools/将C#消息类转换成Java类")]
    static void DoIt()
    {
        Type[] types = GetChildTypes(typeof(MessageClassInterface));

        foreach (var item in types)
        {

        }


        EditorUtility.DisplayDialog("提示", "转换完成!", "OK");
    }

    /// <summary>
    /// 获取父类的所有子类
    /// </summary>
    /// <param name="parentType">父类Type</param>
    /// <returns></returns>
    public static Type[] GetChildTypes(Type parentType, bool isContainsAllChild = true)
    {
        List<Type> lstType = new List<Type>();
        Assembly assem = Assembly.GetAssembly(parentType);
        Type[] types = assem.GetTypes();
        //Debug.Log()
        foreach (Type tChild in types)
        {
            if (tChild.BaseType == parentType)
            {
                lstType.Add(tChild);
                if (isContainsAllChild)
                {
                    Type[] temp = GetChildTypes(tChild, isContainsAllChild);
                    if (temp.Length > 0)
                        lstType.AddRange(temp);
                }
            }
        }
        return lstType.ToArray();
    }
}