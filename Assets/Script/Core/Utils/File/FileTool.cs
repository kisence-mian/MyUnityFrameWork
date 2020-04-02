using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;

public class FileTool  
{
    #region 文件与路径的增加删除创建

    #region 不忽视出错

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
            //Debug.Log(destName);

            if (fsi is FileInfo)          //如果是文件，复制文件
                File.Copy(fsi.FullName, destName);
            else                                    //如果是文件夹，新建文件夹，递归
            {
                Directory.CreateDirectory(destName);
                CopyDirectory(fsi.FullName, destName);
            }
        }
    }

    #endregion

    #region 忽视出错 (会跳过所有出错的操作,一般是用来无视权限)
    /// <summary>
    /// 删除所有可以删除的文件
    /// </summary>
    /// <param name="path"></param>
    public static void SafeDeleteDirectory(string path)
    {
        string[] directorys = Directory.GetDirectories(path);

        //删掉所有子目录
        for (int i = 0; i < directorys.Length; i++)
        {
            string pathTmp = directorys[i];

            if (Directory.Exists(pathTmp))
            {
                SafeDeleteDirectory(pathTmp);
                try
                {
                    Directory.Delete(pathTmp,false);
                }
                catch
                {
                    //Debug.LogError(e.ToString());
                }
            }
        }

        //删掉所有子文件
        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            string pathTmp = files[i];
            if (File.Exists(pathTmp))
            {
                try
                {
                    File.Delete(pathTmp);
                }
                catch/*(Exception e)*/
                {
                    //Debug.LogError(e.ToString());
                }
            }
        }
    }

    /// <summary>
    /// 复制所有可以复制的文件夹（及文件夹下所有子文件夹和文件）
    /// </summary>
    /// <param name="sourcePath">待复制的文件夹路径</param>
    /// <param name="destinationPath">目标路径</param>
    public static void SafeCopyDirectory(string sourcePath, string destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        Directory.CreateDirectory(destinationPath);

        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string destName = Path.Combine(destinationPath, fsi.Name);
            //Debug.Log(destName);

            if (fsi is FileInfo)          //如果是文件，复制文件
                try
                {
                    File.Copy(fsi.FullName, destName);
                }
                catch{}
            else                                    //如果是文件夹，新建文件夹，递归
            {
                Directory.CreateDirectory(destName);
                SafeCopyDirectory(fsi.FullName, destName);
            }
        }
    }

    #endregion

    #endregion

    #region 文件名

    //移除拓展名
    public static string RemoveExpandName(string name)
    {
        if (Path.HasExtension(name))
            name = Path.ChangeExtension(name, null);
        return name;
    }

    public static string GetExpandName(string name)
    {
        return Path.GetExtension(name);
    }

    //取出一个路径下的文件名
    public static string GetFileNameByPath(string path)
    {
        FileInfo fi = new FileInfo(path);
        return fi.Name; // text.txt
    }

    //取出一个相对路径下的文件名
    public static string GetFileNameBySring(string path)
    {
        string[] paths = path.Split('/');
        return paths[paths.Length - 1];
    }

    public static string GetUpperPath(string path)
    {
        int index = path.LastIndexOf('/');

        if(index != -1)
        {
            return path.Substring(0, index);
        }
        else
        {
            return "";
        }
    }

    //修改文件名
    public static void ChangeFileName(string path,string newName)
    {
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Move(path, newName);
        }
    }

    #endregion

    #region 文件编码

    /// <summary>
    /// 文件编码转换
    /// </summary>
    /// <param name="sourceFile">源文件</param>
    /// <param name="destFile">目标文件，如果为空，则覆盖源文件</param>
    /// <param name="targetEncoding">目标编码</param>
    public static void ConvertFileEncoding(string sourceFile, string destFile, System.Text.Encoding targetEncoding)
    {
        destFile = string.IsNullOrEmpty(destFile) ? sourceFile : destFile;
        Encoding sourEncoding = GetEncodingType(sourceFile);

        System.IO.File.WriteAllText(destFile, System.IO.File.ReadAllText(sourceFile, sourEncoding), targetEncoding);
    }

    /// <summary> 
    /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
    /// </summary> 
    /// <param name="FILE_NAME">文件路径</param> 
    /// <returns>文件的编码类型</returns> 
    public static Encoding GetEncodingType(string FILE_NAME)
    {
        FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
        Encoding r = GetEncodingType(fs);
        fs.Close();
        return r;
    }

    /// <summary> 
    /// 通过给定的文件流，判断文件的编码类型 
    /// </summary> 
    /// <param name="fs">文件流</param> 
    /// <returns>文件的编码类型</returns> 
    public static Encoding GetEncodingType(FileStream fs)
    {
        //byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
        //byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
        //byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
        Encoding reVal = Encoding.Default;

        BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
        int i;
        int.TryParse(fs.Length.ToString(), out i);
        byte[] ss = r.ReadBytes(i);
        if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
        {
            reVal = Encoding.UTF8;
        }
        else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
        {
            reVal = Encoding.BigEndianUnicode;
        }
        else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
        {
            reVal = Encoding.Unicode;
        }
        r.Close();
        return reVal;

    }

    /// <summary> 
    /// 判断是否是不带 BOM 的 UTF8 格式 
    /// </summary> 
    /// <param name="data"></param> 
    /// <returns></returns> 
    private static bool IsUTF8Bytes(byte[] data)
    {
        int charByteCounter = 1;
        //计算当前正分析的字符应还有的字节数 
        byte curByte; //当前分析的字节. 
        for (int i = 0; i < data.Length; i++)
        {
            curByte = data[i];
            if (charByteCounter == 1)
            {
                if (curByte >= 0x80)
                {
                    //判断当前 
                    while (((curByte <<= 1) & 0x80) != 0)
                    {
                        charByteCounter++;
                    }
                    //标记位首位若为非0 则至少以2个1开始 如:110XXXXX......1111110X　 
                    if (charByteCounter == 1 || charByteCounter > 6)
                    {
                        return false;
                    }
                }
            }
            else
            {
                //若是UTF-8 此时第一位必须为1 
                if ((curByte & 0xC0) != 0x80)
                {
                    return false;
                }
                charByteCounter--;
            }
        }
        if (charByteCounter > 1)
        {
            throw new Exception("非预期的byte格式");
        }
        return true;
    }
    #endregion

    #region 文件工具
    /// <summary>
    /// 递归处理某路径及其他的子目录
    /// </summary>
    /// <param name="path">目标路径</param>
    /// <param name="expandName">要处理的特定拓展名</param>
    /// <param name="handle">处理函数</param>
    public static void RecursionFileExecute(string path, string expandName, FileExecuteHandle handle)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            try
            {
                if (expandName != null)
                {
                    if (item.EndsWith("." + expandName))
                    {
                        handle(item);
                    }
                }
                else
                {
                    handle(item);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("RecursionFileExecute Error :" + item + " Exception:" + e.ToString());
            }
        }

        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            RecursionFileExecute(dires[i], expandName, handle);
        }
    }
    #endregion

    #region 获取一个路径下的所有文件

    public static List<string> GetAllFileNamesByPath(string path,string[] expandNames = null)
    {
        List<string> list = new List<string>();

        RecursionFind(list,path,expandNames);

        return list;
    }

    static void RecursionFind(List<string> list,string path , string[] expandNames)
    {
        string[] allUIPrefabName = Directory.GetFiles(path);
        foreach (var item in allUIPrefabName)
        {
            if (ExpandFilter(item, expandNames))
            {
                list.Add(item);
            }
        }

        string[] dires = Directory.GetDirectories(path);
        for (int i = 0; i < dires.Length; i++)
        {
            RecursionFind(list, dires[i], expandNames);
        }
    }

    static bool ExpandFilter(string name,string[] expandNames)
    {
        if(expandNames == null)
        {
            return true;
        }

        else if (expandNames.Length == 0)
        {
            return !name.Contains(".");
        }

        else
        {
            for (int i = 0; i < expandNames.Length; i++)
            {
                if(name.EndsWith("." + expandNames[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }

    #endregion
}

public delegate void FileExecuteHandle(string filePath);
