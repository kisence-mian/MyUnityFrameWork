using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;


public class RSAService
{
    /// <summary> 
    /// 根据公钥加密字符串         
    /// </summary> 
    /// <param name="originalString">要加密的字符串</param>         
    /// <param name="publicKey">公钥</param>         
    /// <returns></returns> 
    static public string encriptByPublicKey(string originalString, string publicKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        string res = "";
        try
        {
            rsa.FromXmlString(publicKey);

            byte[] tempByte = rsa.Encrypt(Encoding.UTF8.GetBytes(originalString), false);

            res = Convert.ToBase64String(tempByte);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        return res;
    }

    /// <summary>         
    /// 根据私钥解密        
    /// /// </summary> 
    /// <param name="encriptedString">要解密的字符串</param>         
    /// <param name="privateKey">私钥</param>        
    /// /// <returns></returns> 
    static public string decriptByPrivateKey(string encriptedString, string privateKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

        rsa.FromXmlString(privateKey);

        byte[] tempByte = Convert.FromBase64String(encriptedString);

        return Encoding.UTF8.GetString(rsa.Decrypt(tempByte, false));
    }

    public static string byteToHexStr(byte[] bytes)
    {
        string returnStr = "";
        if (bytes != null)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                returnStr += bytes[i].ToString("X2");
            }
        }
        return returnStr;
    }

    //验证签名
    public static bool VerifySignedHash(string str_DataToVerify, string str_SignedData, string str_Public_Key)
    {
        byte[] SignedData = Convert.FromBase64String(str_SignedData);

        UTF8Encoding ByteConverter = new UTF8Encoding();
        byte[] DataToVerify = ByteConverter.GetBytes(str_DataToVerify);
        try
        {
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            RSAalg.FromXmlString(str_Public_Key);

            return RSAalg.VerifyData(DataToVerify, new MD5CryptoServiceProvider(), SignedData);

        }
        catch (CryptographicException e)
        {
            Console.WriteLine(e.Message);

            return false;
        }
    }
}




       
