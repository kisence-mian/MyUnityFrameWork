using UnityEngine;
using System.Collections;
using System.Text;

public class PathTool
{
    /// <summary>
    /// 获得绝对路径
    /// </summary>
    /// <param name="loadType">资源加载类型</param>
    /// <param name="relativelyPath">相对路径</param>
    /// <returns>绝对路径</returns>

    public static string GetAbsolutePath(ResLoadType loadType, string relativelyPath)
    {
        StringBuilder path = new StringBuilder();
        switch (loadType)
        {
            case ResLoadType.Resource:
                #if UNITY_EDITOR
                    path.Append(Application.dataPath);
                    path.Append("/Resources/");
                    break;
                #endif

            case ResLoadType.Streaming:
                path.Append(Application.streamingAssetsPath);
                path.Append("/");
                break;

            case ResLoadType.Persistent:
                path.Append(Application.persistentDataPath);
                path.Append("/");
                break;

            case ResLoadType.Catch:
                path.Append(Application.temporaryCachePath);
                path.Append("/");
                break;

            default:
                Debug.LogError("Type Error !" + loadType);
                break;
        }

        path.Append(relativelyPath);
        return path.ToString();
    }

    //获取相对路径
    public static string GetRelativelyPath(string directoryName, string fileName, string expandName)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(directoryName);
        builder.Append("/");
        builder.Append(fileName);
        builder.Append(".");
        builder.Append(expandName);

        return builder.ToString();
    }

    #if UNITY_EDITOR

    /// <summary>
    /// 获取编辑器下的路径
    /// </summary>
    /// <param name="directoryName">目录名</param>
    /// <param name="fileName">文件名</param>
    /// <param name="expandName">拓展名</param>
    /// <returns></returns>
    public static string GetEditorPath(string directoryName, string fileName, string expandName)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(Application.dataPath);
        builder.Append("/Editor");
        builder.Append(directoryName);
        builder.Append("/");
        builder.Append(fileName);
        builder.Append(".");
        builder.Append(expandName);

        return builder.ToString();
    }

    #endif
}
