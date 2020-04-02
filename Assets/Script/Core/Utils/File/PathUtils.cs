using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;


    public class PathUtils
    {

        /// <summary>
        /// 根据平台转换路径连接符
        /// </summary>
        /// <param name="path">带“/”分割的路径</param>
        /// <returns></returns>
        public static string CreatePlatformPath(string path)
        {
            string[] temps = path.Split(new char[] { '/' });
            string str = "";
            if (temps.Length > 0)
                str = temps[0];

            for (int i = 1; i < temps.Length; i++)
            {
                if (temps[i] != "")
                {
                    str = Path.Combine(str, temps[i]);
                }
            }

            return str;
        }
        /// <summary>
        /// 切割路径
        /// </summary>
        /// <param name="fullPath">路径</param>
        /// <param name="cutFolderName">要切割的文件夹名</param>
        /// <param name="returnLatterPart">是否返回后半部分</param>
        /// <param name="includeCutFolderName">是否包含切割的文件夹名</param>
        /// <returns></returns>
        public static string CutPath(string fullPath, string cutFolderName, bool returnLatterPart = true, bool includeCutFolderName = false)
        {
            fullPath = fullPath.Replace("\\", "/");
            if (!fullPath.Contains(cutFolderName)) return fullPath;
            if (fullPath.Contains(cutFolderName + "/"))
            {
                if (returnLatterPart)
                {

                    if (includeCutFolderName)
                    {
                        return cutFolderName + fullPath.Split(new string[] { cutFolderName }, StringSplitOptions.None)[1];
                    }
                    else
                    {
                        return fullPath.Split(new string[] { cutFolderName + "/" }, StringSplitOptions.None)[1];
                    }
                }
                else
                {
                    if (includeCutFolderName)
                    {
                        return fullPath.Split(new string[] { cutFolderName }, StringSplitOptions.None)[0] + cutFolderName;
                    }
                    else
                    {
                        return fullPath.Split(new string[] { cutFolderName + "/" }, StringSplitOptions.None)[0];
                    }
                }
            }
            else
            {
                if (returnLatterPart)
                {

                    if (includeCutFolderName)
                    {
                        return cutFolderName + fullPath.Split(new string[] { cutFolderName }, StringSplitOptions.None)[1];
                    }
                    else
                    {
                        return fullPath.Split(new string[] { cutFolderName }, StringSplitOptions.None)[1];
                    }
                }
                else
                {
                    if (includeCutFolderName)
                    {
                        return fullPath.Split(new string[] { cutFolderName }, StringSplitOptions.None)[0] + cutFolderName;
                    }
                    else
                    {
                        return fullPath.Split(new string[] { cutFolderName }, StringSplitOptions.None)[0];
                    }
                }
            }
        }

        /// <summary>
        /// 根据方式获取对应的路径
        /// </summary>
        /// <param name="assetPath">相对路径</param>
        /// <param name="type">加载方式</param>
        /// <returns></returns>
        public static string GetSpecialPath(string assetPath, SpecialPathType type)
        {
            string path = assetPath;

            switch (type)
            {
                case SpecialPathType.Resources:
//#if !UNITY
//                    path= Environment.CurrentDirectory + "/Resources/"+path;
//                    break;
//#else
                    path = Application.dataPath + "/Resources/" + path;
                    break;
                case SpecialPathType.Persistent:
                    path = Application.persistentDataPath + "/" + path;
                    break;
                case SpecialPathType.StreamingAssets:
                    path = Application.streamingAssetsPath + "/" + path;
                    break;
//#endif
            }
            return path;
        }

        public static string RemoveExtension(string path)
        {
            string ss = path;
            if (Path.HasExtension(path))
                ss = Path.ChangeExtension(path, null);
            return ss;
        }
        /// <summary>
        /// 移除规定后缀名的路径
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="endsWith"></param>
        /// <returns></returns>
        public static string[] RemovePathWithEnds(string[] paths, string[] endsWith)
        {
            if (endsWith == null && endsWith.Length == 0)
                return paths;
            List<string> resPath = new List<string>();
            List<string> temp = new List<string>(endsWith);

            for (int i = 0; i < paths.Length; i++)
            {
                string s = Path.GetExtension(paths[i]);
                if (temp.Contains(s))
                    continue;
                else
                    resPath.Add(paths[i]);
            }
            return resPath.ToArray();
        }
        /// <summary>
        /// 获取指定目录下的所有文件路径
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="endsWith">文件后缀名如“.txt”</param>
        /// <returns>返回所有文件的全路径</returns>
        public static string[] GetDirectoryFilePath(string path, string[] endsWith = null, bool isIncludeChildFolder = true)
        {

            List<string> pathList = new List<string>();
            if (!Directory.Exists(path))
            {
                Debug.LogError("不存在目录：" + path);
                return new string[0];
            }

            if (isIncludeChildFolder)
            {
                string[] directorys = Directory.GetDirectories(path);
                //所有目录继续遍历
                for (int i = 0; i < directorys.Length; i++)
                {
                    string pathTmp = directorys[i];


                    string[] tempArray = GetDirectoryFilePath(pathTmp, endsWith);
                    pathList.AddRange(tempArray);

                }
            }

            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                string pathTmp = files[i];
                pathTmp = pathTmp.Replace("\\", "/");
                string ends = Path.GetExtension(pathTmp);
                if (endsWith != null && endsWith.Length > 0)
                {
                    for (int j = 0; j < endsWith.Length; j++)
                    {
                        if (ends.Equals(endsWith[j]))
                        {
                            pathList.Add(pathTmp);
                            break;
                        }
                    }
                }
                else
                {
                    pathList.Add(pathTmp);
                }
            }
            return pathList.ToArray();
        }
        /// <summary>
        /// 获取指定目录下的所有文件名字
        /// </summary>
        /// <param name="path"></param>
        /// <param name="endsWith"></param>
        /// <param name="isNeedExtension">是否需要文件后缀名</param>
        /// <returns></returns>
        public static string[] GetDirectoryFileNames(string path, string[] endsWith, bool isNeedExtension = false, bool isIncludeChildFolder = true)
        {
            string[] temps = GetDirectoryFilePath(path, endsWith, isIncludeChildFolder);
            List<string> names = new List<string>();
            for (int i = 0; i < temps.Length; i++)
            {
                if (isNeedExtension)
                {
                    names.Add(Path.GetFileName(temps[i]));
                }
                else
                    names.Add(Path.GetFileNameWithoutExtension(temps[i]));
            }
            return names.ToArray();
        }
        public static string[] GetDirectorys(string path, bool isIncludeChildFolder = true)
        {

            List<string> pathList = new List<string>();

            if (Directory.Exists(path))
            {
                string[] directorys = Directory.GetDirectories(path);
                pathList.AddRange(directorys);

                if (isIncludeChildFolder)
                {

                    //所有目录继续遍历
                    for (int i = 0; i < directorys.Length; i++)
                    {
                        string pathTmp = directorys[i];

                        string[] tempArray = GetDirectorys(pathTmp);
                        pathList.AddRange(tempArray);

                    }
                }
            }
          
            return pathList.ToArray();
        }
        public static string GetFileName(string path)
        {
            string name = "";
            try
            {
                name = Path.GetFileName(path);
            }
            catch //(Exception e)
            {
                path = path.Replace("\\", "/");
                if (!path.Contains("/"))
                {
                    return path;
                }

                string[] ss = path.Split(new string[] { "/" }, StringSplitOptions.None);
                if (ss.Length > 0)
                    return ss[ss.Length - 1];

            }
            return name;
        }
    }
public enum SpecialPathType
{
    Resources,
//#if UNITY
    Persistent,
    StreamingAssets,
//#endif
}
