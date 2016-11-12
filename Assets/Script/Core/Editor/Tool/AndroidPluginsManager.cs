using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;

public class AndroidPluginsManager : EditorWindow
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
        //manifest列表
        //主manifest信息
        //库列表
    }

    #endregion

    #region Manifest

    //获取所有的清单文件
    void FindALLManiFest()
    {

    }

    #endregion

    #region JAR



    #endregion

    //class ManiFestFileInfo
    //{
    //    public string m_showName;
    //    public string m_path;
    //}

}



public class AndroidManiFestInfo
{
    /// <summary>
    /// 包名
    /// </summary>
    string m_package;

    /// <summary>
    /// 图标
    /// </summary>
    string m_icon;

    /// <summary>
    /// 描述
    /// </summary>
    string m_label;

    /// <summary>
    /// 这个名称是给用户看的
    /// </summary>
    string m_versionName;

    /// <summary>
    /// 设备程序识别版本
    /// </summary>
    string m_versionCode;

    /// <summary>
    /// 用户权限
    /// </summary>
    List<string> m_usesPermission;

    /// <summary>
    /// 自定义权限
    /// </summary>
    List<string> m_permission;

    /// <summary>
    /// 自定义数据
    /// </summary>
    Dictionary<string, string> m_metaData;


    public class ActivityInfo
    {


        /// <summary>
        /// XML数据
        /// </summary>
        XmlNode xmlNode;
    }
}
