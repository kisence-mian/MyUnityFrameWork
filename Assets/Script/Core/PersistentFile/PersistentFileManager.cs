using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class PersistentFileManager
{
    public const string c_directoryName = "PersistentFile";
    public const string c_expandName = "txt";

    public static string GetData(string name)
    {
        string data = "";

        string fullPath = PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                PathTool.GetRelativelyPath(c_directoryName,
                                            name,
                                            c_expandName));
        if (File.Exists(fullPath))
        {
            //永远从沙盒路径读取
            data = ResourceIOTool.ReadStringByFile(fullPath);
        }

        return data;
    }

    public static void SaveData(string name, string content)
    {
#if !UNITY_WEBGL

        ResourceIOTool.WriteStringByFile(
            PathTool.GetAbsolutePath(ResLoadLocation.Persistent,
                PathTool.GetRelativelyPath(c_directoryName,
                                                    name,
                                                    c_expandName)),
                content);

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
#endif
    }

    public static string[] GetFileList()
    {
        FileTool.CreatPath(PathTool.GetAbsolutePath(ResLoadLocation.Persistent, c_directoryName));

        List<string> relpayFileNames = new List<string>();
        string[] allFileName = Directory.GetFiles(PathTool.GetAbsolutePath(ResLoadLocation.Persistent, c_directoryName));
        foreach (var item in allFileName)
        {
            if (item.EndsWith("." + c_expandName))
            {
                string configName = FileTool.RemoveExpandName(FileTool.GetFileNameByPath(item));
                relpayFileNames.Add(configName);
            }
        }

        return relpayFileNames.ToArray() ?? new string[0];
    }

    public static string GetPath(string name)
    {
        return PathTool.GetAbsolutePath(ResLoadLocation.Persistent, PathTool.GetRelativelyPath(c_directoryName, name, c_expandName));
    }
}
