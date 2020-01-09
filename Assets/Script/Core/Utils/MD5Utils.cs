using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public static class MD5Utils
{
    public static string GetObjectMD5(object obj)
    {
        if (obj == null)
        {
            Debug.LogError("obj is Null !");
            return "";
        }

        return GetMD5(Object2Bytes(obj));
    }
    public static int GetObjectMD5Hash(object obj)
    {
        return GetHashMD5(Object2Bytes(obj));

    }
    public static byte[] Object2Bytes(object obj)
    {
        byte[] buff;
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter iFormatter = new BinaryFormatter();
            iFormatter.Serialize(ms, obj);
            buff = ms.GetBuffer();
        }
        return buff;
    }

    public static int GetStringToHash(string content)
    {
        return GetHashMD5(Encoding.UTF8.GetBytes(content));
    }


    public static string GetMD5(byte[] data)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(data);
        StringBuilder fileMD5 = new StringBuilder();
        foreach (byte b in result)
        {
            fileMD5.Append(Convert.ToString(b, 16));
        }

        return fileMD5.ToString();
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
}