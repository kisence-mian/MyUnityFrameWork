using UnityEngine;
using System.Collections;
using System.IO;

public class FileTool  
{
    /// <summary>
    /// 判断有没有这个文件路径，如果没有则创建它
    /// </summary>
    /// <param name="filepath"></param>
    public static void CreatFilePath(string filepath)
    {
        string newPathDir = Path.GetDirectoryName(filepath);

        if (!Directory.Exists(newPathDir))
            Directory.CreateDirectory(newPathDir);
    }
}
