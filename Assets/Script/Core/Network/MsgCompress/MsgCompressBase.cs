using UnityEngine;
using System.Collections;
using System.Text;
using System;

public abstract class MsgCompressBase 
{
    //限制字节长度
    private static int compressLimit = 1024*5;
    /// <summary>
    /// 压缩方式，请使用小写
    /// </summary>
    /// <returns></returns>
    public abstract string GetCompressType();

    public string CompressString(string msg)
    {
        string result = null;
        var compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(msg);
        if (compressBeforeByte.Length >= compressLimit)
        {
            byte[] compressAfterByte = CompressBytes(compressBeforeByte);
            string compressString = Convert.ToBase64String(compressAfterByte);
            string length = GetCompressType().Length > 9 ? GetCompressType().Length.ToString() : "0" + GetCompressType().Length;
             result = length + GetCompressType() + compressString;
        }
        else
        {
            result = msg;
        }
        //Debug.Log("CompressString:" + result+ " compressBeforeByte.Length:"+ compressBeforeByte.Length);
        return result;
    }
    public abstract byte[] CompressBytes(byte[] data);

    public  string DecompressString(string cMsg)
    {
        string msg = null;
        try
        {
            int length = int.Parse(cMsg.Substring(0, 2));
            //string compressType = cMsg.Substring(2, length);
             msg = cMsg.Substring(2 + length);
        }
        catch (Exception)
        {
            return cMsg;
        }


        //return ZipUtils.DecompressString(msg);
        //Debug.Log("length :" + length + " compressType:" + compressType + " msg:" + msg + "\n\n" + cMsg);
        var compressBeforeByte = Convert.FromBase64String(msg);

        var compressAfterByte = DecompressBytes(compressBeforeByte);
        string decompressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
        //Debug.Log("compressBeforeByte:" + compressBeforeByte.Length + " compressAfterByte:" + compressAfterByte.Length + "" + " decompressString :" + decompressString);
        return decompressString;

    }

    public abstract byte[] DecompressBytes(byte[] data);

}
