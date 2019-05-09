using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;

public static class MD5Tool
{
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

                return GetMD5(data);
            }
            return "";
        }
        catch (FileNotFoundException e)
        {
            Debug.Log(e.Message);
            return "";
        }
    }

    public static string GetObjectMD5(object obj)
    {
        if(obj == null)
        {
            Debug.LogError("obj is Null !");
            return "";
        }

        return GetMD5(ByteTool.Object2Bytes(obj));
    }

    public static int GetStringToHash(string content)
    {
        return GetHashMD5(System.Text.Encoding.Default.GetBytes(content));
    }

    public static string GetMD5FromString(string value)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] output = md5.ComputeHash(Encoding.UTF8.GetBytes(value));

        string sign = BitConverter.ToString(output).Replace("-", "");
        return sign;
    }

    public static string GetMD5(byte[] data)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(data);
        string fileMD5 = "";
        foreach (byte b in result)
        {
            fileMD5 += Convert.ToString(b, 16);
        }
        if (!String.IsNullOrEmpty(fileMD5))
        {
            return fileMD5;
        }

        return "";
    }

    public static int GetHashMD5(byte[] data)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(data);
        int hashCode = 0;
        for (int i = 0; i < 4; i++)
        {
            hashCode += (Convert.ToInt32(result[i]) + Convert.ToInt32(result[i + 1]) + Convert.ToInt32(result[i + 2]) + Convert.ToInt32(result[i])) << i * 8;
        }
        return hashCode;
    }

    public static int ToHash(this string content)
    {
        return GetStringToHash(content);
    }
}
