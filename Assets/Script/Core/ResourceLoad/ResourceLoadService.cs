using UnityEngine;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// 资源读取器，负责从不同路径读取资源
/// </summary>
public static class ResourceLoadService 
{
    public static string LoadStringByFile(string path)
    {
        FileInfo t = new FileInfo(path);
        if (!t.Exists)
        {
            return "";
        }

        StreamReader sr = null;
        sr = File.OpenText(path);
        string line = "";

        while ((line += sr.ReadLine()) != null)
        {
            break;
        }

        sr.Close();
        sr.Dispose();
        return line;
    }

    public static void SaveStringByFile(string path,string content)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path );

        sw = t.CreateText();

        sw.WriteLine(content);
        sw.Close();
        sw.Dispose();
    }
}
