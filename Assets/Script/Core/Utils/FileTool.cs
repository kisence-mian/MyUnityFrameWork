using UnityEngine;
using System.Collections;
using System.IO;

public class FileTool  
{
    /// <summary>
    /// 判断有没有这个文件路径，如果没有则创建它(路径会去掉文件名)
    /// </summary>
    /// <param name="filepath"></param>
    public static void CreatFilePath(string filepath)
    {
        string newPathDir = Path.GetDirectoryName(filepath);

        CreatPath(newPathDir);
    }

    /// <summary>
    /// 判断有没有这个路径，如果没有则创建它
    /// </summary>
    /// <param name="filepath"></param>
    public static void CreatPath(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    /// <summary>
    /// 删掉某个目录下的所有子目录和子文件，但是保留这个目录
    /// </summary>
    /// <param name="path"></param>
     public static void DeleteDirectory(string path)
    {
        string[] directorys = Directory.GetDirectories(path);

        //删掉所有子目录
        for (int i = 0; i < directorys.Length; i++)
        {
            string pathTmp = directorys[i];

            if (Directory.Exists(pathTmp))
            {
                Directory.Delete(pathTmp, true);
            }
        }

        //删掉所有子文件
        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            string pathTmp = files[i];
            if (File.Exists(pathTmp))
            {
                File.Delete(pathTmp);
            }
        }
    }

    //移除拓展名
    public static string RemoveExpandName(string name)
    {
        int dirIndex = name.LastIndexOf(".");

        if (dirIndex != -1)
        {
            return name.Remove(dirIndex);
        }
        else
        {
            return name;
        }
    }

    //取出一个路径下的文件名
    public static string GetFileNameByPath(string path)
    {
        FileInfo fi = new FileInfo(path);
        return fi.Name; // text.txt
    }

    /// <summary>
    /// 复制文件夹（及文件夹下所有子文件夹和文件）
    /// </summary>
    /// <param name="sourcePath">待复制的文件夹路径</param>
    /// <param name="destinationPath">目标路径</param>
    public static void CopyDirectory(string sourcePath, string destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string destName = Path.Combine(destinationPath, fsi.Name);

            if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
                File.Copy(fsi.FullName, destName);
            else                                    //如果是文件夹，新建文件夹，递归
            {
                Directory.CreateDirectory(destName);
                CopyDirectory(fsi.FullName, destName);
            }
        }
    }

    public static string GetFileNameBySring(string path)
    {
        string[] paths = path.Split('/');
        return paths[paths.Length - 1];
    }
}
