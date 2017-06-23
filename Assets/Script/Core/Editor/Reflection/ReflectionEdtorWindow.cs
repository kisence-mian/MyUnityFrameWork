using UnityEngine;
using System.Collections;
using UnityEditor;

public class ReflectionEdtorWindow : EditorWindow
{
    [MenuItem("Window/反射查看器")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ReflectionEdtorWindow));
    }

    void OnEnable()
    {
        EditorGUIStyleData.Init();
    }

    void OnGUI()
    {
        //程序集

        //命名空间

        //类

        //方法

        //生成调用代码

        //搜索 或者 指定
    }

}
