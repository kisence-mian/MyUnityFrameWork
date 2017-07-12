using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class EditorTool :MonoBehaviour
{
    public static Type GetType(string typeName)
    {
        return Type.GetType(typeName);
    }

    public static Type[] GetTypes()
    {
        return Assembly.Load("Assembly-CSharp").GetTypes();
    }

    public static string[] GetAllEnumType()
    {
        List<string> listTmp = new List<string>();
#if UNITY_WEBGL
        Type[] types = Assembly.Load(Assembly.GetExecutingAssembly().FullName).GetTypes();
#else
        Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();
#endif

        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsSubclassOf(typeof(Enum)))
            {
                if (EditorTool.GetType(types[i].Name) != null)
                {
                    listTmp.Add(types[i].Name);
                }
            }
        }
        return listTmp.ToArray();

    }

    public static int GetAllEnumTypeIndex(string typeName)
    {
        string[] Types = GetAllEnumType();

        for (int i = 0; i < Types.Length; i++)
        {
            if (typeName == Types[i])
            {
                return i;
            }
        }

        return -1;
    }
}
