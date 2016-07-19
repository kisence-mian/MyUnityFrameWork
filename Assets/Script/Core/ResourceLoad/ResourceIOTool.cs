using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;

/// <summary>
/// 资源读取器，负责从不同路径读取资源
/// </summary>
public static class ResourceIOTool
{
    #region 读操作
    public static string ReadStringByFile(string path)
    {
        string content = "";

        try
        {
            FileInfo t = new FileInfo(path);
            if (!t.Exists)
            {
                return "";
            }

            StreamReader sr = null;
            sr = File.OpenText(path);
            while ((content += sr.ReadLine()) != null)
            {
                break;
            }

            sr.Close();
            sr.Dispose();

        }
        catch (Exception e)
        {
            Debug.Log("Load text fail ! message:" + e.Message);
        }
        return content;
    }

    public static string ReadStringByResource(string path)
    {
        TextAsset text = (TextAsset)Resources.Load(path);

        if(text == null)
        {
            return "";
        }
        else
        {
            return text.text;
        }
    }

    #endregion

    #region 写操作

    public static void WriteStringByFile(string path, string content)
    {
        byte[] dataByte = Encoding.GetEncoding("UTF-8").GetBytes(content);

        CreateFile(path, dataByte);
    }

    //web Player 不支持该函数
#if !UNITY_WEBPLAYER
    public static void CreateFile(string path, byte[] byt)
    {
        try
        {
            FileTool.CreatFilePath(path);

            File.WriteAllBytes(path, byt);
        }
        catch (Exception e)
        {
            Debug.LogError("File Create Fail! \n" + e.Message);
        }
    }
#endif

    #endregion
}
