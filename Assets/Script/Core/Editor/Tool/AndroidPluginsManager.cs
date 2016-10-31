using UnityEngine;
using System.Collections;
using UnityEditor;

public class AndroidPluginsManager :  EditorWindow
{

    [MenuItem("Window/安卓插件管理器")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AndroidPluginsManager));
    }

    void OnProjectChange()
    {
        FindALLManiFest();
    }

    #region GUI
    void OnGUI()
    {
        titleContent.text = "安卓插件管理器";

    }

    #endregion

    #region Manifest

    //获取所有的清单文件
    void FindALLManiFest()
    {

    }

    #endregion

}
