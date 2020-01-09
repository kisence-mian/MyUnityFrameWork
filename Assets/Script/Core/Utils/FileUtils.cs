using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

    public class FileUtils
    {

        /// <summary>
        /// 复制一个目录下的所有文件到另一个目录
        /// </summary>
        /// <param name="oldDirectory"></param>
        /// <param name="newDirectory"></param>
        /// <param name="overwrite">是否覆盖</param>
        public static void CopyDirectory(string oldDirectory, string newDirectory, bool overwrite = true)
        {
            string[] pathArray = PathUtils.GetDirectoryFilePath(oldDirectory);
            for (int i = 0; i < pathArray.Length; i++)
            {
                string newPath = pathArray[i].Replace(oldDirectory, newDirectory);
                string s = Path.GetDirectoryName(newPath);
                if (!Directory.Exists(s))
                {
                    Directory.CreateDirectory(s);
                }
                File.Copy(pathArray[i], newPath, overwrite);
            }
        }

        public static void MoveFile(string oldFilePath, string newFilePath, bool overwrite = true)
        {
            if (!File.Exists(oldFilePath) || oldFilePath == newFilePath)
                return;
            string s = Path.GetDirectoryName(newFilePath);
            if (!Directory.Exists(s))
            {
                Directory.CreateDirectory(s);
            }
            File.Copy(oldFilePath, newFilePath, overwrite);
            DeleteFile(oldFilePath);
        }
        public static string LoadTextFileByPath(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("path dont exists ! : " + path);
                return "";
            }
           return  File.ReadAllText(path);

        }
        public static byte[] LoadByteFileByPath(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("path dont exists ! : " + path);
                return null;
            }
            FileStream fs = new FileStream(path, FileMode.Open);
  
            byte[] array = new byte[fs.Length];

            fs.Read(array, 0, array.Length);
            fs.Close();

            return array;
        }
        /// <summary>
        /// 加载txt 返回每一行数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] LoadTextFileLineByPath(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Log("path dont exists ! : " + path);
                return null;
            }

            StreamReader sr = File.OpenText(path);
            List<string> line = new  List<string>();
            string tmp = "";
            while ((tmp = sr.ReadLine()) != null)
            {
                line.Add(tmp);
            }

            sr.Close();
            sr.Dispose();

            return line.ToArray();

        }
        public static bool DeleteFile(string path)
        {
            FileInfo t = new FileInfo(path);
            try
            {

                if (t.Exists)
                {
                    t.Delete();
                }
                Debug.Log("File Delete: " + path);
            }
            catch (Exception e)
            {
                Debug.LogError("File delete fail: " + path + "  ---:" + e);
                return false;
            }

            return true;
        }
        /// <summary>
        /// 保存文件数据
        /// </summary>
        /// <param name="path">全路径</param>
        /// <param name="_data">数据</param>
        /// <returns></returns>
        public static bool CreateTextFile(string path, string _data)
        {

            byte[] dataByte = Encoding.GetEncoding("UTF-8").GetBytes(_data);

            return CreateFile(path, dataByte);
        }
        public static bool CreateFile(string path, byte[] _data)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            string temp = Path.GetDirectoryName(path);
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
                stream.Write(_data, 0, _data.Length);
                stream.Close();

                Debug.Log("File written: " + path);
            }
            catch (Exception e)
            {
                Debug.LogError("File written fail: " + path + "  ---:" + e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取文件MD5
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileMD5(string filePath)
        {
            try
            {
                FileInfo fileTmp = new FileInfo(filePath);
                if (fileTmp.Exists)
                {
                    FileStream fs = new FileStream(filePath, FileMode.Open);
                    int len = (int)fs.Length;
                    byte[] data = new byte[len];
                    fs.Close();

                  return  MD5Utils.GetMD5(data);
                   
                }
                return "";
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }

        public static IEnumerator LoadTxtFileIEnumerator(string path, CallBack<string> callback)
        {

            WWW www = new WWW(path);
            yield return www;

            string data = "";
            if (string.IsNullOrEmpty(www.error))
            {
                data = www.text;
            }
            if (callback != null)
                callback(data);
            yield return new WaitForEndOfFrame();

        }
    }