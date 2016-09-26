using UnityEngine;
using System.Collections;
using System;

public class EditorTool :MonoBehaviour
{
    public static Type GetType(string l_typeName)
    {
        return Type.GetType(l_typeName);
    }
}
